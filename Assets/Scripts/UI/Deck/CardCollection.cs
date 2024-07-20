using System;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic collection of CardData. Can be used as Decks, Booster Packs, Players Card Collection, etc.
/// </summary>
[Serializable]
public class CardCollection
{
    public List<CardInstance> CardsInCollection { get; private set; } = new();

    public CardCollection(List<CardData> addedCardData)
    {
        foreach (var cardData in addedCardData)
        {
            CardsInCollection.Add(new CardInstance(cardData));
        }
    }

    public CardCollection()
    {
        
    }
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

    public void ClearCards()
    {
        CardsInCollection = new();
    }
    
    public void ShuffleCardCollection()
    {
        for (int i = 0; i < CardsInCollection.Count - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, CardsInCollection.Count - 1);
            var temp = CardsInCollection[i];
            CardsInCollection[i] = CardsInCollection[j];
            CardsInCollection[j] = temp;
        }
    }

    // Multiple Copies of a Card are possible, needs another if-Statement if it should be singles
    public void AddCardToCollection(CardInstance card)
    {
        CardsInCollection.Add(card);
    }

}
