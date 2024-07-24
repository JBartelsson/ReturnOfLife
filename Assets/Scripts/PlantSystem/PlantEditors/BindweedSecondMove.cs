using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BindweedSecondMove : CardSecondMoveBase
{
    private const int BINDWEED_RANGE = 3;

    public override bool CheckField(SecondMoveCallerArgs callerArgs)
    {
        GridTile selectedGridTile = callerArgs.selectedGridTile;
        GridTile caller = callerArgs.playedTile;
        //If not on same axis, return
        if (!callerArgs.EditorCallingCardInstance.GetCardStats().EffectPattern.IsTileInPattern(
                selectedGridTile, caller))
        {
            return false;
        }

        //Check if the selected field can be played with the instance (and copy the caller args because otherwise it writes into the variable)
        CallerArgs selectedFieldCallerArgs = callerArgs.ReturnShallowCopy();
        selectedFieldCallerArgs.playedTile = selectedGridTile;
        selectedFieldCallerArgs.needNeighbor = false;
        if (!callerArgs.EditorCallingCardInstance.CanExecute(selectedFieldCallerArgs)) return false;
        return true;
    }


    public override void ExecuteEditor(SecondMoveCallerArgs callerArgs)
    {
        Debug.Log($"EXECUTING EDITOR FUNCTION");
        callerArgs.playedTile = callerArgs.selectedGridTile;
        callerArgs.EditorCallingCardInstance.CardFunction.ExecutionType = EXECUTION_TYPE.IMMEDIATE;
        CardInstance newBindweedInstance = new CardInstance(callerArgs.EditorCallingCardInstance);
        callerArgs.BlockSecondMove = true;
        callerArgs.CallingCardInstance = newBindweedInstance;
        GameManager.Instance.TryQueueLifeform(callerArgs);
    }
}