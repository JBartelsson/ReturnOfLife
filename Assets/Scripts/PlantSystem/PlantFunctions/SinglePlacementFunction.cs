using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlacementFunction : CardFunctionBase
{
    public override void ExecuteCard(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        Debug.Log($"GETTIJNG CALLED FOR TILE {callerArgs.playedTile} AND SETTING {callerArgs.CallingCardInstance}");
        gridTile.AddObject(callerArgs);
        int i = 0;
        GridManager.Instance.Grid.ForEveryLifeformInCluster(callerArgs.CallingCardInstance.CardData.LifeformType, callerArgs.playedTile, tile => i++ );
        Debug.Log($"There are {i} Instances of {callerArgs.CallingCardInstance.CardData.LifeformType} in a Cluster");

    }

    public override void Clear(CallerArgs callerArgs)
    {
        
    }
}
