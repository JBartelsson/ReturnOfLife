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
    //Hovering Effect
    private GridTile oldGridTile;
    private GridTile currentGridTile;
    public static CardsUIController Instance { get; private set; }
    private void OnEnable()
    {
        EventManager.Game.Level.OnDrawCards += OnDrawCards;
        EventManager.Game.Level.OnUpdateCards += OnUpdateCards;
        GameInput.Instance.OnCancelAction += GameInputOnCancel;
        GameInput.Instance.OnInteractAction += GameInputOnInteract;
    }

    private void GameInputOnInteract(object sender, EventArgs e)
    {
        Debug.Log($"PLAYING CARD {activeCardIndex}");
        GridTile gridTile = GridManager.Instance.Grid.GetGridObject(Mouse.current.position.ReadValue());
        if (gridTile != null || activeCardIndex == -1)
        {
            GameManager.Instance.PlantCard(activeCardIndex, gridTile);
            GameInputOnCancel(this, EventArgs.Empty);
        }
    }


    private void GameInputOnCancel(object sender, EventArgs e)
    {
        Debug.Log("CANCEL HOVER");
        activeCardIndex = -1;
        EventManager.Game.UI.OnPlantHoverCanceled?.Invoke();
        foreach (CardUI cardUI in currentCards)
        {
            cardUI.SetHoverState(false);
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


    private void OnDisable()
    {
        EventManager.Game.Level.OnDrawCards -= OnDrawCards;
        EventManager.Game.Level.OnUpdateCards -= OnUpdateCards;
        GameInput.Instance.OnCancelAction -= GameInputOnCancel;

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
        activeCardIndex = cardIndex;
        
    }

    private void Update()
    {
        if (activeCardIndex != -1)
        {
            currentGridTile = GridManager.Instance.Grid.GetGridObject(Input.mousePosition);
            if (currentGridTile != oldGridTile)
            {
                EventManager.Game.UI.OnPlantHoverChanged?.Invoke(new EventManager.GameEvents.UIEvents.OnHoverChangedArgs()
                {
                    hoveredCardInstance = GameManager.Instance.CurrentHand[activeCardIndex],
                    hoveredGridTile = currentGridTile
                });
            }
        }
    }
}
