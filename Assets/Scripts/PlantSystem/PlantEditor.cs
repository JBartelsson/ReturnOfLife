using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class PlantEditor
{

    public virtual bool CheckField(GridTile gridTile, GridTile caller)
    {
        return true;
    }

    public virtual void ExecuteEditor(Plantable caller, GridTile playedTile, GridTile selectedGridTile)
    {
        Debug.LogWarning("Warning: normal Plant Editor function called!");
    }
}
