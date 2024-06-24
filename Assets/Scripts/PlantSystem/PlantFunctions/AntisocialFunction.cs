using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntisocialFunction : PlantFunctionBase
{
    private static int MAX_DISTANCE = 3;

    public override void ExecuteCard(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        gridTile.AddPlantable(callerArgs);
        bool hasPlantInRange = false;
        Debug.Log($"====================================");
        GridManager.Instance.Grid.ForEachGridTile((tile) =>
        {
            Debug.Log($"CHECKING INSTANCE: {tile.PlantInstance}, CALLING INSTANCE: {callerArgs.callingPlantInstance}");
            if (tile == gridTile) return;
            if (tile.PlantInstance == null) return;
            if (tile.PlantInstance.Plantable.GetType() == callerArgs.callingPlantInstance.Plantable.GetType())
            {
                Debug.Log($"PLANT INSTANCES ARE SAME");
                if (gridTile.DistanceTo(tile) <= MAX_DISTANCE)
                {
                    Debug.Log($"HAS PLANT IN RANGE");

                    hasPlantInRange = true;
                }
            }
        });
        Debug.Log($"Checking if return is working");
        Plantable callingPlantable = callerArgs.callingPlantInstance.Plantable;
        if (hasPlantInRange)
        {
            Debug.Log($"OVERRIDING PLANT FUNCTION!");
            callingPlantable.RuntimePoints = 0;
            callingPlantable.OverridePointFunction = true;
        }
    }

    public override bool CanExecute(CallerArgs callerArgs)
    {
        return true;
    }
}