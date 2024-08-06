using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static GameManager;
using static StartDeckSO;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[Serializable]
public class Deck
{
    #region Fields and Properties

    private CardCollection _playerDeck = new();

    public CardCollection PlayerDeck
    {
        get => _playerDeck;
        set => _playerDeck = value;
    }

    //[SerializeField] private List<CardInstance> _playerDeck = new();

    [FormerlySerializedAs("_deckPile")] public List<CardInstance> _drawPile = new();
    public List<CardInstance> _discardPile = new();

    private StartDeckSO _startDeck;


    public List<CardInstance> HandCards { get; private set; } = new();


    private int _turnDrawCount = 5;
    private int _maxHandSize = 5;

    public int TurnDrawCount => _turnDrawCount;

    public int MaxHandSize => _maxHandSize;

    public enum InsertPosition
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

    public void InitializeDeck(StartDeckSO startDeck, int maxHandSize = 6, int turnDraw = 6)
    {
        _maxHandSize = maxHandSize;
        _turnDrawCount = turnDraw;
        Debug.Log("INITIALIZING DECK!");
        StartDeckSO startDeckCopy = GameObject.Instantiate(startDeck);
        _drawPile.Clear();
        _discardPile.Clear();
        HandCards.Clear();
        //TODO remove when menu is implemented
        _playerDeck.ClearCards();

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        foreach (StartDeckSO.DeckEntry deckEntry in startDeckCopy.Deck)
        {
            for (int i = 0; i < deckEntry.amount; i++)
            {
                _playerDeck.AddCardToCollection(new CardInstance(deckEntry.cardDataReference));
            }
        }

        

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

    public bool IsInitialized()
    {
        return PlayerDeck.CardsInCollection.Count != 0;
    }
    public void Reset()
    {
        Debug.Log("RESETTING DECK");
        _drawPile.Clear();
        _discardPile.Clear();
        HandCards.Clear();
        _drawPile.AddRange(_playerDeck.CardsInCollection);
        ShuffleDeck();
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
        if (_drawPile.Count <= 0)
        {
            // Should not happen by design, but its better to be on the safe side
            if (_discardPile.Count <= 0)
            {
                return false;
            }

            ShuffleDiscardPileIntoDeck();
        }
        CardInstance drawCard = _drawPile.First();
        HandCards.Add(drawCard);
        _drawPile.Remove(drawCard);
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

    public void DiscardCard(int index)
    {
        DiscardCard(HandCards[index]);
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
        EventManager.Game.Level.OnShuffeDiscardPileIntoDrawPile?.Invoke();
        _drawPile.AddRange(_discardPile);
        _discardPile.Clear();
        ShuffleDeck();
    }

    // Fisher-Yates Algorithm for shuffeling
    // For each Position from the last one on, swap positions with a random position
    public void ShuffleDeck()
    {
        for (int i = 0; i < _drawPile.Count - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, _drawPile.Count - 1);
            var temp = _drawPile[i];
            _drawPile[i] = _drawPile[j];
            _drawPile[j] = temp;
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
                _drawPile.Insert(0, card);
                break;
            case InsertPosition.Last:
                _drawPile.Add(card);
                break;
            case InsertPosition.Random:
                _drawPile.Insert(Random.Range(0, _drawPile.Count), card);
                break;
            case InsertPosition.Discard:
                _discardPile.Add(card);
                break;
            case InsertPosition.Hand:
                HandCards.Insert(0,card);
                break;
            default:
                Debug.Log("Fehler im Switch Case zum Einf�gen einer Karte ins Deck");
                break;
        }        
    }

    public void AddPermanentCardToDeck(CardInstance card, InsertPosition pos = InsertPosition.Random)
    {
        AddTemporaryCardToDeck(card, pos);
        _playerDeck.AddCardToCollection(card);
        EventManager.Game.Level.OnCardAdded?.Invoke(card);
        EventManager.Game.Level.OnDeckChanged?.Invoke(new EventManager.GameEvents.DeckChangedArgs()
        {
            ChangedDeck = this
        });
    }

    private void RemoveTemporaryCardFromDeck(CardInstance card)
    {
        if (_drawPile.Remove(card))
            return;
        if (_discardPile.Remove(card))
            return;
        if (HandCards.Remove(card))
            return;
        Debug.Log("Fehler beim Entfernen von Karten aus dem Deck. Karte wurde nicht gefunden.");
    }
    private void RemovePermanentCardFromDeck(CardInstance card)
    {
        RemoveTemporaryCardFromDeck(card);
        _playerDeck.RemoveCardFromCollection(card);
        EventManager.Game.Level.OnDeckChanged?.Invoke(new EventManager.GameEvents.DeckChangedArgs()
        {
            ChangedDeck = this
        });
    }

    #endregion

    // GameManager
}