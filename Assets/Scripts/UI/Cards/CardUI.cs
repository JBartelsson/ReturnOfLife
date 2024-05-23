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
    [SerializeField] private Image _elementBackground;

    [SerializeField] private TextMeshProUGUI _playCost;
    [SerializeField] private TextMeshProUGUI _cardName;
    [SerializeField] private TextMeshProUGUI _cardType;
    [SerializeField] private TextMeshProUGUI _cardText;

    [Header("Sprite Assets")] //references to the art folder in assets
    [SerializeField] private Sprite _basicElementBackground;
    [SerializeField] private Sprite _snowElementBackground;
    [SerializeField] private Sprite _sunElementBackground;
    [SerializeField] private Sprite _windElementBackground;
    [SerializeField] private Sprite _waterElementBackground;

    [SerializeField] private Sprite _basicRarityIcon;
    [SerializeField] private Sprite _commonRarityIcon;
    [SerializeField] private Sprite _rareRarityIcon;
    [SerializeField] private Sprite _epicRarityIcon;
    [SerializeField] private Sprite _legendaryRarityIcon;

    private readonly string EFFECTTYPE_PLANT = "Plant";
    private readonly string EFFECTTYPE_FERTILIZER = "Fertilizer";

    #endregion

    #region Methods

    private void Awake()
    {
        _card = GetComponent<Card>();
        SetCardUI();
    }

    private void OnValidate()
    {
        Awake();
    }

    private void SetCardUI()
    {
        if (_card != null && _card.CardData != null)
        {
            SetCardTexts();
            SetRarityIcon();
            SetElementBackground();
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
            case CardRarity.Basic:
                _cardRarity.sprite = _basicRarityIcon;
                break;
            case CardRarity.Common:
                _cardRarity.sprite = _commonRarityIcon;
                break;
            case CardRarity.Rare:
                _cardRarity.sprite = _rareRarityIcon;
                break;
            case CardRarity.Epic:
                _cardRarity.sprite = _epicRarityIcon;
                break;
            case CardRarity.Legendary:
                _cardRarity.sprite = _legendaryRarityIcon;
                break;
        }
    }

    private void SetElementBackground()
    {
        switch (_card.CardData.Element)
        {
            case CardElement.Basic:
                _elementBackground.sprite = _basicElementBackground;
                break;
            case CardElement.Snow:
                _elementBackground.sprite = _snowElementBackground;
                break;
            case CardElement.Sun:
                _elementBackground.sprite = _sunElementBackground;
                break;
            case CardElement.Wind:
                _elementBackground.sprite = _windElementBackground;
                break;
            case CardElement.Water:
                _elementBackground.sprite = _waterElementBackground;
                break;
        }
    }

    private void SetCardImage()
    {
        _cardImage.sprite = _card.CardData.Image;
    }

    #endregion
}
