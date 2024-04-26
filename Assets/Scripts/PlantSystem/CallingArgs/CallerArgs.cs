using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallerArgs
{
    public Plantable callingPlantable;
    public GridTile playedTile;
    public bool needNeighbor = false;
    public CALLER_TYPE callerType = CALLER_TYPE.NONE;

    public CallerArgs()
    {

    }
    public CallerArgs(Plantable newCallingPlantable, GridTile newPlayedTile, bool newNeedNeighbor, CALLER_TYPE newCallerType)
    {
        SetValues(newCallingPlantable, newPlayedTile, newNeedNeighbor, newCallerType);
    }
    public void SetValues(Plantable newCallingPlantable, GridTile newPlayedTile, bool newNeedNeighbor, CALLER_TYPE newCallerType)
    {
        callingPlantable = newCallingPlantable;
        playedTile = newPlayedTile;
        needNeighbor = newNeedNeighbor;
        callerType = newCallerType;
    }
} 
public enum CALLER_TYPE
{
    NONE,
    EDITOR,
    EFFECT,
    PLACEMENT
}
