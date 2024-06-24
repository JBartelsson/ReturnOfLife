using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlacementFunction : PlantFunctionBase
{


    public override void ExecuteCard(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        Debug.Log($"GETTIJNG CALLED FOR TILE {callerArgs.playedTile} AND SETTING {callerArgs.callingPlantInstance}");
        gridTile.AddPlantable(callerArgs);

    }

    public override bool CanExecute(CallerArgs callerArgs)
    {
        return true;
    }
}
