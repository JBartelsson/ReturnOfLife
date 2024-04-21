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

    public string visualization = "0";
    [SerializeReference] protected PlantFunction plantFunction;

    public PlantFunction PlantFunction { get => plantFunction; set => plantFunction = value; }

    public virtual bool ExecuteFunction(GridTile gridTile)
    {
        if (gridTile.Content.Any((x) => x.type == PlantableType.Plant))
        {
            return false;
        }
       return PlantFunction.Execute(this, gridTile);
    }

}
