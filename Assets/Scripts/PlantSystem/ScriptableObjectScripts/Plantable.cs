using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Plantable")]
public class Plantable : ScriptableObject
{
    public enum PlantableType
    {
        Plant, Fertilizer
    }
    public PlantableType type;
    public int manaCost = 1;
    public int turnDelay = 0;

    public string visualization = "0";
    [SerializeReference] protected PlantFunctionBase plantFunction;
    [SerializeReference] protected PlantEditorBase plantEditor = null;
    [SerializeReference] private PlantAccessCheckBase plantAccessCheck = null;

    public PlantFunctionBase PlantFunction { get => plantFunction; set => plantFunction = value; }
    public PlantEditorBase PlantEditor { get => plantEditor; set => plantEditor = value; }
    public PlantAccessCheckBase PlantAccessCheck { get => plantAccessCheck; set => plantAccessCheck = value; }

    public bool ExecuteFunction(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        Debug.Log(gridTile);
        if (!gridTile.IsAccessible(callerArgs)) return false;
        if (callerArgs.needNeighbor && !gridTile.HasNeighboredPlant()) return false;
       return PlantFunction.Execute(callerArgs);
    }

    public bool IsAccessible(CallerArgs callerArgs)
    {
        return plantAccessCheck.IsAccessible(callerArgs);
    }

    public bool CheckField(EditorCallerArgs editorCallerArgs)
    {
        return PlantEditor.CheckField(editorCallerArgs);
    }

    public void ExecuteEditor(EditorCallerArgs editorCallerArgs)
    {
        plantEditor.ExecuteEditor(editorCallerArgs);
    }

}
