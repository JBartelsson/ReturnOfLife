using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly GameEvents Game = new GameEvents();

    public static void ClearInvocationLists()
    {
        Game.Level.OnEndLevel = delegate { };
        Game.Level.OnManaChanged = delegate { };
        Game.Level.OnDrawCard = delegate { };
        Game.Level.OnDiscardCard = delegate { };
        Game.Level.OnDeckChanged = delegate { };
        Game.Level.OnScoreChanged = delegate { };
        Game.Level.OnTurnChanged = delegate { };
        Game.Level.OnInCardSelection = delegate { };
        Game.Level.OnLevelInitialized = delegate { };
        Game.Level.OnLifeformSacrificed = delegate { };
        Game.Level.OnWisdomChanged = delegate { };
        Game.Level.OnPlanetProgressionWon = delegate { };
        Game.Level.OnEndSingleCardPlay = delegate { };
        Game.Level.OnSecondMoveSuccessful = delegate { };
        Game.Level.OnShuffeDiscardPileIntoDrawPile = delegate { };
        Game.Level.OnCardAdded = delegate { };
        Game.Level.OnDiscardUsed = delegate { };
        Game.Level.OnLifeformPlanted = delegate { };
        Game.Level.OnLifeformKilled = delegate { };
        Game.Level.OnLifeformRevived = delegate { };
        Game.Level.OnTriggerSpecialField = delegate { };


        Game.UI.OnSecondMoveNeeded = delegate { };
        Game.UI.OnHoverForSecondMove = delegate { };
        Game.UI.OnLifeformHoverCanceled = delegate { };
        Game.UI.OnLifeformHoverChanged = delegate { };
        Game.UI.OnHoverForSecondMove = delegate { };
        Game.UI.OnNotEnoughMana = delegate { };
        Game.UI.OnTutorialScreenChange = delegate { };
        Game.UI.OnCardPickScreenChange = delegate { };
        Game.UI.OnChangeOtherCanvasesStatus = delegate { };
        Game.UI.OnBlockGamePlay = delegate { };
        Game.UI.OnSecondMoveQueueEmpty = delegate { };
        Game.UI.OnCardFirstSkipEvent = delegate { };
        Game.UI.OnOpenCardView = delegate { };
        Game.UI.OnEndSingleCardPlay = delegate { };
        Game.UI.OnCardSelected = delegate { };
        Game.UI.OnCardSelectGridTileUpdate = delegate { };
        Game.UI.OnShowDeckPickCanvas = delegate { };
        Game.UI.OnSecondMoveStillOpen = delegate { };
        Game.UI.OnShowDeckPickCanvas = delegate { };


        Game.GameSettings.OnDeckUnlocked = delegate { };
    }


    public class GameEvents
    {
        public LevelEvents Level = new LevelEvents();
        public UIEvents UI = new UIEvents();
        public InputEvents Input = new InputEvents();
        public SceneSwitchEvents SceneSwitch = new SceneSwitchEvents();
        public GameSettingsEvents GameSettings = new GameSettingsEvents();

        public class Args
        {
            public Component sender;
        }

        

        public class SceneReloadArgs : Args
        {
            public SceneLoader.Scene oldSCene;
            public SceneLoader.Scene newScene;
        }

        public class InputEvents
        {
            public UnityAction OnInteract;
            public UnityAction OnCancel;
        }

        public class SceneSwitchEvents
        {
            public UnityAction<SceneReloadArgs> OnSceneReloadStarted;
            public UnityAction<SceneReloadArgs> OnSceneReloadComplete;
        }

        public class GameSettingsEvents
        {
            public UnityAction<DeckUnlockedArgs> OnDeckUnlocked;

            public class DeckUnlockedArgs
            {
                public StartDeckSO UnlockedDeck;
            }
        }

        public class UIEvents
        {
            public class OnLifeformChangedArgs : Args
            {
                public CardInstance hoveredCardInstance;
                public GridTile hoveredGridTile;
                public CallerArgs hoverCallerArgs;
            }

            public class OnHoverForEditorArgs : Args
            {
                public GridTile hoveredGridTile;
            }


            public class OnSecondMoveNeededArgs : Args
            {
                public CardInstance editorCardInstance;
                public GridTile editorOriginGridTile;
                public SecondMoveCallerArgs SecondMoveCallerArgs;
            }

            public class OnOpenCardViewArgs : Args
            {
                public bool State;
                public DeckViewUI.Pile Pile;
                public List<CardInstance> cards;
            }

            public class BoolArgs : Args
            {
                public bool status;
            }
            public class CardSelectGridUpdateArgs : Args
            {
                public bool Status;
                public GridTile UpdatedTile;
            }


            public UnityAction<OnLifeformChangedArgs> OnLifeformHoverChanged;
            public UnityAction OnLifeformHoverCanceled;
            public UnityAction<OnSecondMoveNeededArgs> OnSecondMoveNeeded;
            public UnityAction<OnHoverForEditorArgs> OnHoverForSecondMove;
            public UnityAction OnNotEnoughMana;
            public UnityAction<bool> OnTutorialScreenChange;
            public UnityAction<BoolArgs> OnCardPickScreenChange;
            public UnityAction<bool> OnChangeOtherCanvasesStatus;
            public UnityAction<bool> OnBlockGamePlay;
            public UnityAction OnSecondMoveQueueEmpty;
            public UnityAction OnCardFirstSkipEvent;
            public UnityAction<OnOpenCardViewArgs> OnOpenCardView;
            public UnityAction OnEndSingleCardPlay;
            public UnityAction<CardInstance> OnCardSelected;
            public UnityAction<CardSelectGridUpdateArgs> OnCardSelectGridTileUpdate;
            public UnityAction<bool> OnShowDeckPickCanvas;
            public UnityAction OnSecondMoveStillOpen;
        }

        public class LevelEvents
        {
            public UnityAction<TurnChangedArgs> OnTurnChanged;
            public UnityAction<ScoreChangedArgs> OnScoreChanged;
            public UnityAction<ManaChangedArgs> OnManaChanged;
            public UnityAction<DeckChangedArgs> OnDrawCard;
            public UnityAction<DiscardCardArgs> OnDiscardCard;
            public UnityAction<DeckChangedArgs> OnDeckChanged;
            public UnityAction<Args> OnInCardSelection;
            public UnityAction<LevelEndedArgs> OnEndLevel;
            public UnityAction<LifeformSacrificedArgs> OnLifeformSacrificed;
            public UnityAction<WisdomChangedArgs> OnWisdomChanged;
            public UnityAction<LevelInitializedArgs> OnLevelInitialized;
            public UnityAction OnPlanetProgressionWon;
            public UnityAction OnEndSingleCardPlay;
            public UnityAction OnSecondMoveSuccessful;
            public UnityAction OnShuffeDiscardPileIntoDrawPile;
            public UnityAction<CardInstance> OnCardAdded;
            public UnityAction<int> OnDiscardUsed;
            public UnityAction<OnLifeformPlantedArgs> OnLifeformPlanted;
            public UnityAction<CallerArgs> OnLifeformRevived;
            public UnityAction<CallerArgs> OnLifeformKilled;
            public UnityAction<CallerArgs> OnEffectUsed;
            public UnityAction<TriggerSpecialFieldArgs> OnTriggerSpecialField;

            public class DeckChangedArgs : Args
            {
                public Deck ChangedDeck;
            }

            public class DiscardCardArgs : Args
            {
                public CardInstance DiscardedCard;
                public int DiscardedIndex;

            }
            public class TriggerSpecialFieldArgs : Args
            {
                public SpecialField triggeredField;
            }
            public class LevelEndedArgs : Args
            {
                public bool WonLevel;
                public int CurrentScore;
                public int NeededScore;
            }

            public class OnLifeformPlantedArgs : Args
            {
                public CardInstance plantedCardInstance;
                public GridTile plantedGridTile;
            }

            public class LevelInitializedArgs : Args
            {
                public LevelSO CurrentLevel;
                public int CurrentLevelNumber;
                public int MaxLevelNumber;
                public string LevelName;
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

            public class LifeformSacrificedArgs : Args
            {
                public CallerArgs SacrificeCallerArgs;
            }
        }
    }
}