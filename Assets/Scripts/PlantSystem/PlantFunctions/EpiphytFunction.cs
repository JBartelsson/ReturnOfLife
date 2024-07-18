using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpiphytFunction : CardFunctionBase
{


    public override void ExecuteCard(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        Debug.Log($"GETTIJNG CALLED FOR TILE {callerArgs.playedTile} AND SETTING {callerArgs.CallingCardInstance}");
        
        GameManager.Instance.AddMana(gridTile.CardInstance.GetCardStats().PlayCost);
        gridTile.KillObject(callerArgs);

    }

    public override void Clear(CallerArgs callerArgs)
    {
        
    }
}
