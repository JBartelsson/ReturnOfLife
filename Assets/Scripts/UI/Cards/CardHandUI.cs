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
    private int normalSortingLayer;

    public int NormalSortingLayer
    {
        get => normalSortingLayer;
        set => normalSortingLayer = value;
    }

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
    private Tween shakeTween;
    private Quaternion ogRotation;
    

    private Tween cardUpTween;

    private void Start()
    {
        SetPosition();
        PutCardInBack();
    }

    private void OnEnable()
    {
        EventManager.Game.Level.OnManaChanged += OnManaChanged;
        EventManager.Game.Level.OnWisdomChanged += OnWisdomChanged;
    }
    private void SetPosition()
    {
        Debug.Log(cardUI.CardParent.position);
        originalPosition = cardUI.CardParent.localPosition;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"CARDCLICKENABLED IS {cardClickEnabled}");
        if (!cardClickEnabled)
        {
            EventManager.Game.UI.OnSecondMoveStillOpen?.Invoke();
            OnSecondMoveStillOpen();
            return;
        }
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
            if (cardUI.CardInstance.CardData.WisdomType != WisdomType.Basic)
            {
                PutCardInFront();
            }
            else
            {
                PutCardInFrontWisdom();
            }

        }
        else
        {
            cardUI.BackgroundSprite.material = null;
            OnPointerExit(null);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardUpTween = cardUI.CardParent.DOLocalMove(CardMouseHoverTransform.localPosition, .3f);
        PutCardInFront();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardSelected) return;
        PutCardInBack();
        cardUI.CardParent.DOLocalMove(originalPosition, .3f);
    }

    private void PutCardInFront()
    {
        if (cardCanvas == null) return;
        cardCanvas.sortingOrder = hoveredSortingLayer + 1;
    }
    private void PutCardInFrontWisdom()
    {
        if (cardCanvas == null) return;
        cardCanvas.sortingOrder = hoveredSortingLayer;
    }

    private void PutCardInBack()
    {
        if (cardCanvas == null) return;
        cardCanvas.sortingOrder = normalSortingLayer;
    }
    
    

    private void OnSecondMoveStillOpen()
    {
        Debug.Log("Second Move still open triggerd");
        if (shakeTween != null) shakeTween.Complete(); 
        ogRotation = transform.rotation;
        shakeTween = transform.DOShakeRotation(Constants.UI_FAST_FADE_SPEED, new Vector3(0,0, 10f), vibrato: 10, randomnessMode: ShakeRandomnessMode.Harmonic).OnComplete(() =>
        {
            transform.DORotateQuaternion(ogRotation, .1f);
        });
    }


    private void OnDisable()
    {
        
    }

    private void OnWisdomChanged(EventManager.GameEvents.LevelEvents.WisdomChangedArgs arg0)
    {
        foreach (var arg0CurrentWisdom in arg0.currentWisdoms)
        {
            Debug.Log($"Wisdom changed! {arg0CurrentWisdom}");
        }
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
        if (!GameManager.Instance.AddWisdomsAndCheckMana(cardUI.CardInstance))
        {
            cardUI.CostDrop.sprite = cardUI.DropSpriteRed;
            canPlayCard = true;
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
                
            }
        }
    }
}
