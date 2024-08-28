using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSettings : PersistentSingleton<GameSettings>
{
    public StartDeckSO SelectedStartDeck { get; set; }
    [SerializeField] private List<StartDeckSO> availableStartDecks;

    [Header("Debug")] [SerializeField] private StartDeckSO debugStartDeck;
    [SerializeField] private bool debug;
    [SerializeField] private bool unlockAllStartDecksDebug;
    public List<StartDeckSO> AvailableStartDecks => availableStartDecks;

    protected override void Awake()
    {
        base.Awake();
        SelectedStartDeck = availableStartDecks[0];
#if UNITY_EDITOR
        if (debug)
        {
            SelectedStartDeck = debugStartDeck;
        }
#endif
    }

    private void OnEnable()
    {
        EventManager.Game.SceneSwitch.OnSceneReloadComplete += OnSceneReloadComplete;
        foreach (var startDeck in availableStartDecks)
        {
            startDeck.Unlocked = startDeck.UnlockAtStart;
        }
#if UNITY_EDITOR
        if (unlockAllStartDecksDebug)
        {
            UnlockAllStartDecks();
        }


#endif
    }

    private void UnlockAllStartDecks()
    {
        foreach (StartDeckSO startDeckSo in availableStartDecks)
        {
            startDeckSo.Unlocked = true;
        }
    }

    private void OnSceneReloadComplete(EventManager.GameEvents.SceneReloadArgs arg0)
    {
        if (arg0.newScene == SceneLoader.Scene.GameScene)
        {
            EventManager.Game.Level.OnPlanetProgressionWon += OnPlanetProgressionWon;
        }
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     UnlockAllStartDecks();
        // }
    }

    private void OnPlanetProgressionWon()
    {
        StartDeckSO nextDeck = availableStartDecks.FirstOrDefault((x) => !x.Unlocked);
        Debug.Log("Trying to enable new Deck");
        if (nextDeck == null) return;
        if (nextDeck.Unavailable) return;
        nextDeck.Unlocked = true;
        Debug.Log("Invoking Event for Unlock Deck");

        EventManager.Game.GameSettings.OnDeckUnlocked?.Invoke(
            new EventManager.GameEvents.GameSettingsEvents.DeckUnlockedArgs()
            {
                UnlockedDeck = nextDeck
            });
    }
}