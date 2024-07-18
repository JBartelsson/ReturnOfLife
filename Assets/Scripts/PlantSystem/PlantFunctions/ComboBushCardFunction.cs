using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComboBushCardFunction : CardFunctionBase
{
    [NonSerialized] private CardInstance bushInstance;
    [NonSerialized] private bool alreadyTriggered = false;

    public ComboBushCardFunction()
    {
        alreadyTriggered = false;
    }

    public bool AlreadyTriggered
    {
        get => alreadyTriggered;
        set => alreadyTriggered = value;
    }

    public override void ExecuteCard(CallerArgs callerArgs)
    {
        bushInstance = callerArgs.CallingCardInstance;
        callerArgs.playedTile.AddObject(callerArgs);
        callerArgs.playedTile.OnContentUpdated += PlayedTile_OnContentUpdated;
        EventManager.Game.Level.OnInCardSelection += OnCardPlayingEnded;
    }

    private void OnCardPlayingEnded(EventManager.GameEvents.Args arg0)
    {
        alreadyTriggered = false;
    }

    private void PlayedTile_OnContentUpdated(object sender, CardInstance cardInstance)
    {
        GridTile callingGridTile = sender as GridTile;
        if (callingGridTile == null) return;
        // if (alreadyTriggered) return;
        alreadyTriggered = true;
        CallerArgs bushCallerArgs = new CallerArgs(new CardInstance(bushInstance), null, false, CALLER_TYPE.EFFECT);
        bushCallerArgs.gameManager = GameManager.Instance;
        //Select Effect Pattern depending on Upgrade
        PatternSO effectPattern = null;

        callingGridTile.ForPattern(bushInstance.GetCardStats().EffectPattern, gridTile =>
        {
            bushCallerArgs.playedTile = gridTile;
            bool isComboBush = false;
            if (gridTile.CardInstance != null)
            {
                isComboBush = bushCallerArgs.CallingCardInstance.CardData.CardName ==
                         gridTile.CardInstance.CardData.CardName;
            }
            if (gridTile.IsAccessible(bushCallerArgs, true) && !isComboBush)
            {
                bushInstance.Execute(bushCallerArgs);
            }
        });
    }
}