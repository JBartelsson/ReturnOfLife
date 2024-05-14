using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridVisualization : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material standardVisualizationMaterial;
    [FormerlySerializedAs("MarkedMaterial")] [FormerlySerializedAs("standardMarkedMaterial")] [SerializeField] private Material markedMaterial;
    [SerializeField] private MeshRenderer visualizer;
    [SerializeField] private ParticleSystem fertilzedParticles;

    private void Awake()
    {
        spriteRenderer.sprite = null;
        visualizer.sharedMaterial = standardVisualizationMaterial;
        fertilzedParticles.Stop();
    }

    public void SetNewSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }

    public void SetNewSprite(PlantInstance plantInstance)
    {
        SetNewSprite((plantInstance.Plantable.PlantSprite));
        if (plantInstance.IsBasicFertilized())
        {
            fertilzedParticles.Play();
        }
        else
        {
            fertilzedParticles.Stop();

        }
    }

    public void SetMarkedState(bool marked)
    {
        visualizer.sharedMaterial = marked ? markedMaterial : standardVisualizationMaterial;

    }
}
