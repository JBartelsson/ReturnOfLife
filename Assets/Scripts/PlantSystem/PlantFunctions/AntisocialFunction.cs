using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntisocialFunction : CardFunctionBase
{
    private static int MAX_DISTANCE = 3;

    public override void ExecuteCard(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        gridTile.AddObject(callerArgs);
        bool hasPlantInRange = false;
        Debug.Log($"====================================");
        GridManager.Instance.Grid.ForEachGridTile((tile) =>
        {
            Debug.Log($"CHECKING INSTANCE: {tile.CardInstance}, CALLING INSTANCE: {callerArgs.CallingCardInstance}");
            if (tile == gridTile) return;
            if (tile.CardInstance == null) return;
            if (tile.CardInstance.CardData.GetType() == callerArgs.CallingCardInstance.CardData.GetType())
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
        CardData callingCardData = callerArgs.CallingCardInstance.CardData;
        if (hasPlantInRange)
        {
            Debug.Log($"OVERRIDING PLANT FUNCTION!");
            callingCardData.RuntimePoints = 0;
            callingCardData.OverridePointFunction = true;
        }
    }

    public override bool CanExecute(CallerArgs callerArgs)
    {
        return true;
    }
}