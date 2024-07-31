using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHandUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardUI cardUI;
    [SerializeField] private Canvas cardCanvas;
    [SerializeField] private int normalSortingLayer;
    [SerializeField] private int hoveredSortingLayer;
    public CardUI CardUI
    {
        get => cardUI;
        set => cardUI = value;
    }

    [Header("Card Mouse Hover")] [SerializeField]
    private Transform CardMouseHoverTransform;
    private bool cardClickEnabled = true;
    private bool canPlayCard = true;
    private bool cardSelected = false;
    private Vector3 originalPosition;
    

    private Tween cardUpTween;

    private void Start()
    {
        SetPosition();
        cardCanvas.sortingOrder = normalSortingLayer;
    }

    private void SetPosition()
    {
        Debug.Log(cardUI.CardParent.position);
        originalPosition = cardUI.CardParent.localPosition;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"CARDCLICKENABLED IS {cardClickEnabled}");
        if (!cardClickEnabled) return;
        if (!canPlayCard) return;
        //IF not left click
        if (eventData.pointerId != -1) return;
        if (!cardSelected)
        {
            cardSelected = true;
            CardsUIController.Instance.SelectCard(cardUI.CardIndex);
        }
        else
        {
            cardSelected = false;
            CardsUIController.Instance.DeselectCard(cardUI.CardIndex);
        }
    }

    public void SetHoverState(bool state = true)
    {
        cardSelected = state;
        if (state)
        {
            cardUI.BackgroundSprite.material = cardUI.HoverMaterial;
            cardCanvas.sortingOrder = hoveredSortingLayer;
        }
        else
        {
            cardUI.BackgroundSprite.material = null;
            cardCanvas.sortingOrder = normalSortingLayer;
            OnPointerExit(null);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardUpTween = cardUI.CardParent.DOLocalMove(CardMouseHoverTransform.localPosition, .3f);
        cardCanvas.sortingOrder = hoveredSortingLayer;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardSelected) return;
        cardCanvas.sortingOrder = normalSortingLayer;
        cardUI.CardParent.DOLocalMove(originalPosition, .3f);
    }
    
    private void OnEnable()
    {
        EventManager.Game.Level.OnManaChanged += OnManaChanged;
        EventManager.Game.Level.OnWisdomChanged += OnWisdomChanged;
    }


    private void OnDisable()
    {
        
    }

    private void OnWisdomChanged(EventManager.GameEvents.LevelEvents.WisdomChangedArgs arg0)
    {
        if (arg0.currentWisdoms.Any((wisdom) => wisdom.CardData.WisdomType == WisdomType.Basic))
        {
            cardUI.SetCardUI(cardUI.CardInstance, true);
        }
        else
        {
            cardUI.SetCardUI(cardUI.CardInstance, false);
        }
    }

    public void OnManaChanged(EventManager.GameEvents.LevelEvents.ManaChangedArgs arg0)
    {
        if (cardUI.CardInstance == null) return;
        if (!GameManager.Instance.EnoughMana(cardUI.CardInstance.GetCardStats().PlayCost))
        {
            cardUI.CostDrop.sprite = cardUI.DropSpriteRed;
            canPlayCard = false;
        }
        else
        {
            canPlayCard = true;
            cardUI.CostDrop.sprite = cardUI.DropSpriteBlue;
        }
    }


    public void SetActiveState(bool state)
    {
        cardClickEnabled = state;
        // OnPointerExit(null);
    }

    private void Update()
    {
        if (cardSelected)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                GameManager.Instance.Deck.DiscardCard(cardUI.CardIndex);
                GameManager.Instance.Deck.DrawCards(1);
            }
        }
    }
}
