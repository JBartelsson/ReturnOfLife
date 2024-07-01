using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "ScriptableObjects/PlantSystem/Pattern")]
public class PatternSO : ScriptableObject
{
    [SerializeField] private Pattern pattern;

    public Pattern Pattern
    {
        get => pattern;
        set => pattern = value;
    }

    
  
}
