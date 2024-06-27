using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

public class CardInstance
{
    private CardData cardData;
    private List<Fertilizer> fertilizers = new();
    private CardFunctionBase cardFunction;
    private CardEditorBase cardEditor;
    private CardAccessCheckBase cardAccessCheck;

    public CardInstance(CardData newCardData, List<Fertilizer> newFertilizers = null)
    {
        cardData = newCardData.Copy();
        TryAddFertilizer(newFertilizers);
        InitScripts();
    }
    public CardInstance(CardInstance other, List<Fertilizer> newFertilizers = null)
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




    private void TryAddFertilizer(List<Fertilizer> newFertilizers)
    {
        if (newFertilizers == null) return;
        fertilizers.AddRange(newFertilizers);

    }

    public CardData CardData { get => cardData; set => cardData = value; }
    public List<Fertilizer> Fertilizers { get => fertilizers; set => fertilizers = value; }
    public CardFunctionBase CardFunction { get => cardFunction; set => cardFunction = value; }
    public CardEditorBase CardEditor { get => cardEditor; set => cardEditor = value; }
    public CardAccessCheckBase CardAccessCheck { get => cardAccessCheck; set => cardAccessCheck = value; }

    public bool IsBasicFertilized()
    {
        return fertilizers.Contains(Fertilizer.Basic);
    }
    public int ReturnTriggerAmount()
    {
        return fertilizers.Where((x) => x == Fertilizer.Retrigger).Count();
    }

    public void Execute(CallerArgs callerArgs)
    {
        if (CanExecute(callerArgs))
        cardFunction.Execute(callerArgs);
    }

    public bool CanExecute(CallerArgs callerArgs)
    {
        GridTile gridTile = callerArgs.playedTile;
        if (cardFunction.ExecutionType != EXECUTION_TYPE.IMMEDIATE)
        {
            if (!gridTile.IsAccessible(callerArgs)) return false;
            if (callerArgs.needNeighbor && !gridTile.HasNeighboredPlant()) return false;
        }
        return cardFunction.CanExecute(callerArgs);
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
