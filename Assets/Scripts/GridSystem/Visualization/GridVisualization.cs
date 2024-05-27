using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GridVisualization : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material standardVisualizationMaterial;

    [FormerlySerializedAs("MarkedMaterial")] [FormerlySerializedAs("standardMarkedMaterial")] [SerializeField]
    private Material markedMaterial;

    [SerializeField] private MeshRenderer visualizer;

    [FormerlySerializedAs("fertilzedParticles")] [SerializeField]
    private ParticleSystem fertilizedParticles;

    [Header("Special Fields")] [SerializeField]
    private GameObject fieldMarker;

    [SerializeField] private MeshRenderer fieldMarkerMeshRenderer;
    [SerializeField] private List<FieldMaterial> fieldMaterials;

    [Serializable]
    public class FieldMaterial
    {
        public SpecialFieldType FieldType;
        public Material Material;
    }

    [SerializeField] private Material plantFertilizedMaterial;
    [SerializeField] private Material plantNonFertilizedMaterial;

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
        Sprite newSprite = plantInstance.Plantable.PlantSprite;
        SetNewSprite(newSprite);
        if (plantInstance.IsBasicFertilized())
        {
            spriteRenderer.material = plantFertilizedMaterial;
            Debug.Log($"Sprite Width: {newSprite.rect.width}, {newSprite.rect.height}");
            var croppedTexture = new Texture2D((int)newSprite.rect.width,
                (int)newSprite.rect.height);
            var pixels = newSprite.texture.GetPixels(  0, 
                0, 
                (int)newSprite.rect.width, 
                (int)newSprite.rect.height );
            Debug.Log($"PIXELS LENGTH: {pixels.Length}");
            croppedTexture.SetPixels( pixels );
            croppedTexture.Apply();
            plantFertilizedMaterial.SetTexture("_MainTex", croppedTexture);
        }
        else
        {
            spriteRenderer.material = plantNonFertilizedMaterial;

        }
    }

    private void ClearGridTile()
    {
        fertilizedParticles.Stop();
        SetNewSprite((Sprite)null);
    }

    public void SetMarkedState(bool marked)
    {
        visualizer.sharedMaterial = marked ? markedMaterial : standardVisualizationMaterial;
    }

    public void UpdateContent(GridTile gridObject)
    {
        if (gridObject.Content.Count > 0)
        {
            SetNewSprite(gridObject.Content[0]);
        }
        else
        {
            ClearGridTile();
        }

        //mark grid Tile if its used for an editor
        SetMarkedState(gridObject.Marked);

        ShowFieldType(gridObject.FieldType);
    }

    private void ShowFieldType(SpecialFieldType gridObjectFieldType)
    {
        if (gridObjectFieldType == SpecialFieldType.NONE)
        {
            fieldMarker.SetActive(false);
            return;
        }

        fieldMarker.SetActive(true);
        fieldMarkerMeshRenderer.sharedMaterial =
            fieldMaterials.First((x) => x.FieldType == gridObjectFieldType).Material;
    }
}