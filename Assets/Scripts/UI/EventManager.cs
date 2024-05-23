using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly GameEvents Game = new GameEvents();

    public class GameEvents
    {
<<<<<<< HEAD
        public UnityAction<TurnChangedArgs> OnTurnChanged;

        public UnityAction<MissionCompletedArgs> OnMissionCompleted;

=======
        public LevelEvents Level = new LevelEvents();
>>>>>>> b20a9e888c1acbddf9638a767d278801821b4c09
        public class Args
        {
            public Component sender;
        }
<<<<<<< HEAD
        public class TurnChangedArgs : Args
        {
            public int turnNumber = 0;
        }

        public class MissionCompletedArgs : Args
        {
            public int missionNumber = 0;
            public String missionText = "Blub";
        }
=======
        public class LevelEvents
        {
            public UnityAction<TurnChangedArgs> OnTurnChanged;

            public UnityAction<MissionCompletedArgs> OnMissionCompleted;

            public UnityAction<ScoreChangedArgs> OnScoreChanged;
            public UnityAction<ManaChangedArgs> OnManaChanged;

        
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
        
>>>>>>> b20a9e888c1acbddf9638a767d278801821b4c09

        public event EventHandler OnTest;
    }
}
