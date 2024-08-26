using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private FMOD.Studio.Bus Music;
    private FMOD.Studio.Bus SFX;
    private FMOD.Studio.Bus Master;

    //void Awake()
    //{
    //    MusicMix = FMODUnity.RuntimeManager.GetBus("bus:/Music");
    //    SFXMix = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
    //    MasterMix = FMODUnity.RuntimeManager.GetBus("bus:/");
    //}

    //void Start()
    //{
    //    masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
    //    musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    //    sfxSlider.value = PlayerPrefs.GetFloat("EffectsVolume");
    //}

    //public void ChangeMasterVolume(float volume)
    //{
    //    PlayerPrefs.SetFloat("MasterVolume", volume);
    //    Master.setVolume(volume);
    //}

    //public void ChangeMusicVolume(float volume)
    //{
    //    PlayerPrefs.SetFloat("MusicVolume", volume);
    //    Music.setVolume(volume);
    //}

    //public void ChangeEffectsVolume(float volume)
    //{
    //    PlayerPrefs.SetFloat("EffectsVolume", volume);
    //    SFX.setVolume(volume);
    //}

}
