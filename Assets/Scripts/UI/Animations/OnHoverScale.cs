using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleValue = 1.08f;
    [SerializeField] private float scaleSpeed = .1f;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(scaleValue * originalScale.x, scaleSpeed);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, scaleSpeed);

    }
}
