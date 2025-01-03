using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function_Socius : CardFunctionBase
{
    public override void ExecuteCard(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        gridTile.AddObject(callerArgs);
        gridTile.ForPattern(callerArgs.CallingCardInstance.GetEffectCardStats().EffectPattern, tile =>
        {
            if (tile.ContainsAnyPlant())
            {
                callerArgs.CallingCardInstance.CardData.RuntimeScore +=
                    callerArgs.CallingCardInstance.GetEffectCardStats().Score;
            }
        });

    }

    public override void Clear(CallerArgs callerArgs)
    {
        
    }
}
