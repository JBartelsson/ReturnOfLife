using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BindweedEditor: PlantEditor
{
    private const int BINDWEED_RANGE = 3; 
    public override bool CheckField(GridTile gridTile, GridTile caller)
    {
        //If not on same axis, return
        if (!gridTile.OnSameAxisAs(caller)) return false;
        //if Distance is greater than 3 return
        if (gridTile.DistanceTo(caller) > BINDWEED_RANGE) return false;
        if (gridTile == caller) return false;
        //If tile already contains plant
        if (gridTile.ContainsPlant()) return false;
        return true;
    }

    public override void ExecuteEditor(Plantable caller, GridTile playedGridTile, GridTile selectedTile)
    {
        Debug.Log($"{selectedTile.X}, {selectedTile.Y}");
        selectedTile.AddPlantable(caller);
    }
}
