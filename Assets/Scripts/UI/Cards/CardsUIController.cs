using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CardsUIController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsParent;
    [SerializeField] private Transform hoveredCardsTransform;
    [SerializeField] private Transform discardPileLocation;
    [SerializeField] private Transform drawPileLocation;
    [SerializeField] private Button discardButton;
    [SerializeField] private TextMeshProUGUI discardLeftText;
    [SerializeField] private CardsUIContainer cardsUIContainer;
    
    private List<CardHandUI> currentCards = new();
    private int activePlantIndex = -1;

    private List<CardHandUI> activeWisdoms = new();

    //Hovering Effect
    private GridTile oldGridTile;
    private GridTile currentGridTile;
    private float clickCooldownTimer = 0;
    private bool gameplayBlocked = false;
    private CardInstance hoveredCardInstance;
    public enum State
    {
        SelectCard,
        PlacePlant,
        SecondMove
    }

    private State currentState = State.SelectCard;
    public static CardsUIController Instance { get; private set; }

    public enum AnimationType
    {
        Discard,
        Draw,
        None
    }

    private AnimationType currentAnimationPlaying = AnimationType.None;
    private List<AnimationItem> animationQueue = new();
    private bool blockQueue = false;

    public class AnimationItem
    {
        public Sequence Animation;
        public AnimationType AnimationType;

        public AnimationItem(Sequence animation, AnimationType animationType)
        {
            Animation = animation;
            AnimationType = animationType;
        }
    }
    
    private void OnEnable()
    {
        EventManager.Game.Level.OnDrawCard += OnDrawCard;
        EventManager.Game.Level.OnDiscardCard += OnDiscardCard;
        EventManager.Game.UI.OnSecondMoveNeeded += OnSecondMoveNeeded;
        EventManager.Game.UI.OnEndSingleCardPlay += OnPlantPlanted;
        EventManager.Game.Level.OnTurnChanged += OnTurnChanged;
        EventManager.Game.Input.OnCancel += GameInputOnCancel;
        EventManager.Game.Input.OnInteract += GameInputOnInteract;
        EventManager.Game.Level.OnEndSingleCardPlay += EndSingleCardPlay;
        EventManager.Game.UI.OnBlockGamePlay += OnBlockGamePlay;
        EventManager.Game.Level.OnDiscardUsed += OnDiscardUsed;
        discardButton.onClick.AddListener(() =>
        {
            DiscardCard();
        });
    }

   

    private void OnDiscardUsed(int discardsLeft)
    {
        discardLeftText.text = $"Discard ({discardsLeft})";
        if (discardsLeft == 0)
        {
            discardButton.interactable = false;
        }
        else
        {
            discardButton.interactable = true;

        }
    }

    private void OnBlockGamePlay(bool status)
    {
        gameplayBlocked = status;
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

    private void OnPlantPlanted()
    {
        CancelPlaying();
    }

    private void DiscardCard()
    {
        if (activePlantIndex != -1 )
        {
            GameManager.Instance.DiscardCard(activePlantIndex);
            GameInputOnCancel();
        }
        else
        {
            if (activeWisdoms.Count != 0)
            {
                GameManager.Instance.DiscardCard(activeWisdoms[0].CardUI.CardIndex);
                GameInputOnCancel();

            }
        }
        
    }

    private void OnSecondMoveNeeded(EventManager.GameEvents.UIEvents.OnSecondMoveNeededArgs neededArgs)
    {
        SwitchState(State.SecondMove);
        Debug.Log("EDITOR INITIALIZED");
        foreach (CardHandUI cardUI in currentCards)
        {
            cardUI.SetActiveState(false);
        }

        EventManager.Game.UI.OnLifeformHoverCanceled?.Invoke();
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
                EventManager.Game.UI.OnHoverForSecondMove?.Invoke(
                    new EventManager.GameEvents.UIEvents.OnHoverForEditorArgs()
                    {
                        hoveredGridTile = gridTile
                    });
                secondMovePlayable = true;
            }
        });
        Debug.Log($"Second Move is {secondMovePlayable}!");
        if (!secondMovePlayable)
        {
            CancelPlaying(true);
            return;
        }
        EventManager.Game.Level.OnSecondMoveSuccessful?.Invoke();
    }

    private void GameInputOnInteract()
    {
        if (gameplayBlocked) return;
        // if (clickCooldownTimer > 0) return;
        // clickCooldownTimer = Constants.CLICK_COOLDOWN;
        if (currentState == State.PlacePlant)
        {
            if (!GameManager.Instance.AddWisdomsAndCheckMana(GameManager.Instance.Deck.HandCards[activePlantIndex])) return;
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
        CancelPlaying();
        GameManager.Instance.RemoveAllWisdoms();
        activeWisdoms.Clear();
    }

    private void CancelPlaying(bool overwriteEditorBlock = false)
    {
        if (gameplayBlocked && !overwriteEditorBlock) return;
        //If right click
        Debug.Log("GAME INPUT ON CANCEL IS CALLED");
        if (currentState == State.SecondMove)
        {
            GameManager.Instance.CancelSecondMove();
        }

        activePlantIndex = -1;
        currentState = State.SelectCard;
        EventManager.Game.UI.OnLifeformHoverCanceled?.Invoke();
        
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
        Debug.Log("NEW CARD UI PREFABS ARE BEING GENERATED");
        for (int i = 0; i < handSize ; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardsParent);
            CardHandUI cardHandUI = newCard.GetComponent<CardHandUI>();
            cardHandUI.CardUI.gameObject.SetActive(false);
            cardHandUI.transform.position = new Vector3(0, -10000, 0);
            cardHandUI.NormalSortingOrder = handSize - i;
            cardHandUI.CardUI.CardIndex = -1;
            cardHandUI.DiscardPileLocation = discardPileLocation;
            cardHandUI.DrawPileLocation = drawPileLocation;
            cardHandUI.PutCardInBack();
            currentCards.Add(cardHandUI);
        }
    }

    private CardHandUI GetEmptyHandCardUI()
    {
        return currentCards.FirstOrDefault((cardHandUI => cardHandUI.CardUI.CardIndex == -1));
    }
    private void AddToAnimationQueue(Sequence animationSequence, AnimationType animationType)
    {
        animationQueue.Add(new AnimationItem(animationSequence, animationType));
        Debug.Log($"Queuing {animationType} Animation");
        PlayNextQueueItem();
    }
    private void PlayNextQueueItem(bool force = false)
    {
        if (force)
        {
            animationQueue.RemoveAt(0);
            if (animationQueue.Count == 0) blockQueue = false;
        }
        Debug.Log($"Animation queue count {animationQueue.Count}, blockqueue: {blockQueue}, force: {force}");
        if (animationQueue.Count != 0)
        {
            if (blockQueue && !force) return;
            
            animationQueue.Sort((item, animationItem) =>
            {
                return ((int)item.AnimationType).CompareTo((int)animationItem.AnimationType);
            });
            Debug.Log("Showing Animation Queue");
            foreach (var animationItem in animationQueue)
            {
                Debug.Log($"{animationItem.AnimationType}");
            }
            Debug.Log("End of Animation Queue");

            animationQueue[0].Animation.OnComplete(() =>
            {
                Debug.Log("Callback worked!");
                blockQueue = false;
                PlayNextQueueItem(true);
            });
            blockQueue = true;
            Debug.Log($"Playing a {animationQueue[0].AnimationType} animation at time {Time.time}");
            animationQueue[0].Animation.Play();
        }
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private void OnDrawCard(EventManager.GameEvents.LevelEvents.DeckChangedArgs deckChangedArgs)
    {
        if (currentCards.Count < GameManager.Instance.Deck.MaxHandSize)
        {
            InitCards(GameManager.Instance.Deck.MaxHandSize);
        }

        StartCoroutine(SetCardUI(deckChangedArgs, deckChangedArgs.ChangedDeck.HandCards.Last(), deckChangedArgs.ChangedDeck.HandCards.Count() - 1));
        
    }

    private IEnumerator SetCardUI(EventManager.GameEvents.LevelEvents.DeckChangedArgs deckChangedArgs, CardInstance card, int index)
    {
        while (blockQueue) yield return null;
        cardsUIContainer.CalculatePositions();
        CardHandUI cardHandUI = GetEmptyHandCardUI();
        if (cardHandUI != null)
        {
            cardHandUI.CardUI.SetCardUI(card);
            cardHandUI.CardUI.CardIndex = index;
            cardHandUI.OnManaChanged(null);
            for (int i = 0; i < currentCards.Count; i++)
            
            {
                SetCardTargetPosition(currentCards[i]);
            }
            AddToAnimationQueue(cardHandUI.PlayDrawAnimation(), AnimationType.Draw);
            Debug.Log($"SHOULD PLAY DRAW ANIMATION OF INDEX {cardHandUI.CardUI.CardIndex}");
        }
        else
        {
            Debug.Log("THERE ARE ALREADY MAX Cards and Drawing was possible in UI. FIX THIS!");
        }
    }

    private void OnDiscardCard(EventManager.GameEvents.LevelEvents.DiscardCardArgs arg0)
    {
        cardsUIContainer.CalculatePositions();
        Debug.Log($"DISCARDING CARD {arg0.DiscardedIndex}");

        for (var i = 0; i < currentCards.Count; i++)
        {
            if (currentCards[i].IsDisabled()) continue;
            if (currentCards[i].CardUI.CardIndex == arg0.DiscardedIndex)
            {
                AddToAnimationQueue(currentCards[i].PlayDiscardAnimation(), AnimationType.Discard);
                continue;
            }
            if (currentCards[i].CardUI.CardIndex > arg0.DiscardedIndex)
            {
                currentCards[i].CardUI.CardIndex--;
            }

            SetCardTargetPosition(currentCards[i]);
        }
    }

    public void SelectCard(CardHandUI cardHandUI)
    {
        EventManager.Game.UI.OnLifeformHoverCanceled?.Invoke();
        if (cardHandUI.CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Wisdom)
        {
            HandleWisdomClick(cardHandUI.CardUI.CardIndex);
        }

        if (cardHandUI.CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Plant)
        {
            HandlePlantClick(cardHandUI.CardUI.CardIndex);
        }
        
        EventManager.Game.UI.OnCardSelected?.Invoke(cardHandUI.CardUI.CardInstance);
        //Set Green arrows and stuff
        if (activePlantIndex != -1)
        {
            CallerArgs tempCallerArgs = GameManager.Instance.GetTemporaryCallerArgs(activePlantIndex, null);
            CardInstance tempCardInstance = GameManager.Instance.GetTemporaryCardInstance(activePlantIndex);
            GridManager.Instance.Grid.ForEachGridTile((gridTile) =>
            {
                tempCallerArgs.playedTile = gridTile;
                CardInstance cardInstanceOnGridTile = gridTile.CardInstance;
                bool canBeExecuted = tempCardInstance.CanExecute(tempCallerArgs);

                EventManager.Game.UI.OnCardSelectGridTileUpdate?.Invoke(
                    new EventManager.GameEvents.UIEvents.CardSelectGridUpdateArgs()
                    {
                        Status = canBeExecuted,
                        UpdatedTile = gridTile
                    });
            });
            
        }

        currentGridTile = null;
        cardHandUI.SetHoverState(true);

    }

    private void SetCardTargetPosition(CardHandUI cardHandUI)
    {
        cardHandUI.SetCardLayer(GameManager.Instance.Deck.MaxHandSize - cardHandUI.CardUI.CardIndex);
        cardHandUI.SetTargetPosition(cardsUIContainer.GetCardTargetPositionByIndex(cardHandUI.CardUI.CardIndex));
    }

    private void HandleWisdomClick(int cardIndex)
    {
        CardHandUI currentWisdom = currentCards.FirstOrDefault((x) => x.CardUI.CardIndex == cardIndex);
        if (currentWisdom == null)
        {
            Debug.Log("WISDOM WASNT FOUND!");
            return;
        }
        GameManager.Instance.AddWisdom(currentWisdom.CardUI.CardInstance);
        activeWisdoms.Add(currentWisdom);
        DeselectAllOtherWisdomOfSameType(cardIndex);
        GameManager.Instance.AddMana(0);
    }

    private void HandlePlantClick(int cardIndex)
    {
        activePlantIndex = cardIndex;
        for (int i = 0; i < currentCards.Count; i++)
        {
            if (currentCards[i].CardUI.CardInstance == null) continue;
            if (currentCards[i].CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Plant &&
                currentCards[i].CardUI.CardIndex != cardIndex)
                currentCards[i].SetHoverState(false);
        }

        
        hoveredCardInstance =
            GameManager.Instance.GetTemporaryCardInstance(activePlantIndex);
        SwitchState(State.PlacePlant);
    }

    public void DeselectCard(CardHandUI cardHandUI)
    {
        //DESELECT ALL CARDS AT THE MOMENT, NEEDS TO CHANGE
        if (cardHandUI.CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Wisdom)
        {
            DeselectWisdom(cardHandUI.CardUI.CardIndex);
        }

        if (cardHandUI.CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Plant)
        {
            DeselectPlant(cardHandUI.CardUI.CardIndex);
        }
    }

    private void DeselectPlant(int cardIndex)
    {
        activePlantIndex = -1;
        currentState = State.SelectCard;
        currentCards[cardIndex].SetHoverState(false);
        EventManager.Game.UI.OnLifeformHoverCanceled?.Invoke();
    }

    private void DeselectWisdom(int cardIndex)
    {
        CardHandUI currentWisdom = currentCards.FirstOrDefault((x) => x.CardUI.CardIndex == cardIndex);
        if (currentWisdom == null) return;
        GameManager.Instance.RemoveWisdom(currentWisdom.CardUI.CardInstance);
        //Invoke Mana Change again for Red Drop Sprite
        activeWisdoms.Remove(currentWisdom);
        currentWisdom.SetHoverState(false);
        GameManager.Instance.AddMana(0);
        EventManager.Game.UI.OnLifeformHoverCanceled?.Invoke();
    }

    private void DeselectAllOtherWisdomOfSameType(int cardIndex)
    {
        for (int i = 0; i < currentCards.Count; i++)
        {
            if (currentCards[i].CardUI.CardInstance.CardData.EffectType == CardData.CardEffectType.Wisdom &&
                currentCards[i].CardUI.CardIndex != cardIndex)
            {
                GameManager.Instance.RemoveWisdom(currentCards[i].CardUI.CardInstance);
                activeWisdoms.RemoveAll((x) => x.CardUI.CardIndex == i);
                currentCards[i].SetHoverState(false);
            }
        }
    }

    private bool WisdomInCardStack()
    {
        // foreach (var index in activeWisdoms)
        // {
        //     if (GameManager.Instance.Deck.HandCards[index].CardData.EffectType == CardData.CardEffectType.Wisdom)
        //     {
        //         return true;
        //     }
        // }

        return false;
    }

    private bool WisdomAlreadyPlayed()
    {
        // foreach (var index in activeWisdoms)
        // {
        //     if (GameManager.Instance.Deck.HandCards[index].CardData.EffectType == CardData.CardEffectType.Wisdom)
        //     {
        //         return true;
        //     }
        // }

        return false;
    }

    private bool PlantInCardStack()
    {
        // foreach (var index in activeWisdoms)
        // {
        //     if (GameManager.Instance.Deck.HandCards[index].CardData.EffectType == CardData.CardEffectType.Plant)
        //     {
        //         return true;
        //     }
        // }

        return false;
    }

    private bool SameWisdomAlreadyInStack(int cardIndex)
    {
        if (currentCards[cardIndex].CardUI.CardInstance.CardData.EffectType != CardData.CardEffectType.Wisdom)
        {
            Debug.LogWarning("TRYING TO CHECK FOR A NON WISDOM CARD IN WISDOM LOOP");
            return false;
        }

        // foreach (var index in activeWisdoms)
        // {
        //     if (currentCards[index].CardUI.CardInstance.CardData.CardName ==
        //         GameManager.Instance.Deck.HandCards[cardIndex].CardData.CardName)
        //     {
        //         return true;
        //     }
        // }

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
                
                EventManager.Game.UI.OnLifeformHoverChanged?.Invoke(
                    new EventManager.GameEvents.UIEvents.OnLifeformChangedArgs()
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