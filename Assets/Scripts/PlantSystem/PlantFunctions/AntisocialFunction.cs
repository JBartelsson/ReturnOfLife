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
            if (tile.CardInstance.IsDead()) return;
            Debug.Log(tile.CardInstance.CardData.CardName);
            Debug.Log(callerArgs.CallingCardInstance.CardData.CardName);

            
            if (tile.CardInstance.CardData.CardName == callerArgs.CallingCardInstance.CardData.CardName)
            {
                Debug.Log($"HAS PLANT IN PATTERN!!!");
                hasPlantInPattern = true;
            }
        } );
        Debug.Log($"Checking if return is working");
        CardData callingCardData = callerArgs.CallingCardInstance.CardData;
        if (hasPlantInPattern)
        {
            Debug.Log($"OVERRIDING PLANT FUNCTION!");
            callingCardData.RuntimeScore = new Score(0);
            callingCardData.OverridePointFunction = true;
        }
    }

    public override void Clear(CallerArgs callerArgs)
    {
        
    }
}