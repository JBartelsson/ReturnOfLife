using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using static CardData;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

[Serializable]
public class CardUI : MonoBehaviour
{
    #region Fields and Properties

    protected CardInstance _cardInstance;
    protected int _cardIndex = 0;


    [Header("Prefab Elements")] //references objects in the card prefab
    [SerializeField]
    private Image _cardImage;

    [SerializeField] protected Image _costDrop;
    [SerializeField] protected Sprite _dropSpriteBlue;

    [SerializeField] protected Image _cardBackground;

    [SerializeField] protected Sprite _dropSpriteRed;
    [SerializeField] protected Image _cardRarity;
    [SerializeField] protected Image _elementIcon;
    [SerializeField] protected Image _typeIcon;

    [SerializeField] protected TextMeshProUGUI _playCost;
    [SerializeField] protected TextMeshProUGUI _cardName;
    [SerializeField] protected TextMeshProUGUI _cardType;
    [SerializeField] protected TextMeshProUGUI _cardText;

    [Header("Points")] [SerializeField] protected TextMeshProUGUI _pointsText;
    [SerializeField] protected GameObject _pointsSymbol;
    [SerializeField] private Image pointsImageNormal;
    [SerializeField] private Image pointsImageUpgraded;

    [Header("Card Colors")] [SerializeField]
    protected Color _plantColor;

    [SerializeField] protected Color _wisdomColor;


    [Header("Sprite Assets")] //references to the art folder in assets
    [SerializeField]
    protected Sprite _basicElementIcon;

    [SerializeField] protected Sprite _snowElementIcon;
    [SerializeField] protected Sprite _sunElementIcon;
    [SerializeField] protected Sprite _windElementIcon;
    [SerializeField] protected Sprite _waterElementIcon;

    [SerializeField] protected Sprite _commonRarityIcon;
    [SerializeField] protected Sprite _rareRarityIcon;
    [SerializeField] protected Sprite _epicRarityIcon;


    [SerializeField] protected Sprite _plantTypeIcon;
    [SerializeField] protected Sprite _wisdomTypeIcon;

    [SerializeField] protected Sprite _plantBackground;
    [SerializeField] protected Sprite _wisdomBackground;

    [FormerlySerializedAs("backgroundSprite")] [Header("Hover Effect")] [SerializeField]
    protected Image _backgroundSprite;


    [SerializeField] protected Material hoverMaterial;

    [SerializeField] protected Transform cardParent;

    private readonly string EFFECTTYPE_PLANT = "Plant";
    private readonly string EFFECTTYPE_WISDOM = "Wisdom";

    public CardInstance CardInstance
    {
        get => _cardInstance;
        set => _cardInstance = value;
    }

    public Sprite DropSpriteBlue
    {
        get => _dropSpriteBlue;
        set => _dropSpriteBlue = value;
    }

    public Sprite DropSpriteRed
    {
        get => _dropSpriteRed;
        set => _dropSpriteRed = value;
    }

    public int CardIndex
    {
        get => _cardIndex;
        set => _cardIndex = value;
    }

    public Material HoverMaterial
    {
        get => hoverMaterial;
        set => hoverMaterial = value;
    }

    public Image CostDrop
    {
        get => _costDrop;
        set => _costDrop = value;
    }

    public Image BackgroundSprite
    {
        get => _backgroundSprite;
        set => _backgroundSprite = value;
    }

    public Transform CardParent
    {
        get => cardParent;
        set => cardParent = value;
    }

    #endregion

    #region Methods

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
        }
        else
        {
            Debug.Log($"Set Card UI was called with Instance null on Index {_cardIndex}");
            _cardIndex = -1;
            ToggleVisibility(false);
        }
    }

    private void SetBackground()
    {
        switch (_cardInstance.CardData.EffectType)
        {
            case CardEffectType.Plant:
                _backgroundSprite.color = _plantColor;
                _cardBackground.sprite = _plantBackground;

                break;
            case CardEffectType.Wisdom:
                _backgroundSprite.color = _wisdomColor;
                _cardBackground.sprite = _wisdomBackground;
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
        string pointsString = "";
        if (cardStats.Points != 0)
        {
            _pointsSymbol.SetActive(true);
            if (upgradePreview && _cardInstance.PointsChangeOnUpgrade())
            {
                pointsImageUpgraded.enabled = true;
                pointsImageNormal.enabled = false;
            }
            else
            {
                pointsImageUpgraded.enabled = false;
                pointsImageNormal.enabled = true;
            }
            pointsString += cardStats.Points.ToString();
            _pointsText.text = pointsString;
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
        // switch (_cardInstance.CardData.Rarity)
        // {
        //     case CardData.CardRarity.Common:
        //         _cardRarity.sprite = _commonRarityIcon;
        //         break;
        //     case CardData.CardRarity.Rare:
        //         _cardRarity.sprite = _rareRarityIcon;
        //         break;
        //     case CardData.CardRarity.Epic:
        //         _cardRarity.sprite = _epicRarityIcon;
        //         break;
        // }
    }

    private void SetElementIcon()
    {
        // switch (_cardInstance.CardData.Element)
        // {
        //     case CardData.CardElement.Basic:
        //         _elementIcon.sprite = _basicElementIcon;
        //         break;
        //     case CardData.CardElement.Snow:
        //         _elementIcon.sprite = _snowElementIcon;
        //         break;
        //     case CardData.CardElement.Sun:
        //         _elementIcon.sprite = _sunElementIcon;
        //         break;
        //     case CardData.CardElement.Wind:
        //         _elementIcon.sprite = _windElementIcon;
        //         break;
        //     case CardData.CardElement.Water:
        //         _elementIcon.sprite = _waterElementIcon;
        //         break;
        // }
    }

    private void SetCardImage()
    {
        _cardImage.sprite = _cardInstance.CardData.PlantSprite;
    }

    #endregion
}