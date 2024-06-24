using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CanBeTargetedByEditorAndEffects : CardAccessCheckBase
{
    public override bool IsAccessible(CallerArgs callerArgs)
    {
        //Can be targeted by effects and editors
        if (callerArgs.callerType == CALLER_TYPE.EFFECT || callerArgs.callerType == CALLER_TYPE.EDITOR) return true;

        return false;
    }

}
