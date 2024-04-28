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
    private PlantInstance plantInstance;
    private EXECUTION_TYPE executionType = EXECUTION_TYPE.AFTER_PLACEMENT;
    public PlantInstance PlantInstance { get => plantInstance; set => plantInstance = value; }
    public EXECUTION_TYPE ExecutionType { get => executionType; set => executionType = value; }
}
