using Ionic.Zlib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BindweedEditor : CardEditorBase
{
    private const int BINDWEED_RANGE = 3;

    public override bool CheckField(EditorCallerArgs callerArgs)
    {
        GridTile selectedGridTile = callerArgs.selectedGridTile;
        GridTile caller = callerArgs.playedTile;
        //If not on same axis, return
        if (!selectedGridTile.OnSameAxisAs(caller) &&
            !callerArgs.EditorCallingCardInstance.IsBasicFertilized()) return false;
        //if Distance is greater than 3 return
        if (selectedGridTile.DistanceTo(caller) > BINDWEED_RANGE) return false;
        if (selectedGridTile == caller) return false;
        //If tile is accessible
        callerArgs.playedTile = callerArgs.selectedGridTile;
        if (!selectedGridTile.IsAccessible(callerArgs)) return false;
        return true;
    }


    public override void ExecuteEditor(EditorCallerArgs callerArgs)
    {
        Debug.Log($"EXECUTING EDITOR FUNCTION");
        callerArgs.playedTile = callerArgs.selectedGridTile;
        callerArgs.EditorCallingCardInstance.CardFunction.ExecutionType = EXECUTION_TYPE.IMMEDIATE;
        callerArgs.EditorCallingCardInstance.Execute(callerArgs);
    }
}