using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum Sound
    {
        PlantGrown = 1,
        Point = 2,
        MainMenuMusic = 3,
    }
    public static SoundManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        //Resetting Parenting structure so dontdestroyonload does work
        this.transform.parent = null;
        DontDestroyOnLoad(this);
    }
    
    [Serializable]
    public class SoundItem
    {
        public Sound sound;
        public EventReference EventReference;
    }

    [SerializeField] private List<SoundItem> soundItems;

    private void OnEnable()
    {
        EventManager.Game.UI.OnPlantPlanted += OnPlantPlanted;
    }

    private void OnPlantPlanted(EventManager.GameEvents.UIEvents.OnPlantPlantedArgs arg0)
    {
        Debug.Log("On Plant Planted");
        PlayOneShot(Sound.PlantGrown);
    }

    private void PlayOneShot(Sound sound)
    {
        SoundItem soundItem =
            soundItems.FirstOrDefault((soundItem) => soundItem.sound == sound);
        Debug.Log($"SOUND ITEM IS {soundItem}");
        if (soundItem == null) return;
        FMODUnity.RuntimeManager.PlayOneShot(soundItem.EventReference, Camera.main.transform.position);
    }
}
