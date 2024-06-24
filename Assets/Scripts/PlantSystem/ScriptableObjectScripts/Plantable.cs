using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TypeReferences;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/Plantable")]
public class Plantable : ScriptableObject
{
    private void Awake()
    {
        runtimePoints = regularPoints;
    }

    [Serializable]
    public abstract class Executable
    {
        [SerializeField] protected EXECUTION_TYPE executionType = EXECUTION_TYPE.NONE;

        public EXECUTION_TYPE ExecutionType
        {
            get => executionType;
            set => executionType = value;
        }
    }

    [Serializable]
    public class PlantFunctionCall : Executable
    {
        [ClassExtends(typeof(PlantFunctionBase))]
        public ClassTypeReference scriptType = typeof(PlantFunctionBase);

        public ClassTypeReference ScriptType
        {
            get => scriptType;
            set => scriptType = value;
        }

        public PlantFunctionCall(Type plantScriptType, EXECUTION_TYPE exectutionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = exectutionType;
        }
    }

    [Serializable]
    public class PlantEditorCall : Executable
    {
        [ClassExtends(typeof(PlantEditorBase))]
        public ClassTypeReference scriptType = typeof(PlantFunctionBase);

        public ClassTypeReference ScriptType
        {
            get => scriptType;
            set => scriptType = value;
        }

        public PlantEditorCall(Type plantScriptType, EXECUTION_TYPE exectutionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = exectutionType;
        }
    }

    [Serializable]
    public class PlantAccessCheckCall : Executable
    {
        [ClassExtends(typeof(PlantAccessCheckBase))]
        public ClassTypeReference scriptType = typeof(PlantFunctionBase);

        public ClassTypeReference ScriptType
        {
            get => scriptType;
            set => scriptType = value;
        }

        public PlantAccessCheckCall(Type plantScriptType, EXECUTION_TYPE exectutionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = exectutionType;
        }
    }

    [Serializable]
    public class PlantPassiveCall : Executable
    {
        [ClassExtends(typeof(PlantPassiveBase))]
        public ClassTypeReference scriptType = typeof(PlantPassiveBase);

        public ClassTypeReference ScriptType
        {
            get => scriptType;
            set => scriptType = value;
        }

        public PlantPassiveCall(Type plantScriptType, EXECUTION_TYPE executionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = executionType;
        }
    }

    public enum CardRarity
    {
        Common,
        Rare,
        Epic
    }

    public enum CardElement
    {
        Basic,
        Snow,
        Sun,
        Wind,
        Water
    }

    public enum CardEffectType
    {
        Plant,
        Wisdom
    }

    public Plantable(Plantable plantableToCopy)
    {
        plantSprite = plantableToCopy.plantSprite;
        CardName = plantableToCopy.CardName;
        CardText = plantableToCopy.CardText;
        PlayCost = plantableToCopy.PlayCost;
        Element = plantableToCopy.Element;
        EffectType = plantableToCopy.EffectType;
        Rarity = plantableToCopy.Rarity;
        TurnDelay = plantableToCopy.TurnDelay;
        manaCost = plantableToCopy.manaCost;
        fertilizedPoints = plantableToCopy.fertilizedPoints;
        regularPoints = plantableToCopy.regularPoints;
        overridePointFunction = plantableToCopy.overridePointFunction;
        plantFunction = plantableToCopy.plantFunction;
        plantEditor = plantableToCopy.plantEditor;
        plantAccessCheck = plantableToCopy.plantAccessCheck;
        plantPassiveCall = plantableToCopy.plantPassiveCall;
    }

    public Plantable Copy()
    {
        return Instantiate(this);
    }

    [Header("Card Stats")] [SerializeField]
    private Sprite plantSprite;

    [field: SerializeField] public string CardName { get; private set; }
    [field: SerializeField, TextArea] public string CardText { get; private set; }
    [field: SerializeField] public int PlayCost { get; private set; }
    [field: SerializeField] public CardElement Element { get; private set; }
    [field: SerializeField] public CardEffectType EffectType { get; private set; }
    [field: SerializeField] public CardRarity Rarity { get; private set; }
    [field: SerializeField] public int TurnDelay { get; private set; }
    public int manaCost = 1;
    public int turnDelay = 0;
    public int triggerAmount = 1;



    [field: SerializeField] public int regularPoints;
    [field: SerializeField] public int fertilizedPoints;
    private int runtimePoints = 0;


    [Header("Card Functions")] 
    [SerializeField] private bool overridePointFunction = false;


    [SerializeField] private PlantFunctionCall plantFunction;
    [SerializeField] private PlantEditorCall plantEditor = null;
    [SerializeField] private PlantAccessCheckCall plantAccessCheck = null;

    [FormerlySerializedAs("plantPassive")] [SerializeField]
    private PlantPassiveCall plantPassiveCall = null;

    public bool OverridePointFunction
    {
        get => overridePointFunction;
        set => overridePointFunction = value;
    }

    public int RuntimePoints
    {
        get => runtimePoints;
        set => runtimePoints = value;
    }
    public PlantFunctionCall PlantFunction
    {
        get => plantFunction;
        set => plantFunction = value;
    }

    public PlantEditorCall PlantEditor
    {
        get => plantEditor;
        set => plantEditor = value;
    }

    public PlantAccessCheckCall PlantAccessCheck
    {
        get => plantAccessCheck;
        set => plantAccessCheck = value;
    }

    public PlantPassiveCall PlantPassive
    {
        get => plantPassiveCall;
        set => plantPassiveCall = value;
    }

    public Sprite PlantSprite
    {
        get => plantSprite;
        set => plantSprite = value;
    }
}