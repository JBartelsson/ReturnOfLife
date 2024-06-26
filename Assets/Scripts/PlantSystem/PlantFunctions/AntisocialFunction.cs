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
        bool hasPlantInPattern = false;
        Debug.Log($"====================================");
        gridTile.ForPattern(callerArgs.CallingCardInstance.GetCardStats().EffectPattern, tile =>
        {
            if (tile.CardInstance == null) return;
            if (tile.CardInstance.CardID == callerArgs.CallingCardInstance.CardID)
            {
                hasPlantInPattern = true;
            }
        } );
        Debug.Log($"Checking if return is working");
        CardData callingCardData = callerArgs.CallingCardInstance.CardData;
        if (hasPlantInPattern)
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