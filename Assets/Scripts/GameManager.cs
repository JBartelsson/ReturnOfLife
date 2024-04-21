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
        public Plantable plantableReference;

        public PlantableCard(Plantable plantableReference)
        {
            this.plantableReference = plantableReference;
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
    private PlantableCard selectedCard;

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
                break;
            case GameState.SetPlant:
                break;
            case GameState.PlantEditor:
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
        foreach (StartDeckSO.DeckEntry deckEntry in startDeck.Deck)
        {
            for (int i = 0; i < deckEntry.amount; i++)
            {
                deck.Add(new PlantableCard(deckEntry.plantableReference));
                Debug.Log($"Added {deckEntry.plantableReference.visualization} to deck");
            }
        }
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
                GridTile gridTile = GridManager.Instance.Grid.GetGridObject(UtilsClass.GetMouseWorldPosition());
                if (selectedCard.plantableReference.ExecuteFunction(gridTile))
                {
                    PlayCard();
                } else
                {
                    Debug.Log("CANT EXECUTE FUNCTION THERE");
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

    private void PlayCard()
    {
        currentMana -= selectedCard.plantableReference.manaCost;
        currentHand.Remove(selectedCard);
        discardPile.Add(selectedCard);
        Debug.Log($"PLAYED: {selectedCard}");
        EndTurn();
    }

    private void EndTurn()
    {
        if (currentMana > 0)
        {
            SwitchState(GameState.DrawCards);
        } else
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
        for (int i = 0; i < currentHand.Count; i++)
        {
            Debug.Log($"HAND: Slot {i + 1}: {currentHand[i].plantableReference.visualization}");
        }
    }

    private void DrawSingleCard() {
        int randomIndex = UnityEngine.Random.Range(0, drawPile.Count);
        PlantableCard drawCard = drawPile[randomIndex];
        currentHand.Add(drawCard);
        drawPile.Remove(drawCard);

    }

}
