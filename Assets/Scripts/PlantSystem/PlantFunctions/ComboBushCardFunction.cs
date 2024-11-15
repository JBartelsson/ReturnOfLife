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
        Debug.Log($"{GetHashCode()}  subscribed");
        EventManager.Game.Level.OnInCardSelection += OnCardPlayingEnded;
    }

    public override void Clear(CallerArgs callerArgs)
    {
        Debug.Log($"Trying to Clear Combo Bush on {callerArgs.playedTile}");
        Debug.Log($"{GetHashCode()}  unsubscribed");
        callerArgs.playedTile.OnContentUpdated -= PlayedTile_OnContentUpdated;
    }

    private void OnCardPlayingEnded(EventManager.GameEvents.Args arg0)
    {
        alreadyTriggered = false;
    }

    private void PlayedTile_OnContentUpdated(GridTile.OnContentUpdatedArgs onContentUpdatedArgs)
    {
        GridTile callingGridTile = onContentUpdatedArgs.GridTile;
        if (callingGridTile == null) return;
        Debug.Log($"On Content Updated is called on {callingGridTile}");
        // if (alreadyTriggered) return;
        //Select Effect Pattern depending on Upgrade
        PatternSO effectPattern = null;

        bool effectUsed = false;
        callingGridTile.ForPattern(bushInstance.GetCardStats().EffectPattern, gridTile =>
        {
            if (gridTile.ContainsAnyPlant()) return;

            CardInstance newBushInstance = new CardInstance(bushInstance);
            CallerArgs bushCallerArgs = new CallerArgs(newBushInstance, null, false, CALLER_TYPE.EFFECT);
            bushCallerArgs.gameManager = GameManager.Instance;
            bushCallerArgs.playedTile = gridTile;
            effectUsed = true;
            GameManager.Instance.TryQueueLifeform(bushCallerArgs);
        });
        
        EventManager.Game.Level.OnEffectUsed?.Invoke(null);
    }
}