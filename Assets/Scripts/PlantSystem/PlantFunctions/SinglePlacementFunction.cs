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

    }

    public override void Clear(CallerArgs callerArgs)
    {
        
    }
}
