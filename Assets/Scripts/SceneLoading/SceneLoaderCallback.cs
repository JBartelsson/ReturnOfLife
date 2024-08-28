using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoaderCallback : MonoBehaviour
{
    bool isFirstUpdate = true;
    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
           LoadingAnimation();
        }
    }

    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private RectTransform wipeLeft;
    [SerializeField] private RectTransform wipeRight;
    [SerializeField] private RectTransform wipeLeftSecond;
    [SerializeField] private RectTransform wipeRightSecond;
    [SerializeField] private Image middleImage;

    private Sequence sequence;
    private Sequence endSequence;
    private Vector3 originalScale;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        wipeLeft.localScale = new Vector3(0, 1, 1);
        wipeRight.localScale = new Vector3(0, 1, 1);
        wipeRightSecond.localScale = new Vector3(1, 1, 1);
        wipeRightSecond.localScale = new Vector3(1, 1, 1);
        wipeRightSecond.gameObject.SetActive(false);
        wipeLeftSecond.gameObject.SetActive(false);
        originalScale = middleImage.transform.localScale;
        middleImage.transform.localScale = Vector3.zero;
    }

    private void LoadingAnimation()
    {
        sequence = DOTween.Sequence();
        sequence.Append(wipeLeft.DOScale(new Vector3(1, 1, 1), .25f))
            .Join(wipeRight.DOScale(new Vector3(1, 1, 1), .25f))
            .Append(middleImage.transform.DOScale(originalScale, 0.4f).SetEase(Ease.OutBack))
            .SetEase(Ease.InOutSine).OnComplete(()=>
            {
                EventManager.Game.UI.OnSceneTransition?.Invoke();
                SceneLoader.LoaderCallback();
            });
        sequence.Play();
        // fadeGroup.DOFade(1f, .25f).OnComplete(SceneLoader.LoaderCallback);

    }

    private void OnEnable()
    {
        EventManager.Game.SceneSwitch.OnSceneReloadComplete += OnSceneReloadComplete;
        fadeGroup.DOFade(0f, 0f);
    }

    private void OnSceneReloadComplete(EventManager.GameEvents.SceneReloadArgs arg0)
    {
        endSequence = DOTween.Sequence();
        fadeGroup.DOFade(0f, .25f);
        Debug.Log("Delete LoadingAnimation");
        EventManager.Game.SceneSwitch.OnSceneReloadComplete -= OnSceneReloadComplete;
        wipeLeft.pivot = new Vector2(1f, .5f);
        wipeRight.pivot = new Vector2(0f, .5f);
        wipeLeft.gameObject.SetActive(false);
        wipeRight.gameObject.SetActive(false);
        wipeRightSecond.gameObject.SetActive(true);
        wipeLeftSecond.gameObject.SetActive(true);
        endSequence
            .Append(middleImage.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack))
            .AppendInterval(.08f)
            .Join(wipeLeftSecond.DOScale(new Vector3(0, 1, 1), .25f))
            .Join(wipeRightSecond.DOScale(new Vector3(0, 1, 1), .25f))
            .SetEase(Ease.InOutSine)
        .AppendCallback(() => Destroy(this.gameObject));
        endSequence.Play();
    }

  
}