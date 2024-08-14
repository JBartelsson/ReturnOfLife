using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

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
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        //Resetting Parenting structure so dontdestroyonload does work
        this.transform.parent = null;
        DontDestroyOnLoad(this);
    }

    /*private void Start()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("MusicSelect", 0);
        StartLoop(Sound.Music);
    }*/

    [Serializable]
    public class SoundItem
    {
        public Sound sound;
        public EventReference EventReference;
    }

    public class LoopItem
    {
        public Sound Sound;
        public EventInstance EventInstance;

        public LoopItem(Sound sound, EventReference eventReference)
        {
            Sound = sound;
            EventInstance = RuntimeManager.CreateInstance(eventReference);
            StartLoop();
        }

        public void StartLoop()
        {
            this.EventInstance.start();
        }

        public void StopLoop()
        {
            EventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }

        public void Destroy()
        {
            EventInstance.stop(STOP_MODE.IMMEDIATE);
            EventInstance.release();
        }
    }

    [SerializeField] private List<SoundItem> soundItems;
    private List<LoopItem> loopQueue = new();

    private void OnEnable()
    {
        EventManager.Game.SceneSwitch.OnSceneReloadComplete += OnSceneReloadComplete;
    }

    private void OnSceneReloadComplete(EventManager.GameEvents.SceneReloadArgs arg0)
    {
        if (arg0.newScene == SceneLoader.Scene.GameScene)
        {
            InitializeGameEvents();
        }

        if (arg0.newScene == SceneLoader.Scene.TitleScreen)
        {
            StartLoop(Sound.Music);
        }
    }

    private void InitializeGameEvents()
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

    private void OnDisable()
    {
        //EventManager.Game.SceneSwitch.OnSceneReloadComplete -= OnSceneReloadComplete;
        EventManager.Game.Level.OnLifeformPlanted -= OnPlantPlanted;
        EventManager.Game.Level.OnScoreChanged -= OnScoreChanged;
        EventManager.Game.UI.OnCardSelected -= OnCardSelected;
        EventManager.Game.Level.OnDrawCards -= OnDrawCards;
        EventManager.Game.Level.OnLifeformKilled -= OnLifeformKilled;
        EventManager.Game.Level.OnLifeformRevived -= OnLifeformRevived;
        EventManager.Game.Level.OnEffectUsed -= OnEffectUsed;
        EventManager.Game.Level.OnTurnChanged -= OnTurnChanged;
        EventManager.Game.Level.OnEndLevel -= OnEndLevel;
    }

    /*private void OnSceneReloadComplete(EventManager.GameEvents.SceneReloadArgs scene)
    {
        switch (scene)
        {
            case SceneLoader.Scene.GameScene:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("MusicSelect", 1);
                break;

            case SceneLoader.Scene.TitleScreen:
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("MusicSelect", 0);
                break;
        }
    }*/

    private void OnEndLevel(EventManager.GameEvents.LevelEvents.LevelEndedArgs arg0)
    {
        if (arg0.WonLevel)
        {
            PlayOneShot(Sound.OnEndLevel); //Won
        }
        else
        {
            PlayOneShot(Sound.OnEndLevel); //Lose
        }
    }

    private void OnTurnChanged(EventManager.GameEvents.LevelEvents.TurnChangedArgs arg0)
    {
        PlayOneShot(Sound.EndTurn);
    }


    private void OnEffectUsed(CallerArgs arg0)
    {
        //Lyco Effect
    }

    private void OnLifeformRevived(CallerArgs arg0)
    {
        PlayOneShot(Sound.LifeformRevived);
    }

    private void OnLifeformKilled(CallerArgs arg0)
    {
        PlayOneShot(Sound.LifeformKilled);
    }

    private void OnDrawCards(EventManager.GameEvents.DeckChangedArgs arg0)
    {
        PlayOneShot(Sound.OnDrawCards);
    }

    private void OnCardSelected(CardInstance arg0)
    {
        PlayOneShot(Sound.OnCardSelected);
        if (arg0.CardData.EffectType == CardData.CardEffectType.Wisdom)
        {
            PlayOneShot(Sound.EpiphanyMode);
        }
    }

    private void OnScoreChanged(EventManager.GameEvents.LevelEvents.ScoreChangedArgs args)
    {
        PlayOneShot(Sound.OnScoreChanged);
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
        EventReference? eventReference = GetEventReference(sound);
        if (eventReference.HasValue)
            FMODUnity.RuntimeManager.PlayOneShot(eventReference.Value, Camera.main.transform.position);
    }

    private EventReference? GetEventReference(Sound sound)
    {
        SoundItem soundItem = soundItems.FirstOrDefault((soundItem) => soundItem.sound == sound);
        if (soundItem == null) return null;
        return soundItem.EventReference;
    }

//Starts a Loop
    private void StartLoop(Sound sound)
    {
        if (loopQueue.Any((item => item.Sound == sound)))
        {
            loopQueue.First(item => item.Sound == sound).StartLoop();
            Debug.Log("Started Loop Again");
            return;
        }

        EventReference? eventReference = GetEventReference(sound);
        if (!eventReference.HasValue) return;
        LoopItem loopItem = new LoopItem(sound, eventReference.Value);
        loopQueue.Add(loopItem);
    }

//Stops a Loop
    private void StopLoop(Sound sound)
    {
        if (!loopQueue.Any((item => item.Sound == sound))) return;
        loopQueue.First((item => item.Sound == sound)).StopLoop();
    }

    private void OnDestroy()
    {
        //Clean UP
        foreach (var loopItem in loopQueue)
        {
            loopItem.Destroy();
        }
    }
}