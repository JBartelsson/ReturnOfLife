using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static GameManager;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    [Serializable]
    public class PlantableCard
    {
        public PlantInstance plantBlueprint;

        public PlantableCard(PlantInstance plantBlueprint)
        {
            this.plantBlueprint = plantBlueprint;
        }

        public PlantableCard(Plantable plantable)
        {
            this.plantBlueprint = new PlantInstance(plantable);
        }
    }

    public struct Score
    {
        public int Points
        {
            get => points;
            set => points = value;
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

        private int points;
        private int fields;
        private int specialFields;
    }


    public enum GameState
    {
        Init, DrawCards, StartTurn, SelectCards, SetPlant, PlantEditor, SpecialAbility, LevelEnd, None
    }
    [Header("Game Options")]
    [SerializeField] private StartDeckSO startDeck;
    [SerializeField] private int handSize = 5;
    [SerializeField] private int standardMana = 3;
    [SerializeField] private int standardTurns = 3;
    private GameState currentGameState = GameState.None;
    private List<PlantableCard> deck = new();
    private List<PlantableCard> currentHand = new();
    private List<PlantableCard> drawPile = new();
    private List<PlantableCard> discardPile = new();
    private List<Fertilizer> currentFertilizers = new();
    private int currentMana = 0;
    private int currentTurns = 0;
    private int currentPlayedCards = 0;
    private bool selectedPlantNeedNeighbor = false;
    private PlantableCard selectedCard;
    private GridTile playedTile;
    private PlantInstance selectedPlantBlueprint = null;
    //Score Related
    private Score currentScore;

    public Score CurrentScore
    {
        get => currentScore;
        set => currentScore = value;
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
    // Start is called before the first frame update
    void Start()
    {
        GridManager.Instance.OnGridReady += Instance_OnGridReady;
    }

    private void Instance_OnGridReady(object sender, EventArgs e)
    {
        SwitchState(GameState.Init);
    }

    public void SwitchState(GameState newGameState)
    {
        currentGameState = newGameState;
        //Debug.Log($"GAME MANAGER: Switching to State {currentGameState}");
        switch (newGameState)
        {
            case GameState.Init:
                InitializeLevel();
                SwitchState(GameState.StartTurn);
                break;
            case GameState.StartTurn:
                StartTurn();
                SwitchState(GameState.DrawCards);
                break;
            case GameState.DrawCards:
                DrawCards();
                SwitchState(GameState.SelectCards);
                break;
            case GameState.SelectCards:
                for (int i = 0; i < currentHand.Count; i++)
                {
                    Debug.Log($"HAND: Slot {i + 1}: {currentHand[i].plantBlueprint.Plantable.visualization}");
                }
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


    private void InitializeLevel()
    {
        deck.Clear();
        currentHand.Clear();
        drawPile.Clear();
        discardPile.Clear();
        currentTurns = 0;
        currentPlayedCards = 0;
        selectedPlantNeedNeighbor = false;

        foreach (StartDeckSO.DeckEntry deckEntry in startDeck.Deck)
        {
            for (int i = 0; i < deckEntry.amount; i++)
            {
                deck.Add(new PlantableCard(deckEntry.plantableReference));
                Debug.Log($"Added {deckEntry.plantableReference.visualization} to deck, Its Plant Instance: {deck.Last().plantBlueprint}");
            }
        }
    }

    private void InitEditor()
    {
        editorArgs.SetValues(selectedCard.plantBlueprint, playedTile, selectedPlantNeedNeighbor, CALLER_TYPE.EDITOR);
        GridManager.Instance.Grid.ForEachGridTile((x) =>
        {
            if (selectedPlantBlueprint.CheckField(new EditorCallerArgs()
            {
                playedTile = playedTile,
                selectedGridTile = x,
                callerType = CALLER_TYPE.EDITOR
            }))
            {
                x.ChangeMarkedStatus(true);
            }
        });
    }

    private void Update()
    {
        if (currentGameState == GameState.SelectCards)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlayCard(currentHand[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlayCard(currentHand[1]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                PlayCard(currentHand[2]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                PlayCard(currentHand[3]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                PlayCard(currentHand[4]);
            }
        }
        if (currentGameState == GameState.SetPlant)
        {

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("SET PLANT CLICK");

                GridTile gridTile = GridManager.Instance.Grid.GetGridObject(UtilsClass.GetMouseWorldPosition());
                callerArgs.playedTile = gridTile;
                if (selectedPlantBlueprint.Execute(callerArgs))
                {
                    
                    PlantCard(gridTile);
                }
                else
                {
                    Debug.Log("CANT EXECUTE FUNCTION THERE");
                }
            }
        }
        if (currentGameState == GameState.PlantEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GridTile selectedGridTile = GridManager.Instance.Grid.GetGridObject(UtilsClass.GetMouseWorldPosition());
                editorArgs.selectedGridTile = selectedGridTile;
                if (selectedPlantBlueprint.CheckField(editorArgs))
                {
                    selectedPlantBlueprint.ExecuteEditor(editorArgs);
                    EndCardPlaying();
                }
                else
                {
                    Debug.Log("CANT EXECUTE EDITOR FUNCTION THERE");
                }
            }
        }
    }

    private void PlayCard(PlantableCard plantableCard)
    {
        selectedCard = plantableCard;
        selectedPlantBlueprint = new PlantInstance(selectedCard.plantBlueprint, currentFertilizers);
        Debug.Log($"PLAYED {selectedPlantBlueprint}");
        callerArgs = new CallerArgs()
        {
            needNeighbor = selectedPlantNeedNeighbor,
            callingPlantInstance = selectedPlantBlueprint,
            gameManager = this
        };
        if (selectedPlantBlueprint.GetPlantFunctionExecuteType() == EXECUTION_TYPE.IMMEDIATE)
        {
            selectedPlantBlueprint.Execute(callerArgs);
            EndCardPlaying();
        }
        if (selectedPlantBlueprint.GetPlantFunctionExecuteType() == EXECUTION_TYPE.AFTER_PLACEMENT)
        {
            SwitchState(GameState.SetPlant);
        }
    }

    private void PlantCard(GridTile selectedTile)
    {
        this.playedTile = selectedTile;
        
        selectedPlantNeedNeighbor = true;

        Debug.Log($"PLANTED: {selectedCard}");
        if (selectedPlantBlueprint.PlantEditor != null)
        {
            SwitchState(GameState.PlantEditor);
            return;
        }
        EndCardPlaying();
    }

    private void EndCardPlaying()
    {
        GridManager.Instance.Grid.ForEachGridTile((x) => x.ChangeMarkedStatus(false));
        currentMana -= selectedPlantBlueprint.Plantable.manaCost;
        currentHand.Remove(selectedCard);
        discardPile.Add(selectedCard);
        currentPlayedCards++;
        if (currentMana > 0)
        {
            SwitchState(GameState.SelectCards);
        }
        else
        {
            SwitchState(GameState.StartTurn);
        }

    }

    private void StartTurn()
    {

        currentTurns++;
        if (currentTurns > standardTurns)
        {
            EndLevel();
            return;
        }
        Debug.Log($"=====START TURN {currentTurns}=====");
        currentMana = standardMana;
        discardPile.AddRange(currentHand);
        drawPile.AddRange(deck);

    }

    private void EndLevel()
    {
        Debug.Log($"LEVEL ENDED");
        SwitchState(GameState.LevelEnd);
    }

    private void DrawCards()
    {
        while (currentHand.Count < handSize)
        {
            DrawSingleCard();
        }

    }

    private void DrawSingleCard()
    {
        int randomIndex = UnityEngine.Random.Range(0, drawPile.Count);
        PlantableCard drawCard = drawPile[randomIndex];
        currentHand.Add(drawCard);
        drawPile.Remove(drawCard);

    }

    public void AddFertilizer(Fertilizer fertilizer)
    {
        currentFertilizers.Add(fertilizer);
    }
    
    //Score related
    public void AddFieldScore(int amount)
    {
        currentScore.Fields += amount;
    }
    
    public void AddPointScore(int amount)
    {
        currentScore.Points += amount;
    }
    
    public void AddSpecialFieldScore(int amount)
    {
        currentScore.SpecialFields += amount;
    }
    

}
