using Ionic.Zlib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherBushPlantFunction : PlantFunctionBase
{

    public override bool Execute(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        Plantable caller = callerArgs.callingPlantable;
        gridTile.AddPlantable(caller);
        gridTile.BottomNeighbor?.AddPlantable(caller);
        gridTile.TopNeighbor?.AddPlantable(caller);
        gridTile.LeftNeighbor?.AddPlantable(caller);
        gridTile.RightNeighbor?.AddPlantable(caller);
        return true;
    }
}
