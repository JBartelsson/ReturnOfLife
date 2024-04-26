using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCallerArgs : CallerArgs
{
    public GridTile selectedGridTile;

    public EditorCallerArgs()
    {
        if (callerType == CALLER_TYPE.NONE)
        {
            callerType = CALLER_TYPE.EDITOR;
        }
    }

    public void SetValues(Plantable newCallingPlantable, GridTile newPlayedTile, GridTile newSelectedTile, bool newNeedNeighbor, CALLER_TYPE newCallerType)
    {
        SetValues(newCallingPlantable, newPlayedTile, newNeedNeighbor, newCallerType);
        selectedGridTile = newSelectedTile;
    }
} 
