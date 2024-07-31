using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipScreenUI : MonoBehaviour
{
    public enum TipType
    {
        MultiplyField,
        CardsEmpty,
        ThirdTurn,
        ExtraMove
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

    public static List<TipType> tipMemory = new();

    private void Start()
    {
        continueButton.onClick.AddListener(CloseTip);
    }

    private void OnEnable()
    {
        EventManager.Game.Level.OnLevelInitialized += OnLevelInitialized;
        EventManager.Game.Level.OnSecondMoveSuccessful += OnSecondMoveNeeded;
    }

    private void OnSecondMoveNeeded()
    {
        ShowTip(TipType.ExtraMove);
        EventManager.Game.Level.OnSecondMoveSuccessful -= OnSecondMoveNeeded;
    }

    private void OnLevelInitialized(EventManager.GameEvents.LevelEvents.LevelInitializedArgs arg0)
    {
        if (GridManager.Instance.Grid.SpecialFields.Any((x) => x.FieldType == SpecialFieldType.MULTIPLY))
        {
            ShowTip(TipType.MultiplyField);
            EventManager.Game.Level.OnLevelInitialized -= OnLevelInitialized;
            return;
        }
        CloseTip();
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
        tipText.text = tipItem.tipText;
        tipCanvas.gameObject.SetActive(true);
        tipCanvas.DOFade(1f, animationSpeed);
        EventManager.Game.UI.OnBlockGamePlay?.Invoke(true);
        tipMemory.Add(tipToShow);
    }

    private void CloseTip()
    {
        tipCanvas.DOFade(0f, animationSpeed).OnComplete(() => { tipCanvas.gameObject.SetActive(false); });
        EventManager.Game.UI.OnBlockGamePlay?.Invoke(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowTip(TipType.MultiplyField);
        }
    }
}