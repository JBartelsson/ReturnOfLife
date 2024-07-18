using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GridVisualization : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material standardVisualizationMaterial;

    [SerializeField] private Material markedMaterial;


    [SerializeField] private MeshRenderer visualizer;

    [Header("Special Fields")] [SerializeField]
    private GameObject fieldMarker;

    [SerializeField] private MeshRenderer fieldMarkerMeshRenderer;
    [SerializeField] private List<FieldMaterial> fieldMaterials;

    [Header("Status References")] [SerializeField]
    private SpriteRenderer statusSpriteRenderer;

    [SerializeField] private Sprite deathSprite;
    [Header("Preview Settings")] 
    [SerializeField]
    private SpriteRenderer redCrossSpriteRenderer;

    [SerializeField] private Sprite cantPlaceSprite;
    [SerializeField] private Sprite canPlaceSprite;
    [SerializeField] private Material previewMaterial;
    [SerializeField]
    private SpriteRenderer previewSpriteRenderer;

    private GridTile ownGridTile;


    private VisualizationState visualizationState = VisualizationState.NONE;

    public GridTile OwnGridTile => ownGridTile;
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
        statusSpriteRenderer.gameObject.SetActive(false);

        redCrossSpriteRenderer.gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        EventManager.Game.UI.OnPlantHoverChanged += OnPlantHoverChanged;
        EventManager.Game.UI.OnPlantHoverCanceled += OnPlantHoverCanceled;
        EventManager.Game.UI.OnHoverForEditor += OnHoverForEditor;
    }
    private void OnDisable()
    {
        EventManager.Game.UI.OnPlantHoverChanged -= OnPlantHoverChanged;
        EventManager.Game.UI.OnPlantHoverCanceled -= OnPlantHoverCanceled;
        EventManager.Game.UI.OnHoverForEditor -= OnHoverForEditor;
    }

    private void OnHoverForEditor(EventManager.GameEvents.UIEvents.OnHoverForEditorArgs args)
    {
        if (args.hoveredGridTile != ownGridTile) return;
        visualizationState = VisualizationState.MARKED_FOR_EDITOR;
        UpdateContent();
    }

    private void OnPlantHoverCanceled()
    {
        visualizationState = VisualizationState.NONE;
        redCrossSpriteRenderer.gameObject.SetActive(false);
        previewSpriteRenderer.sprite = null;
        UpdateContent();
    }


    private void OnPlantHoverChanged(EventManager.GameEvents.UIEvents.OnHoverChangedArgs args)
    {
        ClearGridTile();
        visualizationState = VisualizationState.NONE;
        if (args.hoveredCardInstance.GetCardStats().EffectPattern.IsTileInPattern(args.hoveredGridTile, ownGridTile))
        {
            visualizationState = VisualizationState.PLANTING_PREVIEW;
        }

        redCrossSpriteRenderer.gameObject.SetActive(false);
        UpdateContent();
        if (args.hoveredGridTile == ownGridTile)
        {
            redCrossSpriteRenderer.gameObject.SetActive(true);

            if (!args.hoveredCardInstance.CanExecute(args.hoverCallerArgs))
            {
                redCrossSpriteRenderer.sprite = cantPlaceSprite;
            }
            else
            {
                redCrossSpriteRenderer.sprite = canPlaceSprite;

            }
            SetNewSprite(args.hoveredCardInstance, previewSpriteRenderer, true);
        }
    }

    private void SetNewSprite(Sprite newSprite, SpriteRenderer target)
    {
        target.sprite = newSprite;
    }

    private void SetPreviewSprite(Sprite newPreviewSprite)
    {
        previewSpriteRenderer.sprite = newPreviewSprite;
    }

    private void SetNewSprite(CardInstance cardInstance, SpriteRenderer target, bool ghost = false)
    {
        Sprite newSprite = cardInstance.CardData.PlantSprite;
        SetNewSprite(newSprite, target);
        if (cardInstance.IsUpgraded())
        {
            target.material = plantFertilizedMaterial;
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
            target.material = plantNonFertilizedMaterial;
        }

        if (ghost)
        {
            target.material.SetFloat("_Alpha", Constants.HOVERED_ALPHA_VALUE);
        }

        //Dead Vision
        statusSpriteRenderer.gameObject.SetActive(false);
        if (cardInstance.IsDead())
        {
            statusSpriteRenderer.gameObject.SetActive(true);
            statusSpriteRenderer.sprite = deathSprite;
            target.material.SetFloat("_Saturation", Constants.DEATH_SATURATION_VALUE);
        }

    }

    private void ClearGridTile()
    {
        SetNewSprite((Sprite)null, spriteRenderer);
        SetNewSprite((Sprite)null, previewSpriteRenderer);
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
            SetNewSprite(ownGridTile.Content[0], spriteRenderer);
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
        if (gridObjectFieldType == SpecialFieldType.NONE || gridObjectFieldType == SpecialFieldType.NORMAL_FIELD)
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