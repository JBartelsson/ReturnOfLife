using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckViewUI : MonoBehaviour, IPointerClickHandler
{
    public enum Pile
    {
        DrawPile, DiscardPile, DeckPile
    }

    [SerializeField] private Pile pile;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Canvas deckViewCanvas;

    private bool cardViewOpen = false;
    private List<CardInstance> cards;
    private void OnEnable()
    {
        EventManager.Game.Level.OnDrawCard += OnDrawCards;
        EventManager.Game.Level.OnDiscardCard += OnDiscardCard;
        EventManager.Game.Level.OnDeckChanged += OnDeckChanged;
        EventManager.Game.UI.OnOpenCardView += OnOpenCardView;
    }

    private void OnOpenCardView(EventManager.GameEvents.UIEvents.OnOpenCardViewArgs arg0)
    {
        if (arg0.Pile == this.pile)
        {
            cardViewOpen = arg0.State;
        }
    }

    private void OnDiscardCard(EventManager.GameEvents.LevelEvents.DiscardCardArgs arg0)
    {
        UpdatePile();
    }

    private void OnDeckChanged(EventManager.GameEvents.LevelEvents.DeckChangedArgs arg0)
    {
        UpdatePile();
    }

    private void OnDrawCards(EventManager.GameEvents.LevelEvents.DeckChangedArgs arg0)
    {
        UpdatePile();
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
        numberText.text = cards.Count.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cards.Count == 0) return;
        cardViewOpen = !cardViewOpen;
        EventManager.Game.UI.OnOpenCardView?.Invoke(new EventManager.GameEvents.UIEvents.OnOpenCardViewArgs()
        {
            cards = this.cards,
            Pile = pile,
            State = cardViewOpen
        });
        if (cardViewOpen)
        {
            deckViewCanvas.overrideSorting = true;
            deckViewCanvas.sortingOrder = 900;
        }
        else
        {
            deckViewCanvas.overrideSorting = false;
            deckViewCanvas.sortingOrder = 0;
        }
    }
}
