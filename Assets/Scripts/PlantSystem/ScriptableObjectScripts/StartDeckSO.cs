using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/StartDeck")]

public class StartDeckSO : ScriptableObject
{
    [Serializable]
    public class DeckEntry
    {
        public Plantable plantableReference;
        public int amount;
    }
    [SerializeField] private List<DeckEntry> deck = new();

    public List<DeckEntry> Deck { get => deck; set => deck = value; }
}
