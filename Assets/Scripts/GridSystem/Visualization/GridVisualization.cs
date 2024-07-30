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
    [FormerlySerializedAs("spriteRenderer")] [SerializeField] private SpriteRenderer lifeFormSpriteRenderer;

    [FormerlySerializedAs("markedSprite")]
    [FormerlySerializedAs("markedMaterial")]
    [Header("UI Hover Effects")]
    [SerializeField] private Sprite editorSprite;
    [FormerlySerializedAs("previewMaterial")] [SerializeField] private Sprite hoverSprite;

    [FormerlySerializedAs("visualizer")] [SerializeField] private SpriteRenderer hoverEffectSpriteRenderer;

    [Header("Special Fields")] [SerializeField]
    private GameObject fieldMarker;

    [SerializeField] private List<FieldSprite> fieldMaterials;

    [SerializeField] private Sprite lavaSprite;
    [SerializeField] private Sprite normalGroundSprite;
    [SerializeField] private Sprite fieldPlantedSprite;
    [SerializeField] private SpriteRenderer groundStatusSpriteRenderer;
    [Header("Status References")] [SerializeField]
    private SpriteRenderer statusSpriteRenderer;

    [SerializeField] private Sprite deathSprite;
    [Header("Preview Settings")] 
    [SerializeField]
    private SpriteRenderer redCrossSpriteRenderer;

    [SerializeField] private Sprite cantPlaceSprite;
    [SerializeField] private Sprite canPlaceSprite;
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
    public class FieldSprite
    {
        public SpecialFieldType FieldType;
        public Sprite Sprite;
    }

    [SerializeField] private Material plantFertilizedMaterial;
    [SerializeField] private Material plantNonFertilizedMaterial;

    private void Awake()
    {
        lifeFormSpriteRenderer.sprite = null;
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
        
        visualizationState = VisualizationState.NONE;
        if (args.hoveredCardInstance.GetCardStats().EffectPattern.IsTileInPattern(args.hoveredGridTile, ownGridTile))
        {
            visualizationState = VisualizationState.PLANTING_PREVIEW;
        }

        redCrossSpriteRenderer.gameObject.SetActive(false);
        SetMarkedState();
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
        else
        {
            SetNewSprite((Sprite)null, previewSpriteRenderer);
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
        SetNewSprite((Sprite)null, lifeFormSpriteRenderer);
        SetNewSprite((Sprite)null, previewSpriteRenderer);
    }

    private void SetMarkedState()
    {
        hoverEffectSpriteRenderer.gameObject.SetActive(false);
        switch (visualizationState)
        {
            case VisualizationState.MARKED_FOR_EDITOR:
                hoverEffectSpriteRenderer.gameObject.SetActive(true);
                hoverEffectSpriteRenderer.sprite = editorSprite;
                break;
            case VisualizationState.PLANTING_PREVIEW:
                hoverEffectSpriteRenderer.gameObject.SetActive(true);
                hoverEffectSpriteRenderer.sprite = hoverSprite;
                break;
            case VisualizationState.NONE:
                break;
            default:
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
            SetNewSprite(ownGridTile.Content[0], lifeFormSpriteRenderer);
        }
        else
        {
            ClearGridTile();
        }

        //mark grid Tile if its used for an editor
        SetMarkedState();
        UpdateGroundStatus();
    }

    private void UpdateGroundStatus()
    {
        if (ownGridTile.IsLava())
        {
            groundStatusSpriteRenderer.sprite = lavaSprite;
            return;
        }

        if (ownGridTile.CardInstance != null)
        {
            bool condition = false;
            if (ownGridTile.SpecialField == null)
            {
                groundStatusSpriteRenderer.sprite = fieldPlantedSprite;
                return;
            }
            if (ownGridTile.SpecialField.AlreadyFulfilled)
            {
                groundStatusSpriteRenderer.sprite = fieldPlantedSprite;
                return;
            }
            
        }
        //Set Ground Sprite
        groundStatusSpriteRenderer.sprite =
            fieldMaterials.First((x) => x.FieldType == ownGridTile.FieldType).Sprite;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked {name}");
    }
}