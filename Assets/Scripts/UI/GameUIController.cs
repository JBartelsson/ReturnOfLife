using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnsText;
    [SerializeField] private TextMeshProUGUI manaText;
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI ecoText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI neededScoreText;

    [Header("Level name")] [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField] private Button tutorialButton;

    private void OnEnable()
    {
        EventManager.Game.Level.OnTurnChanged += UpdateTurn;
        EventManager.Game.Level.OnManaChanged += OnManaChanged;
        EventManager.Game.Level.OnScoreChanged += OnScoreChanged;
        EventManager.Game.Level.OnLevelInitialized += OnLevelInitialized;
        
    }

    private void Start()
    {
        tutorialButton.onClick.AddListener(() =>
        {
            EventManager.Game.UI.OnTutorialScreenChange?.Invoke(true);
        });
    }

    private void OnLevelInitialized(EventManager.GameEvents.LevelEvents.LevelInitializedArgs arg0)
    {
        levelText.text = $"Level {arg0.CurrentLevelNumber + 1}/{arg0.MaxLevelNumber}";
        neededScoreText.text = $"{arg0.CurrentLevel.NeededEcoPoints}";
    }

    private void OnScoreChanged(EventManager.GameEvents.LevelEvents.ScoreChangedArgs args)
    {
        ecoText.SetText($"{args.NewScore.EcoPoints.ToString()}x{args.NewScore.Mult.ToString()}");
    }

    private void OnManaChanged(EventManager.GameEvents.LevelEvents.ManaChangedArgs args)
    {
        manaText.SetText(args.NewMana.ToString());
    }



    private void UpdateTurn(EventManager.GameEvents.LevelEvents.TurnChangedArgs args)
    {
        turnsText.SetText(args.TurnNumber.ToString() + "/3");
        ecoText.SetText($"{GameManager.Instance.TurnScore.EcoPoints.ToString()}x{GameManager.Instance.TurnScore.Mult.ToString()}");

        currentScoreText.text = $"{GameManager.Instance.TotalScore}";

    }

    public void EndTurn()
    {
        GameManager.Instance.EndTurn();
    }
}
