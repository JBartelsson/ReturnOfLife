using System;
using System.Collections;
using System.Collections.Generic;
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

    private CardData _cardData;
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

    [Header("Hover Effect")] [SerializeField]
    private Image backgroundSprite;

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
    }

    private void OnManaChanged(EventManager.GameEvents.LevelEvents.ManaChangedArgs arg0)
    {
        Debug.Log($"CHECKING FOR MANA CHANGE On CARD {_cardIndex}: {_cardInstance}");
        if (!GameManager.Instance.EnoughMana(_cardInstance.GetCardStats().PlayCost))
        {
            Debug.Log($"{_cardInstance.GetCardStats().PlayCost} is not enough mana");
            _costDrop.sprite = _dropSpriteRed;
            canPlayCard = false;
        }
        else
        {
            Debug.Log($"{_cardInstance.GetCardStats().PlayCost} is enough mana");
            canPlayCard = true;
            _costDrop.sprite = _dropSpriteBlue;

        }
    }

    private void OnValidate()
    {
        SetCardUI(_cardData);
    }

    public void SetActiveState(bool state)
    {
        cardClickEnabled = state;
    }

    public void SetCardUI(CardInstance cardInstance)
    {
        _cardInstance = cardInstance;
        OnManaChanged(new EventManager.GameEvents.LevelEvents.ManaChangedArgs());
        SetCardUI(cardInstance.CardData);
    }

    public void SetCardUI(CardData cardData)
    {
        _cardData = cardData;
        if (_cardData != null)
        {
            ToggleVisibility(true);
            SetCardTexts();
            SetRarityIcon();
            SetElementIcon();
            //SetTypeIcon();
            SetCardImage();
        }
        else
        {
            ToggleVisibility(false);
        }
    }

    private void ToggleVisibility(bool visible)
    {
        this.gameObject.SetActive(visible);
    }

    private void SetCardTexts()
    {
        SetCardEffectTypeText();

        _cardName.text = _cardData.CardName;
        _playCost.text = _cardData.RegularCardStats.PlayCost.ToString();
        _cardText.text = _cardData.RegularCardStats.CardText;
    }

    private void SetTypeIcon()
    {
        switch (_cardData.EffectType)
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
        switch (_cardData.EffectType)
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
        switch (_cardData.Rarity)
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
        switch (_cardData.Element)
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
        _cardImage.sprite = _cardData.PlantSprite;
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
            
            backgroundSprite.material = hoverMaterial;
        }
        else
        {
            backgroundSprite.material = null;
        }
        cardSelected = state;

    }
}