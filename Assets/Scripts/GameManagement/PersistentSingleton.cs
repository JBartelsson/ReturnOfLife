using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this as T;
        //Resetting Parenting structure so dontdestroyonload does work
        this.transform.parent = null;
        DontDestroyOnLoad(this);
    }
}
