using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartLoader : MonoBehaviour
{
    private void Start()
    {
        SceneLoader.Load(SceneLoader.Scene.TitleScreen);
    }
}
