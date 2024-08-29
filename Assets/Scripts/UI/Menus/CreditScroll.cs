using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    [SerializeField] private GameObject scrollingObjects;
    [SerializeField] private float scrollStoppingPoint;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float scrollSpeedMultipier;
    private float currentSpeed;

    void Start()
    {
        currentSpeed = scrollSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollingObjects.transform.position.y >= scrollStoppingPoint)
            // start speed damping
            return;
        if (Input.GetMouseButtonDown(0)) currentSpeed = scrollSpeed * scrollSpeedMultipier;
        if (Input.GetMouseButtonUp(0)) currentSpeed = scrollSpeed;
        scrollingObjects.transform.position += new Vector3(0, currentSpeed, 0);
        
    }
}
