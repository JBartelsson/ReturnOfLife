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
    }

    private static Scene targetScene = Scene.GameScene;

    public static void Load(Scene targetScene)
    {
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
            EventManager.Game.SceneSwitch.OnSceneReloadComplete?.Invoke(targetScene);
        };
    }
}