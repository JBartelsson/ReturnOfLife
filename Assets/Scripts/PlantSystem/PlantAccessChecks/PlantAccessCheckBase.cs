using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PlantAccessCheckBase : PlantScriptBase
{
    public abstract bool IsAccessible(CallerArgs callerArgs);
}
