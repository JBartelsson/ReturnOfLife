using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene {
        GameScene,
        MainMenuScene,
        LoadingScene,
        None
    }

    private static Scene targetScene = Scene.GameScene;
    private static Scene oldScene = Scene.MainMenuScene;

    public static void Load(Scene targetScene)
    {
        oldScene = targetScene;
        SceneLoader.targetScene = targetScene;
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneLoader.Scene.LoadingScene.ToString());
    }

    public static void Reload()
    {
        Load(targetScene);
    }

    public static void LoaderCallback()
    {
        AsyncOperation sceneAsyncOperation = SceneManager.LoadSceneAsync(targetScene.ToString());
        if (sceneAsyncOperation == null)return;
        sceneAsyncOperation.completed += operation =>
        {
            EventManager.Game.SceneSwitch.OnSceneReloadComplete?.Invoke(new EventManager.GameEvents.SceneReloadArgs()
            {
                oldSCene = oldScene,
                newScene = targetScene
            });
        };
    }
}