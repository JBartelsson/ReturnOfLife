using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene {
        GameScene,
        LoadingScene,
        TitleScreen,
        Settings,
        Credits,
        None
    }

    private static Scene targetScene = Scene.None;
    private static Scene oldScene = Scene.None;

    public static void Load(Scene targetScene)
    {
        oldScene = SceneLoader.targetScene;
        SceneLoader.targetScene = targetScene;
        Debug.Log($"Load Scene {targetScene}");
        // DOTween.Clear(true);
        SceneManager.LoadScene(SceneLoader.Scene.LoadingScene.ToString(), LoadSceneMode.Additive);
    }

    public static Scene GetActiveScene()
    {
        return targetScene;
    }

    public static void Reload()
    {
        Load(targetScene);
    }

    public static void LoaderCallback()
    {
        EventManager.GameEvents.SceneReloadArgs reloadArgs= new EventManager.GameEvents.SceneReloadArgs()
        {
            oldSCene = oldScene,
            newScene = targetScene
        };
        AsyncOperation sceneAsyncOperation = SceneManager.LoadSceneAsync(targetScene.ToString());
        EventManager.Game.SceneSwitch.OnSceneReloadStarted?.Invoke(reloadArgs);
        if (sceneAsyncOperation == null)return;
        sceneAsyncOperation.completed += operation =>
        {
            Debug.Log($"Reload successful");
            EventManager.Game.SceneSwitch.OnSceneReloadComplete?.Invoke(reloadArgs);
        };
    }
}