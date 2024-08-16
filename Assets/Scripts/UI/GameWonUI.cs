using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWonUI : MonoBehaviour
{
    [SerializeField] private Button gameOverButton;
    [SerializeField] private CanvasGroup gameWonCanvas;
    [SerializeField] private CanvasGroup unlockTextCanvasGroup;
    [SerializeField] private TextMeshProUGUI unlockText;

    private void Awake()
    {
        gameOverButton.onClick.AddListener(GameOver);
        UIUtils.InitFadeState(gameWonCanvas);
        // UIUtils.InitFadeState(unlockTextCanvasGroup);

    }

    private void OnEnable()
    {
        EventManager.Game.Level.OnPlanetProgressionWon += OnPlanetProgressionWon;
        EventManager.Game.GameSettings.OnDeckUnlocked += OnDeckUnlocked;

    }

    private void OnDeckUnlocked(EventManager.GameEvents.GameSettingsEvents.DeckUnlockedArgs arg0)
    {
        Debug.Log("Deck Event is listened");
        UIUtils.FadeStandard(unlockTextCanvasGroup, true);
        unlockText.text = $"Unlocked {arg0.UnlockedDeck.StartDeckName}!";
    }

    private void OnDisable()
    {
        EventManager.Game.Level.OnPlanetProgressionWon -= OnPlanetProgressionWon;
    }

    private void OnPlanetProgressionWon()
    {
        UIUtils.FadeStandard(gameWonCanvas, true);

    }

    private void GameOver()
    {
        GameManager.Instance.GameOver();
        UIUtils.FadeStandard(gameWonCanvas, false);

    }
}
