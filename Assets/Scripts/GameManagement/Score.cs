using System;
using UnityEngine;

[Serializable]
public struct Score
{
    public Score(int ecoPoints = 0, float mult = 0, float multMultiplier = 1)
    {
        this.ecoPoints = ecoPoints;
        this.mult = mult;
        this.multMultiplier = multMultiplier;
    }

    public int EcoPoints
    {
        get => ecoPoints;
        set => ecoPoints = value;
    }

    public float Mult
    {
        get => mult;
        set => mult = value;
    }

    public int GetTotalScore()
    {
        return Mathf.FloorToInt(ecoPoints * mult);
    }
    
   
    public float MultMultiplier
    {
        get => multMultiplier;
        set => multMultiplier = value;
    }

    [SerializeField] private int ecoPoints;
    [SerializeField] private float mult;

   

    [SerializeField] private float multMultiplier;
    
    public static Score operator +(Score a, Score b)
        => new Score(a.ecoPoints + b.ecoPoints, (a.mult + b.mult) * b.multMultiplier);

    public override string ToString()
    {
        return $"Eco: {ecoPoints}, Mult: {mult}, MultMult: {multMultiplier}";
    }
}