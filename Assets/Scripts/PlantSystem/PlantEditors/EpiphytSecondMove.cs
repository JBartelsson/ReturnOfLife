using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EpiphytSecondMove : CardSecondMoveBase
{
    public override bool CheckField(SecondMoveCallerArgs callerArgs)
    {
        GridTile selectedGridTile = callerArgs.selectedGridTile;
        GridTile caller = callerArgs.playedTile;
        //If not on same axis, return
        if (!callerArgs.EditorCallingCardInstance.GetCardStats().EffectPattern.IsTileInPattern(
                caller, selectedGridTile))
        {
            return false;
        }

        //Check if the selected field can be played with the instance (and copy the caller args because otherwise it writes into the variable)
        if (!selectedGridTile.ContainsLivingPlant()) return false;
        return true;
    }


    public override void ExecuteSecondMove(SecondMoveCallerArgs callerArgs)
    {
        GameManager.Instance.AddMana(callerArgs.selectedGridTile.CardInstance.GetCardStats().PlayCost);
        callerArgs.selectedGridTile.KillObject(callerArgs);
    }
}