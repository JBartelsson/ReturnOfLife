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
        PlantedLifeform = 1,
        PlantedUpgradedLifeform = 2,
        PlantedNormalo = 22,
        PlantedBindweed = 3,
        PlantedAntisocial = 4,
        PlantedEpiphyt = 5,
        PlantedLycoperdon = 6,
        PlantedReanimate = 7,
        LifeformKilled = 8,
        LifeformRevived = 9,
        OnDrawCards = 10,
        OnScoreChanged = 11,
        ButtonClick = 12,
        CardHover = 13,
        OnCardSelected = 14,
        CardDiscarded = 15,
        EpiphanyMode = 16,
        EndTurn = 17,
        OnEndLevel = 18,
        OnTipShowed = 19,
        OnDeckView = 20,
        Music = 21,
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
        EventManager.Game.Level.OnLifeformPlanted += OnPlantPlanted;
        EventManager.Game.Level.OnScoreChanged += OnScoreChanged;
        EventManager.Game.UI.OnCardSelected += OnCardSelected;
        EventManager.Game.Level.OnDrawCards += OnDrawCards;
        EventManager.Game.Level.OnLifeformKilled += OnLifeformKilled;
        EventManager.Game.Level.OnLifeformRevived += OnLifeformRevived;
        EventManager.Game.Level.OnEffectUsed += OnEffectUsed;
        EventManager.Game.Level.OnTurnChanged += OnTurnChanged;
        EventManager.Game.Level.OnEndLevel += OnEndLevel;
    }

    private void OnEndLevel(EventManager.GameEvents.LevelEvents.LevelEndedArgs arg0)
    {
        if (arg0.WonLevel)
        {
            //Won level   
        }
        else
        {
            //Lose level   
        }
    }

    private void OnTurnChanged(EventManager.GameEvents.LevelEvents.TurnChangedArgs arg0)
    {
        //Turn changes
    }


    private void OnEffectUsed(CallerArgs arg0)
    {
        //Lyco Effect
    }

    private void OnLifeformRevived(CallerArgs arg0)
    {
        
    }

    private void OnLifeformKilled(CallerArgs arg0)
    {
        
    }

    private void OnDrawCards(EventManager.GameEvents.DeckChangedArgs arg0)
    {
        //Wird pro Karte aufgerufen
    }

    private void OnCardSelected(CardInstance arg0)
    {
        //Card Click Sound
    }

    private void OnScoreChanged(EventManager.GameEvents.LevelEvents.ScoreChangedArgs args)
    {
        // on Score change
        //args.ScoreAdded sind die Anzahl der Punkte
    }

    private void OnPlantPlanted(EventManager.GameEvents.LevelEvents.OnLifeformPlantedArgs args)
    {
        
        if (!args.plantedCardInstance.IsUpgraded())
        {
            PlayOneShot(Sound.PlantedLifeform);
        }
        else
        {
            PlayOneShot(Sound.PlantedUpgradedLifeform);
        }
        
        //Lifeform plant
        switch (args.plantedCardInstance.CardData.LifeformType)
        {
            case CardData.LifeformTypeEnum.Antisocial:
                PlayOneShot(Sound.PlantedAntisocial);
                break;
            case CardData.LifeformTypeEnum.Bindweed:
                PlayOneShot(Sound.PlantedBindweed);
                break;
            case CardData.LifeformTypeEnum.Lycoperdon:
                PlayOneShot(Sound.PlantedLycoperdon);
                break;
            case CardData.LifeformTypeEnum.Epiphyt:
                PlayOneShot(Sound.PlantedEpiphyt);
                break;
            case CardData.LifeformTypeEnum.Normalo:
                PlayOneShot(Sound.PlantedNormalo);
                break;
            case CardData.LifeformTypeEnum.Reanimate:
                PlayOneShot(Sound.PlantedReanimate);
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
