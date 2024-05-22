using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SpecialFieldPriority", menuName = "ScriptableObjects/SpecialFieldPriority")]
public class SpecialFieldPrioritySO : ScriptableObject
{
    [SerializeField] List<SpecialFieldType> priority;

    public List<SpecialFieldType> Priority => priority;
}
