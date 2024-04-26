using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PlantFunctionBase
{
    public abstract bool Execute(CallerArgs callerArgs);
}
