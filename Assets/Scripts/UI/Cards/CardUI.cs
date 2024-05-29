using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    #region Fields and Properties

    private Card _card;

    [Header("Prefab Elements")] //references objects in the card prefab
    [SerializeField] private Image _cardImage;
    [SerializeField] private Image _costDrop;
    [SerializeField] private Image _cardRarity;
    [SerializeField] private Image _elementIcon;

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

    private readonly string EFFECTTYPE_PLANT = "Plant";
    private readonly string EFFECTTYPE_FERTILIZER = "Fertilizer";

    #endregion

    #region Methods

    private void Awake()
    {
        _card = GetComponent<Card>();
        SetHiddenProperties();
        SetCardUI();
    }

    private void OnValidate()
    {
        Awake();
    }

    private void SetHiddenProperties()
    {
        if (_card != null && _card.CardData != null)
        {
            SetTurnDelay();
        }
    }

    private void SetTurnDelay()
    {
        _turnDelay = _card.CardData.TurnDelay;
    }

    private void SetCardUI()
    {
        if (_card != null && _card.CardData != null)
        {
            SetCardTexts();
            SetRarityIcon();
            SetElementIcon();
            SetCardImage();
        }
    }

    private void SetCardTexts()
    {
        SetCardEffectTypeText();

        _cardName.text = _card.CardData.CardName;
        _playCost.text = _card.CardData.PlayCost.ToString();
        _cardText.text = _card.CardData.CardText;
    }

    private void SetCardEffectTypeText()
    {
        switch(_card.CardData.EffectType)
        {
            case CardEffectType.Plant:
                _cardType.text = EFFECTTYPE_PLANT;
                break;
            case CardEffectType.Fertilizer:
                _cardType.text = EFFECTTYPE_FERTILIZER;
                break;

        }
    }

    private void SetRarityIcon()
    {
        switch( _card.CardData.Rarity)
        {
            case CardRarity.Common:
                _cardRarity.sprite = _commonRarityIcon;
                break;
            case CardRarity.Rare:
                _cardRarity.sprite = _rareRarityIcon;
                break;
            case CardRarity.Epic:
                _cardRarity.sprite = _epicRarityIcon;
                break;
        }
    }

    private void SetElementIcon()
    {
        switch (_card.CardData.Element)
        {
            case CardElement.Basic:
                _elementIcon.sprite = _basicElementIcon;
                break;
            case CardElement.Snow:
                _elementIcon.sprite = _snowElementIcon;
                break;
            case CardElement.Sun:
                _elementIcon.sprite = _sunElementIcon;
                break;
            case CardElement.Wind:
                _elementIcon.sprite = _windElementIcon;
                break;
            case CardElement.Water:
                _elementIcon.sprite = _waterElementIcon;
                break;
        }
    }

    private void SetCardImage()
    {
        _cardImage.sprite = _card.CardData.Image;
    }

    #endregion
}
