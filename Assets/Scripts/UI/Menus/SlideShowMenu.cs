using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideShowMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup popupWindow = null;

    float totTime;
    [SerializeField] float timeBeforePause = 5f;
    Vector3 updatedMousePosition;

    private void Start()
    {
        UIUtils.InitFadeState(popupWindow);
    }

    private void Update()
    {
        //  Add the time delta between frames to the totTime var
        totTime += Time.deltaTime;

        //  Check to see if the current mouse position input is equivalent to updateMousePosition from the previous update
        //  If they are equivalent, this means that the user hasn't moved the mouse
        if (Input.mousePosition == updatedMousePosition)
        {
            //  Since the user hasn't moved the mouse, check to see if the total Time is greater than the timeBeforePause
            if (totTime >= timeBeforePause)
            {
                //  Set the popup window to true in order to show the window (instantiate instead it if if doesn't exist already)
                UIUtils.FadeStandard(popupWindow, true);
            }
        }

        //  If the user has moved the mouse, set the totTime back to 0 in order to restart the totTime tracking variable
        else
        {
            UIUtils.FadeStandard(popupWindow, false);
            totTime = 0;
        }

        //  Update the updatedMousePosition before the next frame/update loop executes
        updatedMousePosition = Input.mousePosition;
    }
}
