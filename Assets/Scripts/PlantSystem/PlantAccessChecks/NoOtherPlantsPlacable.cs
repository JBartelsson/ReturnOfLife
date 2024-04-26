using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NoOtherPlantsPlacable : PlantAccessCheckBase
{
    public override bool IsAccessible(CallerArgs callerArgs)
    {
        return false;
    }

}
