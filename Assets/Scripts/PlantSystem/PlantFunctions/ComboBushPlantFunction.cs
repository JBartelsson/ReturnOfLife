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

    public override void Execute(CallerArgs callerArgs)
    {
        bushInstance = callerArgs.callingPlantInstance;
        callerArgs.playedTile.AddPlantable(callerArgs);
        callerArgs.playedTile.OnContentUpdated += PlayedTile_OnContentUpdated;
        EventManager.Game.Level.OnInCardSelection += OnCardPlayingEnded;
    }

    private void OnCardPlayingEnded(EventManager.GameEvents.Args arg0)
    {
        alreadyTriggered = false;
    }

    public override bool CanExecute(CallerArgs callerArgs)
    {
        return true;
    }

    private void PlayedTile_OnContentUpdated(object sender, EventArgs e)
    {
        GridTile callingGridTile = sender as GridTile;
        if (callingGridTile == null) return;
        Debug.Log($"TILE {callingGridTile.X}, {callingGridTile.Y} UPDATED, Executing Lycoperdon Function alreadyTriggered: {alreadyTriggered}");
        Debug.Log(this.GetHashCode());
        // if (alreadyTriggered) return;
        alreadyTriggered = true;
        CallerArgs bushCallerArgs = new CallerArgs(new PlantInstance(bushInstance), null, false, CALLER_TYPE.EFFECT);
        if (!bushInstance.IsBasicFertilized())
        {
            callingGridTile.ForEachNeighbor((gridTile) =>
            {
                bushCallerArgs.playedTile = gridTile;
                Debug.Log($"Trying to Execute Bush on {gridTile.X}, {gridTile.Y}");
                bushInstance.Execute(bushCallerArgs);
            });
        } else
        {
            callingGridTile.ForEachAdjacentTile((gridTile) =>
            {
                bushCallerArgs.playedTile = gridTile;
                bushInstance.Execute(bushCallerArgs);
            });
        }
        
    }
}
