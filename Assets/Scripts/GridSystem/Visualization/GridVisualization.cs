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
    [FormerlySerializedAs("fertilzedParticles")] [SerializeField] private ParticleSystem fertilizedParticles;
    [SerializeField] private GameObject fieldMarker;

    private void Awake()
    {
        spriteRenderer.sprite = null;
        visualizer.sharedMaterial = standardVisualizationMaterial;
        fertilizedParticles.Stop();
        fieldMarker.SetActive(false);

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
            fertilizedParticles.Play();
        }
        else
        {
            fertilizedParticles.Stop();

        }
    }

    public void SetMarkedState(bool marked)
    {
        visualizer.sharedMaterial = marked ? markedMaterial : standardVisualizationMaterial;

    }

    public void UpdateContent(GridTile gridObject)
    {
        if (gridObject.Content.Count > 0)
            SetNewSprite(gridObject.Content[0]);

        //mark grid Tile if its used for an editor
        SetMarkedState(gridObject.Marked);

        ShowFieldType(gridObject.FieldType);
    }

    private void ShowFieldType(SpecialFieldType gridObjectFieldType)
    {
        fieldMarker.SetActive(false);
        if (gridObjectFieldType != SpecialFieldType.NONE)
        {
            fieldMarker.SetActive(true);
        }
        switch (gridObjectFieldType)
        {
            case SpecialFieldType.SHOP:
                break;
            case SpecialFieldType.CARD_REMOVE:
                break;
            case SpecialFieldType.CARD_ADD:
                break;
            case SpecialFieldType.RETRIGGER:
                break;
            case SpecialFieldType.DUPLICATE:
                break;
            case SpecialFieldType.MANA:
                fieldMarker.SetActive(true);
                break;
            case SpecialFieldType.ESSENCE:
                break;
            case SpecialFieldType.UNLOCK_PLANT:
                break;
            case SpecialFieldType.HALF_ECO:
                break;
            case SpecialFieldType.TIME_PLAY:
                break;
            case SpecialFieldType.NONE:
                break;
            default:
                break;
        }
    }
}
