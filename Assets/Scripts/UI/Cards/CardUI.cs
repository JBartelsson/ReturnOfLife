using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;
using static CardData;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    #region Fields and Properties

    private CardInstance _cardInstance;
    private int _cardIndex = 0;
    public CardInstance CardInstance => _cardInstance;

    public int CardIndex
    {
        get => _cardIndex;
        set => _cardIndex = value;
    }

    [Header("Prefab Elements")] //references objects in the card prefab
    [SerializeField]
    private Image _cardImage;

    [SerializeField] private Image _costDrop;
    [SerializeField] private Sprite _dropSpriteBlue;
    [SerializeField] private Sprite _dropSpriteRed;
    [SerializeField] private Image _cardRarity;
    [SerializeField] private Image _elementIcon;
    [SerializeField] private Image _typeIcon;

    [SerializeField] private TextMeshProUGUI _playCost;
    [SerializeField] private TextMeshProUGUI _cardName;
    [SerializeField] private TextMeshProUGUI _cardType;
    [SerializeField] private TextMeshProUGUI _cardText;

    [Header("Points")] [SerializeField] private TextMeshProUGUI _pointsText;
    [SerializeField] private GameObject _pointsSymbol;

    [Header("Card Colors")] 
    [SerializeField] private Color _plantColor;
    [SerializeField] private Color _wisdomColor;

    [Header("Hidden Properties ")] //references properties of the cards, that arent shown directly on the card, but in mechanics
    [SerializeField]
    private int _turnDelay;

    [Header("Sprite Assets")] //references to the art folder in assets
    [SerializeField]
    private Sprite _basicElementIcon;

    [SerializeField] private Sprite _snowElementIcon;
    [SerializeField] private Sprite _sunElementIcon;
    [SerializeField] private Sprite _windElementIcon;
    [SerializeField] private Sprite _waterElementIcon;

    [SerializeField] private Sprite _commonRarityIcon;
    [SerializeField] private Sprite _rareRarityIcon;
    [SerializeField] private Sprite _epicRarityIcon;

    [SerializeField] private Sprite _plantTypeIcon;
    [SerializeField] private Sprite _wisdomTypeIcon;

    [FormerlySerializedAs("backgroundSprite")] [Header("Hover Effect")] [SerializeField]
    private Image _backgroundSprite;

    [SerializeField] private Material hoverMaterial;
    private readonly string EFFECTTYPE_PLANT = "Plant";
    private readonly string EFFECTTYPE_WISDOM = "Wisdom";

    private bool cardClickEnabled = true;
    private bool canPlayCard = true;
    private bool cardSelected = false;

    #endregion

    #region Methods

    private void OnEnable()
    {
        EventManager.Game.Level.OnManaChanged += OnManaChanged;
        EventManager.Game.Level.OnWisdomChanged += OnWisdomChanged;
    }

    private void OnDisable()
    {
        EventManager.Game.Level.OnManaChanged -= OnManaChanged;
        EventManager.Game.Level.OnWisdomChanged -= OnWisdomChanged;
    }

    private void OnWisdomChanged(EventManager.GameEvents.LevelEvents.WisdomChangedArgs arg0)
    {
        if (arg0.currentWisdoms.Any((wisdom) => wisdom.CardData.WisdomType == WisdomType.Basic))
        {
            SetCardUI(this._cardInstance, true);
        }
        else
        {
            SetCardUI(this._cardInstance, false);
        }
    }

    private void OnManaChanged(EventManager.GameEvents.LevelEvents.ManaChangedArgs arg0)
    {
        if (_cardInstance == null) return;
        if (!GameManager.Instance.EnoughMana(_cardInstance.GetCardStats().PlayCost))
        {
            _costDrop.sprite = _dropSpriteRed;
            canPlayCard = false;
        }
        else
        {
            canPlayCard = true;
            _costDrop.sprite = _dropSpriteBlue;
        }
    }


    public void SetActiveState(bool state)
    {
        cardClickEnabled = state;
    }

    public void SetCardUI(CardInstance cardInstance, bool upgradePreview = false)
    {
        _cardInstance = cardInstance;
        //When card is initialized, check if the mana is enough to play it
        if (_cardInstance != null)
        {
            ToggleVisibility(true);
            SetCardTexts(upgradePreview);
            SetRarityIcon();
            SetElementIcon();
            //SetTypeIcon();
            SetCardImage();
            SetBackground();
            OnManaChanged(new EventManager.GameEvents.LevelEvents.ManaChangedArgs());
        }
        else
        {
            ToggleVisibility(false);
        }
    }

    private void SetBackground()
    {
        switch (_cardInstance.CardData.EffectType)
        {
            case CardEffectType.Plant:
                _backgroundSprite.color = _plantColor;
                break;
            case CardEffectType.Wisdom:
                _backgroundSprite.color = _wisdomColor;
                break;
            case CardEffectType.Instant:
                break;
            default:
                // _backgroundSprite.color = _plantColor;
                break;
        }
    }

    private void ToggleVisibility(bool visible)
    {
        this.gameObject.SetActive(visible);
    }

    private void SetCardTexts(bool upgradePreview)
    {
        SetCardEffectTypeText();
        CardStats cardStats = !upgradePreview
            ? _cardInstance.CardData.RegularCardStats
            : _cardInstance.CardData.UpgradedCardStats;
        string cardName;
        if (_cardInstance.CardData.EffectType == CardEffectType.Plant)
        {
            cardName = !upgradePreview ? _cardInstance.CardData.CardName : _cardInstance.CardData.CardName + "+";
        }
        else
        {
            cardName = _cardInstance.CardData.CardName;
        }

        _cardName.text = cardName;
        _playCost.text = cardStats.PlayCost.ToString();
        _cardText.text = cardStats.CardText;
        if (cardStats.Points != 0)
        {
            _pointsSymbol.SetActive(true);
            _pointsText.text = cardStats.Points.ToString();
        }
        else
        {
            _pointsSymbol.SetActive(false);
        }
    }

    private void SetTypeIcon()
    {
        switch (_cardInstance.CardData.EffectType)
        {
            case CardData.CardEffectType.Plant:
                _typeIcon.sprite = _plantTypeIcon;
                break;
            case CardData.CardEffectType.Wisdom:
                _typeIcon.sprite = _wisdomTypeIcon;
                break;
        }
    }

    private void SetCardEffectTypeText()
    {
        switch (_cardInstance.CardData.EffectType)
        {
            case CardData.CardEffectType.Plant:
                // _cardType.text = EFFECTTYPE_PLANT;
                break;
            case CardData.CardEffectType.Wisdom:
                // _cardType.text = EFFECTTYPE_WISDOM;
                break;
        }
    }

    private void SetRarityIcon()
    {
        switch (_cardInstance.CardData.Rarity)
        {
            case CardData.CardRarity.Common:
                _cardRarity.sprite = _commonRarityIcon;
                break;
            case CardData.CardRarity.Rare:
                _cardRarity.sprite = _rareRarityIcon;
                break;
            case CardData.CardRarity.Epic:
                _cardRarity.sprite = _epicRarityIcon;
                break;
        }
    }

    private void SetElementIcon()
    {
        switch (_cardInstance.CardData.Element)
        {
            case CardData.CardElement.Basic:
                _elementIcon.sprite = _basicElementIcon;
                break;
            case CardData.CardElement.Snow:
                _elementIcon.sprite = _snowElementIcon;
                break;
            case CardData.CardElement.Sun:
                _elementIcon.sprite = _sunElementIcon;
                break;
            case CardData.CardElement.Wind:
                _elementIcon.sprite = _windElementIcon;
                break;
            case CardData.CardElement.Water:
                _elementIcon.sprite = _waterElementIcon;
                break;
        }
    }

    private void SetCardImage()
    {
        _cardImage.sprite = _cardInstance.CardData.PlantSprite;
    }

    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"CARDCLICKENABLED IS {cardClickEnabled}");
        if (!cardClickEnabled) return;
        if (!canPlayCard) return;
        //IF not left click
        if (eventData.pointerId != -1) return;
        if (!cardSelected)
        {
            cardSelected = true;
            CardsUIController.Instance.SelectCard(_cardIndex);
        }
        else
        {
            cardSelected = false;
            CardsUIController.Instance.DeselectCard(_cardIndex);
        }
    }

    public void SetHoverState(bool state = true)
    {
        if (state)
        {
            _backgroundSprite.material = hoverMaterial;
        }
        else
        {
            _backgroundSprite.material = null;
        }

        cardSelected = state;
    }
}