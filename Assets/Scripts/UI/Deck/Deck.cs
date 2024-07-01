using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StartDeckSO;

public class Deck : MonoBehaviour
{
    #region Fields and Properties

    public static Deck Instance { get; private set; } //Singleton

    [SerializeField] private CardCollection _playerDeck;
    //[SerializeField] private CardInstance _cardPrefab; ??? //the Card Prefab, that will be copied and filled with the CardData

    //[SerializeField] private Canvas _cardCanvas;???

    private List<CardInstance> _deckPile;
    private List<CardInstance> _discardPile;

    public List<CardInstance> HandCards { get; private set; }

    [SerializeField] private int _turnDrawCount = 5;
    [SerializeField] private int _maxHandSize = 5;

    #endregion

    #region Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Instantiate the Deck once, at the Start of a Level
        
    }

    private void DrawCards(int amount = 1)
    {
        for (; amount > 0; amount--)
        {
            if (HandCards.Count <= _maxHandSize)
            {
                print("Hand is full");
                return;
            }
            if (_deckPile.Count <= 0)
            {
                // Should not happen by design, but its better to be on the safe side
                if (_discardPile.Count <= 0)
                { return; }
                ShuffleDiscardPileIntoDeck();
            }
            CardInstance drawCard = _deckPile.First();
            HandCards.Add(drawCard);
            _deckPile.Remove(drawCard);
        }
    }

    private void DrawForTurn()
    {
        DrawCards(_turnDrawCount);
    }

    private void DiscardCard(CardInstance card)
    {
        _discardPile.Add(card);
        HandCards.Remove(card);
    }

    private void DiscardRandomCard()
    {
        int randomIndex = Random.Range(0, HandCards.Count);
        CardInstance randomCard = HandCards[randomIndex];
        HandCards.Remove(randomCard);
        _discardPile.Add(randomCard);
    }

    private void DiscardHand()
    {
        foreach(CardInstance card in HandCards)
        {
            DiscardCard(card);
        }
    }

    private void ShuffleDiscardPileIntoDeck()
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
    private void ShuffleDeck()
    {
        for(int i = _deckPile.Count - 1; i < 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = _deckPile[i];
            _deckPile[i] = _deckPile[j];
            _deckPile[j] = temp;
        }
    }

    //private void InstantiateDeck()???
    //{
    //    //CardInstance card = new CardInstance(deckEntry.cardDataReference);
    //}

    #endregion

    /* GameManager
     * private List<CardInstance> deck = new();
     * private List<CardInstance> currentHand = new();
     * private List<CardInstance> drawPile = new();
     * private List<CardInstance> discardPile = new();
     */
}

