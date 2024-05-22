using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PlantFunctionBase : PlantScriptBase
{
    public abstract void Execute(CallerArgs callerArgs);
    public abstract bool CanExecute(CallerArgs callerArgs);
}
