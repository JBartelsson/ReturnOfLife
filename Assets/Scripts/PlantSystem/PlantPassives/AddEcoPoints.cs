using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEcoPoints : CardPassiveBase
{
    public override bool ExecutePassive(CallerArgs callerArgs)
    {
        if (callerArgs.callerType != CALLER_TYPE.PASSIVE) return false;
            // callerArgs.gameManager.AddPointScore(callerArgs.CallingCardInstance.GetCardStats().Points);

        return true;
    }
}