using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComboBushAccessCheck : CardAccessCheckBase
{
    public override bool CanBeBePlantedOn(CallerArgs callerArgs)
    {
        if (callerArgs.playedTile.CardInstance == null) return true;
        if (callerArgs.playedTile.CardInstance.IsDead()) return false;
        return true;
    }

}
