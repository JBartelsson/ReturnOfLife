using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function_TheNeighbor : CardFunctionBase
{
    public override void ExecuteCard(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        gridTile.AddObject(callerArgs);
        int i = 0;
        Debug.Log($"Runtime Score Prior is: " + callerArgs.CallingCardInstance.CardData.RuntimeScore);
        GridManager.Instance.Grid.ForEveryLifeformInCluster(callerArgs.CallingCardInstance.CardData.LifeformType, callerArgs.playedTile, tile =>
        {
            i++;
            callerArgs.playedTile = tile;
            callerArgs.gameManager.AddPointScore(callerArgs.CallingCardInstance.GetCardStats().Score, callerArgs,
                GameManager.SCORING_ORIGIN.LIFEFORM);
        });
        Debug.Log($"There are {i} Instances of {callerArgs.CallingCardInstance.CardData.LifeformType} in a Cluster");

    }

    public override void Clear(CallerArgs callerArgs)
    {
        
    }
}
