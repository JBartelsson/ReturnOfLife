using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NoOtherCardsPlacable : CardAccessCheckBase
{
    public override bool IsAccessible(CallerArgs callerArgs)
    {
        return false;
    }

}
