using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReanimateFunction : CardFunctionBase
{
    public override void ExecuteCard(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        gridTile.ForPattern(callerArgs.CallingCardInstance.GetCardStats().EffectPattern, tile =>
        {
            tile.TryReviveLifeform(callerArgs);
        } );
        gridTile.AddObject(callerArgs);
    }

    public override void Clear(CallerArgs callerArgs)
    {
        
    }
}
