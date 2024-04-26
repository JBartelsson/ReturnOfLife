using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushPlantFunction : PlantFunctionBase
{
    Plantable bushPlantable;
    public override bool Execute(CallerArgs callerArgs)
    {
        bushPlantable = callerArgs.callingPlantable;
        callerArgs.playedTile.AddPlantable(callerArgs);
        callerArgs.playedTile.OnContentUpdated += PlayedTile_OnContentUpdated;
        return true;
    }

    private void PlayedTile_OnContentUpdated(object sender, EventArgs e)
    {
        Debug.Log("Event Of Bush Plant Updated!");
        GridTile callingGridTile = sender as GridTile;
        if (callingGridTile == null) return;

        CallerArgs bushCallerArgs = new CallerArgs(bushPlantable, null, false, CALLER_TYPE.EFFECT);
        callingGridTile.ForEachNeighbor((x) =>
        {
            bushCallerArgs.playedTile = x;
            bushPlantable.ExecuteFunction(bushCallerArgs);
        });
    }
}
