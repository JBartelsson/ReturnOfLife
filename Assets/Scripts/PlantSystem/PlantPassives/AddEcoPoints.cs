using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEcoPoints : PlantPassiveBase
{
    public override bool ExecutePassive(CallerArgs callerArgs)
    {
        if (callerArgs.callerType != CALLER_TYPE.PASSIVE) return false;
        if (!callerArgs.callingPlantInstance.IsBasicFertilized())
        {
            callerArgs.gameManager.AddPointScore(callerArgs.callingPlantInstance.Plantable.regularPoints);
        }
        else
        {
            callerArgs.gameManager.AddPointScore(callerArgs.callingPlantInstance.Plantable.fertilizedPoints);
        }

        return true;
    }
}