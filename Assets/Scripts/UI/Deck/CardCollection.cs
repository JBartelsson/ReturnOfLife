using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic collection of CardData. Can be used as Decks, Booster Packs, Players Card Collection, etc.
/// </summary>

public class CardCollection : ScriptableObject
{
    [field: SerializeField] public List<CardInstance> CardsInCollection {  get; private set; }

    public void RemoveCardFromCollection(CardInstance card)
    {
        if (CardsInCollection.Contains(card))
        {
            CardsInCollection.Remove(card);
        }
        else
        {
            Debug.LogWarning("Card is not in Collection");
        }
    }

    // Multiple Copies of a Card are possible, needs another if-Statement if it should be singles
    public void AddCardToCollection(CardInstance card)
    {
        CardsInCollection.Add(card);
    }
}
