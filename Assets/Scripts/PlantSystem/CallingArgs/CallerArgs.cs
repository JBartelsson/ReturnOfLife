using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallerArgs
{
    public PlantInstance callingPlantInstance;
    public GridTile playedTile;
    public bool needNeighbor = false;
    public CALLER_TYPE callerType = CALLER_TYPE.NONE;
    public GameManager gameManager;

    public CallerArgs()
    {

    }
    public CallerArgs(PlantInstance newCallingPlantable, GridTile newPlayedTile, bool newNeedNeighbor, CALLER_TYPE newCallerType)
    {
        SetValues(newCallingPlantable, newPlayedTile, newNeedNeighbor, newCallerType);
    }
    public void SetValues(PlantInstance newCallingPlantable, GridTile newPlayedTile, bool newNeedNeighbor, CALLER_TYPE newCallerType)
    {
        callingPlantInstance = newCallingPlantable;
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
    PLACEMENT,
    PASSIVE
}
