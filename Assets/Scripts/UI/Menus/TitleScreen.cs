using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void PlayGame()
    {
        //Start Game Loop
        //SceneManager.LoadScene("SceneNameXY");
        EventManager.Game.UI.OnShowDeckPickCanvas?.Invoke(true);
    }

    public void OpenSettings()
    {
        EventManager.Game.UI.OnPauseMenuOpen?.Invoke(true);
    }

    public void OpenCredits()
    {
        //Load Credits
        SceneLoader.Load(SceneLoader.Scene.Credits);
        //SceneManager.LoadScene(4);
    }

    public void QuitGame()
    {
        Debug.Log("boop");
        Application.Quit();
    }
}
