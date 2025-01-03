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
    [SerializeField] private TextMeshProUGUI rewardsText;
    [SerializeField] private Button pickACardButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Image statusImage;
    [SerializeField] private Sprite happyNadi;
    [SerializeField] private Sprite sadgeNadi;
    [SerializeField] private CanvasGroup endLevelCanvas;
    [SerializeField] private Color gameOverColor;
    [SerializeField] private Color winColor;
    [SerializeField] private Image bgImage;
    private static bool cardAddUsed = false;

    private void Start()
    {
        UIUtils.InitFadeState(endLevelCanvas);
        EventManager.Game.Level.OnEndLevel += OnEndLevel;
        EventManager.Game.Level.OnCardAdded += OnCardAdded;
        nextButton.onClick.AddListener(NextLevel);
        gameOverButton.onClick.AddListener(GameOver);
        pickACardButton.onClick.AddListener(PickACardClick);
    }

    private void OnCardAdded(CardInstance arg0)
    {
        cardAddUsed = true;
    }

    private void PickACardClick()
    {
        EventManager.Game.UI.OnCardPickScreenChange?.Invoke(new EventManager.GameEvents.UIEvents.BoolArgs()
        {
            sender = pickACardButton,
            status = true
        });
    }

    private void OnDestroy()
    {
        nextButton.onClick.RemoveListener(NextLevel);
        nextButton.onClick.RemoveListener(GameOver);
        pickACardButton.onClick.RemoveListener(PickACardClick);
    }

    private void OnEndLevel(EventManager.GameEvents.LevelEvents.LevelEndedArgs args)
    {
        gameOverButton.gameObject.SetActive(!args.WonLevel);
        nextButton.gameObject.SetActive(args.WonLevel);
        rewardsText.gameObject.SetActive(args.WonLevel);
        pickACardButton.gameObject.SetActive(args.WonLevel);
        UIUtils.FadeStandard(endLevelCanvas, true);
        statusText.text = (args.WonLevel ? "<color=#009711>" : "<color=red>") + $"{args.TotalScore}</color>/{args.NeededScore}";
        if (args.WonLevel)
        {
            headerText.text = "Spledongl\napproves!";
            bgImage.color = winColor;
            statusImage.sprite = happyNadi;
        }
        else
        {
            headerText.text = "This field\nremains toxic";
            bgImage.color = gameOverColor;
            statusImage.sprite = sadgeNadi;
        }
    }

    private void NextLevel()
    {
        if (!cardAddUsed)
        {
            EventManager.Game.UI.OnCardFirstSkipEvent?.Invoke();
            cardAddUsed = true;
            return;
        }

        GameManager.Instance.NextLevel();
        UIUtils.FadeStandard(endLevelCanvas, false);

    }

    private void GameOver()
    {
        Debug.Log("Clicked Game OVer Button");
        GameManager.Instance.GameOver();
        UIUtils.FadeStandard(endLevelCanvas, false);
    }

    private void OnDisable()
    {
        Debug.Log("Level End got disabled");
    }
}