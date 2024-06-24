using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CardFunctionBase : PlantScriptBase
{
    public abstract void ExecuteCard(CallerArgs callerArgs);
    public abstract bool CanExecute(CallerArgs callerArgs);

    public void Execute(CallerArgs callerArgs)
    {
        ExecuteCard(callerArgs);
        //Reward points if no override is defined
        CardInstance cardInstance = callerArgs.CallingCardInstance;
        if (!callerArgs.CallingCardInstance.CardData.OverridePointFunction)
        {
            if (callerArgs.CallingCardInstance.IsBasicFertilized())
            {
                cardInstance.CardData.RuntimePoints = cardInstance.CardData.fertilizedPoints;
            }
            else
            {
                cardInstance.CardData.RuntimePoints = cardInstance.CardData.regularPoints;
            }
        }

        RewardPoints(callerArgs, cardInstance.CardData.RuntimePoints);
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