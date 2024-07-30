using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SimpleTutorialUI : MonoBehaviour
{
    [Serializable]
    public class TutorialItem
    {
        [FormerlySerializedAs("tutorialTexture")] public GameObject tutorialImageObject;
        [TextArea]
        public string tutorialText;
    }

    [SerializeField] private List<TutorialItem> tutorialList;
    [SerializeField] private Canvas tutorialScreen;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private Button continueButton;
    [SerializeField] private CanvasGroup tutorialCanvasGroup;
    [Header("Animation Settings")] [SerializeField]
    private float animationSpeed = .3f;
    private int currentIndex = 0;
    private void OnEnable()
    {
        EventManager.Game.UI.OnTutorialScreenChange += OnTutorialScreenChange;
        continueButton.onClick.AddListener(ContinueButtonOnclicked);
        tutorialCanvasGroup.DOFade(0f, 0f);
    }

    private void OnDisable()
    {
        continueButton.onClick.RemoveListener(ContinueButtonOnclicked);
    }

    private void ContinueButtonOnclicked()
    {
        currentIndex++;
        ChangeSlide(currentIndex);
    }

    private void OnTutorialScreenChange(bool status)
    {
        currentIndex = 0;
        tutorialScreen.gameObject.SetActive(status);
        ChangeSlide(0);
    }

    private void ChangeSlide(int index)
    {
        tutorialCanvasGroup.DOFade(0f, animationSpeed).OnComplete(() =>
        {
            foreach (var tutorialItem in tutorialList)
            {
                tutorialItem.tutorialImageObject.SetActive(false); 
            }
            tutorialList[currentIndex].tutorialImageObject.SetActive(true);
            tutorialText.text = tutorialList[currentIndex].tutorialText;
            
            if (index >= tutorialList.Count)
            {
                OnTutorialScreenChange(false);
            }
            tutorialCanvasGroup.DOFade(1f, animationSpeed);
        });
        
    }
}
