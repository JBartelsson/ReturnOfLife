using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic collection of CardData. Can be used as Decks, Booster Packs, Players Card Collection, etc.
/// </summary>

public class CardCollection : ScriptableObject
{
    [field: SerializeField] public List<ScriptableCard> CardsInCollection {  get; private set; }
}
