using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] private int handSize = 5;


    [SerializeField] private int standardMana = 3;
    [SerializeField] private int standardTurns = 3;
    [SerializeField] private PlanetProgressionSO planetProgression;
    private GameState currentGameState = GameState.None;

    private Deck _deck = new Deck();

    public Deck Deck
    {
        get => _deck;
        set => _deck = value;
    }

    private List<CardInstance> currentWisdoms = new();
    private int currentMana = 0;
    private int currentTurns = 0;
    private int currentPlayedCards = 0;
    private bool selectedPlantNeedNeighbor = false;
    private CardInstance selectedCard;
    private GridTile playedTile;
    private bool tutorialShowed = false;

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
    private CallerArgs callerArgs = new CallerArgs();
    private EditorCallerArgs editorArgs = new EditorCallerArgs();


    private void Awake()
    {
        Debug.Log($"Instance on Awake is {Instance}");
        if (Instance == null)
        {
            Instance = this;
            //Resetting Parenting structure so dontdestroyonload does work
            this.transform.parent = null;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Game Manager already exists!");
        }
    }

#if  UNITY_EDITOR
    private void Start()
    {
        GridManager.Instance.OnGridReady += Instance_OnGridReady;
        BuildLevel();
    }
#endif

    private void OnEnable()
    {
        EventManager.Game.Level.OnPlantSacrificed += OnPlantSacrificed;
        EventManager.Game.SceneSwitch.OnSceneReloadComplete += OnSceneReloadComplete;
    }

    private void OnSceneReloadComplete(SceneLoader.Scene newScene)
    {
        if (newScene == SceneLoader.Scene.GameScene)
        {
            Debug.Log("On scene Load");
            GridManager.Instance.OnGridReady += Instance_OnGridReady;
            BuildLevel();
        }
    }

    private void OnDisable()
    {
        EventManager.Game.Level.OnPlantSacrificed -= OnPlantSacrificed;
    }

    private void OnPlantSacrificed(EventManager.GameEvents.LevelEvents.PlantSacrificedArgs arg0)
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
                InitEditor();
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
        EventManager.Game.Level.OnLevelInitialized?.Invoke(new EventManager.GameEvents.LevelEvents.LevelInitializedArgs()
        {
            currentLevel = currentLevel,
            levelName = currentLevel.name
        });
    }

    private void InitializeLevel()
    {
        _deck.InitializeDeck(startDeck);

        Debug.Log("RESETTING LEVEL");
        SetPointScore(0);
        currentTurns = 0;
        currentPlayedCards = 0;
        selectedPlantNeedNeighbor = false;

        SwitchState(GameState.EndTurn);
    }

    private void InitEditor()
    {
        editorArgs.SetValues(selectedCardBlueprint, playedTile, false, CALLER_TYPE.EDITOR);
        editorArgs.EditorCallingCardInstance = selectedCardBlueprint;
        editorArgs.gameManager = this;
        EventManager.Game.UI.OnEditorNeeded?.Invoke(new EventManager.GameEvents.UIEvents.OnEditorNeededArgs()
        {
            editorCardInstance = selectedCardBlueprint,
            editorOriginGridTile = playedTile
        });
    }

    private void Update()
    {
        if (currentGameState == GameState.SetPlant)
        {
        }

        if (currentGameState == GameState.PlantEditor)
        {
        }
    }

    public void ExecuteEditor(GridTile selectedGridTile)
    {
        if (selectedGridTile == null) return;
        editorArgs.selectedGridTile = selectedGridTile;
        
        if (selectedCardBlueprint.CheckField(editorArgs))
        {
            Debug.Log($"CAN EXECUTE THE EDITOR THERE");
            selectedCardBlueprint.ExecuteEditor(editorArgs);
            CheckSpecialFields();
            EndCardPlaying();
        }
        else
        {
            Debug.Log($"CANT EXECUTE THE EDITOR THERE");

        }
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

    public void TryPlantCard(int cardIndex, GridTile selectedGridTile)
    {
        if (!InitCallerArgsForCard(cardIndex, selectedGridTile)) return;
        if (selectedCardBlueprint.CanExecute(callerArgs))
        {
            selectedCardBlueprint.Execute(callerArgs);
            PlantCard(selectedGridTile);
            CheckSpecialFields();
        }
        else
        {
            Debug.Log("CANT EXECUTE FUNCTION THERE");
        }
    }

    private void PlantCard(GridTile selectedTile)
    {
        this.playedTile = selectedTile;

        selectedPlantNeedNeighbor = true;

        //If card has an editor (2nd move) and the editor is not blocked e.g. by plant sacrifice
        if (selectedCardBlueprint.CardEditor != null && !editorBlocked)
        {
            SwitchState(GameState.PlantEditor);

            return;
        }

        EndCardPlaying();
    }


    public bool InitCallerArgsForCard(int cardIndex, GridTile playedGridTile)
    {
        if (currentGameState != GameState.SelectCards) return false;


        selectedCard = _deck.HandCards[cardIndex];
        if (selectedCard == null)
        {
            return false;
        }

        selectedCardBlueprint = GetTemporaryCardInstance(cardIndex);
        callerArgs = GetTemporaryCallerArgs(cardIndex, playedGridTile);
        return true;
    }


    private void EndCardPlaying()
    {
        //Deck manipulation, in the future in the Deck class
        ReduceMana(selectedCardBlueprint.GetCardStats().PlayCost);
        foreach (var wisdom in currentWisdoms)
        {
            Debug.Log($"REDUCE MANA FOR {wisdom.GetCardStats().PlayCost}");
            ReduceMana(wisdom.GetCardStats().PlayCost);
            _deck.DiscardCard(wisdom);
        }

        _deck.DiscardCard(selectedCard);
        //Event calling

        currentPlayedCards++;
        Debug.Log("PLANT PLANTED!");
        EventManager.Game.UI.OnPlantPlanted?.Invoke(new EventManager.GameEvents.UIEvents.OnPlantPlantedArgs()
        {
            plantedCardInstance = selectedCardBlueprint,
            plantedGridTile = playedTile
        });
        RemoveAllWisdoms();
        editorBlocked = false;
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

        SetMana(standardMana);
        RemoveAllWisdoms();

        _deck.DiscardHand();

        EventManager.Game.Level.OnTurnChanged?.Invoke(new EventManager.GameEvents.LevelEvents.TurnChangedArgs()
        {
            sender = this,
            TurnNumber = currentTurns
        });
        SwitchState(GameState.DrawCards);
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
        List<WisdomType> currentWisdomTypes = new();
        foreach (var wisdom in currentWisdoms)
        {
            currentWisdomTypes.Add(wisdom.CardData.WisdomType);
        }

        CardInstance newCardInstance = new CardInstance(_deck.HandCards[cardIndex], currentWisdomTypes);
        return newCardInstance;
    }

    public CallerArgs GetTemporaryCallerArgs(int cardIndex, GridTile gridTile)
    {
        return new CallerArgs()
        {
            needNeighbor = selectedPlantNeedNeighbor,
            CallingCardInstance = GetTemporaryCardInstance(cardIndex),
            playedTile = gridTile,
            gameManager = this,
            callerType = CALLER_TYPE.PLACEMENT
        };
    }

    public bool EnoughMana(int cardManaCost)
    {
        if (currentMana - cardManaCost < 0)
        {
            EventManager.Game.UI.OnNotEnoughMana?.Invoke();
            return false;
        }

        return true;
    }

    public void RemoveAllWisdoms()
    {
        currentWisdoms.Clear();
        EventManager.Game.Level.OnWisdomChanged?.Invoke(new EventManager.GameEvents.LevelEvents.WisdomChangedArgs()
        {
            currentWisdoms = this.currentWisdoms
        });
    }
}