using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFertilizerFunction : PlantFunctionBase
{
    public override bool Execute(CallerArgs callerArgs)
    {
        callerArgs.gameManager.AddFertilizer(Fertilizer.Basic);
        return true;
    }
}
