using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly GameEvents Game = new GameEvents();

    public static void ClearInvocationLists()
    {
        Game.Level.OnEndLevel = delegate {};
        Game.Level.OnManaChanged = delegate {};
        Game.Level.OnDrawCards = delegate {};
        Game.Level.OnUpdateCards = delegate {};
        Game.Level.OnScoreChanged = delegate {};
        Game.Level.OnTurnChanged = delegate {};
        Game.Level.OnInCardSelection = delegate {};
        
        Game.UI.OnEditorNeeded = delegate {};
        Game.UI.OnHoverForEditor = delegate {};
        Game.UI.OnPlantHoverCanceled = delegate {};
        Game.UI.OnPlantHoverChanged = delegate {};
        Game.UI.OnHoverForEditor = delegate {};
        Game.UI.OnPlantPlanted = delegate {};

    }

    
    public class GameEvents
    {
        public LevelEvents Level = new LevelEvents();
        public UIEvents UI = new UIEvents();
        public InputEvents Input = new InputEvents();
        public class Args
        {
            public Component sender;
        }
        
        public class DeckChangedArgs : Args
        {
            public Deck ChangedDeck;
        }

        public class InputEvents
        {
            public UnityAction OnInteract;
            public UnityAction OnCancel;
        }
        public class UIEvents
        {
            public class OnHoverChangedArgs : Args
            {
                public CardInstance hoveredCardInstance;
                public GridTile hoveredGridTile;
                public CallerArgs hoverCallerArgs;
            }
            public class OnHoverForEditorArgs : Args
            {
                public GridTile hoveredGridTile;
            }
            public class OnPlantPlantedArgs : Args
            {
                public CardInstance plantedCardInstance;
                public GridTile plantedGridTile;
            }
            
            public class OnEditorNeededArgs : Args
            {
                public CardInstance editorCardInstance;
                public GridTile editorOriginGridTile;
                public EditorCallerArgs EditorCallerArgs;
            }
            

            public UnityAction<OnHoverChangedArgs> OnPlantHoverChanged;
            public UnityAction OnPlantHoverCanceled; 
            public UnityAction<OnPlantPlantedArgs> OnPlantPlanted;
            public UnityAction<OnEditorNeededArgs> OnEditorNeeded;
            public UnityAction<OnHoverForEditorArgs> OnHoverForEditor;
            public UnityAction OnNotEnoughMana;

        }
        
        public class LevelEvents
        {
            public UnityAction<TurnChangedArgs> OnTurnChanged;
            public UnityAction<ScoreChangedArgs> OnScoreChanged;
            public UnityAction<ManaChangedArgs> OnManaChanged;
            public UnityAction<DeckChangedArgs> OnDrawCards;
            public UnityAction<DeckChangedArgs> OnUpdateCards;
            public UnityAction<Args> OnInCardSelection;
            public UnityAction<LevelEndedArgs> OnEndLevel;
            public UnityAction<PlantSacrificedArgs> OnPlantSacrificed;
            public UnityAction<WisdomChangedArgs> OnWisdomChanged;

        
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
            
            public class WisdomChangedArgs : Args
            {
                public List<CardInstance> currentWisdoms;
            }
            
            public class ManaChangedArgs : Args
            {
                public int NewMana = 0;
            }
            
            public class ScoreChangedArgs : Args
            {
                public GameManager.Score ScoreAdded;
                public GameManager.Score NewScore;
                public LevelSO CurrentLevel;
                public CallerArgs scoreChangedCallerArgs;
                public GameManager.SCORING_ORIGIN ScoringOrigin;
            }

            public class PlantSacrificedArgs : Args
            {
                public CallerArgs SacrificeCallerArgs;
            }


        }
    }
}
