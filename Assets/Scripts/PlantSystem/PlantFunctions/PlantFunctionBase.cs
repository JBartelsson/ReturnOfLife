using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PlantFunctionBase : PlantScriptBase
{
    public abstract bool Execute(CallerArgs callerArgs);
}
