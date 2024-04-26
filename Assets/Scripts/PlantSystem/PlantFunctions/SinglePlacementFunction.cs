using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlacementFunction : PlantFunctionBase
{


    public override bool Execute(CallerArgs plantableExecuteArgs)
    {
        GridTile gridTile = plantableExecuteArgs.playedTile;
        gridTile.AddPlantable(plantableExecuteArgs.callingPlantable);
        return true;
    }
}
