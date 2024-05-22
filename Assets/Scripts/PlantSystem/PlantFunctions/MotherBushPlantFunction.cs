using Ionic.Zlib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherBushPlantFunction : PlantFunctionBase
{

    public override void Execute(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        // Play first normal
        gridTile.AddPlantable(callerArgs);
        // then with effect caller type
        callerArgs.callerType = CALLER_TYPE.EFFECT;
        gridTile.BottomNeighbor?.AddPlantable(callerArgs);
        gridTile.TopNeighbor?.AddPlantable(callerArgs);
        gridTile.LeftNeighbor?.AddPlantable(callerArgs);
        gridTile.RightNeighbor?.AddPlantable(callerArgs);
    }

    public override bool CanExecute(CallerArgs callerArgs)
    {
        return true;
    }
}
