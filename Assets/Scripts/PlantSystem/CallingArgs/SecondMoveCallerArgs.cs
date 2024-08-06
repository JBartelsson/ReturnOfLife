using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondMoveCallerArgs : CallerArgs
{
    //Used for editor access checks, so playedTile and selectedTile is different
    public GridTile selectedGridTile;
    public CardInstance EditorCallingCardInstance;
    public int SecondMoveNumber;

    public SecondMoveCallerArgs()
    {
        if (callerType == CALLER_TYPE.NONE)
        {
            callerType = CALLER_TYPE.SECOND_MOVE;
        }
    }

    public override string ToString()
    {
        return base.ToString() + $", played on: {selectedGridTile} from CardInstance: {EditorCallingCardInstance}";
    }

    public void SetValues(CardInstance newCallingPlantable, CardInstance newEditorCallingCardInstance, GridTile newPlayedTile, GridTile newSelectedTile, bool newNeedNeighbor, CALLER_TYPE newCallerType)
    {
        SetValues(newCallingPlantable, newPlayedTile, newNeedNeighbor, newCallerType);
        selectedGridTile = newSelectedTile;
        EditorCallingCardInstance = newEditorCallingCardInstance;
    }
} 
