using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SpecialFieldsLayout", menuName = "ScriptableObjects/Enemies/SpecialFieldLayout")]
public class LevelSO : PatternSO
{
    

    
    [SerializeField] private int neededECOPoints;
    public int NeededEcoPoints => neededECOPoints;

    
    
    // private void Awake()
    // {
    //     PatternPropertyDrawer.ChangeGridSize(this);
    //     Pattern.LoadDataString();
    // }


    public bool RequirementsMet(GameManager gameManager)
    {
        //Is the needed Eco points smaller than the current points?
        return gameManager.CurrentScore.EcoPoints > neededECOPoints;
    }
}