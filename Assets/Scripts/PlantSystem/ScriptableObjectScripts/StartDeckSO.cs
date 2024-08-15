using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/StartDeck")]

public class StartDeckSO : ScriptableObject
{
    [Serializable]
    public class DeckEntry
    {
        [FormerlySerializedAs("plantableReference")] public CardData cardDataReference;
        public int amount;
    }

    [SerializeField] private string startDeckName;


    [SerializeField] private List<DeckEntry> deck = new();
    [SerializeField] private bool unlockAtStart;
    [FormerlySerializedAs("available")] [SerializeField] private bool unavailable;


    public bool UnlockAtStart
    {
        get => unlockAtStart;
        set => unlockAtStart = value;
    }


    public string StartDeckName => startDeckName;

    private bool unlocked = false;
    public List<DeckEntry> Deck { get => deck; set => deck = value; }
    public bool Unavailable => unavailable;
    
    public bool Unlocked
    {
        get => unlocked;
        set => unlocked = value;
    }
}
