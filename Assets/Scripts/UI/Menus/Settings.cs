using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    #region Fields and Properties

    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    [SerializeField] private Toggle _toggleHints;
    [SerializeField] private Toggle _toggleFastMode;

    #endregion



    #region Methods

    public void ChangeVolume()
    {
        //prolly in demo

    }

    public void ToggleHints()
    {
        //maybe in demo
    }

    public void ToggleFastMode()
    {
        //not in demo
    }

    public void ResumeGame()
    {
        // Disable Menu
    }

    public void BackToTitle()
    {
        SceneLoader.Load(SceneLoader.Scene.TitleScreen);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion
}
