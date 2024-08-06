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
        SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    public void OpenSettings()
    {
        return;
        //Load Settings Menu
        SceneLoader.Load(SceneLoader.Scene.Settings);
        //SceneManager.LoadScene(3);
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
