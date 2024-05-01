using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColorChange : MonoBehaviour //, IPointerClickHandler
{
    [SerializeField] private Material[] material;
    private SpriteRenderer rend;

    public Camera mainCamera;

    public bool thisItemSelected;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[0];
    }

    public void Update()
    {
        OnLeftClick();
    }

    /*public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
            Debug.Log("Klick");
        }
    }*/

    public void OnLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.zero), 10f);
            //Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // funktioniert super (sogar für einzelnes Objekt)
            //if (Physics2D.Raycast(ray, RaycastHit2D hit)) // funktioniert super
            {                                               // funktioniert nur, wenn Script auf anzuklickendem Objekt liegt
                if (hit)
                {
                    Debug.Log("Treffer XXX"); // funktioniert
                    if (!thisItemSelected)
                    {
                        rend.sharedMaterial = material[1];
                        thisItemSelected = true;
                    }
                    else if (thisItemSelected)
                    {
                        rend.sharedMaterial = material[0];
                        thisItemSelected = false;
                    }
                }

            }
        }
    }
}
        
    
