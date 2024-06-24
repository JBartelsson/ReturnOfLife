using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallerArgs
{
    public CardInstance CallingCardInstance;
    public GridTile playedTile;
    public bool needNeighbor = false;
    public CALLER_TYPE callerType = CALLER_TYPE.NONE;
    public GameManager gameManager;

    public CallerArgs()
    {

    }
    public CallerArgs(CardInstance newCallingPlantable, GridTile newPlayedTile, bool newNeedNeighbor, CALLER_TYPE newCallerType)
    {
        SetValues(newCallingPlantable, newPlayedTile, newNeedNeighbor, newCallerType);
    }
    public void SetValues(CardInstance newCallingPlantable, GridTile newPlayedTile, bool newNeedNeighbor, CALLER_TYPE newCallerType)
    {
        CallingCardInstance = newCallingPlantable;
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
