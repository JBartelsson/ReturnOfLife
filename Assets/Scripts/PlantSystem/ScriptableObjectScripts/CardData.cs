using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TypeReferences;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/Card")]
public class CardData : ScriptableObject, ICloneable
{
  
    private void Awake()
    {
        runtimePoints = regularCardStats.Points;
        plantTypeID = cardDataIDCounter;
        cardDataIDCounter++;
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
    public class CardFunctionCall : Executable
    {
        [ClassExtends(typeof(CardFunctionBase))]
        public ClassTypeReference scriptType = typeof(CardFunctionBase);

        public ClassTypeReference ScriptType
        {
            get => scriptType;
            set => scriptType = value;
        }

        public CardFunctionCall(Type plantScriptType, EXECUTION_TYPE exectutionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = exectutionType;
        }
    }

    [Serializable]
    public class CardEditorCall : Executable
    {
        [ClassExtends(typeof(CardSecondMoveBase))]
        public ClassTypeReference scriptType = typeof(CardFunctionBase);

        public ClassTypeReference ScriptType
        {
            get => scriptType;
            set => scriptType = value;
        }

        public CardEditorCall(Type plantScriptType, EXECUTION_TYPE exectutionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = exectutionType;
        }
    }

    [Serializable]
    public class CardAccessCheckCall : Executable
    {
        [ClassExtends(typeof(CardAccessCheckBase))]
        public ClassTypeReference scriptType = typeof(CardFunctionBase);
        public bool OverrideNeighboring = false;

        public ClassTypeReference ScriptType
        {
            get => scriptType;
            set => scriptType = value;
        }

        public CardAccessCheckCall(Type plantScriptType, EXECUTION_TYPE exectutionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = exectutionType;
        }
    }

    [Serializable]
    public class CardPassiveCall : Executable
    {
        [ClassExtends(typeof(CardPassiveBase))]
        public ClassTypeReference scriptType = typeof(CardPassiveBase);

        public ClassTypeReference ScriptType
        {
            get => scriptType;
            set => scriptType = value;
        }

        public CardPassiveCall(Type plantScriptType, EXECUTION_TYPE executionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = executionType;
        }
    }
    [Serializable]
    public class CardCanExecuteCheckCall : Executable
    {
        [ClassExtends(typeof(CardCanExecuteCheckBase))]
        public ClassTypeReference scriptType = typeof(CardCanExecuteCheckBase);

        public ClassTypeReference ScriptType
        {
            get => scriptType;
            set => scriptType = value;
        }

        public CardCanExecuteCheckCall(Type plantScriptType, EXECUTION_TYPE executionType)
        {
            this.scriptType = plantScriptType;
            this.executionType = EXECUTION_TYPE.NONE;
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
        Wisdom,
        Instant
    }

    public CardData Copy()
    {
        return Instantiate(this);
    }

    [Header("Card Stats")] [SerializeField]
    private Sprite plantSprite;
    [field: Header("Text Information")]
    [field: SerializeField] public string CardName { get; private set; }

    [SerializeField] private CardStats regularCardStats;
    [SerializeField] private CardStats upgradedCardStats;
    [field: SerializeField] public CardElement Element { get; private set; }
    [FormerlySerializedAs("EffectType")] [SerializeField] private CardEffectType effectType;

    

    [FormerlySerializedAs("WisdomType")]
    [ConditionalHide("effectType", CardEffectType.Wisdom)]
    [SerializeField] private WisdomType wisdomType;

    [field: SerializeField] public CardRarity Rarity { get; private set; }
    private int runtimePoints = 0;
    
    private static int cardDataIDCounter = 0;
    private int plantTypeID;

    public int PlantTypeID => plantTypeID;


    [Serializable]
    public class CardStats
    {
        public int Points;
        [FormerlySerializedAs("effectPattern")] public PatternSO EffectPattern;
        [TextArea]
        public string CardText;
        public int PlayCost;
        public int SecondMoveCallAmount = 1;
    }


    [Header("Card Functions")] 
    [SerializeField] private bool overridePointFunction = false;


    [FormerlySerializedAs("plantFunction")] [SerializeField] private CardFunctionCall cardFunction;
    [FormerlySerializedAs("plantEditor")] [SerializeField] private CardEditorCall cardEditor = null;
    [FormerlySerializedAs("plantAccessCheck")] [SerializeField] private CardAccessCheckCall cardAccessCheck = null;
    [SerializeField] private CardCanExecuteCheckCall cardCanExecuteCheck = null;


    [FormerlySerializedAs("plantPassiveCall")] [FormerlySerializedAs("plantPassive")] [SerializeField]
    private CardPassiveCall cardPassiveCall = null;

    public CardCanExecuteCheckCall CardCanExecuteCheck => cardCanExecuteCheck;
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
    public CardFunctionCall CardFunction
    {
        get => cardFunction;
        set => cardFunction = value;
    }

    public CardEditorCall CardEditor
    {
        get => cardEditor;
        set => cardEditor = value;
    }

    public CardAccessCheckCall CardAccessCheck
    {
        get => cardAccessCheck;
        set => cardAccessCheck = value;
    }

    public CardPassiveCall CardPassive
    {
        get => cardPassiveCall;
        set => cardPassiveCall = value;
    }

    public Sprite PlantSprite
    {
        get => plantSprite;
        set => plantSprite = value;
    }
    
    public CardStats RegularCardStats
    {
        get => regularCardStats;
        set => regularCardStats = value;
    }

    public CardStats UpgradedCardStats
    {
        get => upgradedCardStats;
        set => upgradedCardStats = value;
    }
    
    public CardEffectType EffectType => effectType;

    public WisdomType WisdomType => wisdomType;
    public object Clone()
    {
        return this.Copy();
    }
}