using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
/// <summary>
/// Defines what a card is and what it can be, will connect all data and behaviours
/// </summary>

[RequireComponent(typeof(CardUI))] // will automatically attack the CardUI Script to every object that is a card

public class Card : MonoBehaviour
{
    #region Fields and Properties
    [field: SerializeField] public ScriptableCard CardData { get; private set; }
    #endregion

    #region Methods

    #endregion
}
