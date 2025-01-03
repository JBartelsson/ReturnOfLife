using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PointsRewardText : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private GridVisualization gridVisualization;
    [SerializeField] private Transform textTarget;

    private Vector3 originalPosition;
    private Sequence animationSequence;
    private List<ScoreAnimationEntry> scoreQueue = new();

    public class ScoreAnimationEntry
    {
        public enum ScoreTypeEnum
        {
            EcoPoints, AdditiveMultiplication, MultMultiplication
        }

        public ScoreTypeEnum ScoreType;
        public float number;
        
    }
    private void OnEnable()
    {
        EventManager.Game.Level.OnScoreChanged += OnScoreChanged;
    }
    private void OnDisable()
    {
        EventManager.Game.Level.OnScoreChanged -= OnScoreChanged;
    }

    private void Start()
    {
        scoreText.gameObject.SetActive(false);
        textTarget.gameObject.SetActive(false);
        originalPosition = scoreText.transform.position;
        animationSequence = DOTween.Sequence();
    }

    private void OnScoreChanged(EventManager.GameEvents.LevelEvents.ScoreChangedArgs args)
    {
        if (args.scoreChangedCallerArgs == null) return;
        if (args.scoreChangedCallerArgs.playedTile != gridVisualization.OwnGridTile) return;
        if (args.ScoreAdded.EcoPoints != 0)
        {
            scoreQueue.Add(new ScoreAnimationEntry()
            {
                number = args.ScoreAdded.EcoPoints,
                ScoreType =  ScoreAnimationEntry.ScoreTypeEnum.EcoPoints
            });
        }
        if (args.ScoreAdded.Mult != 0)
        {
            scoreQueue.Add(new ScoreAnimationEntry()
            {
                number = args.ScoreAdded.Mult,
                ScoreType =  ScoreAnimationEntry.ScoreTypeEnum.AdditiveMultiplication
            });
        }
        if (args.ScoreAdded.MultMultiplier != 1f)
        {
            scoreQueue.Add(new ScoreAnimationEntry()
            {
                number = args.ScoreAdded.MultMultiplier,
                ScoreType =  ScoreAnimationEntry.ScoreTypeEnum.MultMultiplication
            });
        }
    }

    public void AnimateText(ScoreAnimationEntry scoreAnimationEntry)
    {
        SetUpText(scoreAnimationEntry);
        // animationSequence.Append(scoreText.DOFade(1f, Constants.UI_POINT_SPEED));
        animationSequence = DOTween.Sequence();
        animationSequence.Append(scoreText.DOFade(1f, Constants.UI_POINT_DISAPPEAR_SPEED));

        animationSequence.Join(scoreText.transform.DOMove(textTarget.position, Constants.UI_POINT_SPEED));
        animationSequence.Append(scoreText.DOFade(0f, Constants.UI_POINT_DISAPPEAR_SPEED));
        animationSequence.AppendInterval(Constants.UI_POINT_WAIT_INTERVAL);
        animationSequence.AppendCallback(() =>
        {
            scoreQueue.RemoveAt(0);
        });
        animationSequence.Play();
    }

    private void Update()
    {
        if (scoreQueue.Count != 0 && !animationSequence.IsPlaying())
        {
            AnimateText(scoreQueue[0]);
        }
    }

    public void SetUpText(ScoreAnimationEntry scoreAnimationEntry)
    {
        scoreText.gameObject.SetActive(true);
        scoreText.transform.position = originalPosition;
        string preFix = "";
        scoreText.color = Constants.NEUTRAL_GRAY_COLOR;
        if (scoreAnimationEntry.number > 0)
        {
            preFix = "+";
        }
        else if (scoreAnimationEntry.number < 0)
        {
            preFix = "-";
        }

        switch (scoreAnimationEntry.ScoreType)
        {
            case ScoreAnimationEntry.ScoreTypeEnum.EcoPoints:
                scoreText.color = Constants.ECO_POINTS_COLOR;
                scoreAnimationEntry.number = Mathf.FloorToInt(scoreAnimationEntry.number);
                break;
            case ScoreAnimationEntry.ScoreTypeEnum.AdditiveMultiplication:
                scoreText.color = Constants.MULTIPLICATION_COLOR;
                break;
            case ScoreAnimationEntry.ScoreTypeEnum.MultMultiplication:
                scoreText.color = Constants.MULTIPLICATION_COLOR;
                preFix = "x";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        scoreText.DOFade(0f, 0f);
        scoreText.text = preFix + scoreAnimationEntry.number.ToString() + " ";
    }

}
