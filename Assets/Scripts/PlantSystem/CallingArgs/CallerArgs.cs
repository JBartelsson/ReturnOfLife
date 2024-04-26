using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallerArgs
{
    public Plantable callingPlantable;
    public GridTile playedTile;
    public bool needNeighbor = false;
    public CALLER_TYPE callerType = CALLER_TYPE.NONE;
} 
public enum CALLER_TYPE
{
    NONE,
    EDITOR,
    PLACEMENT
}
