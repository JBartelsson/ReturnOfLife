using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static GameManager;
using static StartDeckSO;
using Random = UnityEngine.Random;

[Serializable]
public class Deck
{
    #region Fields and Properties

    private CardCollection _playerDeck = new();

    //[SerializeField] private List<CardInstance> _playerDeck = new();

    public List<CardInstance> _deckPile = new();
    public List<CardInstance> _discardPile = new();

    private StartDeckSO _startDeck;


    public List<CardInstance> HandCards { get; private set; } = new();


    private int _turnDrawCount = 5;
    private int _maxHandSize = 5;

    public int TurnDrawCount => _turnDrawCount;

    public int MaxHandSize => _maxHandSize;

    enum InsertPosition
    {
        First,
        Last,
        Random,
        Discard,
        Hand
    }

    #endregion

    #region Methods

    public Deck()
    {
    }

    //public Deck(int turnDrawCount, CardCollection playerDeck)
    //{
    //    _turnDrawCount = turnDrawCount;
    //    _playerDeck = playerDeck;
    //}

    public void InitializeDeck(StartDeckSO startDeck)
    {
        _deckPile.Clear();
        _discardPile.Clear();
        HandCards.Clear();
        //TODO remove when menu is implemented
        _playerDeck.ClearCards();

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        foreach (StartDeckSO.DeckEntry deckEntry in startDeck.Deck)
        {
            for (int i = 0; i < deckEntry.amount; i++)
            {
                _playerDeck.AddCardToCollection(new CardInstance(deckEntry.cardDataReference));
            }
        }

        _deckPile.AddRange(_playerDeck.CardsInCollection);
        ShuffleDeck();

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
            if (HandCards.Count >= _maxHandSize)
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
            {
                return false;
            }

            ShuffleDiscardPileIntoDeck();
        }
        CardInstance drawCard = _deckPile.First();
        HandCards.Add(drawCard);
        _deckPile.Remove(drawCard);
        OnCardsDrawn();
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
        OnHandCardsChanged();
    }

    public void DiscardRandomCard()
    {
        int randomIndex = Random.Range(0, HandCards.Count);
        CardInstance randomCard = HandCards[randomIndex];
        DiscardCard(randomCard);
    }

    public void DiscardHand()
    {
        int discardedHandCount = HandCards.Count;
        for (int i = discardedHandCount - 1; i >= 0; i--)
        {
            DiscardCard(HandCards[i]);
        }
    }

    public void ShuffleDiscardPileIntoDeck()
    {
        _deckPile.AddRange(_discardPile);
        _discardPile.Clear();
        ShuffleDeck();
    }

    // Fisher-Yates Algorithm for shuffeling
    // For each Position from the last one on, swap positions with a random position
    public void ShuffleDeck()
    {
        for (int i = 0; i < _deckPile.Count - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, _deckPile.Count - 1);
            var temp = _deckPile[i];
            _deckPile[i] = _deckPile[j];
            _deckPile[j] = temp;
        }
    }

    private void OnHandCardsChanged()
    {
        EventManager.Game.Level.OnUpdateCards?.Invoke(new EventManager.GameEvents.DeckChangedArgs()
        {
            ChangedDeck = this
        });
    }

    private void OnCardsDrawn()
    {
        EventManager.Game.Level.OnDrawCards?.Invoke(new EventManager.GameEvents.DeckChangedArgs()
        {
            ChangedDeck = this
        });
    }

    private void AddTemporaryCardToDeck(CardInstance card, InsertPosition pos)
    {
        switch(pos)
        {
            case InsertPosition.First:
                _deckPile.Insert(0, card);
                break;
            case InsertPosition.Last:
                _deckPile.Add(card);
                break;
            case InsertPosition.Random:
                _deckPile.Insert(Random.Range(0, _deckPile.Count), card);
                break;
            case InsertPosition.Discard:
                _discardPile.Add(card);
                break;
            case InsertPosition.Hand:
                HandCards.Insert(0,card);
                break;
            default:
                Debug.Log("Fehler im Switch Case zum Einfügen einer Karte ins Deck");
                break;
        }        
    }

    private void AddPermanentCardToDeck(CardInstance card, InsertPosition pos)
    {
        AddTemporaryCardToDeck(card, InsertPosition pos);
        _playerDeck.AddCardToCollection(card);
    }

    private void RemoveTemporaryCardFromDeck(CardInstance card)
    {
        if (_deckPile.Remove(card))
            return;
        if (_discardPile.Remove(card))
            return;
        if (HandCards.Remove(card))
            return;
        Debug.Log("Fehler beim Entfernen von Karten aus dem Deck. Karte wurde nicht gefunden.");
    }
    private void RemovePermanentCardFromDeck(CardInstance card)
    {
        _deckPile.RemoveTemporaryCardFromDeck(card);
        _playerDeck.RemoveCardFromCollection(card);
    }

    #endregion

    // GameManager
}