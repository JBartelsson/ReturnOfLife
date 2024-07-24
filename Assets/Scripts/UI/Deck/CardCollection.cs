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

    public override string ToString()
    {
        string s = "Card Collection contains: ";
        foreach (var cardInstance in CardsInCollection)
        {
            s += cardInstance.CardData.CardName + ",";
        }

        s.Remove(s.Length - 1);

        return s;
    }

    public CardCollection(List<CardData> addedCardData)
    {
        foreach (var cardData in addedCardData)
        {
            Debug.Log($"Added {cardData} to {this}");
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
            //Range is maxExclusive so we get a Number from i to CardInCollection.Count - 1
            int j = UnityEngine.Random.Range(i, CardsInCollection.Count);
            var temp = CardsInCollection[i];
            CardsInCollection[i] = CardsInCollection[j];
            CardsInCollection[j] = temp;
        }

        Debug.Log(this);
    }

    // Multiple Copies of a Card are possible, needs another if-Statement if it should be singles
    public void AddCardToCollection(CardInstance card)
    {
        CardsInCollection.Add(card);
    }
}