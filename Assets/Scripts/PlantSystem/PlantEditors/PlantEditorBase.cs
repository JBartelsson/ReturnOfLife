using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public abstract class PlantEditorBase
{

    public abstract bool CheckField(EditorCallerArgs callerArgs);


    public abstract void ExecuteEditor(EditorCallerArgs callerArgs);
   
}
