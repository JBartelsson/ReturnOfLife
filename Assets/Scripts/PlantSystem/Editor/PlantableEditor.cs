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
    private int selectedPlant, selectedEditor;
    private const string DEFAULT_OPTION = "None";
    public override void OnInspectorGUI()
    {
        Plantable targetPlantable = target as Plantable;
        base.OnInspectorGUI();
        var allPlantFunctions = GetAll<PlantFunction>();
        var allPlantEditors = GetAll<PlantEditor>();

        //Plant Functions
        List<string> optionsPlants = new() { DEFAULT_OPTION};
        foreach (var plantFunctions in allPlantFunctions)
        {
            optionsPlants.Add(plantFunctions.ToString());
        }
        int lastSelectedPlantFunction = 0;
        if (targetPlantable.PlantFunction != null)
        {
            lastSelectedPlantFunction = allPlantFunctions.IndexOf(targetPlantable.PlantFunction.GetType()) + 1;
        }
        selectedPlant = EditorGUILayout.Popup("Plant Execution Function", lastSelectedPlantFunction, optionsPlants.ToArray());
        
        //Plant Editors
        List<string> optionsEditors = new() { DEFAULT_OPTION };
        foreach (var plantEditor in allPlantEditors)
        {
            optionsEditors.Add(plantEditor.ToString());
        }
        int lastSelectedPlantEditor = 0;
        if (targetPlantable.PlantEditor != null)
        {
            lastSelectedPlantEditor = allPlantEditors.IndexOf(targetPlantable.PlantEditor.GetType()) + 1;
        }


        selectedEditor = EditorGUILayout.Popup("Plant Editor", lastSelectedPlantEditor, optionsEditors.ToArray());

        if (lastSelectedPlantFunction != selectedPlant)
        {
            if (optionsPlants[selectedPlant] == DEFAULT_OPTION)
            {
                targetPlantable.PlantFunction = null;
            }
            else
            {
                targetPlantable.PlantFunction = Activator.CreateInstance(allPlantFunctions[selectedPlant - 1]) as PlantFunction;

            }
            Debug.Log(targetPlantable.PlantFunction.GetType());
            EditorUtility.SetDirty(target);
        }

        if (lastSelectedPlantEditor != selectedEditor)
        {
            if (optionsEditors[selectedEditor] == DEFAULT_OPTION)
            {
                Debug.Log($"{selectedEditor}: None picked");
                targetPlantable.PlantEditor = null;
            }
            else
            {
                Debug.Log("Editor set");

                targetPlantable.PlantEditor = Activator.CreateInstance(allPlantEditors[selectedEditor - 1]) as PlantEditor;

            }
            EditorUtility.SetDirty(target);
        }
        
        AssetDatabase.SaveAssetIfDirty(target);
    }

    

    List<System.Type> GetAll<TType>()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(TType))).ToList();
    }
}
