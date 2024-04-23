using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PlantFunction
{
    public virtual bool Execute(Plantable caller, GridTile gridTile)
    {
        gridTile.AddPlantable(caller);
        return true;
    }
}
