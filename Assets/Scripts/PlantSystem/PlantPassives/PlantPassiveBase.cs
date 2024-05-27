using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PlantPassiveBase : PlantScriptBase
{
    public abstract bool ExecutePassive(CallerArgs callerArgs);
}