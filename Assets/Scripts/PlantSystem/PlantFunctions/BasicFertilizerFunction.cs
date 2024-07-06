using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFertilizerFunction : CardFunctionBase
{
    public override void ExecuteCard(CallerArgs callerArgs)
    {
        // callerArgs.gameManager.AddWisdom(WisdomType.Basic);
    }

    public override bool CanExecute(CallerArgs callerArgs)
    {
        return true;
    }
}
