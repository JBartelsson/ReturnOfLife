using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherBushPlantFunction : PlantFunction
{

    public override bool Execute(Plantable caller, GridTile gridTile)
    {
        base.Execute(caller, gridTile);
        gridTile.BottomNeighbor?.AddPlantable(caller);
        gridTile.TopNeighbor?.AddPlantable(caller);
        gridTile.LeftNeighbor?.AddPlantable(caller);
        gridTile.RightNeighbor?.AddPlantable(caller);
        return true;
    }
}
