using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all data for each indiviual card
/// </summary>

[CreateAssetMenu(menuName = "CardData")]
public class ScriptableCard : ScriptableObject
{
    [field: SerializeField] public string CardName {  get; private set; }
    [field: SerializeField, TextArea] public string CardText { get; private set; }
    [field: SerializeField] public int PlayCost { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public CardElement Element { get; private set; }
    [field: SerializeField] public CardEffectType EffectType { get; private set; }
    [field: SerializeField] public CardRarity Rarity { get; private set; }
}

public enum CardRarity
{
    Basic,
    Common,
    Rare,
    Epic,
    Legendary
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
    Fertilizer
}