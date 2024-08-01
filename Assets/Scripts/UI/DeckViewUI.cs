using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckViewUI : MonoBehaviour
{
    public enum Pile
    {
        DrawPile, DiscardPile, DeckPile
    }

    [SerializeField] private Pile pile;
    [SerializeField] private TextMeshProUGUI numberText;
    
    private List<CardInstance> cards;
    private void OnEnable()
    {
        EventManager.Game.Level.OnDrawCards += OnDrawCards;
        EventManager.Game.Level.OnUpdateCards += OnDrawCards;
    }

    private void OnDrawCards(EventManager.GameEvents.DeckChangedArgs arg0)
    {
        UpdatePile();
        numberText.text = cards.Count.ToString();
    }

    private void UpdatePile()
    {
        switch (pile)
        {
            case Pile.DrawPile:
                cards = GameManager.Instance.Deck._drawPile;
                break;
            case Pile.DiscardPile:
                cards = GameManager.Instance.Deck._discardPile;
                break;
            case Pile.DeckPile:
                cards = GameManager.Instance.Deck.PlayerDeck.CardsInCollection;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
