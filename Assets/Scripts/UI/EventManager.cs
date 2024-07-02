using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly GameEvents Game = new GameEvents();

    public class GameEvents
    {
        public LevelEvents Level = new LevelEvents();
        public UIEvents UI = new UIEvents();
        public class Args
        {
            public Component sender;
        }

        public class UIEvents
        {
            public class OnHoverChangedArgs : Args
            {
                public CardInstance hoveredCardInstance;
                public GridTile hoveredGridTile;
            }
            

            public UnityAction<OnHoverChangedArgs> OnPlantHoverChanged;
            public UnityAction OnPlantHoverCanceled;

        }
        
        public class LevelEvents
        {
            public UnityAction<TurnChangedArgs> OnTurnChanged;
            public UnityAction<ScoreChangedArgs> OnScoreChanged;
            public UnityAction<ManaChangedArgs> OnManaChanged;
            public UnityAction<Args> OnDrawCards;
            public UnityAction<Args> OnUpdateCards;
            public UnityAction<Args> OnInCardSelection;
            public UnityAction<LevelEndedArgs> OnEndLevel;

        
            public class LevelEndedArgs : Args
            {
                public bool WonLevel;
                public int CurrentScore;
                public int NeededScore;
            }
            public class TurnChangedArgs : Args
            {
                public int TurnNumber = 0;
            }
            
            public class ManaChangedArgs : Args
            {
                public int NewMana = 0;
            }
            
            public class ScoreChangedArgs : Args
            {
                public GameManager.Score NewScore;
                public LevelSO CurrentLevel;
            }


        }
        

        public event EventHandler OnTest;
    }
}
