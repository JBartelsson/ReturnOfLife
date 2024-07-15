using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameWonUI : MonoBehaviour
{
    [SerializeField] private Button gameOverButton;
    [SerializeField] private Canvas gameWonCanvas;

    private void Start()
    {
        gameOverButton.onClick.AddListener(GameOver);
        gameWonCanvas.gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        EventManager.Game.Level.OnPlanetProgressionWon += OnPlanetProgressionWon;

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
