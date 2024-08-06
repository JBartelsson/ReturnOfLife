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
    [SerializeField] private CanvasGroup tutorialScreen;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private Button continueButton;
    [SerializeField] private CanvasGroup tutorialCanvasGroup;
    [Header("Animation Settings")] [SerializeField]
    private float animationSpeed = .3f;
    private int currentIndex = 0;

    private void Start()
    {
        
        tutorialCanvasGroup.DOFade(0f, 0f);
        tutorialScreen.DOFade(0f, 0f);
        tutorialScreen.gameObject.SetActive(false);
        EventManager.Game.UI.OnTutorialScreenChange += OnTutorialScreenChange;

    }

    private void OnEnable()
    {
        continueButton.onClick.AddListener(ContinueButtonOnclicked);
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
        if (status)
        {
            ChangeSlide(0, true);
            tutorialScreen.gameObject.SetActive(true);
        }
        tutorialScreen.DOFade(status ? 1f : 0f, animationSpeed).OnComplete(() =>
        {
            tutorialScreen.gameObject.SetActive(status);
        });
        
    }

    

    private void ChangeSlide(int index, bool instant = false)
    {
        float localAnimationSpeed = instant ? 0f : this.animationSpeed;
        if (index >= tutorialList.Count)
        {
            OnTutorialScreenChange(false);
            return;
        }
        
        tutorialCanvasGroup.DOFade(0f, localAnimationSpeed).OnComplete(() =>
        {
            foreach (var tutorialItem in tutorialList)
            {
                tutorialItem.tutorialImageObject.SetActive(false); 
            }
            tutorialList[index].tutorialImageObject.SetActive(true);
            tutorialText.text = tutorialList[index].tutorialText;
            tutorialCanvasGroup.DOFade(1f, localAnimationSpeed);
        });
        
    }
}
