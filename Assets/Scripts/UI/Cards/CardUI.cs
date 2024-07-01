using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using static CardData;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    #region Fields and Properties

    [SerializeField] private CardData _card;
    private int _cardIndex = 0;

    public int CardIndex
    {
        get => _cardIndex;
        set => _cardIndex = value;
    }

    [Header("Prefab Elements")] //references objects in the card prefab
    [SerializeField] private Image _cardImage;
    [SerializeField] private Image _costDrop;
    [SerializeField] private Image _cardRarity;
    [SerializeField] private Image _elementIcon;
    [SerializeField] private Image _typeIcon;

    [SerializeField] private TextMeshProUGUI _playCost;
    [SerializeField] private TextMeshProUGUI _cardName;
    [SerializeField] private TextMeshProUGUI _cardType;
    [SerializeField] private TextMeshProUGUI _cardText;

    [Header("Hidden Properties ")] //references properties of the cards, that arent shown directly on the card, but in mechanics
    [SerializeField] private int _turnDelay;

    [Header("Sprite Assets")] //references to the art folder in assets
    [SerializeField] private Sprite _basicElementIcon;
    [SerializeField] private Sprite _snowElementIcon;
    [SerializeField] private Sprite _sunElementIcon;
    [SerializeField] private Sprite _windElementIcon;
    [SerializeField] private Sprite _waterElementIcon;

    [SerializeField] private Sprite _commonRarityIcon;
    [SerializeField] private Sprite _rareRarityIcon;
    [SerializeField] private Sprite _epicRarityIcon;

    [SerializeField] private Sprite _plantTypeIcon;
    [SerializeField] private Sprite _wisdomTypeIcon;

    private readonly string EFFECTTYPE_PLANT = "Plant";
    private readonly string EFFECTTYPE_WISDOM = "Wisdom";

    #endregion

    #region Methods

    private void OnValidate()
    {
        SetCardUI(_card);
    }


    public void SetCardUI(CardInstance cardInstance)
    {
        SetCardUI(cardInstance.CardData);
    }
    public void SetCardUI(CardData card)
    {
        _card = card;
        if (_card != null)
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

        _cardName.text = _card.CardName;
        _playCost.text = _card.RegularCardStats.PlayCost.ToString();
        _cardText.text = _card.RegularCardStats.CardText;
    }

    private void SetTypeIcon()
    {
        switch (_card.EffectType)
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
        switch(_card.EffectType)
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
        switch( _card.Rarity)
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
        switch (_card.Element)
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
        _cardImage.sprite = _card.PlantSprite;
    }

    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.TryPlayCard(_cardIndex);
    }
}
