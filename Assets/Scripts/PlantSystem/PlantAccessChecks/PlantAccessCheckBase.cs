using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PlantAccessCheckBase
{
    public abstract bool Execute(CallerArgs plantableExecuteArgs);
}
