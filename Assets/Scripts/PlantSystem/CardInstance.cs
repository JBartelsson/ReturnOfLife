using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardInstance : ICloneable
{
    private CardData cardData;
    private List<WisdomType> fertilizers = new();
    private CardFunctionBase cardFunction;
    private CardSecondMoveBase cardSecondMove;
    private CardAccessCheckBase cardAccessCheck;
    private CardCanExecuteCheckBase cardCanExecuteCheckBase;
    private CardStatusEnum cardStatus = CardStatusEnum.Alive;

   


    public enum CardStatusEnum
    {
        Alive, Dead, Exhausted
    }
    
    private int cardID = 0;

    public int CardID => cardID;

    private static int cardInstanceID = 0;

    public CardInstance(CardData newCardData, List<WisdomType> newFertilizers = null)
    {
        cardData = newCardData.Copy();
        TryAddFertilizer(newFertilizers);
        InitScripts();
    }

    public CardInstance(CardInstance other, List<WisdomType> newFertilizers = null)
    {
        cardData = other.cardData.Copy();
        TryAddFertilizer(newFertilizers);
        InitScripts();
    }

    public CardInstance(CardInstance other)
    {
        cardData = other.cardData.Copy();
        fertilizers.AddRange(other.fertilizers);
        InitScripts();
    }

    private void InitScripts()
    {
        cardID = cardInstanceID;
        cardInstanceID++;
        if (cardData.CardFunction.ScriptType.Type != null)
        {
            cardFunction = (CardFunctionBase)Activator.CreateInstance(cardData.CardFunction.ScriptType);
            cardFunction.ExecutionType = cardData.CardFunction.ExecutionType;
        }
        else
        {
            Debug.LogWarning($"{this} tried to create a plantFunction from Default");
        }

        if (cardData.CardEditor.ScriptType.Type != null)
        {
            cardSecondMove = (CardSecondMoveBase)Activator.CreateInstance(cardData.CardEditor.ScriptType);
            cardSecondMove.ExecutionType = cardData.CardEditor.ExecutionType;
        }
        else
        {
            Debug.LogWarning($"{this} tried to create a plantEditor from Default");
        }

        if (cardData.CardAccessCheck.ScriptType.Type != null)
        {
            cardAccessCheck = (CardAccessCheckBase)Activator.CreateInstance(cardData.CardAccessCheck.ScriptType);
            cardAccessCheck.ExecutionType = cardData.CardAccessCheck.ExecutionType;
        }
        else
        {
            Debug.LogWarning($"{this} tried to create a plantAccessCheck from Default");
        }
        if (cardData.CardCanExecuteCheck.ScriptType.Type != null)
        {
            cardCanExecuteCheckBase = (CardCanExecuteCheckBase)Activator.CreateInstance(cardData.CardCanExecuteCheck.ScriptType);
            cardCanExecuteCheckBase.ExecutionType = cardData.CardAccessCheck.ExecutionType;
        }
    }

    public override string ToString()
    {
        string fertilizerString = "";
        foreach (var item in this.fertilizers)
        {
            fertilizerString += item.ToString();
        }

        return $"{cardData} played with fertilizers: {fertilizerString}";
    }

    public object Clone()
    {
        return null;
    }


    private void TryAddFertilizer(List<WisdomType> newFertilizers)
    {
        if (newFertilizers == null) return;
        fertilizers.AddRange(newFertilizers);
    }

    public CardData CardData
    {
        get => cardData;
        set => cardData = value;
    }

    public List<WisdomType> Fertilizers
    {
        get => fertilizers;
        set => fertilizers = value;
    }

    public CardFunctionBase CardFunction
    {
        get => cardFunction;
        set => cardFunction = value;
    }

    public CardSecondMoveBase CardSecondMove
    {
        get => cardSecondMove;
        set => cardSecondMove = value;
    }

    public CardAccessCheckBase CardAccessCheck
    {
        get => cardAccessCheck;
        set => cardAccessCheck = value;
    }
    
    public CardCanExecuteCheckBase CardCanExecuteCheckBase
    {
        get => cardCanExecuteCheckBase;
        set => cardCanExecuteCheckBase = value;
    }
    
    public CardStatusEnum CardStatus
    {
        get => cardStatus;
    }

    public bool IsUpgraded()
    {
        return fertilizers.Contains(WisdomType.Basic);
    }

    public int ReturnTriggerAmount()
    {
        return fertilizers.Where((x) => x == WisdomType.Retrigger).Count();
    }

    public CardData.CardStats GetCardStats()
    {
        if (!IsUpgraded())
        {
            return cardData.RegularCardStats;
        }
        else
        {
            return cardData.UpgradedCardStats;
        }
    }

    public void Execute(CallerArgs callerArgs)
    {
        if (CanExecute(callerArgs))
            cardFunction.Execute(callerArgs);
    }

    public bool CanExecute(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        bool accessCheck = gridTile.IsAccessible(callerArgs);
        bool canExecuteCheck = callerArgs.CallingCardInstance.CanExecuteWith(callerArgs);

        return accessCheck || canExecuteCheck;

    }

    public bool CanExecuteWith(CallerArgs callerArgs)
    {
        return cardCanExecuteCheckBase.CanExecuteWith(callerArgs);
    }

    public bool CanBeBePlantedOn(CallerArgs callerArgs)
    {
        return cardAccessCheck.CanBeBePlantedOn(callerArgs);
    }

    public bool CheckField(SecondMoveCallerArgs secondMoveCallerArgs)
    {
        return cardSecondMove.CheckField(secondMoveCallerArgs);
    }

    public void ExecuteSecondMove(SecondMoveCallerArgs secondMoveCallerArgs)
    {
        cardSecondMove.ExecuteEditor(secondMoveCallerArgs);
    }

    public EXECUTION_TYPE GetPlantFunctionExecuteType()
    {
        if (cardFunction == null) return EXECUTION_TYPE.NONE;
        return cardFunction.ExecutionType;
    }

    public void KillLifeform(CallerArgs callerArgs)
    {
        cardFunction.Clear(callerArgs);
        cardStatus = CardStatusEnum.Dead;
        CardFunctionBase.RewardPoints(callerArgs, -1 * CardData.RuntimePoints);
    }

    public void TryReviveLifeform(CallerArgs callerArgs)
    {
        if (!IsDead()) return;
        cardStatus = CardStatusEnum.Alive;
        CallerArgs reviveCallerArgs = new CallerArgs()
        {
            CallingCardInstance = this,
            playedTile = callerArgs.playedTile,
            gameManager = GameManager.Instance,
            callerType = CALLER_TYPE.REVIVE
        };
       
        GameManager.Instance.TryQueueLifeform(reviveCallerArgs);
    }

    public bool IsDead()
    {
        return cardStatus == CardStatusEnum.Dead;
    }
    
    
}