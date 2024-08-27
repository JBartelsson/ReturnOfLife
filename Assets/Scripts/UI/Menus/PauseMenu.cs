using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;
    [SerializeField] private Button mainMenuButton;

    private bool pauseMenuOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        UIUtils.InitFadeState(pauseMenuCanvasGroup);
    }

    private void OnEnable()
    {
        EventManager.Game.UI.OnPauseMenuOpen += OnPauseMenuOpen;
        EventManager.Game.UI.OnPauseMenuClosed += OnPauseMenuClosed;
        EventManager.Game.Input.OnPause += OnPause;
    }

    private void OnDisable()
    {
        EventManager.Game.UI.OnPauseMenuOpen -= OnPauseMenuOpen;
        EventManager.Game.UI.OnPauseMenuClosed -= OnPauseMenuClosed;
        EventManager.Game.Input.OnPause -= OnPause;

    }

    private void OnPause()
    {
        if (SceneLoader.GetActiveScene() == SceneLoader.Scene.TitleScreen) return;
        if (!pauseMenuOpen)
        {
            OnPauseMenuOpen(false);
        }
        else
        {
            ClosePauseMenu();
        }
    }

    private void OnPauseMenuClosed()
    {
        UIUtils.FadeStandard(pauseMenuCanvasGroup, false);
        pauseMenuOpen = false;
    }

    private void OnPauseMenuOpen(bool isMainMenu)
    {
        UIUtils.FadeStandard(pauseMenuCanvasGroup, true);
        mainMenuButton.gameObject.SetActive(!isMainMenu);
        pauseMenuOpen = true;

    }

    public void ClosePauseMenu()
    {
        EventManager.Game.UI.OnPauseMenuClosed?.Invoke();
    }

    public void BackToMenu()
    {
        SceneLoader.Load(SceneLoader.Scene.TitleScreen);
    }

    
}
