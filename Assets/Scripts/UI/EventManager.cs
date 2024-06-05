using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly GameEvents Game = new GameEvents();

    public class GameEvents
    {
        public LevelEvents Level = new LevelEvents();
        public class Args
        {
            public Component sender;
        }
       
        
        public class LevelEvents
        {
            public UnityAction<TurnChangedArgs> OnTurnChanged;

            public UnityAction<MissionCompletedArgs> OnMissionCompleted;

            public UnityAction<ScoreChangedArgs> OnScoreChanged;
            public UnityAction<ManaChangedArgs> OnManaChanged;
            public UnityAction<Args> OnDrawCards;
            public UnityAction<Args> OnUpdateCards;
            public UnityAction<Args> OnInCardSelection;

        
            public class TurnChangedArgs : Args
            {
                public int turnNumber = 0;
            }
            
            public class ManaChangedArgs : Args
            {
                public int newMana = 0;
            }
            
            public class ScoreChangedArgs : Args
            {
                public GameManager.Score newScore;
            }

            public class MissionCompletedArgs : Args
            {
                public int missionNumber = 0;
                public String missionText = "Blub";
            }

        }
        

        public event EventHandler OnTest;
    }
}
