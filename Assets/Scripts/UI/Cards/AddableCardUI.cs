using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddableCardUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardUI cardUI;
    private CardPickController cardPickController;

    public CardPickController CardPickController
    {
        get => cardPickController;
        set => cardPickController = value;
    }

    public CardUI CardUI
    {
        get => cardUI;
        set => cardUI = value;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.Deck.PlayerDeck.AddCardToCollection(cardUI.CardInstance);
        cardPickController.CardPicked();
    }
}
