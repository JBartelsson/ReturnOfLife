using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

public class CardInstance
{
    private CardData cardData;
    private List<WisdomType> fertilizers = new();
    private CardFunctionBase cardFunction;
    private CardEditorBase cardEditor;
    private CardAccessCheckBase cardAccessCheck;
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
            cardEditor = (CardEditorBase)Activator.CreateInstance(cardData.CardEditor.ScriptType);
            cardEditor.ExecutionType = cardData.CardEditor.ExecutionType;
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

    public CardEditorBase CardEditor
    {
        get => cardEditor;
        set => cardEditor = value;
    }

    public CardAccessCheckBase CardAccessCheck
    {
        get => cardAccessCheck;
        set => cardAccessCheck = value;
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
        //If either one of these conditions is true, then the card can be played
        bool gridTileAcessible = false;
        bool neighborNeededAndNeighborThere = false;
        bool cardCanExecuteOverride = cardFunction.CanExecute(callerArgs);
        gridTileAcessible = gridTile.IsAccessible(callerArgs);
        if (!callerArgs.needNeighbor)
        {
            neighborNeededAndNeighborThere = true;
        }
        else
        {
            neighborNeededAndNeighborThere = gridTile.HasNeighboredPlant();
        }
        
        Debug.Log($"Grid tile accessible: {gridTileAcessible}");
        Debug.Log($"Neighborneeded and Neighbor there: {neighborNeededAndNeighborThere}");
        Debug.Log($"cardCanExecuteOverride: {cardCanExecuteOverride}");

        return (cardCanExecuteOverride || gridTileAcessible) && neighborNeededAndNeighborThere;
    }

    public bool CanBePlayedWith(CallerArgs callerArgs)
    {
        return cardAccessCheck.CanBePlayedWith(callerArgs);
    }

    public bool CheckField(EditorCallerArgs editorCallerArgs)
    {
        return cardEditor.CheckField(editorCallerArgs);
    }

    public void ExecuteEditor(EditorCallerArgs editorCallerArgs)
    {
        cardEditor.ExecuteEditor(editorCallerArgs);
    }

    public EXECUTION_TYPE GetPlantFunctionExecuteType()
    {
        if (cardFunction == null) return EXECUTION_TYPE.NONE;
        return cardFunction.ExecutionType;
    }
}