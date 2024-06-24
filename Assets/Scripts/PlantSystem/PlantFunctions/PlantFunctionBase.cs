using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PlantFunctionBase : PlantScriptBase
{
    public abstract void ExecuteCard(CallerArgs callerArgs);
    public abstract bool CanExecute(CallerArgs callerArgs);

    public void Execute(CallerArgs callerArgs)
    {
        ExecuteCard(callerArgs);
        //Reward points if no override is defined
        PlantInstance plantInstance = callerArgs.callingPlantInstance;
        if (!callerArgs.callingPlantInstance.Plantable.OverridePointFunction)
        {
            if (callerArgs.callingPlantInstance.IsBasicFertilized())
            {
                plantInstance.Plantable.RuntimePoints = plantInstance.Plantable.fertilizedPoints;
            }
            else
            {
                plantInstance.Plantable.RuntimePoints = plantInstance.Plantable.regularPoints;
            }
        }

        RewardPoints(callerArgs, plantInstance.Plantable.RuntimePoints);
    }

    public static void RewardPoints(CallerArgs callerArgs, int rewardedPoints)
    {
        float basePoints = rewardedPoints;
        if (callerArgs.playedTile == null)
        {
            Debug.Log("PLAYED TILE IS NULL!!!");
            return;
        }
        foreach (var modifier in callerArgs.playedTile.FieldModifiers)
        {
            switch (modifier.modifierType)
            {
                case FieldModifier.ModifierType.Multiplier:
                    basePoints *= modifier.modifierAmount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        callerArgs.gameManager.AddPointScore(Mathf.FloorToInt(basePoints));
    }
}