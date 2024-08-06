using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class TipScreenUI : MonoBehaviour
{
    public enum TipType
    {
        MultiplyField,
        CardsEmpty,
        ThirdTurn,
        ExtraMove,
        CardSkip
    }

    [Serializable]
    public class TipItem
    {
        public Transform tipPosition;
        public TipType tipType;
        [TextArea] public string tipText;

        [NonSerialized] public bool TipShowed;
    }

    [Header("Tips")] [SerializeField] private List<TipItem> tips;

    [Header("References")] [SerializeField]
    private CanvasGroup tipCanvas;

    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private float animationSpeed = .3f;
    [SerializeField] private RectTransform tipScreenRect;

    public static List<TipType> tipMemory = new();

    public List<TipType> tipQueue = new ();
    private bool blockQueue;
    private bool tipCanBeClosed;

    private void Start()
    {
        continueButton.onClick.AddListener(UnBlockQueue);
    }

    private void OnEnable()
    {
        EventManager.Game.Level.OnLevelInitialized += OnLevelInitialized;
        EventManager.Game.Level.OnSecondMoveSuccessful += OnSecondMoveNeeded;
        EventManager.Game.Level.OnTurnChanged += OnTurnChanged;
        EventManager.Game.Level.OnShuffeDiscardPileIntoDrawPile += OnShuffeDiscardPileIntoDrawPile;
        EventManager.Game.UI.OnCardFirstSkipEvent += OnCardFirstSkipEvent;
    }

    private void OnCardFirstSkipEvent()
    {
        QueueTip(TipType.CardSkip);
    }

    private void OnShuffeDiscardPileIntoDrawPile()
    {
        QueueTip(TipType.CardsEmpty);
    }

    private void OnTurnChanged(EventManager.GameEvents.LevelEvents.TurnChangedArgs arg0)
    {
        if (arg0.TurnNumber != 3) return;
        QueueTip(TipType.ThirdTurn);
    }

    private void OnSecondMoveNeeded()
    {
        QueueTip(TipType.ExtraMove);
        EventManager.Game.Level.OnSecondMoveSuccessful -= OnSecondMoveNeeded;
    }

    private void OnLevelInitialized(EventManager.GameEvents.LevelEvents.LevelInitializedArgs arg0)
    {
        if (GridManager.Instance.Grid.SpecialFields.Any((x) => x.FieldType == SpecialFieldType.MULTIPLY))
        {
            QueueTip(TipType.MultiplyField);
            EventManager.Game.Level.OnLevelInitialized -= OnLevelInitialized;
        }
        else
        {
        CloseTip(true);
            
        }
            
    }

    public void QueueTip(TipType tipToShow)
    {
        tipQueue.Add(tipToShow);
    }
    

    public void ShowTip(TipType tipToShow)
    {
        TipItem tipItem = tips.FirstOrDefault((x) => x.tipType == tipToShow);

        if (tipItem == null) return;
        if (tipMemory.Contains(tipToShow))
        {
            return;
        }

        Debug.Log($"Tip {tipToShow} showed");
        tipScreenRect.position = tipItem.tipPosition.position;
        tipText.text = tipItem.tipText;
        tipCanvas.gameObject.SetActive(true);
        tipCanvas.DOFade(1f, animationSpeed);
        EventManager.Game.UI.OnBlockGamePlay?.Invoke(true);
        tipMemory.Add(tipToShow);
    }

    private void CloseTip(bool instant = false)
    {
        tipCanBeClosed = false;
        float speed = instant ? 0f : animationSpeed;
        tipCanvas.DOFade(0f, speed).OnComplete(() => { tipCanvas.gameObject.SetActive(false); });
        EventManager.Game.UI.OnBlockGamePlay?.Invoke(false);
    }

    private void UnBlockQueue()
    {
        blockQueue = false;
        tipCanBeClosed = true;

    }

    private void Update()
    {
        if (tipQueue.Count == 0 && !blockQueue && tipCanBeClosed)
        {
            CloseTip();
        }
        if (tipQueue.Count != 0 && ! blockQueue)
        {
            ShowTip(tipQueue[0]);
            tipQueue.RemoveAt(0);
            blockQueue = true;
            tipCanBeClosed = false;

        }

        
    }
}