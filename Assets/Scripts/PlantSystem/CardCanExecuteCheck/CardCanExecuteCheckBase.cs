using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CardCanExecuteCheckBase : PlantScriptBase
{
    public abstract bool CanExecuteWith(CallerArgs callerArgs);
}