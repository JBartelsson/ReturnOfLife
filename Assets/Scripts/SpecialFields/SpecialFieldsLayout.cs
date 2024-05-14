using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// @kurtdekker - ultra Cheesy grid with in-built editor for Unity3D
//
// To use:
//	make an empty game object
//	drag this script on it
// 	make a prefab out of it
//	select the prefab to edit the grid
//
// now you can cheesy-easy edit a smallish grid in the Unity editor window
//
public class SpecialFieldsLayout : MonoBehaviour
{
	[Header( "Cell values cycle from 0 to this-1")]
	public int MaxValue = 4;

	[Header( "Actual saved payload. Use GetCell(x,y) to read!")]
	[Header( "WARNING: changing this will nuke your data!")]
	public string data;

	public int across = 6;
	public int down = 4;

	// stretch goals for you:
	// TODO: make an array of colors perhaps?
	// TODO: make a color mapper??
	// TODO: map above characters to graphics??

	// for you to get stuff out of the grid to use in your game
	public string GetCell( int x, int y)
	{
		int n = GetIndex( x, y);
		return data.Substring( n, 1);
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		if (data == null || data.Length != (across * down))
		{
			Undo.RecordObject( this, "Resize");

			if (across < 1) across = 1;
			if (down < 1) down = 1;

			// make a default layout
			data = "";
			for (int y = 0; y < down; y++)
			{
				for (int x = 0; x < across; x++)
				{
					string cell = "0";
					data = data + cell;
				}
			}

			EditorUtility.SetDirty( this);
		}
	}
	void Reset()
	{
		OnValidate();
	}
#endif

	public int GetIndex( int x, int y)
	{
		if (x < 0) return -1;
		if (y < 0) return -1;
		if (x >= across) return -1;
		if (y >= down) return -1;
		return x + y * across;
	}

	public void ToggleCell( int x, int y)
	{
		int n = GetIndex( x, y);
		if (n >= 0)
		{
			var cell = data.Substring( n, 1);

			int c = 0;
			int.TryParse( cell, out c);
			c++;
			if (c >= MaxValue) c = 0;

			cell = c.ToString();

#if UNITY_EDITOR
			Undo.RecordObject( this, "Toggle Cell");
#endif
			// reassemble
			data = data.Substring( 0, n) + cell + data.Substring( n + 1);
#if UNITY_EDITOR
			EditorUtility.SetDirty( this);
#endif
		}
	}
}