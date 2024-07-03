using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventClearer : MonoBehaviour
{
    private void OnDestroy()
    {
        EventManager.ClearInvocationLists();
    }
}
