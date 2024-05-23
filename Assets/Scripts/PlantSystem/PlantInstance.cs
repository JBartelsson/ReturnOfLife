using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

public class PlantInstance
{
    private Plantable plantable;
    private List<Fertilizer> fertilizers = new();
    private PlantFunctionBase plantFunction;
    private PlantEditorBase plantEditor;
    private PlantAccessCheckBase plantAccessCheck;

    public PlantInstance(Plantable newPlantable, List<Fertilizer> newFertilizers = null)
    {
        plantable = newPlantable;
        TryAddFertilizer(newFertilizers);
        InitScripts();
    }
    public PlantInstance(PlantInstance other, List<Fertilizer> newFertilizers = null)
    {
        plantable = other.plantable;
        TryAddFertilizer(newFertilizers);
        InitScripts();

    }

    public PlantInstance(PlantInstance other)
    {
        plantable = other.plantable;
        fertilizers.AddRange(other.fertilizers);
        InitScripts();
    }

    private void InitScripts()
    {
        if (plantable.PlantFunction.ScriptType.Type != null)
        {
            plantFunction = (PlantFunctionBase)Activator.CreateInstance(plantable.PlantFunction.ScriptType);
            plantFunction.ExecutionType = plantable.PlantFunction.ExecutionType;
        }
        else
        {
            Debug.LogWarning($"{this} tried to create a plantFunction from Default");
        }
        if (plantable.PlantEditor.ScriptType.Type != null)
        {
            plantEditor = (PlantEditorBase)Activator.CreateInstance(plantable.PlantEditor.ScriptType);
            plantEditor.ExecutionType = plantable.PlantEditor.ExecutionType;
        }
        else
        {
            Debug.LogWarning($"{this} tried to create a plantEditor from Default");
        }
        if (plantable.PlantAccessCheck.ScriptType.Type != null)
        {
            plantAccessCheck = (PlantAccessCheckBase)Activator.CreateInstance(plantable.PlantAccessCheck.ScriptType);
            plantAccessCheck.ExecutionType = plantable.PlantAccessCheck.ExecutionType;
        }
        else
        {
            Debug.LogWarning($"{this} tried to create a plantAccessCheck from Default");
        }

    }
    public override string ToString()
    {
        string fertilizerString = "";
        foreach (var item in this.fertilizers)
        {
            fertilizerString += item.ToString();
        }
        return $"{plantable} played with fertilizers: {fertilizerString}";
    }

    public string DebugVisualization()
    {
        string fertilized = IsBasicFertilized() ? "*" : "";
        return plantable.visualization + fertilized;
    }



    private void TryAddFertilizer(List<Fertilizer> newFertilizers)
    {
        if (newFertilizers == null) return;
        fertilizers.AddRange(newFertilizers);

    }

    public Plantable Plantable { get => plantable; set => plantable = value; }
    public List<Fertilizer> Fertilizers { get => fertilizers; set => fertilizers = value; }
    public PlantFunctionBase PlantFunction { get => plantFunction; set => plantFunction = value; }
    public PlantEditorBase PlantEditor { get => plantEditor; set => plantEditor = value; }
    public PlantAccessCheckBase PlantAccessCheck { get => plantAccessCheck; set => plantAccessCheck = value; }

    public bool IsBasicFertilized()
    {
        return fertilizers.Contains(Fertilizer.Basic);
    }
    public int ReturnTriggerAmount()
    {
        return fertilizers.Where((x) => x == Fertilizer.Retrigger).Count() + plantable.triggerAmount;
    }

    public void Execute(CallerArgs callerArgs)
    {
        if (CanExecute(callerArgs))
        plantFunction.Execute(callerArgs);
    }

    public bool CanExecute(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        if (plantFunction.ExecutionType != EXECUTION_TYPE.IMMEDIATE)
        {
            if (!gridTile.IsAccessible(callerArgs)) return false;
            if (callerArgs.needNeighbor && !gridTile.HasNeighboredPlant()) return false;
        }
        return plantFunction.CanExecute(callerArgs);
    }

    public bool IsAccessible(CallerArgs callerArgs)
    {
        return plantAccessCheck.IsAccessible(callerArgs);
    }

    public bool CheckField(EditorCallerArgs editorCallerArgs)
    {
        return plantEditor.CheckField(editorCallerArgs);
    }

    public void ExecuteEditor(EditorCallerArgs editorCallerArgs)
    {
        plantEditor.ExecuteEditor(editorCallerArgs);
    }

    public EXECUTION_TYPE GetPlantFunctionExecuteType()
    {
        if (plantFunction == null) return EXECUTION_TYPE.NONE;
        return plantFunction.ExecutionType;
    }
}
