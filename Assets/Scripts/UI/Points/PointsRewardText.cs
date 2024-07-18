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
    private List<EventManager.GameEvents.LevelEvents.ScoreChangedArgs> scoreQueue = new();
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
        Debug.Log($"{gridVisualization.OwnGridTile} received a {args.ScoreAdded.EcoPoints} of {args.ScoringOrigin.ToString()}");
        scoreQueue.Add(args);
    }

    public void AnimateText(EventManager.GameEvents.LevelEvents.ScoreChangedArgs args)
    {
        SetUpText(args);
        // animationSequence.Append(scoreText.DOFade(1f, Constants.UI_POINT_SPEED));
        Debug.Log($"STARTING TWEEEN {gridVisualization.OwnGridTile} with {args.ScoreAdded.EcoPoints} of {args.ScoringOrigin.ToString()}");
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

    public void SetUpText(EventManager.GameEvents.LevelEvents.ScoreChangedArgs args)
    {
        scoreText.gameObject.SetActive(true);
        scoreText.transform.position = originalPosition;
        string preFix = "";
        scoreText.color = Constants.NEUTRAL_GRAY_COLOR;
        if (args.ScoreAdded.EcoPoints > 0)
        {
            preFix = "+";
            scoreText.color = Constants.POSITIVE_GREEN_COLOR;
        }
        else if (args.ScoreAdded.EcoPoints < 0)
        {
            preFix = "";
            scoreText.color = Constants.NEGATIVE_RED_COLOR;
        }

        if (args.ScoringOrigin == GameManager.SCORING_ORIGIN.MULTIPLICATION)
        {
            scoreText.color = Constants.MULTIPLICATION_COLOR;
        }

        scoreText.DOFade(0f, 0f);
        scoreText.text = preFix + args.ScoreAdded.EcoPoints.ToString() + " ";
    }

}
