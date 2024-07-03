using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndUIController : MonoBehaviour
{
    
    [SerializeField] private Button nextButton;
    [SerializeField] private Button gameOverButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Canvas endLevelCanvas;
    [SerializeField] private Color gameOverColor;
    [SerializeField] private Color winColor;
    [SerializeField] private Image bgImage;
    private void Start()
    {
        endLevelCanvas.gameObject.SetActive(false);
        EventManager.Game.Level.OnEndLevel += OnEndLevel;
        nextButton.onClick.AddListener(NextLevel);
        gameOverButton.onClick.AddListener(GameOver);
    }

    private void OnDestroy()
    {
        nextButton.onClick.RemoveListener(NextLevel);
        nextButton.onClick.RemoveListener(GameOver);
    }

    private void OnEndLevel(EventManager.GameEvents.LevelEvents.LevelEndedArgs args)
    {
        gameOverButton.gameObject.SetActive(!args.WonLevel);
        nextButton.gameObject.SetActive(args.WonLevel);
        statusText.text = $"{args.CurrentScore}/{args.NeededScore}";
        endLevelCanvas.gameObject.SetActive(true);
        if (args.WonLevel)
        {
            headerText.text = "Success!";
            bgImage.color = winColor;
        }
        else
        {
            headerText.text = "Too bad!";
            bgImage.color = gameOverColor;

        }
    }

    private void NextLevel()
    {
        GameManager.Instance.NextLevel();
        endLevelCanvas.gameObject.SetActive(false);

    }
    
    private void GameOver()
    {
        GameManager.Instance.GameOver();
        endLevelCanvas.gameObject.SetActive(false);

    }
}
