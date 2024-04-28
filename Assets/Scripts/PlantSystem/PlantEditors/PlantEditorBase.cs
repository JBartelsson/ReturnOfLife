using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public abstract class PlantEditorBase : PlantScriptBase
{

    public PlantEditorBase()
    {
        if (ExecutionType == EXECUTION_TYPE.AFTER_PLACEMENT)
        {
            ExecutionType = EXECUTION_TYPE.IMMEDIATE;
        }
    }

    public abstract bool CheckField(EditorCallerArgs callerArgs);


    public abstract void ExecuteEditor(EditorCallerArgs callerArgs);
   
}
