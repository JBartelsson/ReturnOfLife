using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TypeReferences;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Plantable")]
public class Plantable : ScriptableObject
{
    [Serializable]

    public abstract class Executable
    {
        [SerializeField] protected EXECUTION_TYPE executionType;
        public EXECUTION_TYPE ExecutionType { get => executionType; set => executionType = value; }

    }
    [Serializable]
    public class PlantFunctionCall : Executable
    {
        [ClassExtends(typeof(PlantFunctionBase))]
        public ClassTypeReference scriptType = typeof(PlantFunctionBase);

        public ClassTypeReference ScriptType { get => scriptType; set => scriptType = value; }

        public PlantFunctionCall(Type plantScriptType, EXECUTION_TYPE exectutionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = exectutionType;
        }
    }
    [Serializable]

    public class PlantEditorCall : Executable
    {
        [ClassExtends(typeof(PlantEditorBase))]
        public ClassTypeReference scriptType = typeof(PlantFunctionBase);

        public ClassTypeReference ScriptType { get => scriptType; set => scriptType = value; }

        public PlantEditorCall(Type plantScriptType, EXECUTION_TYPE exectutionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = exectutionType;
        }
    }
    [Serializable]

    public class PlantAccessCheckCall : Executable
    {
        [ClassExtends(typeof(PlantAccessCheckBase))]
        public ClassTypeReference scriptType = typeof(PlantFunctionBase);

        public ClassTypeReference ScriptType { get => scriptType; set => scriptType = value; }

        public PlantAccessCheckCall(Type plantScriptType, EXECUTION_TYPE exectutionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = exectutionType;
        }
    }


    public enum PlantableType
    {
        Plant, Fertilizer
    }
    public PlantableType type;
    public int manaCost = 1;
    public int turnDelay = 0;
    public int triggerAmount = 1;

    public string visualization = "0";
    [SerializeField] private PlantFunctionCall plantFunction;
    [SerializeField] private PlantEditorCall plantEditor = null;
    [SerializeField] private PlantAccessCheckCall plantAccessCheck = null;

    public PlantFunctionCall PlantFunction { get => plantFunction; set => plantFunction = value; }
    public PlantEditorCall PlantEditor { get => plantEditor; set => plantEditor = value; }
    public PlantAccessCheckCall PlantAccessCheck { get => plantAccessCheck; set => plantAccessCheck = value; }
}

