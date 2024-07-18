using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComboBushAccessCheck : CardAccessCheckBase
{
    public override bool CanBeBePlantedOn(CallerArgs callerArgs)
    {
        if (callerArgs.CallingCardInstance.IsDead()) return false;
        return true;
    }

}
