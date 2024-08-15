using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeckChooseButton : MonoBehaviour
{
    [SerializeField] private Image frameImage;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    public StartDeckSO StartDeck { get; set; }
    
    public void SetActiveState(bool state)
    {
        frameImage.gameObject.SetActive(state);
    }

    public void Init(StartDeckSO startDeckSo, UnityAction buttonAction)
    {
        SetActiveState(false);
        string buttonTextString = startDeckSo.StartDeckName;
        if (!startDeckSo.Unlocked)
        {
            buttonTextString += @"<sprite name=""Lock"">";
            this.buttonText.text = buttonTextString;

            return;
        }
        this.buttonText.text = buttonTextString;
        StartDeck = startDeckSo;
        button.onClick.AddListener(buttonAction);
    }
}
