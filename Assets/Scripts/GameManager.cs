using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public int EcoPoints
        {
            get => ecoPoints;
            set => ecoPoints = value;
        }

        public int Fields
        {
            get => fields;
            set => fields = value;
        }

        public int SpecialFields
        {
            get => specialFields;
            set => specialFields = value;
        }

        private int ecoPoints;
        private int fields;
        private int specialFields;
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
        None
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

    private CardInstance selectedCardBlueprint = null;

    //Score Related
    private Score currentScore;

    // Progression Related
    private LevelSO currentLevel;
    private PlanetProgressionSO.Stage currentStage = PlanetProgressionSO.Stage.STAGE1;

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

    //Args
    private CallerArgs callerArgs = new CallerArgs();
    private EditorCallerArgs editorArgs = new EditorCallerArgs();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Game Manager already exists!");
        }
    }

    private void OnEnable()
    {
        EventManager.Game.Level.OnPlantSacrificed += OnPlantSacrificed;
    }
    private void OnDisable()
    {
        EventManager.Game.Level.OnPlantSacrificed -= OnPlantSacrificed;
    }
    private void OnPlantSacrificed(EventManager.GameEvents.LevelEvents.PlantSacrificedArgs arg0)
    {
        editorBlocked = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        GridManager.Instance.OnGridReady += Instance_OnGridReady;
        BuildLevel();
    }

    private void Instance_OnGridReady(object sender, EventArgs e)
    {
        SwitchState(GameState.Init);
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

            default:
                break;
        }
    }

    private void BuildLevel()
    {
        currentLevel = planetProgression.GetRandomEnemy(currentStage);
        GridManager.Instance.BuildGrid(currentLevel);
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
            selectedCardBlueprint.ExecuteEditor(editorArgs);
            CheckSpecialFields();
            EndCardPlaying();
        }
    }

    private void CheckSpecialFields()
    {
        int specialFieldAmount = 0;
        foreach (var specialField in GridManager.Instance.Grid.SpecialFields)
        {
            Debug.Log($"SpecialField {specialField.FieldType} is {specialField.IsFulfilled()}");
            if (specialField.IsFulfilled()) specialFieldAmount++;
        }

        currentScore.SpecialFields = specialFieldAmount;
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

        ChangeMana(standardMana, true);
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

    public void AddPointScore(int amount)
    {
        SetPointScore(currentScore.EcoPoints + amount);
    }

    private void SetPointScore(int newScore)
    {
        currentScore.EcoPoints = newScore;
        EventManager.Game.Level.OnScoreChanged?.Invoke(new EventManager.GameEvents.LevelEvents.ScoreChangedArgs()
        {
            sender = this,
            NewScore = currentScore,
            CurrentLevel = currentLevel
        });
    }

    private bool ChangeMana(int amount, bool overrideMana = false)
    {
        if (!overrideMana)
        {
            if (currentMana + amount < 0)
            {
                return false;
            }

            currentMana += amount;
        }
        else
        {
            currentMana = amount;
        }

        EventManager.Game.Level.OnManaChanged?.Invoke(new EventManager.GameEvents.LevelEvents.ManaChangedArgs()
        {
            NewMana = currentMana
        });
        return true;
    }

    public bool AddMana(int amount)
    {
        return ChangeMana(amount);
    }

    public bool ReduceMana(int amount)
    {
        return ChangeMana(-amount);
    }

    private void AddSpecialFieldScore(int amount)
    {
        currentScore.SpecialFields += amount;
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Debug.Log($"WON LEVEL");

        if (currentStage == PlanetProgressionSO.Stage.BOSS)
        {
            WinGame();
            return;
        }

        currentStage++;
        BuildLevel();
    }

    private void WinGame()
    {
        Debug.Log($"GAME WON!");
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
        Debug.Log($"{currentMana}, incoming Mana: {cardManaCost}");
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