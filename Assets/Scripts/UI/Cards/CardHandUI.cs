using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class CardHandUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardUI cardUI;
    [SerializeField] private Canvas cardCanvas;
    [SerializeField] private float targetPosFollowDuration = .5f;
    private int normalSortingOrder;
    private Vector3 cardTargetPosition;

    public Vector3 CardTargetPosition
    {
        get => cardTargetPosition;
        set => cardTargetPosition = value;
    }

    public int NormalSortingOrder
    {
        get => normalSortingOrder;
        set => normalSortingOrder = value;
    }

    [SerializeField] private int hoveredSortingLayer;

    public CardUI CardUI
    {
        get => cardUI;
        set => cardUI = value;
    }

    public Transform DiscardPileLocation { get; set; }
    public Transform DrawPileLocation { get; set; }

    [Header("Card Mouse Hover")] [SerializeField]
    private Transform CardMouseHoverTransform;

    [SerializeField] private CanvasGroup trailRenderer;
    [SerializeField] private float animationDuration = 2;
    private bool cardClickEnabled = true;
    private bool canPlayCard = true;
    private bool cardSelected = false;
    private Vector3 originalPosition;
    private Tween shakeTween;
    private Quaternion ogRotation;
    private Vector3 originalScale;
    private bool animating = false;
    
    private Tween cardUpTween;

    private void Awake()
    {
        SetPosition();
        PutCardInBack();
        UIUtils.InitFadeState(trailRenderer);

        originalScale = transform.localScale;
        Debug.Log($"Original Scale: {originalScale}");
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
            Debug.Log($"Card index is {cardUI.CardIndex}");
            CardsUIController.Instance.SelectCard(this);
        }
        else
        {
            cardSelected = false;
            CardsUIController.Instance.DeselectCard(this);
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

    public void PutCardInBack()
    {
        if (cardCanvas == null) return;
        cardCanvas.sortingOrder = normalSortingOrder;
    }


    private void OnSecondMoveStillOpen()
    {
        Debug.Log("Second Move still open triggerd");
        if (shakeTween != null) shakeTween.Complete();
        ogRotation = transform.rotation;
        shakeTween = transform
            .DOShakeRotation(Constants.UI_FAST_FADE_SPEED, new Vector3(0, 0, 10f), vibrato: 10,
                randomnessMode: ShakeRandomnessMode.Harmonic).OnComplete(() =>
            {
                transform.DORotateQuaternion(ogRotation, .1f);
            });
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

    public void SetTargetPosition(Vector3 newTargetPosition)
    {
        cardTargetPosition = newTargetPosition;
        // transform.DOMove(newTargetPosition, targetPosFollowDuration).SetEase(Ease.InOutSine);
    }

    private void Update()
    {
        float speed = 5f;
        if (IsDisabled())return;
        if (animating) return;
        if (Vector3.Distance(cardTargetPosition, transform.position) > 0.00001f)
        {
            Vector3 direction = cardTargetPosition - transform.position;
            transform.position = Vector3.Lerp(transform.position, cardTargetPosition, speed * Time.deltaTime);
        }
    }

    public void SetCardLayer(int i)
    {
        normalSortingOrder = i;
        PutCardInBack();
    }

    public Sequence PlayDiscardAnimation()
    {
        cardUI.CardIndex = -1;
        // trailRenderer.transform.SetParent(null);
        // trailRenderer.transform.DOMove(DiscardPileLocation.position, .25f).SetEase(Ease.InOutSine);
        Debug.Log($"Playing discard animation of card {cardUI.CardIndex}");
        Sequence discardSequence = DOTween.Sequence();
        discardSequence
            .OnStart(()=>
            {
                animating = true;
                UIUtils.FadeStandard(trailRenderer, true);
            })
            .Append(transform.DOMove(DiscardPileLocation.position, animationDuration))
            .JoinCallback(() => transform.DOScale(0f, animationDuration * .7f).SetDelay(animationDuration *.3f))
            .SetEase(Ease.InOutCubic)
            .AppendCallback(() =>
            {
                animating = false;
                UIUtils.FadeStandard(trailRenderer, false);

                Debug.Log($"Setting card of index {CardUI.CardIndex} deactive from animation");
            })
            ;
        return discardSequence;

    }

    public Sequence PlayDrawAnimation()
    {
        Sequence drawSequence = DOTween.Sequence();
        drawSequence.OnStart(() =>
            {
                transform.localScale = Vector3.zero;
                transform.position = DrawPileLocation.position;
                Debug.Log($"Setting card of index {CardUI.CardIndex} active from animation");
                animating = true;
                gameObject.SetActive(true);
                trailRenderer.DOFade(0f, 0f);
            })
            .Append(transform.DOMove(cardTargetPosition, animationDuration))
            .Join(transform.DOScale(originalScale, animationDuration * .5f))
            .SetEase(Ease.InOutCubic)
            .AppendCallback(() =>
            {
                animating = false;
                UIUtils.FadeStandard(trailRenderer, false);

            })
            ;
        return drawSequence;
    }

    

    

    public bool IsDisabled()
    {
        return cardUI.CardIndex == -1;
    }
}