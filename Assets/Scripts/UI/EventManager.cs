using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly GameEvents Game = new GameEvents();

    public class GameEvents
    {
        public UnityAction<TurnChangedArgs> OnTurnChanged;

        public UnityAction<MissionCompletedArgs> OnMissionCompleted;

        public class Args
        {
            public Component sender;
        }
        public class TurnChangedArgs : Args
        {
            public int turnNumber = 0;
        }

        public class MissionCompletedArgs : Args
        {
            public int missionNumber = 0;
            public String missionText = "Blub";
        }

        public event EventHandler OnTest;
    }
}
