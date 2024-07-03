using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFertilizerFunction : CardFunctionBase
{
    public override void ExecuteCard(CallerArgs callerArgs)
    {
        callerArgs.gameManager.AddWisdom(Fertilizer.Basic);
    }

    public override bool CanExecute(CallerArgs callerArgs)
    {
        return true;
    }
}
