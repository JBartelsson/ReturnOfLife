using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CardFunctionBase : PlantScriptBase
{
    public abstract void ExecuteCard(CallerArgs callerArgs);

    public void Execute(CallerArgs callerArgs)
    {
        Debug.Log($"EXECUTING PLANT FUNCTION");
        ExecuteCard(callerArgs);
        //Reward points if no override is defined
        CardInstance cardInstance = callerArgs.CallingCardInstance;
        if (!cardInstance.CardData.OverridePointFunction)
        {
            cardInstance.CardData.RuntimePoints = cardInstance.GetCardStats().Points;
        }

        //Only give points for planting a plant, when it is the first on its tile
        Debug.Log(callerArgs);
        if (callerArgs.playedTile == null) return;
        if (cardInstance.CardData.RuntimePoints != 0 || cardInstance.CardData.OverridePointFunction)
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

        callerArgs.gameManager.AddPointScore(Mathf.FloorToInt(basePoints), callerArgs,
            GameManager.SCORING_ORIGIN.LIFEFORM);
        foreach (var modifier in callerArgs.playedTile.FieldModifiers)
        {
            switch (modifier.modifierType)
            {
                case FieldModifier.ModifierType.Multiplier:
                    // callerArgs.gameManager.AddPointScore(Mathf.FloorToInt(basePoints * Constants.MULTIPLICATION_FIELD_MULTIPLIER), callerArgs, GameManager.SCORING_ORIGIN.MULTIPLICATION);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}