using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Serializable]
    public struct Score
    {
        public Score(int ecoPoints = 0)
        {
            this.ecoPoints = ecoPoints;
        }

        public int EcoPoints
        {
            get => ecoPoints;
            set => ecoPoints = value;
        }

        private int ecoPoints;
    }


    public enum GameState
    {
        Init,
        DrawCards,
        EndTurn,
        SelectCards,
        SetPlant,
        PlantEditor,
        SpecialAbility,
        LevelEnd,
        None,
        Tutorial
    }

    [Header("Game Options")] [SerializeField]
    private StartDeckSO startDeck;

    [SerializeField] private StartDeckSO debugStartDeck;

    [SerializeField] private int handSize = 5;


    [SerializeField] private int standardMana = 3;
    [SerializeField] private int standardTurns = 3;

    [FormerlySerializedAs("standardDiscard")] [SerializeField]
    private int standardDiscards = 3;

    [SerializeField] private PlanetProgressionSO planetProgression;
    private GameState currentGameState = GameState.None;

    [FormerlySerializedAs("debug")] [Header("Debug Settings")] [SerializeField]
    private bool sceneDebug = false;

    private Deck _deck = new Deck();

    public Deck Deck
    {
        get => _deck;
        set => _deck = value;
    }

    private List<CardInstance> currentWisdoms = new();
    private int currentMana = 0;
    private int currentTurns = 0;
    private int currentDiscards = 0;
    private int currentPlayedCards = 0;
    private bool selectedPlantNeedNeighbor = false;
    private int selectedCardIndex;
    private bool tutorialShowed = false;
    private bool blockQueue = false;
    private bool blockSecondMoveQueue = false;
    private int blockSecondMoveTimer = 0;
    private int blockQueueTimer = 0;

    private CardInstance selectedCardBlueprint = null;

    //Score Related
    private Score currentScore;

    // Progression Related
    private LevelSO currentLevel;
    private int currentStage = 0;

    private bool editorBlocked = false;

    public Score CurrentScore
    {
        get => currentScore;
        set => currentScore = value;
    }

    public StartDeckSO StartDeck
    {
        get => startDeck;
        set => startDeck = value;
    }

    public enum SCORING_ORIGIN
    {
        LIFEFORM,
        MULTIPLICATION
    }

    //Args
    private List<CallerArgs> playingQueue = new();

    public class SecondMoveQueueItem
    {
        public CallerArgs CallerArgs;
        public int secondMoveNumber = 1;

        public SecondMoveQueueItem(CallerArgs callerArgs, int secondMoveNumber)
        {
            CallerArgs = callerArgs;
            this.secondMoveNumber = secondMoveNumber;
        }
    }

    private List<SecondMoveQueueItem> secondMoveQueue = new();
    private SecondMoveCallerArgs secondMoveArgs = new SecondMoveCallerArgs();
    private bool secondMoveQueueEmptyCalled = false;
    private float queueTimer;

    private void Awake()
    {
        Debug.Log($"Instance on Awake is {Instance}");
        if (Instance == null)
        {
            Instance = this;
            InitEventSubscriptions();

            //Resetting Parenting structure so dontdestroyonload does work
            this.transform.parent = null;
            DontDestroyOnLoad(this);
#if UNITY_EDITOR
            if (!sceneDebug)
                SceneLoader.Load(SceneLoader.Scene.GameScene);
#endif
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Game Manager already exists!");
        }
    }

    private void Start()
    {
    }


    private void InitEventSubscriptions()
    {
        Debug.Log("SUBSCRIBING FROM GAMEMANAGER TO FUNCTIONS");
        EventManager.Game.Level.OnLifeformSacrificed += OnPlantSacrificed;
        EventManager.Game.SceneSwitch.OnSceneReloadComplete += OnSceneReloadComplete;
    }

    private void OnSceneReloadComplete(EventManager.GameEvents.SceneReloadArgs args)
    {
        if (args.newScene == SceneLoader.Scene.GameScene)
        {
            Debug.Log("On scene Load");
            GridManager.Instance.OnGridReady += Instance_OnGridReady;
            if (!_deck.IsInitialized())
            {
                #if UNITY_EDITOR
                _deck.InitializeDeck(debugStartDeck, handSize, handSize);
                #else
                _deck.InitializeDeck(startDeck, handSize, handSize);
#endif
            }

            BuildLevel();
        }
    }

    private void OnPlantSacrificed(EventManager.GameEvents.LevelEvents.LifeformSacrificedArgs arg0)
    {
    }

    private void Instance_OnGridReady(object sender, EventArgs e)
    {
        if (tutorialShowed)
        {
            EventManager.Game.UI.OnTutorialScreenChange?.Invoke(false);

            SwitchState(GameState.Init);
        }
        else
        {
            tutorialShowed = true;
            Debug.Log("SHOULD SHOW TUTORIAL");
            EventManager.Game.UI.OnTutorialScreenChange?.Invoke(true);
            SwitchState(GameState.Tutorial);
        }

        GridManager.Instance.OnGridReady -= Instance_OnGridReady;
    }

    private void SwitchState(GameState newGameState)
    {
        currentGameState = newGameState;
        //Debug.Log($"GAME MANAGER: Switching to State {currentGameState}");
        switch (newGameState)
        {
            case GameState.Init:
                InitializeLevel();
                break;
            case GameState.EndTurn:
                EndTurn();
                break;
            case GameState.DrawCards:
                DrawCards();
                break;
            case GameState.SelectCards:
                EventManager.Game.Level.OnInCardSelection?.Invoke(new EventManager.GameEvents.Args()
                {
                    sender = this
                });
                break;
            case GameState.SetPlant:
                break;
            case GameState.PlantEditor:
                break;
            case GameState.SpecialAbility:
                break;
            case GameState.None:
                break;

            case GameState.LevelEnd:
                break;
            case GameState.Tutorial:
                Debug.Log("TUTORIAL SHOW");
                SwitchState(GameState.Init);
                break;
            default:
                break;
        }
    }

    private void BuildLevel()
    {
        currentLevel = planetProgression.GetRandomEnemy(currentStage);
        GridManager.Instance.BuildGrid(currentLevel);
        EventManager.Game.Level.OnLevelInitialized?.Invoke(
            new EventManager.GameEvents.LevelEvents.LevelInitializedArgs()
            {
                CurrentLevel = currentLevel,
                CurrentLevelNumber = currentStage,
                MaxLevelNumber = planetProgression.Progression.Count,
                LevelName = currentLevel.name
            });
    }

    private void InitializeLevel()
    {
        _deck.Reset();
        Debug.Log("RESETTING LEVEL");
        SetPointScore(0);
        currentTurns = 0;
        currentPlayedCards = 0;
        selectedPlantNeedNeighbor = false;

        SwitchState(GameState.EndTurn);
    }

    private void InitSecondMove(SecondMoveQueueItem secondMoveQueueItem)
    {
        CallerArgs callerArgs = secondMoveQueueItem.CallerArgs;
        selectedCardBlueprint = callerArgs.CallingCardInstance;
        Debug.Log($"INITIALIZES ONE SECOND MOVE for {callerArgs}");
        secondMoveQueue.RemoveAt(0);
        if (selectedCardBlueprint.IsDead())
        {
            CancelSecondMove();
            return;
        }

        secondMoveArgs = new SecondMoveCallerArgs()
        {
            callerType = CALLER_TYPE.SECOND_MOVE,
            CallingCardInstance = callerArgs.CallingCardInstance,
            EditorCallingCardInstance = callerArgs.CallingCardInstance,
            gameManager = this,
            needNeighbor = false,
            playedTile = callerArgs.playedTile,
            SecondMoveNumber = secondMoveQueueItem.secondMoveNumber
        };
        Debug.Log($"SECOND MOVE CARD ISNTANCE {callerArgs.CallingCardInstance}");
        EventManager.Game.UI.OnSecondMoveNeeded?.Invoke(new EventManager.GameEvents.UIEvents.OnSecondMoveNeededArgs()
        {
            editorCardInstance = callerArgs.CallingCardInstance,
            editorOriginGridTile = callerArgs.playedTile,
            SecondMoveCallerArgs = secondMoveArgs
        });
    }

    public void CancelSecondMove()
    {
        blockSecondMoveQueue = false;
        CheckForEmptyQueue();
    }

    private void Update()
    {
        if (playingQueue.Count != 0)
        {
            if (blockQueue && playingQueue[0].callerType != CALLER_TYPE.EFFECT) return;
            blockQueue = true;
            PlayLifeForm(playingQueue[0]);

            return;
        }

        if (secondMoveQueue.Count != 0 && !blockSecondMoveQueue)
        {
            blockSecondMoveQueue = true;
            secondMoveQueueEmptyCalled = false;
            blockQueue = true;
            InitSecondMove(secondMoveQueue[0]);
            return;
        }
        else
        {
            if (!secondMoveQueueEmptyCalled && !blockSecondMoveQueue)
            {
                secondMoveQueueEmptyCalled = true;
                EventManager.Game.UI.OnSecondMoveQueueEmpty?.Invoke();
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddPointScore(500, new CallerArgs(), SCORING_ORIGIN.LIFEFORM);
        }
#endif
    }

    public void ExecuteSecondMove(GridTile selectedGridTile)
    {
        if (selectedGridTile == null) return;
        secondMoveArgs.selectedGridTile = selectedGridTile;

        Debug.Log($"Trying to Execute Editor of {selectedCardBlueprint}");
        if (selectedCardBlueprint.CheckField(secondMoveArgs))
        {
            Debug.Log($"CAN EXECUTE THE EDITOR THERE");
            selectedCardBlueprint.ExecuteSecondMove(secondMoveArgs);
            blockSecondMoveQueue = false;
            CheckForEmptyQueue();
        }

        Debug.Log($"CANT EXECUTE THE EDITOR THERE");

        Debug.Log(secondMoveArgs);
    }

    private void CheckSpecialFields()
    {
        int specialFieldAmount = 0;
        Debug.Log($"CHECKING SPECIAL FIELDS");
        foreach (var specialField in GridManager.Instance.Grid.SpecialFields)
        {
            Debug.Log($"SpecialField {specialField.FieldType} is {specialField.IsFulfilled()}");
            specialField.TryExecuteFunction();
            specialFieldAmount++;
        }
    }

    public void TryQueueLifeform(int cardIndex, GridTile selectedGridTile)
    {
        CardInstance cardInstance = GetTemporaryCardInstance(cardIndex);
        selectedCardIndex = cardIndex;

        TryQueueLifeform(cardInstance, selectedGridTile);
    }

    public void TryQueueLifeform(CardInstance cardInstance, GridTile selectedGridTile)
    {
        selectedCardBlueprint = cardInstance;
        CallerArgs newCallerArgs = GetTemporaryCallerArgs(cardInstance, selectedGridTile);
        TryQueueLifeform(newCallerArgs);
    }

    public void TryQueueLifeform(CallerArgs callerArgs)
    {
        if (callerArgs.CallingCardInstance.CanExecute(callerArgs))
        {
            Debug.Log($"QUEUED {callerArgs}");
            playingQueue.Insert(0, callerArgs);
        }
    }

    private void CheckForEmptyQueue()
    {
        blockQueue = false;
        if (selectedCardIndex != -1)
        {
            Debug.Log($"Trying to end turn of card {selectedCardIndex}");
            //Deck manipulation, in the future in the Deck class
            CardInstance playedCard = _deck.HandCards[selectedCardIndex];

            playedCard.Upgrades.AddRange(currentWisdoms);

            ReduceMana(playedCard.GetPlayCost());
            foreach (var wisdom in currentWisdoms)
            {
                _deck.DiscardCard(wisdom);
            }
            RemoveAllWisdoms();
            

            _deck.DiscardCard(playedCard);
            //Event calling

            currentPlayedCards++;
            Debug.Log("PLANT PLANTED!");

            selectedPlantNeedNeighbor = true;
            selectedCardIndex = -1;
        }

        if (playingQueue.Count != 0) return;
        if (secondMoveQueue.Count != 0) return;
        EndCardPlaying();
    }

    public void PlayLifeForm(CallerArgs callerArgs)
    {
        playingQueue.RemoveAt(0);
        if (callerArgs.callerType == CALLER_TYPE.REVIVE)
        {
            callerArgs.playedTile.ClearTile();
        }

        Debug.Log($"Playing Lifeform with {callerArgs}");
        selectedCardBlueprint = callerArgs.CallingCardInstance;
        selectedCardBlueprint.Execute(callerArgs);
        CheckSpecialFields();
        EventManager.Game.UI.OnEndSingleCardPlay?.Invoke();

        //If card has an editor (2nd move) and the editor is not blocked e.g. by plant sacrifice
        Debug.Log($"Editor is blocked: {callerArgs.BlockSecondMove}");
        if (selectedCardBlueprint.CardSecondMove != null && !callerArgs.BlockSecondMove)
        {
            for (int i = 0; i < selectedCardBlueprint.GetCardStats().SecondMoveCallAmount; i++)
            {
                secondMoveQueue.Insert(0, new SecondMoveQueueItem(callerArgs, i + 1));
            }

            return;
        }

        //If there are still cards to plant, return
        if (callerArgs.callerType == CALLER_TYPE.SECOND_MOVE) return;
        CheckForEmptyQueue();
    }


    private void EndCardPlaying()
    {
        editorBlocked = false;
        EventManager.Game.Level.OnEndSingleCardPlay?.Invoke();
        SwitchState(GameState.SelectCards);
    }

    public void EndTurn()
    {
        currentTurns++;
        if (currentTurns > standardTurns)
        {
            EndLevel();
            return;
        }

        RemoveAllWisdoms();
        SetMana(standardMana);
        SetDiscards(standardDiscards);


        EventManager.Game.Level.OnTurnChanged?.Invoke(new EventManager.GameEvents.LevelEvents.TurnChangedArgs()
        {
            sender = this,
            TurnNumber = currentTurns
        });
        _deck.DiscardHand();
        SwitchState(GameState.DrawCards);
    }

    private void SetDiscards(int discards)
    {
        currentDiscards = discards;
        EventManager.Game.Level.OnDiscardUsed(currentDiscards);
    }

    private void EndLevel()
    {
        EventManager.Game.Level.OnEndLevel?.Invoke(new EventManager.GameEvents.LevelEvents.LevelEndedArgs()
        {
            WonLevel = currentLevel.RequirementsMet(this),
            CurrentScore = currentScore.EcoPoints,
            NeededScore = currentLevel.NeededEcoPoints
        });
    }

    private void DrawCards()
    {
        _deck.DrawForTurn();
        SwitchState(GameState.SelectCards);
    }

    public void AddWisdom(CardInstance wisdom)
    {
        currentWisdoms.Add(wisdom);
        EventManager.Game.Level.OnWisdomChanged?.Invoke(new EventManager.GameEvents.LevelEvents.WisdomChangedArgs()
        {
            currentWisdoms = this.currentWisdoms
        });
    }

    public void RemoveWisdom(CardInstance wisdom)
    {
        currentWisdoms.Remove(wisdom);
        EventManager.Game.Level.OnWisdomChanged?.Invoke(new EventManager.GameEvents.LevelEvents.WisdomChangedArgs()
        {
            currentWisdoms = this.currentWisdoms
        });
    }

    //Score related

    public void AddPointScore(int amount, CallerArgs scoreCallerArgs, SCORING_ORIGIN scoringOrigin)
    {
        SetPointScore(currentScore.EcoPoints + amount, scoreCallerArgs, scoringOrigin);
    }

    private void SetPointScore(int newScore, CallerArgs scoreCallerArgs = null,
        SCORING_ORIGIN scoringOrigin = SCORING_ORIGIN.LIFEFORM)
    {
        int oldScore = currentScore.EcoPoints;
        currentScore.EcoPoints = newScore;
        EventManager.Game.Level.OnScoreChanged?.Invoke(new EventManager.GameEvents.LevelEvents.ScoreChangedArgs()
        {
            sender = this,
            NewScore = currentScore,
            CurrentLevel = currentLevel,
            scoreChangedCallerArgs = scoreCallerArgs,
            ScoringOrigin = scoringOrigin,
            ScoreAdded = new Score(newScore - oldScore)
        });
        // if (currentLevel.RequirementsMet(this))
        // {
        //     EndLevel();
        // }
    }

    private bool SetMana(int amount)
    {
        if (amount < 0)
        {
            return false;
        }

        currentMana = amount;

        EventManager.Game.Level.OnManaChanged?.Invoke(new EventManager.GameEvents.LevelEvents.ManaChangedArgs()
        {
            NewMana = currentMana
        });
        return true;
    }

    public bool AddMana(int amount)
    {
        Debug.Log($"ADD MANA CALLED FOR {amount}");
        return SetMana(currentMana + amount);
    }

    public bool ReduceMana(int amount)
    {
        return SetMana(currentMana - amount);
    }


    public void GameOver()
    {
        currentStage = 0;
        _deck.InitializeDeck(startDeck);
        SceneLoader.Reload();
    }

    public void NextLevel()
    {
        DOTween.Clear(true);
        if (planetProgression.IsBoss(currentStage))
        {
            WinGame();
            return;
        }

        currentStage++;
        SceneLoader.Reload();
    }

    private void WinGame()
    {
        EventManager.Game.Level.OnPlanetProgressionWon?.Invoke();
    }

    public CardInstance GetTemporaryCardInstance(int cardIndex)
    {
        List<CardInstance> currentWisdomTypes = new();
        foreach (var wisdom in currentWisdoms)
        {
            currentWisdomTypes.Add(wisdom);
        }

        CardInstance newCardInstance = new CardInstance(_deck.HandCards[cardIndex], currentWisdomTypes);
        return newCardInstance;
    }

    public CallerArgs GetTemporaryCallerArgs(int cardIndex, GridTile gridTile)
    {
        return GetTemporaryCallerArgs(GetTemporaryCardInstance(cardIndex), gridTile);
    }

    public CallerArgs GetTemporaryCallerArgs(CardInstance cardInstance, GridTile gridTile)
    {
        return new CallerArgs()
        {
            needNeighbor = selectedPlantNeedNeighbor,
            CallingCardInstance = cardInstance,
            playedTile = gridTile,
            gameManager = this,
            callerType = CALLER_TYPE.PLACEMENT
        };
    }

    public bool EnoughMana(int cardManaCost)
    {
        if (currentMana - cardManaCost < 0)
        {
            // EventManager.Game.UI.OnNotEnoughMana?.Invoke();
            return false;
        }

        return true;
    }

    public bool EnoughManaFor(CardInstance cardInstance)
    {
        return EnoughMana(cardInstance.GetPlayCost());
    }

    public bool AddWisdomsAndCheckMana(CardInstance cardInstance)
    {
        if (cardInstance.CardData.EffectType == CardData.CardEffectType.Plant)
        {
            cardInstance.Upgrades.AddRange(currentWisdoms);
        }
        bool enoughMana = GameManager.Instance.EnoughManaFor(cardInstance);
        cardInstance.Upgrades.Clear();
        return enoughMana;
    }


    public void RemoveAllWisdoms()
    {
        currentWisdoms.Clear();
        AddMana(0);
        EventManager.Game.Level.OnWisdomChanged?.Invoke(new EventManager.GameEvents.LevelEvents.WisdomChangedArgs()
        {
            currentWisdoms = this.currentWisdoms
        });
    }

    public void DiscardCard(int index)
    {
        if (currentDiscards <= 0) return;
        Deck.DiscardCard(index);
        Deck.DrawCards(1);
        SetDiscards(currentDiscards - 1);
    }
}