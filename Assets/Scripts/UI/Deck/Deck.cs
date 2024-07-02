using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StartDeckSO;
using Random = UnityEngine.Random;

public class Deck
{
    #region Fields and Properties

    [SerializeField] private CardCollection _playerDeck;

    private List<CardInstance> _deckPile;
    private List<CardInstance> _discardPile;

    private StartDeckSO _startDeck;


    public List<CardInstance> HandCards { get; private set; }

    [SerializeField] private int _turnDrawCount = 5;
    [SerializeField] private int _maxHandSize = 5;

    #endregion

    #region Methods

    public void InitializeDeck(StartDeckSO startDeck)
    {
        _deckPile.Clear();
        _discardPile.Clear();
        HandCards.Clear();

        foreach (StartDeckSO.DeckEntry deckEntry in startDeck.Deck)
        {
            for (int i = 0; i < deckEntry.amount; i++)
            {
                _playerDeck.AddCardToCollection(new CardInstance(deckEntry.cardDataReference));
            }
            
        }
        _deckPile.AddRange(_playerDeck.CardsInCollection);

        /*
         * Debug.Log("RESETTING LEVEL");
         * SetPointScore(0);
         * deck.Clear();
         * currentHand.Clear();
         * drawPile.Clear();
         * discardPile.Clear();
         * currentTurns = 0;
         * currentPlayedCards = 0;
         * selectedPlantNeedNeighbor = false;
         * 
         * foreach (StartDeckSO.DeckEntry deckEntry in startDeck.Deck)
         * {
         *     for (int i = 0; i < deckEntry.amount; i++)
         *     {
         *         deck.Add(new CardInstance(deckEntry.cardDataReference));
         *     }
         * }
         * 
         * drawPile.AddRange(deck);
         * SwitchState(GameState.EndTurn);
         */
    }

    public void DrawCards(int amount = 1)
    {
        for (; amount > 0; amount--)
        {
            if (HandCards.Count <= _maxHandSize)
            {
                Debug.Log("Hand is full");
                return;
            }
            if (!DrawSingleCard()) // If no card can be drawn, just return. To minimize unnecessary calls
                return;
        }
    }

    private bool DrawSingleCard()
    {
        if (_deckPile.Count <= 0)
        {
            // Should not happen by design, but its better to be on the safe side
            if (_discardPile.Count <= 0)
            { return false; }
            ShuffleDiscardPileIntoDeck();
        }
        CardInstance drawCard = _deckPile.First();
        HandCards.Add(drawCard);
        _deckPile.Remove(drawCard);
        return true;
    }

    public void DrawForTurn()
    {
        DrawCards(_turnDrawCount);
    }

    public void DiscardCard(CardInstance card)
    {
        _discardPile.Add(card);
        HandCards.Remove(card);
    }

    public void DiscardRandomCard()
    {
        int randomIndex = Random.Range(0, HandCards.Count);
        CardInstance randomCard = HandCards[randomIndex];
        HandCards.Remove(randomCard);
        _discardPile.Add(randomCard);
    }

    public void DiscardHand()
    {
        foreach(CardInstance card in HandCards)
        {
            DiscardCard(card);
        }
    }

    public void ShuffleDiscardPileIntoDeck()
    {
        foreach(CardInstance card in _discardPile)
        {
            _deckPile.Add(card);
            _discardPile.Remove(card);
        }
        ShuffleDeck();
    }

    // Fisher-Yates Algorithm for shuffeling
    // For each Position from the last one on, swap positions with a random position
    public void ShuffleDeck()
    {
        for(int i = _deckPile.Count - 1; i < 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var temp = _deckPile[i];
            _deckPile[i] = _deckPile[j];
            _deckPile[j] = temp;
        }
    }

    #endregion

    // GameManager
}

