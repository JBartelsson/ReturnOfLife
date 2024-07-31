using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class TutorialScreenUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;

    private void OnEnable()
    {
        Debug.Log("Tutorial subscribed");
        EventManager.Game.UI.OnTutorialScreenChange += OnTutorialScreenChange;
    }

    private void OnTutorialScreenChange(bool activeState)
    {
        Debug.Log($"Setting Tutorial Screen to {activeState}");
        gameObject.SetActive(activeState);
    }

   
    // Start is called before the first frame update
    void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            EventManager.Game.UI.OnTutorialScreenChange?.Invoke(false);
        });
    }

}
