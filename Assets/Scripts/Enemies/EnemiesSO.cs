using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/Enemies/Enemy")]
public class EnemiesSO : ScriptableObject
{
   

    public enum MISSION_TYPE
    {
        ECO,
        FIELDS,
        SPECIAL
    }

    [Serializable]
    public class MissionGoal
    {
        public MISSION_TYPE MissionType;
        public int Amount = 0;
    }

    [SerializeField] private List<MissionGoal> missions;
    [FormerlySerializedAs("specialFieldsLayout")] [SerializeField] private SpecialFieldsPositionsSO specialFieldsPositions;
    [FormerlySerializedAs("allowedSpecialFields")] [SerializeField] private List<SpecialFieldsLayoutSO> allowedSpecialFieldLayouts;
    [SerializeField] private List<SpecialFieldType> allowedSpecialFields;

    public SpecialFieldPrioritySO SpecialFieldPriority => specialFieldPriority;

    [SerializeField] private SpecialFieldPrioritySO specialFieldPriority;
    

    public List<MissionGoal> Missions
    {
        get => missions;
        set => missions = value;
    }

    public SpecialFieldsPositionsSO SpecialFieldsPositions
    {
        get => specialFieldsPositions;
        set => specialFieldsPositions = value;
    }
    public List<SpecialFieldsLayoutSO> AllowedSpecialFieldLayouts
    {
        get => allowedSpecialFieldLayouts;
        set => allowedSpecialFieldLayouts = value;
    }
    public List<SpecialFieldType> AllowedSpecialFields
    {
        get => allowedSpecialFields;
        set => allowedSpecialFields = value;
    }

    public bool RequirementsMet()
    {
        return true;
    }
}