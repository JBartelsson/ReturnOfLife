using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFertilizerFunction : PlantFunctionBase
{
    public override void Execute(CallerArgs callerArgs)
    {
        callerArgs.gameManager.AddFertilizer(Fertilizer.Basic);
    }

    public override bool CanExecute(CallerArgs callerArgs)
    {
        return true;
    }
}
