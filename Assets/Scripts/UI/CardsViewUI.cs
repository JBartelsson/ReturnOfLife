using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardsViewUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CanvasGroup cardsViewCanvasGroup;

    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private RectTransform cardsSpawnTarget;

    [SerializeField] private TextMeshProUGUI cardCanvasTitle;

    private List<CardUI> currentCardUIs = new ();
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Game.UI.OnOpenCardView += OnOpenCardView;
        UIUtils.InitFadeState(cardsViewCanvasGroup);
    }

    private void OnOpenCardView(EventManager.GameEvents.UIEvents.OnOpenCardViewArgs args)
    {
        //Fade in or out
        UIUtils.FadeStandard(cardsViewCanvasGroup, args.State);

        string title = "";
        switch (args.Pile)
        {
            case DeckViewUI.Pile.DrawPile:
                title = "Draw Pile";
                break;
            case DeckViewUI.Pile.DiscardPile:
                title = "Discard Pile";
                break;
            case DeckViewUI.Pile.DeckPile:
                title = "Deck";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        //Show Cards
        if (args.State)
        {
            cardsSpawnTarget.anchoredPosition = Vector2.zero;
            cardCanvasTitle.text = title;
            SpawnCards(args.cards);
        }
    }

    private void SpawnCards(List<CardInstance> cards)
    {
        List<CardInstance> cardsCopy = new List<CardInstance>(cards);
        cardsCopy.Sort((Comparison));
        for (var i = 0; i < currentCardUIs.Count; i++)
        {
            currentCardUIs[i].SetCardUI(null);
        }
        for (var i = 0; i < cardsCopy.Count; i++)
        {

            if (i >= currentCardUIs.Count)
            {
                GameObject cardUIGameObject = Instantiate(cardPrefab, cardsSpawnTarget);
                CardUI cardUI = cardUIGameObject.GetComponent<CardUI>();
                currentCardUIs.Add(cardUI); 
            }
            currentCardUIs[i].SetCardUI(cardsCopy[i]);
        }
            
    }

    private int Comparison(CardInstance x, CardInstance y)
    {
        return x.CardData.CardName.CompareTo(y.CardData.CardName);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventManager.Game.UI.OnOpenCardView?.Invoke(new EventManager.GameEvents.UIEvents.OnOpenCardViewArgs()
        {
            State = false
        });
    }
}
