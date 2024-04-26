using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CanBeTargetedByEditorAndEffects : PlantAccessCheckBase
{
    public override bool IsAccessible(CallerArgs callerArgs)
    {
        Debug.Log("CAN BE TARGETED BY OTHER EFFECTS ASKED: ");
        Debug.Log(callerArgs.callerType);
        //Can be targeted by effects and editors
        if (callerArgs.callerType == CALLER_TYPE.EFFECT || callerArgs.callerType == CALLER_TYPE.EDITOR) return true;

        return false;
    }

}
