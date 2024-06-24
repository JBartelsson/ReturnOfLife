using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEcoPoints : CardPassiveBase
{
    public override bool ExecutePassive(CallerArgs callerArgs)
    {
        if (callerArgs.callerType != CALLER_TYPE.PASSIVE) return false;
        if (!callerArgs.CallingCardInstance.IsBasicFertilized())
        {
            callerArgs.gameManager.AddPointScore(callerArgs.CallingCardInstance.CardData.regularPoints);
        }
        else
        {
            callerArgs.gameManager.AddPointScore(callerArgs.CallingCardInstance.CardData.fertilizedPoints);
        }

        return true;
    }
}