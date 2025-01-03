using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardInstance : ICloneable
{
    private CardData cardData;
    private List<CardInstance> upgrades = new();
    private CardFunctionBase cardFunction;
    private CardSecondMoveBase cardSecondMove;
    private CardAccessCheckBase cardAccessCheck;
    private CardCanExecuteCheckBase cardCanExecuteCheckBase;
    private CardStatusEnum cardStatus = CardStatusEnum.Alive;


    public enum CardStatusEnum
    {
        Alive,
        Dead,
        Exhausted
    }

    private int cardID = 0;

    public int CardID => cardID;

    private static int cardInstanceID = 0;

    public CardInstance(CardData newCardData, List<CardInstance> newFertilizers = null)
    {
        cardData = newCardData.Copy();
        TryAddFertilizer(newFertilizers);
        InitScripts();
    }

    public CardInstance(CardInstance other, List<CardInstance> newFertilizers = null)
    {
        cardData = other.cardData.Copy();
        TryAddFertilizer(newFertilizers);
        InitScripts();
    }

    public CardInstance(CardInstance other)
    {
        cardData = other.cardData.Copy();
        upgrades.AddRange(other.upgrades);
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
            cardCanExecuteCheckBase =
                (CardCanExecuteCheckBase)Activator.CreateInstance(cardData.CardCanExecuteCheck.ScriptType);
            cardCanExecuteCheckBase.ExecutionType = cardData.CardAccessCheck.ExecutionType;
        }
    }

    public override string ToString()
    {
        string fertilizerString = "";
        foreach (var item in this.upgrades)
        {
            fertilizerString += item.ToString();
        }

        return $"{cardData} played with fertilizers: {fertilizerString}";
    }

    public object Clone()
    {
        return null;
    }


    private void TryAddFertilizer(List<CardInstance> newFertilizers)
    {
        if (newFertilizers == null) return;
        upgrades.AddRange(newFertilizers);
    }

    public CardData CardData
    {
        get => cardData;
        set => cardData = value;
    }

    public List<CardInstance> Upgrades
    {
        get => upgrades;
        set => upgrades = value;
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
        return upgrades.Any((x) => x.cardData.WisdomType == WisdomType.Basic);
    }

    public int ReturnTriggerAmount()
    {
        return upgrades.Count(x => x.cardData.WisdomType == WisdomType.Retrigger);
    }

    public CardData.CardStats GetCardStats()
    {
            return cardData.RegularCardStats;
    }
    
    public Score GetTotalCardScore()
    {
        return cardData.RegularCardStats.Score + cardData.RuntimeScore;
    }

    public CardData.CardStats GetEffectCardStats()
    {
        return cardData.EffectCardStats;
    }
    public void Execute(CallerArgs callerArgs)
    {
        callerArgs.CallingCardInstance = this;
        if (CanExecute(callerArgs))
            cardFunction.Execute(callerArgs);
    }

    public bool CanExecute(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        bool accessCheck = gridTile.IsAccessible(callerArgs);
        bool canExecuteCheck = callerArgs.CallingCardInstance.CanExecuteWith(callerArgs);
        bool revive = callerArgs.callerType == CALLER_TYPE.REVIVE;
        
        return accessCheck || canExecuteCheck || revive;
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
        secondMoveCallerArgs.EditorCallingCardInstance = this;
        cardSecondMove.ExecuteSecondMove(secondMoveCallerArgs);
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
        EventManager.Game.Level.OnLifeformRevived?.Invoke(callerArgs);
        // CardFunctionBase.RewardPoints(callerArgs, -1 * CardData.RuntimeScore);
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
            callerType = CALLER_TYPE.REVIVE, 
            needNeighbor = false
        };
        Debug.Log($"TRYING TO REVIVE LIFEFORM");
        EventManager.Game.Level.OnLifeformRevived?.Invoke(callerArgs);

        GameManager.Instance.TryQueueLifeform(reviveCallerArgs);
    }

    public int GetPlayCost()
    {
        int playcost = GetCardStats().PlayCost;
        foreach (var upgrade in upgrades)
        {
            playcost += upgrade.GetCardStats().PlayCost;

        }

        return playcost;
    }

    public bool IsDead()
    {
        return cardStatus == CardStatusEnum.Dead;
    }
}