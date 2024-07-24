using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(CardData), true)]
public class PlantableEditor : Editor
{
    private const string ASSEMBLY_NAME = "PlantFunctionAssembly";
    private int selectedScript, selectedEditor;
    private const string DEFAULT_OPTION = "None";
    public class ReflectionDropDown
    {
        public int lastSelected = 0;
        public Type scriptType;
        public List<object> allScripts;
        public string fieldName = "BASE";
    }
    private List<ReflectionDropDown> reflectionDropDownList = new();
    private void Awake()
    {
        SearchNewClasses();

    }

    private void SearchNewClasses()
    {
        reflectionDropDownList.Clear();
        reflectionDropDownList.Add(new ReflectionDropDown()
        {
            scriptType = typeof(CardFunctionBase),
            allScripts = GetAll<CardFunctionBase>(),
            fieldName = "Plant Function"
        });
        reflectionDropDownList.Add(new ReflectionDropDown()
        {
            scriptType = typeof(CardAccessCheckBase),
            allScripts = GetAll<CardAccessCheckBase>(),
            fieldName = "Plant Access Check"
        });
        reflectionDropDownList.Add(new ReflectionDropDown()
        {
            scriptType = typeof(CardSecondMoveBase),
            allScripts = GetAll<CardSecondMoveBase>(),
            fieldName = "Plant Editor"
        });

    }
    public override void OnInspectorGUI()
    {
        CardData targetCardData = target as CardData;
        base.OnInspectorGUI();


        //foreach (var dropDown in reflectionDropDownList)
        //{
        //    //Plant Functions
        //    List<string> options = new() { DEFAULT_OPTION };
        //    foreach (var scripts in dropDown.allScripts)
        //    {
        //        options.Add(scripts.ToString());
        //    }
        //    int lastSelected = 0;
        //    bool drawExecutionDropDown = true;
        //    EXECUTION_TYPE lastExecutionType = EXECUTION_TYPE.NONE;
        //    switch (dropDown.scriptType.ToString())
        //    {
        //        case nameof(PlantFunctionBase):
        //            if (targetPlantable.PlantFunctionCall != null)
        //            {
        //                lastSelected = dropDown.allScripts.IndexOf(targetPlantable.PlantFunctionCall.PlantScriptType) + 1;
        //                lastExecutionType = targetPlantable.PlantFunctionCall.ExecutionType;
        //            }
        //            break;
        //        case nameof(PlantEditorBase):
        //            if (targetPlantable.PlantEditorCall != null)
        //            {
        //                lastSelected = dropDown.allScripts.IndexOf(targetPlantable.PlantEditorCall.ScriptType) + 1;
        //                lastExecutionType = targetPlantable.PlantEditorCall.ExecutionType;
        //            }
        //            break;
        //        case nameof(PlantAccessCheckBase):
        //            if (targetPlantable.PlantAccessCheckCall != null)
        //            {
        //                lastSelected = dropDown.allScripts.IndexOf(targetPlantable.PlantAccessCheckCall.ScriptType) + 1;
        //                lastExecutionType = targetPlantable.PlantAccessCheckCall.ExecutionType;
        //            }
        //            drawExecutionDropDown = false;
        //            break;
        //        default:
        //            Debug.LogError("Type not found in Plantable Editor!");
        //            break;
        //    }
        //    //Drawing Fields
        //    selectedScript = EditorGUILayout.Popup(dropDown.fieldName, lastSelected, options.ToArray());
        //    EXECUTION_TYPE chosenExecutionType = EXECUTION_TYPE.NONE;
        //    if (drawExecutionDropDown)
        //    {
        //        chosenExecutionType = (EXECUTION_TYPE)EditorGUILayout.EnumPopup($"{dropDown.fieldName} Execution Method", lastExecutionType);
        //    }
        //    bool defaultOption = options[selectedScript] == DEFAULT_OPTION;

        //    EditorGUILayout.Space();

        //    //Update PlantScript
        //    if (lastSelected != selectedScript)
        //    {
        //        Type searchClass = typeof(PlantFunctionBase);
        //        if (!defaultOption)
        //        searchClass = GetTypeFromString(dropDown.allScripts[selectedScript - 1].ToString());
        //        switch (dropDown.scriptType.ToString())
        //        {
        //            case nameof(PlantFunctionBase):
        //                if (defaultOption)
        //                {
        //                    break;
        //                }
        //                targetPlantable.PlantFunctionCall = new Plantable.PlantFunctionCall(searchClass, chosenExecutionType);
        //                break;
        //            case nameof(PlantEditorBase):
        //                if (defaultOption)
        //                {
        //                    break;
        //                }
        //                targetPlantable.PlantEditorCall = new Plantable.PlantFunctionCall(searchClass, chosenExecutionType);
        //                break;
        //            case nameof(PlantAccessCheckBase):
        //                if (defaultOption)
        //                {
        //                    break;
        //                }
        //                targetPlantable.PlantAccessCheckCall = new Plantable.PlantFunctionCall(searchClass, chosenExecutionType);
        //                break;
        //            default:
        //                Debug.LogError("Type not found in Plantable Editor!");
        //                break;
        //        }
        //        EditorUtility.SetDirty(target);
        //    }

        //    if (lastExecutionType != chosenExecutionType)
        //    {
        //        Debug.Log("Set execution method");
        //        switch (dropDown.scriptType.ToString())
        //        {
        //            case nameof(PlantFunctionBase):
        //                targetPlantable.PlantFunctionCall.ExecutionType = chosenExecutionType;
        //                break;
        //            case nameof(PlantEditorBase):
        //                targetPlantable.PlantEditorCall.ExecutionType = chosenExecutionType;
        //                break;
        //            case nameof(PlantAccessCheckBase):
        //                targetPlantable.PlantAccessCheckCall.ExecutionType = chosenExecutionType;
        //                break;
        //            default:
        //                Debug.LogError("Type not found in Plantable Editor!");
        //                break;
        //        }
        //        EditorUtility.SetDirty(target);
        //    }
        //}

        //AssetDatabase.SaveAssetIfDirty(target);
        //GUI.backgroundColor = Color.yellow;
        //if (GUILayout.Button("Search for new Classes"))
        //{
        //    SearchNewClasses();
        //}
        //GUI.backgroundColor = Color.green;

        if (GUILayout.Button("Show Debug Information"))
        {
            ShowDebug(targetCardData);
        }
    }

    public void ShowDebug(CardData targetCardData)
    {
        Debug.Log($"{targetCardData} Plant Function: {targetCardData.CardFunction.ScriptType}, {targetCardData.CardFunction.ExecutionType}");
        Debug.Log($"{targetCardData} Plant Editor: {targetCardData.CardEditor.ScriptType}, {targetCardData.CardEditor.ExecutionType}");
        Debug.Log($"{targetCardData} Plant Access Check: {targetCardData.CardAccessCheck.ScriptType}, {targetCardData.CardAccessCheck.ExecutionType}");
    }

    public void OnValidate()
    {
    }
    List<object> GetAll<TType>()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(TType))).Cast<object>().ToList();
    }

    Type GetTypeFromString(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes()).First(t => t.Name == name);
    }
}
