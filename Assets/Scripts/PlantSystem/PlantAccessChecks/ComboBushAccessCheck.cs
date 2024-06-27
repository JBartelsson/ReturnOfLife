using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComboBushAccessCheck : CardAccessCheckBase
{
    public override bool CanBePlayedWith(CallerArgs callerArgs)
    {
        //Can be targeted by effects and editors
        Debug.Log($"CHECKING ACCESS FOR {callerArgs.playedTile} with {callerArgs.CallingCardInstance} as {callerArgs.callerType}");
        if (callerArgs.playedTile.Content.Count == 0) return true;
        Debug.Log($"{callerArgs.CallingCardInstance}");
        Debug.Log($"{callerArgs.CallingCardInstance.CardData}");
        Debug.Log($"{callerArgs.CallingCardInstance.CardData.CardName}");
        if (callerArgs.CallingCardInstance.CardData.CardName ==
            callerArgs.playedTile.Content[0].CardData.CardName) return false;
        if (callerArgs.callerType == CALLER_TYPE.EFFECT || callerArgs.callerType == CALLER_TYPE.EDITOR) return true;

        return false;
    }

}
