using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
[ExecuteInEditMode]
public class WiggleAnimation : MonoBehaviour
{
    [SerializeField] private float strength = 10f;

    [SerializeField] private float speed = 1f;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Y;
    [SerializeField] private bool localMove = false;
    private float topPosition;
    private float bottomPosition;
    private Vector3? ogPosition;
    private Tween animationTween;

    public enum MoveDirection
    {
        X,
        Y
    }

    // private void OnValidate()
    // {
    //     StartAnimation();
    // }

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!ogPosition.HasValue)
        {
            ogPosition = rectTransform.position;
        }

        StartAnimation();
    }

    public void StartAnimation()
    {
        rectTransform.position = ogPosition.Value;
        if (animationTween != null)
        {
            animationTween.Kill();
            animationTween = null;
        }
        if (moveDirection == MoveDirection.Y)
        {
            

            if (!localMove)
            {
                topPosition = rectTransform.position.y + strength;
                bottomPosition = rectTransform.position.y - strength;
                rectTransform.position = new Vector3(rectTransform.position.x, topPosition, rectTransform.position.z);
                animationTween = rectTransform.DOMoveY(bottomPosition, speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }
            else
            {
                topPosition = rectTransform.localPosition.y + strength;
                bottomPosition = rectTransform.localPosition.y - strength;
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, topPosition, rectTransform.localPosition.z);
                animationTween = rectTransform.DOLocalMoveY(bottomPosition, speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

            }
        }
        else
        {
            topPosition = rectTransform.position.x + strength;
            bottomPosition = rectTransform.position.x - strength;
            rectTransform.position = new Vector3(topPosition, rectTransform.position.y, rectTransform.position.z);
            animationTween = rectTransform.DOMoveX(bottomPosition, speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}