using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function_Duolyt : CardFunctionBase
{
    public override void ExecuteCard(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        gridTile.AddObject(callerArgs);
        GridManager.Instance.Grid.ForEveryLifeformInCluster(callerArgs.CallingCardInstance.CardData.LifeformType, callerArgs.playedTile, tile =>
        {
            callerArgs.playedTile = tile;
            callerArgs.gameManager.AddPointScore(callerArgs.CallingCardInstance.GetEffectCardStats().Score, callerArgs,
                GameManager.SCORING_ORIGIN.LIFEFORM);
        });
        
    }

    public override void Clear(CallerArgs callerArgs)
    {
        
    }
}
