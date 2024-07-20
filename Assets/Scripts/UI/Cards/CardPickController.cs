using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardPickController : MonoBehaviour
{
    [SerializeField] private GameObject cardPickCanvas;
    private List<AddableCardUI> addableCards = new();
    [SerializeField] private Transform cardsParent;
    [SerializeField] private GameObject addableCardPrefab;
    [SerializeField] private Button skipButton;

    private CardCollection cardCollection;
    private Button buttonThatCalled;

    public Button ButtonThatCalled
    {
        get => buttonThatCalled;
        set => buttonThatCalled = value;
    }

    private void OnEndLevel(EventManager.GameEvents.LevelEvents.LevelEndedArgs arg0)
    {
        InitLootCards();
    }

    private void Start()
    {
        EventManager.Game.UI.OnCardPickScreenChange += OnCardPickScreenChange;
        EventManager.Game.Level.OnEndLevel += OnEndLevel;
        cardPickCanvas.SetActive(false);
        skipButton.onClick.AddListener(SkipButton);
    }

    private void OnDestroy()
    {
        skipButton.onClick.RemoveListener(SkipButton);
    }

    private void SkipButton()
    {
        CloseCardController();
    }

    private void InitCards(CardCollection cardCollection)
    {
        while (addableCards.Count < cardCollection.CardsInCollection.Count)
        {
            GameObject newAddableCard = Instantiate(addableCardPrefab, cardsParent);
            AddableCardUI addableCardUI = newAddableCard.GetComponent<AddableCardUI>();
            addableCardUI.CardPickController = this;
            addableCards.Add(addableCardUI);
        }
    }

    private void InitLootCards()
    {
        cardCollection = CardLibrary.Instance.ReturnAmountOfCardInstances(Constants.AMOUNT_OF_PICK_CARDS);
    }

    private void OnCardPickScreenChange(EventManager.GameEvents.UIEvents.BoolArgs args)
    {
        buttonThatCalled = args.sender as Button;
        if (cardCollection == null)
        {
            Debug.LogWarning("Card Pick Window Not Initialized");
        }
        EventManager.Game.UI.OnChangeOtherCanvasesStatus(!args.status);
        cardPickCanvas.SetActive(args.status);
        if (args.status)
        {
            if (cardCollection == null)
            {
                InitLootCards();
            }

            InitCards(cardCollection);

            for (int i = 0; i < addableCards.Count; i++)
            {
                if (i < cardCollection.CardsInCollection.Count)
                {
                    addableCards[i].CardUI.SetCardUI(cardCollection.CardsInCollection[i]);
                }
                else
                {
                    addableCards[i].CardUI.SetCardUI(null);
                }
            }
        }
    }

    public void CardPicked()
    {
        cardCollection = null;
        buttonThatCalled.gameObject.SetActive(false);
        CloseCardController();
    }

    private void CloseCardController()
    {
        EventManager.Game.UI.OnCardPickScreenChange?.Invoke(new EventManager.GameEvents.UIEvents.BoolArgs()
        {
            status = false
        });
    }
}