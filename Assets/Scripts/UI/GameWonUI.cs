using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWonUI : MonoBehaviour
{
    [SerializeField] private Button gameOverButton;
    [SerializeField] private Canvas gameWonCanvas;
    [SerializeField] private CanvasGroup unlockTextCanvasGroup;
    [SerializeField] private TextMeshProUGUI unlockText;

    private void Awake()
    {
        gameOverButton.onClick.AddListener(GameOver);
        gameWonCanvas.gameObject.SetActive(false);
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
        gameWonCanvas.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        GameManager.Instance.GameOver();
        gameWonCanvas.gameObject.SetActive(false);
    }
}
