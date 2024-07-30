using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardsUIController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsParent;
    [SerializeField] private Transform hoveredCardsTransform;
    private List<CardHandUI> currentCards = new();
    private int activePlantIndex = -1;

    private List<int> activeWisdoms = new();

    //Hovering Effect
    private GridTile oldGridTile;
    private GridTile currentGridTile;
    private float clickCooldownTimer = 0;

    public enum State
    {
        SelectCard,
        PlacePlant,
        SecondMove
    }

    private State currentState = State.SelectCard;
    public static CardsUIController Instance { get; private set; }

    private void OnEnable()
    {
        EventManager.Game.Level.OnDrawCards += OnDrawCards;
        EventManager.Game.Level.OnUpdateCards += OnUpdateCards;
        EventManager.Game.UI.OnSecondMoveNeeded += OnSecondMoveNeeded;
        EventManager.Game.UI.OnPlantPlanted += OnPlantPlanted;
        EventManager.Game.Level.OnTurnChanged += OnTurnChanged;
        EventManager.Game.Input.OnCancel += GameInputOnCancel;
        EventManager.Game.Input.OnInteract += GameInputOnInteract;
        EventManager.Game.Level.EndSingleCardPlay += EndSingleCardPlay;
    }

    private void EndSingleCardPlay()
    {
        SwitchState(State.SelectCard);
        GameInputOnCancel();
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
        GameInputOnCancel();
    }

    private void OnSecondMoveNeeded(EventManager.GameEvents.UIEvents.OnSecondMoveNeededArgs neededArgs)
    {
        SwitchState(State.SecondMove);
        Debug.Log("EDITOR INITIALIZED");
        foreach (CardHandUI cardUI in currentCards)
        {
            cardUI.SetActiveState(false);
        }

        EventManager.Game.UI.OnPlantHoverCanceled?.Invoke();
        bool secondMovePlayable = false;
        GridManager.Instance.Grid.ForEachGridTile((gridTile) =>
        {
            if (neededArgs.editorCardInstance.CheckField(new SecondMoveCallerArgs()
                {
                    playedTile = neededArgs.editorOriginGridTile,
                    selectedGridTile = gridTile,
                    CallingCardInstance = neededArgs.editorCardInstance,
                    EditorCallingCardInstance = neededArgs.editorCardInstance,
                    callerType = CALLER_TYPE.SECOND_MOVE
                }))
            {
                EventManager.Game.UI.OnHoverForEditor?.Invoke(
                    new EventManager.GameEvents.UIEvents.OnHoverForEditorArgs()
                    {
                        hoveredGridTile = gridTile
                    });
                secondMovePlayable = true;
            }
        });
        if (!secondMovePlayable)
        {
            GameInputOnCancel();
        }
    }

    private void GameInputOnInteract()
    {
        // if (clickCooldownTimer > 0) return;
        // clickCooldownTimer = Constants.CLICK_COOLDOWN;
        if (currentState == State.PlacePlant)
        {
            GridTile gridTile = GridManager.Instance.Grid.GetGridObject(Mouse.current.position.ReadValue());
            if (gridTile != null && activePlantIndex != -1)
            {
                GameManager.Instance.TryQueueLifeform(activePlantIndex, gridTile);
            }
        }

        if (currentState == State.SecondMove)
        {
            GridTile selectedGridTile = GridManager.Instance.Grid.GetGridObject(Mouse.current.position.ReadValue());
            Debug.Log(selectedGridTile);
            if (selectedGridTile != null)
            {
                GameManager.Instance.ExecuteSecondMove(selectedGridTile);
            }
        }
    }


    private void GameInputOnCancel()
    {
        //If right click
        Debug.Log("GAME INPUT ON CANCEL IS CALLED");
        if (currentState == State.SecondMove)
        {
            GameManager.Instance.CancelSecondMove();
        }

        activePlantIndex = -1;
        currentState = State.SelectCard;
        EventManager.Game.UI.OnPlantHoverCanceled?.Invoke();
        GameManager.Instance.RemoveAllWisdoms();
        activeWisdoms.Clear();
        if (currentCards.Count >= 0)
            foreach (CardHandUI cardUI in currentCards)
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
            CardHandUI cardHandUI = newCard.GetComponent<CardHandUI>();
            cardHandUI.CardUI.CardIndex = i;
            currentCards.Add(cardHandUI);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnDrawCards(EventManager.GameEvents.DeckChangedArgs arg0)
    {
        if (arg0.ChangedDeck.MaxHandSize != currentCards.Count)
        {
            Debug.Log(arg0.ChangedDeck.MaxHandSize);
            InitCards(arg0.ChangedDeck.MaxHandSize);
        }

        ;
        for (int i = 0; i < currentCards.Count; i++)
        {
            if (i < arg0.ChangedDeck.HandCards.Count)
            {
                currentCards[i].CardUI.SetCardUI(arg0.ChangedDeck.HandCards[i]);
                currentCards[i].OnManaChanged(null);
            }
            else
            {
                currentCards[i].CardUI.SetCardUI(null);
            }
        }
    }

    private void OnUpdateCards(EventManager.GameEvents.DeckChangedArgs args)
    {
        OnDrawCards(args);
    }

    public void SelectCard(int cardIndex)
    {
        if (currentCards[cardIndex].CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Wisdom)
        {
            HandleWisdomClick(cardIndex);
        }

        if (currentCards[cardIndex].CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Plant)
        {
            HandlePlantClick(cardIndex);
        }


        currentGridTile = null;
        currentCards[cardIndex].SetHoverState(true);

        EventManager.Game.UI.OnPlantHoverCanceled?.Invoke();
    }

    private void HandleWisdomClick(int cardIndex)
    {
        GameManager.Instance.AddWisdom(currentCards[cardIndex].CardUI.CardInstance);
        activeWisdoms.Add(cardIndex);
        DeselectAllOtherWisdomOfSameType(cardIndex);
    }

    private void HandlePlantClick(int cardIndex)
    {
        activePlantIndex = cardIndex;
        for (int i = 0; i < currentCards.Count; i++)
        {
            if (currentCards[i].CardUI.CardInstance == null) continue;
            if (currentCards[i].CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Plant &&
                i != cardIndex)
                currentCards[i].SetHoverState(false);
        }

        SwitchState(State.PlacePlant);
    }

    public void DeselectCard(int cardIndex)
    {
        //DESELECT ALL CARDS AT THE MOMENT, NEEDS TO CHANGE
        if (currentCards[cardIndex].CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Wisdom)
        {
            DeselectWisdom(cardIndex);
        }

        if (currentCards[cardIndex].CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Plant)
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
        GameManager.Instance.RemoveWisdom(currentCards[cardIndex].CardUI.CardInstance);
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
                GameManager.Instance.RemoveWisdom(currentCards[i].CardUI.CardInstance);
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
        if (currentCards[cardIndex].CardUI.CardInstance.CardData.EffectType != CardData.CardEffectType.Wisdom)
        {
            Debug.LogWarning("TRYING TO CHECK FOR A NON WISDOM CARD IN WISDOM LOOP");
            return false;
        }

        foreach (var index in activeWisdoms)
        {
            if (currentCards[index].CardUI.CardInstance.CardData.CardName ==
                GameManager.Instance.Deck.HandCards[cardIndex].CardData.CardName)
            {
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        clickCooldownTimer -= Time.deltaTime;

        if (currentState == State.PlacePlant && activePlantIndex != -1)
        {
            currentGridTile = GridManager.Instance.Grid.GetGridObject(Input.mousePosition);
            if (currentGridTile == null) return;
            if (currentGridTile != oldGridTile)
            {
                CardInstance hoveredCardInstance =
                    GameManager.Instance.GetTemporaryCardInstance(activePlantIndex);
                EventManager.Game.UI.OnPlantHoverChanged?.Invoke(
                    new EventManager.GameEvents.UIEvents.OnHoverChangedArgs()
                    {
                        hoveredCardInstance = hoveredCardInstance,
                        hoveredGridTile = currentGridTile,
                        hoverCallerArgs = GameManager.Instance.GetTemporaryCallerArgs(activePlantIndex, currentGridTile)
                    });
                oldGridTile = currentGridTile;
            }
        }
    }
}