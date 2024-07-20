using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private List<Canvas> allMainCanvases;
    private Dictionary<Canvas, bool> canvasStates = new ();
    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Game.UI.OnChangeOtherCanvasesStatus += OnChangeOtherCanvasesStatus;
        foreach (var mainCanvas in allMainCanvases)
        {
            canvasStates.Add(mainCanvas, false);
        }
    }

    private void OnChangeOtherCanvasesStatus(bool status)
    {
        if (!status)
        {
            foreach (var mainCanvas in allMainCanvases)
            {
                bool canvasState = mainCanvas.gameObject.activeInHierarchy;
                Debug.Log($"Saving state of {mainCanvas}: {canvasState}");
                canvasStates[mainCanvas] = canvasState;
                mainCanvas.gameObject.SetActive(false);
            } 
        }
        else
        {
            foreach (var mainCanvas in allMainCanvases)
            {
                Debug.Log($" {mainCanvas} is {canvasStates[mainCanvas]}");
                mainCanvas.gameObject.SetActive(canvasStates[mainCanvas]);
                Debug.Log(mainCanvas.gameObject.activeInHierarchy);
            }  
        }
        
    }

}
