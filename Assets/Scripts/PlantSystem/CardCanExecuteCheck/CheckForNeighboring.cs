using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CheckForNeighboring : CardCanExecuteCheckBase
{
    public override bool CanExecuteWith(CallerArgs callerArgs)
    {
        if (callerArgs.playedTile.ContainsAnyPlant()) return false;
        if (callerArgs.needNeighbor && !callerArgs.playedTile.HasNeighboredPlant()) return false;

        return true;
    }
}