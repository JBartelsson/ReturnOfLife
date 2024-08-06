using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipMemory : MonoBehaviour
{
    public static TipMemory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Resetting Parenting structure so dontdestroyonload does work
            this.transform.parent = null;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
