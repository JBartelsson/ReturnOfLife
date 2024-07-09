using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardsUIController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsParent;
    private List<CardUI> currentCards = new();
    private int activePlantIndex = -1;

    private List<int> activeWisdoms = new();
    //Hovering Effect
    private GridTile oldGridTile;
    private GridTile currentGridTile;

    public enum State
    {
        SelectCard, PlacePlant, Editor
    }

    private State currentState = State.SelectCard;
    public static CardsUIController Instance { get; private set; }
    private void OnEnable()
    {
        EventManager.Game.Level.OnDrawCards += OnDrawCards;
        EventManager.Game.Level.OnUpdateCards += OnUpdateCards;
        EventManager.Game.UI.OnEditorNeeded += OnEditorNeeded;
        EventManager.Game.UI.OnPlantPlanted += OnPlantPlanted;
        EventManager.Game.Level.OnTurnChanged += OnTurnChanged;
        EventManager.Game.Input.OnCancel += GameInputOnCancel;
        EventManager.Game.Input.OnInteract += GameInputOnInteract;
    }

    private void OnTurnChanged(EventManager.GameEvents.LevelEvents.TurnChangedArgs arg0)
    {
        GameInputOnCancel();
    }

    private void SwitchState(State newState)
    {
        currentState = newState;
    }

    private void OnPlantPlanted(EventManager.GameEvents.UIEvents.OnPlantPlantedArgs arg0)
    {
        currentState = State.SelectCard;
        GameInputOnCancel();
    }

    private void OnEditorNeeded(EventManager.GameEvents.UIEvents.OnEditorNeededArgs args)
    {
        SwitchState(State.Editor);
        Debug.Log("EDITOR INITIALIZED");
        foreach (CardUI cardUI in currentCards)
        {
            cardUI.SetActiveState(false);
        }
        GridManager.Instance.Grid.ForEachGridTile((gridTile) =>
        {
            if (args.editorCardInstance.CheckField(new EditorCallerArgs()
                {
                    playedTile = args.editorOriginGridTile,
                    selectedGridTile = gridTile,
                    CallingCardInstance = args.editorCardInstance,
                    EditorCallingCardInstance = args.editorCardInstance,
                    callerType = CALLER_TYPE.EDITOR
                }))
            {
                EventManager.Game.UI.OnHoverForEditor?.Invoke(new EventManager.GameEvents.UIEvents.OnHoverForEditorArgs()
                {
                    hoveredGridTile = gridTile
                });
            }
        });
    }

    private void GameInputOnInteract()
    {
        if (currentState == State.PlacePlant)
        {
            GridTile gridTile = GridManager.Instance.Grid.GetGridObject(Mouse.current.position.ReadValue());
            if (gridTile != null && activePlantIndex != -1)
            {
                GameManager.Instance.TryPlantCard(activePlantIndex, gridTile);
            }
        }

        if (currentState == State.Editor)
        {
            GridTile selectedGridTile = GridManager.Instance.Grid.GetGridObject(Mouse.current.position.ReadValue());
            if (selectedGridTile != null)
            {
                GameManager.Instance.ExecuteEditor(selectedGridTile);
            }
        }
        
    }


    private void GameInputOnCancel()
    {
        //If right click
        Debug.Log("GAME INPUT ON CANCEL IS CALLED");
        //Editor is uncancelable
        if (currentState == State.Editor) return;
        activePlantIndex = -1;
        currentState = State.SelectCard;
        EventManager.Game.UI.OnPlantHoverCanceled?.Invoke();
        GameManager.Instance.RemoveAllWisdoms();
        activeWisdoms.Clear();
        if (currentCards.Count >= 0)
        foreach (CardUI cardUI in currentCards)
        {
            cardUI.SetHoverState(false);
            cardUI.SetActiveState(true);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void InitCards(int handSize)
    {
        int currentCardSize = currentCards.Count;
        Debug.Log("NEW CARD UI PREFABS ARE BEING GENERATED");
        for (int i = 0; i < handSize - currentCardSize; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardsParent);
            CardUI cardUI = newCard.GetComponent<CardUI>();
            cardUI.CardIndex = i;
            currentCards.Add(cardUI);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnDrawCards(EventManager.GameEvents.DeckChangedArgs arg0)
    {
        if (arg0.ChangedDeck.HandCards.Count != currentCards.Count)
        {
            InitCards(arg0.ChangedDeck.HandCards.Count);
        };
        for (int i = 0; i < currentCards.Count; i++)
        {
            if (i < arg0.ChangedDeck.HandCards.Count)
            {
                currentCards[i].SetCardUI(arg0.ChangedDeck.HandCards[i]);
            }
            else
            {
                currentCards[i].SetCardUI(null);
            }
        }
    }
    
    private void OnUpdateCards(EventManager.GameEvents.DeckChangedArgs args)
    {
        OnDrawCards(args);
    }

    public void SelectCard(int cardIndex)
    {
        if (currentCards[cardIndex].CardInstance.CardData.EffectType == CardData.CardEffectType.Wisdom)
        {
            HandleWisdomClick(cardIndex);
           
        }
        if (currentCards[cardIndex].CardInstance.CardData.EffectType == CardData.CardEffectType.Plant)
        {
            HandlePlantClick(cardIndex);
           
        }
        

        currentGridTile = null;
        currentCards[cardIndex].SetHoverState(true);
        EventManager.Game.UI.OnPlantHoverCanceled?.Invoke();
       
    }

    private void HandleWisdomClick(int cardIndex)
    {
        GameManager.Instance.AddWisdom(currentCards[cardIndex].CardInstance);
        activeWisdoms.Add(cardIndex);
        DeselectAllOtherWisdomOfSameType(cardIndex);
    }

    private void HandlePlantClick(int cardIndex)
    {
        activePlantIndex = cardIndex;
        for (int i = 0; i < currentCards.Count; i++)
        {
            if (currentCards[i].CardInstance == null) continue;
            if (currentCards[i].CardInstance.CardData.EffectType == CardData.CardEffectType.Plant && i != cardIndex)
                currentCards[i].SetHoverState(false);
        }
        SwitchState(State.PlacePlant);
    }

    public void DeselectCard(int cardIndex)
    {
        //DESELECT ALL CARDS AT THE MOMENT, NEEDS TO CHANGE
        if (currentCards[cardIndex].CardInstance.CardData.EffectType == CardData.CardEffectType.Wisdom)
        {
            DeselectWisdom(cardIndex);
        }
        
        if (currentCards[cardIndex].CardInstance.CardData.EffectType == CardData.CardEffectType.Plant)
        {
            DeselectPlant(cardIndex);
        }
    }

    private void DeselectPlant(int cardIndex)
    {
        activePlantIndex = -1;
        currentState = State.SelectCard;
        currentCards[cardIndex].SetHoverState(false);
        EventManager.Game.UI.OnPlantHoverCanceled?.Invoke();
    }

    private void DeselectWisdom(int cardIndex)
    {
        GameManager.Instance.RemoveWisdom(currentCards[cardIndex].CardInstance);
        activeWisdoms.Remove(cardIndex);
        currentCards[cardIndex].SetHoverState(false);
        EventManager.Game.UI.OnPlantHoverCanceled?.Invoke();
    }

    private void DeselectAllOtherWisdomOfSameType(int cardIndex)
    {
        for (int i = 0; i < GameManager.Instance.Deck.HandCards.Count; i++)
        {
            if (SameWisdomAlreadyInStack(i) && i != cardIndex)
            {
                GameManager.Instance.RemoveWisdom(currentCards[i].CardInstance);
                activeWisdoms.Remove(i);
                currentCards[i].SetHoverState(false);
            }
        }
            
    }
    private bool WisdomInCardStack()
    {
        foreach (var index in activeWisdoms)
        {
            if (GameManager.Instance.Deck.HandCards[index].CardData.EffectType == CardData.CardEffectType.Wisdom)
            {
                return true;
            }
        }

        return false;
    }

    private bool WisdomAlreadyPlayed()
    {
        foreach (var index in activeWisdoms)
        {
            if (GameManager.Instance.Deck.HandCards[index].CardData.EffectType == CardData.CardEffectType.Wisdom)
            {
                return true;
            }
        }

        return false;
    }

    private bool PlantInCardStack()
    {
        foreach (var index in activeWisdoms)
        {
            if (GameManager.Instance.Deck.HandCards[index].CardData.EffectType == CardData.CardEffectType.Plant)
            {
                return true;
            }
        }

        return false;
    }

    private bool SameWisdomAlreadyInStack(int cardIndex)
    {
        if (currentCards[cardIndex].CardInstance.CardData.EffectType != CardData.CardEffectType.Wisdom)
        {
            Debug.LogWarning("TRYING TO CHECK FOR A NON WISDOM CARD IN WISDOM LOOP");
            return false;
        }
        foreach (var index in activeWisdoms)
        {
            if (currentCards[index].CardInstance.CardData.CardName == GameManager.Instance.Deck.HandCards[cardIndex].CardData.CardName)
            {
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (currentState == State.PlacePlant && activePlantIndex != -1)
        {
            currentGridTile = GridManager.Instance.Grid.GetGridObject(Input.mousePosition);
            if (currentGridTile == null) return;
            if (currentGridTile != oldGridTile)
            {
                CardInstance hoveredCardInstance =
                    GameManager.Instance.GetTemporaryCardInstance(activePlantIndex);
                EventManager.Game.UI.OnPlantHoverChanged?.Invoke(new EventManager.GameEvents.UIEvents.OnHoverChangedArgs()
                {
                    hoveredCardInstance = hoveredCardInstance,
                    hoveredGridTile = currentGridTile,
                    hoverCallerArgs = GameManager.Instance.GetTemporaryCallerArgs(activePlantIndex, currentGridTile)
                });
                currentGridTile = oldGridTile;
            }
        }
    }
}
