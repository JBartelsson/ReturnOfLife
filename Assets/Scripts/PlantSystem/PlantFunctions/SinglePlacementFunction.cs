using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlacementFunction : PlantFunctionBase
{


    public override void Execute(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        gridTile.AddPlantable(callerArgs);
        callerArgs.gameManager.AddPointScore(callerArgs.callingPlantInstance.Plantable.regularPoints);

    }

    public override bool CanExecute(CallerArgs callerArgs)
    {
        return true;
    }
}
