using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsUIController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsParent;
    private List<CardUI> currentCards = new();
    private void OnEnable()
    {
        EventManager.Game.Level.OnDrawCards += OnDrawCards;
        EventManager.Game.Level.OnUpdateCards += OnUpdateCards;
    }

  

    private void OnDisable()
    {
        EventManager.Game.Level.OnDrawCards -= OnDrawCards;
        EventManager.Game.Level.OnUpdateCards -= OnUpdateCards;

    }

    private void InitCards(int handSize)
    {
        int currentCardSize = currentCards.Count;
        for (int i = 0; i < handSize - currentCardSize; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardsParent);
            CardUI cardUI = newCard.GetComponent<CardUI>();
            cardUI.CardIndex = i;
            currentCards.Add(cardUI);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnDrawCards(EventManager.GameEvents.Args arg0)
    {
        if (GameManager.Instance.HandSize != currentCards.Count)
        {
            InitCards(GameManager.Instance.HandSize);
        };
        for (int i = 0; i < currentCards.Count; i++)
        {
            if (i < GameManager.Instance.CurrentHand.Count)
            {
                currentCards[i].SetCardUI(GameManager.Instance.CurrentHand[i].PlantBlueprint);
            }
            else
            {
                currentCards[i].SetCardUI((Plantable)null);
            }
        }
    }
    
    private void OnUpdateCards(EventManager.GameEvents.Args arg0)
    {
        OnDrawCards(arg0);
    }
}
