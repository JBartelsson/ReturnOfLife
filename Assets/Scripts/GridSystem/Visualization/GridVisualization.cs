using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class GridVisualization : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material standardVisualizationMaterial;

    [SerializeField] private Material markedMaterial;

    [SerializeField] private Material previewMaterial;

    [SerializeField] private MeshRenderer visualizer;

    [Header("Special Fields")] [SerializeField]
    private GameObject fieldMarker;

    [SerializeField] private MeshRenderer fieldMarkerMeshRenderer;
    [SerializeField] private List<FieldMaterial> fieldMaterials;
    private GridTile ownGridTile;
    private VisualizationState visualizationState = VisualizationState.NONE;

    public enum VisualizationState
    {
        MARKED_FOR_EDITOR,
        PLANTING_PREVIEW,
        NONE
    }

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
        fieldMarker.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Game.UI.OnPlantHoverChanged += OnPlantHoverChanged;
        EventManager.Game.UI.OnPlantHoverCanceled += OnPlantHoverCanceled;
    }

    private void OnPlantHoverCanceled()
    {
        visualizationState = VisualizationState.NONE;
        UpdateContent();
    }

    private void OnDisable()
    {
        EventManager.Game.UI.OnPlantHoverChanged -= OnPlantHoverChanged;
    }


    private void OnPlantHoverChanged(EventManager.GameEvents.UIEvents.OnHoverChangedArgs args)
    {
        ClearGridTile();
        visualizationState = VisualizationState.NONE;
        if (args.hoveredCardInstance.GetCardStats().EffectPattern.IsTileInPattern(args.hoveredGridTile, ownGridTile))
        {
            visualizationState = VisualizationState.PLANTING_PREVIEW;
        }

        UpdateContent();
        if (args.hoveredGridTile == ownGridTile)
        {
            SetNewSprite(args.hoveredCardInstance, true);
        }
    }

    private void SetNewSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }

    private void SetNewSprite(CardInstance cardInstance, bool ghostPreview = false)
    {
        Sprite newSprite = cardInstance.CardData.PlantSprite;
        SetNewSprite(newSprite);
        if (cardInstance.IsUpgraded())
        {
            spriteRenderer.material = plantFertilizedMaterial;
            var croppedTexture = new Texture2D((int)newSprite.rect.width,
                (int)newSprite.rect.height);
            var pixels = newSprite.texture.GetPixels(0,
                0,
                (int)newSprite.rect.width,
                (int)newSprite.rect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            plantFertilizedMaterial.SetTexture("_MainTex", croppedTexture);
        }
        else
        {
            spriteRenderer.material = plantNonFertilizedMaterial;
        }

        if (ghostPreview)
        {
            spriteRenderer.material.SetFloat("_Alpha", Constants.HOVERED_ALPHA_VALUE);
        }
    }

    private void ClearGridTile()
    {
        SetNewSprite((Sprite)null);
    }

    private void SetMarkedState()
    {
        switch (visualizationState)
        {
            case VisualizationState.MARKED_FOR_EDITOR:
                visualizer.sharedMaterial = markedMaterial;
                break;
            case VisualizationState.PLANTING_PREVIEW:
                visualizer.sharedMaterial = previewMaterial;
                break;
            case VisualizationState.NONE:
                visualizer.sharedMaterial = standardVisualizationMaterial;
                break;
            default:
                visualizer.sharedMaterial = markedMaterial;
                break;
        }
    }

    public void UpdateContent(GridTile gridObject)
    {
        ownGridTile = gridObject;
        UpdateContent();
    }

    public void UpdateContent()
    {
        if (ownGridTile == null) return;

        if (ownGridTile.Content.Count > 0)
        {
            SetNewSprite(ownGridTile.Content[0]);
        }
        else
        {
            ClearGridTile();
        }

        //mark grid Tile if its used for an editor
        SetMarkedState();

        ShowFieldType(ownGridTile.FieldType);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked {name}");
    }
}