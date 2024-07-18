using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHandUI : CardUI, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
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
    }

    private void SetPosition()
    {
        Debug.Log(cardParent.position);
        originalPosition = cardParent.localPosition;
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
            CardsUIController.Instance.SelectCard(_cardIndex);
        }
        else
        {
            cardSelected = false;
            CardsUIController.Instance.DeselectCard(_cardIndex);
        }
    }

    public void SetHoverState(bool state = true)
    {
        cardSelected = state;
        if (state)
        {
            _backgroundSprite.material = hoverMaterial;
        }
        else
        {
            _backgroundSprite.material = null;
            OnPointerExit(null);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardUpTween = cardParent.DOLocalMove(CardMouseHoverTransform.localPosition, .3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardSelected) return;
        cardParent.DOLocalMove(originalPosition, .3f);
    }
    
    private void OnEnable()
    {
        EventManager.Game.Level.OnManaChanged += OnManaChanged;
        EventManager.Game.Level.OnWisdomChanged += OnWisdomChanged;
    }


    private void OnDisable()
    {
        EventManager.Game.Level.OnManaChanged -= OnManaChanged;
        EventManager.Game.Level.OnWisdomChanged -= OnWisdomChanged;
    }

    private void OnWisdomChanged(EventManager.GameEvents.LevelEvents.WisdomChangedArgs arg0)
    {
        if (arg0.currentWisdoms.Any((wisdom) => wisdom.CardData.WisdomType == WisdomType.Basic))
        {
            SetCardUI(_cardInstance, true);
        }
        else
        {
            SetCardUI(_cardInstance, false);
        }
    }

    private void OnManaChanged(EventManager.GameEvents.LevelEvents.ManaChangedArgs arg0)
    {
        if (_cardInstance == null) return;
        if (!GameManager.Instance.EnoughMana(_cardInstance.GetCardStats().PlayCost))
        {
            CostDrop.sprite = _dropSpriteRed;
            canPlayCard = false;
        }
        else
        {
            canPlayCard = true;
            CostDrop.sprite = _dropSpriteBlue;
        }
    }


    public void SetActiveState(bool state)
    {
        cardClickEnabled = state;
        // OnPointerExit(null);
    }
}
