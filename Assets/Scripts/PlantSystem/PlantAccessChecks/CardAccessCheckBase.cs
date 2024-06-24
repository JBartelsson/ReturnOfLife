using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CardAccessCheckBase : PlantScriptBase
{
    public abstract bool IsAccessible(CallerArgs callerArgs);
}
