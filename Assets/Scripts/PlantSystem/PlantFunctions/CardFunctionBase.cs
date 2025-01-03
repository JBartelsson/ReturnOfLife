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
        bool alreadyContainedPlant = callerArgs.playedTile.ContainsAnyPlant();
        Debug.Log("Before EXECUTING:" + callerArgs.playedTile);
        ExecuteCard(callerArgs);
        //Reward points if no override is defined
        CardInstance cardInstance = callerArgs.CallingCardInstance;

        Debug.Log(cardInstance.CardData.RuntimeScore);

        //Only give points for planting a plant, when it is the first on its tile
        if (alreadyContainedPlant) return;
        if (callerArgs.playedTile == null) return;
        Debug.Log("Before ADDING:" + callerArgs.playedTile);
        // if (cardInstance.CardData.RuntimeScore != 0 || cardInstance.CardData.OverridePointFunction)
        callerArgs.gameManager.AddPointScore(callerArgs.CallingCardInstance.GetTotalCardScore(), callerArgs,
            GameManager.SCORING_ORIGIN.LIFEFORM);    
    }

    public static void RewardPoints(CallerArgs callerArgs, int rewardedPoints)
    {
        float basePoints = rewardedPoints;
        if (callerArgs.playedTile == null)
        {
            Debug.Log("PLAYED TILE IS NULL!!!");
            return;
        }

        // callerArgs.gameManager.AddPointScore(Mathf.FloorToInt(basePoints), callerArgs,
            // GameManager.SCORING_ORIGIN.LIFEFORM);
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

    public abstract void Clear(CallerArgs callerArgs);
}