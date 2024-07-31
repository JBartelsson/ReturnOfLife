using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecondMoveUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup secondMoveCanvasGroup;
    [SerializeField] private TextMeshProUGUI secondMoveText;
    [SerializeField] private Image secondMoveSpriteRenderer;

    private void Start()
    {
        secondMoveCanvasGroup.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Game.UI.OnSecondMoveNeeded += OnSecondMoveNeeded;
        EventManager.Game.UI.OnSecondMoveQueueEmpty += OnSecondMoveQueueEmpty;
    }

    private void OnSecondMoveQueueEmpty()
    {
        HideWindow();
    }

    private void OnSecondMoveNeeded(EventManager.GameEvents.UIEvents.OnSecondMoveNeededArgs arg0)
    {
        Debug.Log(arg0.editorCardInstance);
        Debug.Log(arg0.editorCardInstance.CardData);
        Debug.Log(arg0.editorCardInstance.CardData.PlantSprite);
        secondMoveSpriteRenderer.sprite = arg0.editorCardInstance.CardData.PlantSprite;
        secondMoveText.text = arg0.editorCardInstance.CardData.CardName + " " + arg0.SecondMoveCallerArgs.SecondMoveNumber + "x";
        secondMoveCanvasGroup.gameObject.SetActive(true);
        secondMoveCanvasGroup.DOFade(1f, Constants.UI_FAST_FADE_SPEED).SetEase(Ease.OutQuad);
    }

    private void HideWindow()
    {
        secondMoveCanvasGroup.DOFade(0f, Constants.UI_FADE_SPEED)
            .OnComplete(()=> secondMoveCanvasGroup.gameObject.SetActive(false));
    }
}
