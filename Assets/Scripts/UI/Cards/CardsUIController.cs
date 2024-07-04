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
    private int activeCardIndex = -1;

    private List<int> activeCardStack = new();
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
        GameInputOnCancel();
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
            if (gridTile != null && activeCardIndex != -1)
            {
                GameManager.Instance.TryPlantCard(activeCardIndex, gridTile);
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
        //Editor is uncancelable
        if (currentState == State.Editor) return;
        activeCardIndex = -1;
        currentState = State.SelectCard;
        EventManager.Game.UI.OnPlantHoverCanceled?.Invoke();
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
        for (int i = 0; i < handSize - currentCardSize; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardsParent);
            CardUI cardUI = newCard.GetComponent<CardUI>();
            cardUI.CardIndex = i;
            currentCards.Add(cardUI);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnDrawCards(EventManager.GameEvents.Args arg0)
    {
        if (GameManager.Instance.HandSize != currentCards.Count)
        {
            InitCards(GameManager.Instance.HandSize);
        };
        for (int i = 0; i < currentCards.Count; i++)
        {
            if (i < GameManager.Instance.CurrentHand.Count)
            {
                currentCards[i].SetCardUI(GameManager.Instance.CurrentHand[i]);
            }
            else
            {
                currentCards[i].SetCardUI((CardData)null);
            }
        }
    }
    
    private void OnUpdateCards(EventManager.GameEvents.Args args)
    {
        OnDrawCards(args);
    }

    public void SelectCard(int cardIndex)
    {
        if (GameManager.Instance.CurrentHand[cardIndex].CardData.EffectType == CardData.CardEffectType.Wisdom)
        {
            activeCardStack.Add(cardIndex);
            DisableAllOtherWisdomOfSameType(cardIndex);
        }
        foreach (CardUI cardUI in currentCards)
        {
            cardUI.SetHoverState(false);
        }

        currentGridTile = null;
        currentCards[cardIndex].SetHoverState(true);
        EventManager.Game.UI.OnPlantHoverCanceled?.Invoke();
        activeCardIndex = cardIndex;
        SwitchState(State.PlacePlant);
    }

    public void DeselectCard(int cardIndex)
    {
        GameInputOnCancel();
    }

    private void DisableAllOtherWisdomOfSameType(int cardIndex)
    {
        foreach (var cardUI in currentCards)
        {
            if (SameWisdomAlreadyInStack(cardIndex))
            {
                cardUI.SetActiveState(false);
            }
        }
    }
    private bool WisdomInCardStack()
    {
        foreach (var index in activeCardStack)
        {
            if (GameManager.Instance.CurrentHand[index].CardData.EffectType == CardData.CardEffectType.Wisdom)
            {
                return true;
            }
        }

        return false;
    }

    private bool WisdomAlreadyPlayed()
    {
        foreach (var index in activeCardStack)
        {
            if (GameManager.Instance.CurrentHand[index].CardData.EffectType == CardData.CardEffectType.Wisdom)
            {
                return true;
            }
        }

        return false;
    }

    private bool PlantInCardStack()
    {
        foreach (var index in activeCardStack)
        {
            if (GameManager.Instance.CurrentHand[index].CardData.EffectType == CardData.CardEffectType.Plant)
            {
                return true;
            }
        }

        return false;
    }

    private bool SameWisdomAlreadyInStack(int cardIndex)
    {
        if (GameManager.Instance.CurrentHand[cardIndex].CardData.EffectType != CardData.CardEffectType.Wisdom)
        {
            Debug.LogWarning("TRYING TO CHECK FOR A NON WISDOM CARD IN WISDOM LOOP");
            return false;
        }
        foreach (var index in activeCardStack)
        {
            if (GameManager.Instance.CurrentHand[index].CardData.CardName == GameManager.Instance.CurrentHand[cardIndex].CardData.CardName)
            {
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (currentState == State.PlacePlant && activeCardIndex != -1)
        {
            currentGridTile = GridManager.Instance.Grid.GetGridObject(Input.mousePosition);
            if (currentGridTile == null) return;
            if (!PlantInCardStack()) return;
            if (currentGridTile != oldGridTile)
            {
                EventManager.Game.UI.OnPlantHoverChanged?.Invoke(new EventManager.GameEvents.UIEvents.OnHoverChangedArgs()
                {
                    hoveredCardInstance = GameManager.Instance.CurrentHand[activeCardIndex],
                    hoveredGridTile = currentGridTile
                });
                currentGridTile = oldGridTile;
            }
        }
    }
}
