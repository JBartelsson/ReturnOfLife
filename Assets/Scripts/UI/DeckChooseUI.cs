using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckChooseUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private CanvasGroup deckPickcanvasGroup;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject buttonPrefab;

    private List<DeckChooseButton> deckChooseButtons = new ();
    private DeckChooseButton activeButton;

    private void OnEnable()
    {
        UIUtils.InitFadeState(deckPickcanvasGroup);
        EventManager.Game.UI.OnShowDeckPickCanvas += OnShowDeckPickCanvas;
    }

    private void OnShowDeckPickCanvas(bool state)
    {
        UIUtils.FadeStandard(deckPickcanvasGroup, state);
        if (state && deckChooseButtons.Count != 0)
        {
            activeButton = deckChooseButtons[0];
            activeButton.SetActiveState(true);
            GameSettings.Instance.SelectedStartDeck = activeButton.StartDeck;
        }
    }

    private void Start()
    {
        foreach (var startDeck in GameSettings.Instance.AvailableStartDecks)
        {
            GameObject buttonGameObject = Instantiate(buttonPrefab, buttonParent);
            DeckChooseButton deckChooseButton = buttonGameObject.GetComponent<DeckChooseButton>();
            Debug.Log($"{startDeck}, {deckChooseButton}");
            deckChooseButtons.Add(deckChooseButton);
            deckChooseButton.Init(startDeck, () =>
            {
                activeButton = deckChooseButton;
                deckChooseButtons.ForEach((x) => x.SetActiveState(false));
                activeButton.SetActiveState(true);
            });
        }
        startButton.onClick.AddListener(() =>
        {
            GameSettings.Instance.SelectedStartDeck = activeButton.StartDeck;
            Debug.Log($"Set Start Deck to {GameSettings.Instance.SelectedStartDeck}");
            SceneLoader.Load(SceneLoader.Scene.GameScene);
        });
        
        closeButton.onClick.AddListener(() =>
        {
            EventManager.Game.UI.OnShowDeckPickCanvas?.Invoke(false);
        });
    }
}
