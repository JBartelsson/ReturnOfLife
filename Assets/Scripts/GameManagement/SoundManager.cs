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
        PlantDeath = 4,
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
        EventManager.Game.Level.OnPlantPlanted += OnPlantPlanted;
        EventManager.Game.Level.OnScoreChanged += OnScoreChanged;
    }

    private void OnScoreChanged(EventManager.GameEvents.LevelEvents.ScoreChangedArgs args)
    {
    }

    private void OnPlantPlanted(EventManager.GameEvents.LevelEvents.OnPlantPlantedArgs args)
    {
        switch (args.plantedCardInstance.CardData.LifeformType)
        {
            case CardData.LifeformTypeEnum.Antisocial:
                PlayOneShot(Sound.PlantGrown);
                break;
            case CardData.LifeformTypeEnum.Bindweed:
                PlayOneShot(Sound.PlantGrown);
                break;
            case CardData.LifeformTypeEnum.Lycoperdon:
                PlayOneShot(Sound.PlantGrown);
                break;
            case CardData.LifeformTypeEnum.Epiphyt:
                PlayOneShot(Sound.PlantGrown);
                break;
            case CardData.LifeformTypeEnum.Normalo:
                PlayOneShot(Sound.PlantGrown);
                break;
            case CardData.LifeformTypeEnum.Reanimate:
                PlayOneShot(Sound.PlantGrown);
                break;
            case CardData.LifeformTypeEnum.Epiphany:
                PlayOneShot(Sound.PlantGrown);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
