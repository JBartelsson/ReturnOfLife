using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CanBeOnlyPlacedOnAlivePlants : CardCanExecuteCheckBase
{
    public override bool CanExecuteWith(CallerArgs callerArgs)
    {
        if (callerArgs.playedTile.ContainsLivingPlant())
        {
            if (!callerArgs.playedTile.CardInstance.IsDead())
            {
                return true;
            }
        }
        return false;
    }
}