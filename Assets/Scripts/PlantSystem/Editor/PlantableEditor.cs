using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
[CustomEditor(typeof(Plantable), true)]
public class PlantableEditor : Editor
{
    private const string ASSEMBLY_NAME = "PlantFunctionAssembly";
    private int selected;
    private const string DEFAULT_OPTION = "None";
    public override void OnInspectorGUI()
    {
        Plantable targetPlantable = target as Plantable;
        base.OnInspectorGUI();
        var allPlantFunctions = GetAll();
        

        List<string> options = new() { DEFAULT_OPTION};

        foreach (var item in allPlantFunctions)
        {
            options.Add(item.ToString());
        }
        int lastSelected = 0;
        if (targetPlantable.PlantFunction != null)
        {
            lastSelected = allPlantFunctions.IndexOf(targetPlantable.PlantFunction.GetType()) + 1;
        }

        selected = EditorGUILayout.Popup("Plant Execution Function", lastSelected, options.ToArray());

        if (lastSelected == selected) return;
        if (options[selected] == DEFAULT_OPTION)
        {
            targetPlantable.PlantFunction = null;
        } else
        {
            targetPlantable.PlantFunction = Activator.CreateInstance(allPlantFunctions[selected - 1]) as PlantFunction;

        }
        Debug.Log(targetPlantable.PlantFunction.GetType());
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssetIfDirty(target);
    }

    

    List<System.Type> GetAll()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(PlantFunction))).ToList();
    }
}
