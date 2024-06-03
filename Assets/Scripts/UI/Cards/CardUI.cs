using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    #region Fields and Properties

    [SerializeField] private Plantable _card;

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
    private readonly string EFFECTTYPE_WISDOM = "Wisdom";

    #endregion

    #region Methods

    private void OnValidate()
    {
        SetCardUI(_card);
    }

    private void SetHiddenProperties()
    {
        if (_card != null)
        {
            SetTurnDelay();
        }
    }

    private void SetTurnDelay()
    {
        _turnDelay = _card.TurnDelay;
    }

    public void SetCardUI(PlantInstance plantInstance)
    {
        SetCardUI(plantInstance.Plantable);
    }
    public void SetCardUI(Plantable card)
    {
        _card = card;
        if (_card != null)
        {
            ToggleVisibility(true);
            SetCardTexts();
            SetRarityIcon();
            SetElementIcon();
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
        _playCost.text = _card.PlayCost.ToString();
        _cardText.text = _card.CardText;
    }

    private void SetCardEffectTypeText()
    {
        switch(_card.EffectType)
        {
            case Plantable.CardEffectType.Plant:
                // _cardType.text = EFFECTTYPE_PLANT;
                break;
            case Plantable.CardEffectType.Wisdom:
                // _cardType.text = EFFECTTYPE_WISDOM;
                break;

        }
    }

    private void SetRarityIcon()
    {
        switch( _card.Rarity)
        {
            case Plantable.CardRarity.Common:
                _cardRarity.sprite = _commonRarityIcon;
                break;
            case Plantable.CardRarity.Rare:
                _cardRarity.sprite = _rareRarityIcon;
                break;
            case Plantable.CardRarity.Epic:
                _cardRarity.sprite = _epicRarityIcon;
                break;
        }
    }

    private void SetElementIcon()
    {
        switch (_card.Element)
        {
            case Plantable.CardElement.Basic:
                _elementIcon.sprite = _basicElementIcon;
                break;
            case Plantable.CardElement.Snow:
                _elementIcon.sprite = _snowElementIcon;
                break;
            case Plantable.CardElement.Sun:
                _elementIcon.sprite = _sunElementIcon;
                break;
            case Plantable.CardElement.Wind:
                _elementIcon.sprite = _windElementIcon;
                break;
            case Plantable.CardElement.Water:
                _elementIcon.sprite = _waterElementIcon;
                break;
        }
    }

    private void SetCardImage()
    {
        _cardImage.sprite = _card.PlantSprite;
    }

    #endregion
}
