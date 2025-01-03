using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function_Triolyt : CardFunctionBase
{
    public override void ExecuteCard(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        gridTile.AddObject(callerArgs);
        int trioAmount = 0;
        GridManager.Instance.Grid.ForEveryLifeformInCluster(callerArgs.CallingCardInstance.CardData.LifeformType, callerArgs.playedTile, tile =>
        {
            trioAmount++;
        });
        if (trioAmount > 2)
        {
            callerArgs.CallingCardInstance.CardData.RuntimeScore += callerArgs.CallingCardInstance.GetEffectCardStats().Score;
        }

    }

    public override void Clear(CallerArgs callerArgs)
    {
        
    }
}
