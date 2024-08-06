using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class UIUtils
{
    public static void FadeStandard(CanvasGroup canvasGroup, bool status)
    {
        Fade(canvasGroup, status, Constants.UI_FADE_SPEED);
    }

    public static void InitFadeState(CanvasGroup canvasGroup)
    {
        canvasGroup.gameObject.SetActive(false);
        canvasGroup.DOFade(0f, 0f);
    }
    
    public static void Fade(CanvasGroup canvasGroup, bool status, float animationSpeed)
    {
        if (!status)
        {
            canvasGroup.DOFade(0f, animationSpeed).OnComplete(() =>
            {
                canvasGroup.gameObject.SetActive(false);
            });
        }
        else
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.DOFade(1f, animationSpeed);
        }
    }
}
