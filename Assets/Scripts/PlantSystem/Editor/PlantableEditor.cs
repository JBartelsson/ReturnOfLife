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
            scriptType = typeof(PlantFunctionBase),
            allScripts = GetAll<PlantFunctionBase>(),
            fieldName = "Plant Function"
        });
        reflectionDropDownList.Add(new ReflectionDropDown()
        {
            scriptType = typeof(PlantAccessCheckBase),
            allScripts = GetAll<PlantAccessCheckBase>(),
            fieldName = "Plant Access Check"
        });
        reflectionDropDownList.Add(new ReflectionDropDown()
        {
            scriptType = typeof(PlantEditorBase),
            allScripts = GetAll<PlantEditorBase>(),
            fieldName = "Plant Editor"
        });

    }
    public override void OnInspectorGUI()
    {
        Plantable targetPlantable = target as Plantable;
        base.OnInspectorGUI();


        foreach (var dropDown in reflectionDropDownList)
        {
            //Plant Functions
            List<string> options = new() { DEFAULT_OPTION};
            foreach (var scripts in dropDown.allScripts)
            {
                options.Add(scripts.ToString());
            }
            int lastSelected = 0;
            switch (dropDown.scriptType.ToString())
            {
                case nameof(PlantFunctionBase):
                    if (targetPlantable.PlantFunction != null)
                    {
                        lastSelected = dropDown.allScripts.IndexOf(targetPlantable.PlantFunction.GetType()) + 1;
                    }
                    break;
                case nameof(PlantEditorBase):
                    if (targetPlantable.PlantEditor != null)
                    {
                        lastSelected = dropDown.allScripts.IndexOf(targetPlantable.PlantEditor.GetType()) + 1;
                    }
                    break;
                case nameof(PlantAccessCheckBase):
                    if (targetPlantable.PlantAccessCheck != null)
                    {
                        lastSelected = dropDown.allScripts.IndexOf(targetPlantable.PlantAccessCheck.GetType()) + 1;
                    }
                    break;
                default:
                    Debug.LogError("Type not found in Plantable Editor!");
                    break;
            }

            selectedScript = EditorGUILayout.Popup(dropDown.fieldName, lastSelected, options.ToArray());
            bool defaultOption = options[selectedScript] == DEFAULT_OPTION;

            if (lastSelected != selectedScript)
            {
                Type searchClass = GetTypeFromString(dropDown.allScripts[selectedScript - 1].ToString());
                switch (dropDown.scriptType.ToString())
                {
                    case nameof(PlantFunctionBase):
                        if (defaultOption)
                        {
                            targetPlantable.PlantFunction = null;
                            break;
                        }
                        targetPlantable.PlantFunction = (PlantFunctionBase)Activator.CreateInstance(searchClass);
                        break;
                    case nameof(PlantEditorBase):
                        if (defaultOption)
                        {
                            targetPlantable.PlantEditor = null;
                            break;
                        }
                        targetPlantable.PlantEditor = (PlantEditorBase)Activator.CreateInstance(searchClass);
                        break;
                    case nameof(PlantAccessCheckBase):
                        if (defaultOption)
                        {
                            targetPlantable.PlantAccessCheck = null;
                            break;
                        }
                        targetPlantable.PlantAccessCheck = (PlantAccessCheckBase)Activator.CreateInstance(searchClass);
                        break;
                    default:
                        Debug.LogError("Type not found in Plantable Editor!");
                        break;
                }
                EditorUtility.SetDirty(target);

            }
        }

        AssetDatabase.SaveAssetIfDirty(target);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Search for new Classes"))
        {
            SearchNewClasses();
        }
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
