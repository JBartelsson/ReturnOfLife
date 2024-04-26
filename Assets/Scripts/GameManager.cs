using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GameManager;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    [Serializable]
    public class PlantableCard
    {
        public Plantable plantable;

        public PlantableCard(Plantable plantableReference)
        {
            this.plantable = plantableReference;
        }
    }


    public enum GameState
    {
        Init, DrawCards, StartTurn, SelectCards, SetPlant, PlantEditor, SpecialAbility, LevelEnd, None
    }
    private List<PlantableCard> deck = new();
    [Header("Game Options")]
    [SerializeField] private StartDeckSO startDeck;
    [SerializeField] private int handSize = 5;
    [SerializeField] private int standardMana = 3;
    [SerializeField] private int standardTurns = 3;
    private GameState currentGameState = GameState.None;
    private List<PlantableCard> currentHand = new();
    private List<PlantableCard> drawPile = new();
    private List<PlantableCard> discardPile = new();
    private int currentMana = 0;
    private int currentTurns = 0;
    private int currentPlayedCards = 0;
    private bool selectedPlantNeedNeighbor = false;
    private PlantableCard selectedCard;
    private GridTile playedTile;

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
                    Debug.Log($"HAND: Slot {i + 1}: {currentHand[i].plantable.visualization}");
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
                Debug.Log($"Added {deckEntry.plantableReference.visualization} to deck");
            }
        }
    }

    private void InitEditor()
    {
        GridManager.Instance.Grid.ForEachGridTile((x) =>
        {
            if (selectedCard.plantable.PlantEditor.CheckField(new EditorCallerArgs()
            {
                playedTile = playedTile,
                selectedGridTile = x
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
                SelectCard(currentHand[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectCard(currentHand[1]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectCard(currentHand[2]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SelectCard(currentHand[3]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SelectCard(currentHand[4]);
            }
        }
        if (currentGameState == GameState.SetPlant)
        {

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("SET PLANT CLICK");

                GridTile gridTile = GridManager.Instance.Grid.GetGridObject(UtilsClass.GetMouseWorldPosition());
                if (selectedCard.plantable.ExecuteFunction(gridTile, selectedPlantNeedNeighbor))
                {
                    PlayCard(gridTile);
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
                EditorCallerArgs editorArgs = new EditorCallerArgs()
                {
                    playedTile = playedTile,
                    selectedGridTile = selectedGridTile,
                    callingPlantable = selectedCard.plantable
                };
                if (selectedCard.plantable.PlantEditor.CheckField(editorArgs))
                {
                    selectedCard.plantable.PlantEditor.ExecuteEditor(editorArgs);
                    EndCardPlaying();
                }
                else
                {
                    Debug.Log("CANT EXECUTE EDITOR FUNCTION THERE");
                }
            }
        }
    }

    private void SelectCard(PlantableCard plantableCard)
    {
        selectedCard = plantableCard;
        Debug.Log($"selected: {selectedCard}");
        SwitchState(GameState.SetPlant);
    }

    private void PlayCard(GridTile selectedTile)
    {
        this.playedTile = selectedTile;
        currentMana -= selectedCard.plantable.manaCost;
        currentHand.Remove(selectedCard);
        discardPile.Add(selectedCard);
        currentPlayedCards++;
        selectedPlantNeedNeighbor = true;

        Debug.Log($"PLAYED: {selectedCard}");
        if (selectedCard.plantable.PlantEditor != null)
        {
            SwitchState(GameState.PlantEditor);
            return;
        }
        EndCardPlaying();
    }

    private void EndCardPlaying()
    {
        GridManager.Instance.Grid.ForEachGridTile((x) => x.ChangeMarkedStatus(false));
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

}
