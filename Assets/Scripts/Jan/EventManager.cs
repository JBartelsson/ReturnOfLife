using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly GameEvents Game = new GameEvents();

    public class GameEvents
    {
        public UnityAction<TurnChangedArgs> OnTurnChanged;
        public class Args
        {
            public Component sender;
        }
        public class TurnChangedArgs : Args
        {
            public int turnNumber = 0;
        }
        //public UnityAction<Component, int> OnMissionCompleted;
        public event EventHandler OnTest;
    }
}
