using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public abstract class CardSecondMoveBase : PlantScriptBase
{

    public CardSecondMoveBase()
    {
        if (ExecutionType == EXECUTION_TYPE.AFTER_PLACEMENT)
        {
            ExecutionType = EXECUTION_TYPE.IMMEDIATE;
        }
    }

    public abstract bool CheckField(SecondMoveCallerArgs callerArgs);


    public abstract void ExecuteSecondMove(SecondMoveCallerArgs callerArgs);
   
}
