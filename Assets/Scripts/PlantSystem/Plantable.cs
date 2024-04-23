using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Plantable")]
public class Plantable : ScriptableObject
{
    public enum PlantableType
    {
        Plant, Fertilizer
    }
    public PlantableType type;
    public int manaCost = 1;
    public int turnDelay = 0;

    public string visualization = "0";
    [SerializeReference] protected PlantFunction plantFunction;
    [SerializeReference] protected PlantEditor plantEditor = null;

    public PlantFunction PlantFunction { get => plantFunction; set => plantFunction = value; }
    public PlantEditor PlantEditor { get => plantEditor; set => plantEditor = value; }

    public virtual bool ExecuteFunction(GridTile gridTile, bool needNeighbor)
    {
        if (gridTile.ContainsPlant()) return false;
        if (needNeighbor && !gridTile.HasNeighboredPlant()) return false;
       return PlantFunction.Execute(this, gridTile);
    }

}
