using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ComboBushPlantFunction : PlantFunctionBase
{
    [NonSerialized] private PlantInstance bushInstance;
    [NonSerialized] private bool alreadyTriggered = false;

    public ComboBushPlantFunction()
    {
        alreadyTriggered = false;
        Debug.Log($"ALREADY TRIGGERED CONSTRUCTOR: {alreadyTriggered}");

    }

    public bool AlreadyTriggered { get => alreadyTriggered; set => alreadyTriggered = value; }

    public override bool Execute(CallerArgs callerArgs)
    {
        Debug.Log($"ALREADY TRIGGERED EXECUTE: {alreadyTriggered}");
        Debug.Log($"ALREADY TRIGGERED prop EXECUTE: {AlreadyTriggered}");

        bushInstance = callerArgs.callingPlantInstance;
        callerArgs.playedTile.AddPlantable(callerArgs);
        callerArgs.playedTile.OnContentUpdated += PlayedTile_OnContentUpdated;
        return true;
    }

    private void PlayedTile_OnContentUpdated(object sender, EventArgs e)
    {
        Debug.Log("Event Of Bush Plant Updated!");
        GridTile callingGridTile = sender as GridTile;
        if (callingGridTile == null) return;
        Debug.Log($"ALREADY TRIGGERED: {alreadyTriggered}");
        if (alreadyTriggered) return;
        alreadyTriggered = true;

        CallerArgs bushCallerArgs = new CallerArgs(new PlantInstance(bushInstance), null, false, CALLER_TYPE.EFFECT);
        if (!bushInstance.IsBasicFertilized())
        {
            callingGridTile.ForEachNeighbor((x) =>
            {
                bushCallerArgs.playedTile = x;
                bushInstance.Execute(bushCallerArgs);
            });
        } else
        {
            callingGridTile.ForEachAdjacentTile((x) =>
            {
                bushCallerArgs.playedTile = x;
                bushInstance.Execute(bushCallerArgs);
            });
        }
        
    }
}
