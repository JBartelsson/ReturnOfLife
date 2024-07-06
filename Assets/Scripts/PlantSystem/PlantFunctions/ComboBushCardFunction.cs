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

    public bool AlreadyTriggered { get => alreadyTriggered; set => alreadyTriggered = value; }

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

    public override bool CanExecute(CallerArgs callerArgs)
    {
        return true;
    }

    private void PlayedTile_OnContentUpdated(object sender, CardInstance cardInstance)
    {
        GridTile callingGridTile = sender as GridTile;
        if (callingGridTile == null) return;
        if (cardInstance.CardData.CardName == bushInstance.CardData.CardName) return;
        Debug.Log($"TILE {callingGridTile.X}, {callingGridTile.Y} UPDATED, Executing Lycoperdon Function alreadyTriggered: {alreadyTriggered}");
        Debug.Log(this.GetHashCode());
        // if (alreadyTriggered) return;
        alreadyTriggered = true;
        CallerArgs bushCallerArgs = new CallerArgs(new CardInstance(bushInstance), null, false, CALLER_TYPE.EFFECT);
        bushCallerArgs.gameManager = GameManager.Instance;
        //Select Effect Pattern depending on Upgrade
        PatternSO effectPattern = null;
       
        callingGridTile.ForPattern(bushInstance.GetCardStats().EffectPattern, gridTile =>
        {
            bushCallerArgs.playedTile = gridTile;
            Debug.Log($"Trying to Execute Bush on {gridTile.X}, {gridTile.Y}");
            bushInstance.Execute(bushCallerArgs);
        } );
        
    }
}
