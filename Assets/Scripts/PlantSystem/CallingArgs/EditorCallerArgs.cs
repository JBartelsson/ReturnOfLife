using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCallerArgs : CallerArgs
{
    //Used for editor access checks, so playedTile and selectedTile is different
    public GridTile selectedGridTile;
    public CardInstance EditorCallingCardInstance;

    public EditorCallerArgs()
    {
        if (callerType == CALLER_TYPE.NONE)
        {
            callerType = CALLER_TYPE.EDITOR;
        }
    }

    public void SetValues(CardInstance newCallingPlantable, CardInstance newEditorCallingCardInstance, GridTile newPlayedTile, GridTile newSelectedTile, bool newNeedNeighbor, CALLER_TYPE newCallerType)
    {
        SetValues(newCallingPlantable, newPlayedTile, newNeedNeighbor, newCallerType);
        selectedGridTile = newSelectedTile;
        EditorCallingCardInstance = newEditorCallingCardInstance;
    }
} 
