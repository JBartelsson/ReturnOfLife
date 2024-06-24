using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldModifier
{
    public enum ModifierType
    {
        Multiplier
    }

    public ModifierType modifierType;
    public float modifierAmount;


    public FieldModifier(ModifierType modifierType, float modifierAmount)
    {
        this.modifierType = modifierType;
        this.modifierAmount = modifierAmount;
    }
}
