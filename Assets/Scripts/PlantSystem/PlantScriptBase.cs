using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EXECUTION_TYPE
{
    IMMEDIATE,
    DELAY,
    AFTER_PLACEMENT,
    NONE
}
public class PlantScriptBase
{
    private CardInstance cardInstance;
    private EXECUTION_TYPE executionType = EXECUTION_TYPE.AFTER_PLACEMENT;
    public CardInstance CardInstance { get => cardInstance; set => cardInstance = value; }
    public EXECUTION_TYPE ExecutionType { get => executionType; set => executionType = value; }
}
