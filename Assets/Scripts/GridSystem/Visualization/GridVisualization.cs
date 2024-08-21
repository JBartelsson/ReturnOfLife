using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GridVisualization : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer lifeFormSpriteRenderer;

    [SerializeField] private Transform lifeFormSpriteRendererAnimationTarget;

    [Header("UI Hover Effects")] [SerializeField]
    private Sprite editorSprite;

    [SerializeField] private Sprite hoverSprite;

    [SerializeField] private SpriteRenderer hoverEffectSpriteRenderer;

    [Header("Special Fields")] [SerializeField]
    private GameObject fieldMarker;

    [SerializeField] private List<FieldSprite> fieldMaterials;


    [SerializeField] private Sprite fieldPlantedSprite;
    [SerializeField] private Sprite fieldPlantedMPSprite;
    [SerializeField] private SpriteRenderer groundStatusSpriteRenderer;

    [Header("Status References")] [SerializeField]
    private SpriteRenderer statusSpriteRenderer;

    [SerializeField] private Sprite deathSprite;

    [Header("Preview Settings")] [SerializeField]
    private SpriteRenderer redCrossSpriteRenderer;

    [SerializeField] private Sprite cantPlaceSprite;
    [SerializeField] private Sprite canPlaceSprite;
    [SerializeField] private SpriteRenderer previewSpriteRenderer;

    [FormerlySerializedAs("placementArrowSpriteRenderer")] [SerializeField]
    private WiggleAnimation placementArrowWiggle;

    [Header("Text Settings")] [SerializeField]
    private TextMeshPro secondMoveText;

    [SerializeField] private float secondMoveScale = 1.1f;
    [SerializeField] private float secondMoveArrowScale = .54f;
    [SerializeField] private TextMeshPro notEnoughManaText;
    private Tween secondMoveArrowTween;
    private Tween secondMoveScaleTween;

    private GridTile ownGridTile;

    private Vector3 lifeformSpriteOGPosition;


    private VisualizationState visualizationState = VisualizationState.NONE;
    private bool markedForMove = false;

    public GridTile OwnGridTile => ownGridTile;

    public enum VisualizationState
    {
        MARKED_FOR_MOVE,
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

    [Header("Particle Systems")] [SerializeField]
    private ParticleSystem groundSpawnParticle;

    [SerializeField] private ParticleSystem spawnRaysParticle;

    private void Awake()
    {
        lifeFormSpriteRenderer.sprite = null;
        fieldMarker.SetActive(false);
        statusSpriteRenderer.gameObject.SetActive(false);
        secondMoveText.gameObject.SetActive(false);
        redCrossSpriteRenderer.gameObject.SetActive(false);
        notEnoughManaText.gameObject.SetActive(false);
        placementArrowWiggle.gameObject.SetActive(false);
    }

    private void Start()
    {
        lifeformSpriteOGPosition = lifeFormSpriteRenderer.transform.position;
    }

    private void OnEnable()
    {
        EventManager.Game.UI.OnLifeformHoverChanged += OnPlantHoverChanged;
        EventManager.Game.UI.OnLifeformHoverCanceled += OnPlantHoverCanceled;
        EventManager.Game.UI.OnHoverForSecondMove += OnHoverForSecondMove;
        EventManager.Game.UI.OnSecondMoveNeeded += OnSecondMoveNeeded;
        EventManager.Game.UI.OnSecondMoveQueueEmpty += OnSecondMoveQueueEmpty;
        EventManager.Game.UI.OnCardSelectGridTileUpdate += OnCardSelectGridTileUpdate;
        EventManager.Game.Level.OnLifeformPlanted += OnLifeformPlanted;
        EventManager.Game.Level.OnTriggerSpecialField += OnTriggerMpField;
        EventManager.Game.UI.OnSecondMoveStillOpen += OnSecondMoveStillOpen;
    }

    private void OnSecondMoveStillOpen()
    {
        if (secondMoveArrowTween != null) secondMoveArrowTween.Complete();
        if (secondMoveScaleTween != null) secondMoveScaleTween.Complete();
        secondMoveScaleTween =
            secondMoveText.transform.DOScale(secondMoveScale, Constants.UI_FAST_FADE_SPEED).SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutCubic);
        secondMoveArrowTween =
            placementArrowWiggle.transform.DOScale(secondMoveArrowScale, Constants.UI_FAST_FADE_SPEED)
                .SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutCubic);
    }

    private void OnTriggerMpField(EventManager.GameEvents.LevelEvents.TriggerSpecialFieldArgs arg0)
    {
        if (arg0.triggeredField.FieldType != SpecialFieldType.MULTIPLY) return;
        if (!arg0.triggeredField.SpecialFieldGridTiles.Contains(ownGridTile)) return;
        spawnRaysParticle.Play();
    }

    private void OnLifeformPlanted(EventManager.GameEvents.LevelEvents.OnLifeformPlantedArgs arg0)
    {
        markedForMove = false;
        SetMarkedState();
    }

    private void OnCardSelectGridTileUpdate(EventManager.GameEvents.UIEvents.CardSelectGridUpdateArgs args)
    {
        if (args.UpdatedTile != ownGridTile) return;
        if (args.Status)
        {
            markedForMove = true;
            SetMarkedState();
        }
        else
        {
            markedForMove = false;
            SetMarkedState();
        }
    }

    private void OnSecondMoveQueueEmpty()
    {
        secondMoveText.DOFade(0f, Constants.UI_FAST_FADE_SPEED)
            .OnComplete(() => secondMoveText.gameObject.SetActive(false));
        markedForMove = false;
        SetMarkedState();
    }

    private void OnSecondMoveNeeded(EventManager.GameEvents.UIEvents.OnSecondMoveNeededArgs args)
    {
        if (args.editorOriginGridTile == ownGridTile)
        {
            secondMoveText.DOFade(0f, 0f);
            secondMoveText.gameObject.SetActive(true);
            secondMoveText.text = args.SecondMoveCallerArgs.CallingCardInstance.CardData.SecondMoveText + "\n" +
                                  args.SecondMoveCallerArgs.SecondMoveNumber + "x";
            Tween fadeTween = secondMoveText.DOFade(1f, Constants.UI_FAST_FADE_SPEED);
            if (args.SecondMoveCallerArgs.CallingCardInstance.CardData.RuntimePoints != 0)
            {
                fadeTween.SetDelay(Constants.UI_POINT_DISAPPEAR_SPEED * 3);
            }

            fadeTween.Play();
        }
    }


    private void OnDisable()
    {
        EventManager.Game.UI.OnLifeformHoverChanged -= OnPlantHoverChanged;
        EventManager.Game.UI.OnLifeformHoverCanceled -= OnPlantHoverCanceled;
        EventManager.Game.UI.OnHoverForSecondMove -= OnHoverForSecondMove;
    }

    private void OnHoverForSecondMove(EventManager.GameEvents.UIEvents.OnHoverForEditorArgs args)
    {
        if (args.hoveredGridTile != ownGridTile) return;
        markedForMove = true;
        SetMarkedState();
    }

    private void ShowWiggleArrow()
    {
        if (placementArrowWiggle.gameObject.activeInHierarchy) return;
        placementArrowWiggle.gameObject.SetActive(true);
        placementArrowWiggle.StartAnimation();
    }

    private void OnPlantHoverCanceled()
    {
        visualizationState = VisualizationState.NONE;
        markedForMove = false;
        redCrossSpriteRenderer.gameObject.SetActive(false);
        previewSpriteRenderer.sprite = null;
        secondMoveText.gameObject.SetActive(false);
        notEnoughManaText.gameObject.SetActive(false);
        placementArrowWiggle.gameObject.SetActive(false);
        SetMarkedState();
    }


    private void OnPlantHoverChanged(EventManager.GameEvents.UIEvents.OnLifeformChangedArgs args)
    {
        visualizationState = VisualizationState.NONE;
        if (args.hoveredCardInstance.GetCardStats().EffectPattern.IsTileInPattern(args.hoveredGridTile, ownGridTile))
        {
            visualizationState = VisualizationState.PLANTING_PREVIEW;
        }

        redCrossSpriteRenderer.gameObject.SetActive(false);
        notEnoughManaText.gameObject.SetActive(false);
        SetMarkedState();
        if (args.hoveredGridTile == ownGridTile)
        {
            SetNewSprite(args.hoveredCardInstance, previewSpriteRenderer, true);
            if (!GameManager.Instance.EnoughManaFor(args.hoveredCardInstance))
            {
                notEnoughManaText.gameObject.SetActive(true);
                return;
            }

            if (!args.hoveredCardInstance.CanExecute(args.hoverCallerArgs))
            {
                redCrossSpriteRenderer.gameObject.SetActive(true);
                redCrossSpriteRenderer.sprite = cantPlaceSprite;
            }
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

    private void AnimateSpawn()
    {
        groundSpawnParticle.Play();
        lifeFormSpriteRenderer.transform.position = lifeformSpriteOGPosition;
        lifeFormSpriteRenderer.transform.DOMove(lifeFormSpriteRendererAnimationTarget.position, .3f)
            .SetEase(Ease.OutSine);
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
            // statusSpriteRenderer.gameObject.SetActive(true);
            // statusSpriteRenderer.sprite = deathSprite;
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

        // placementArrowWiggle.gameObject.SetActive(false);
        if (markedForMove)
        {
            hoverEffectSpriteRenderer.gameObject.SetActive(true);
            ShowWiggleArrow();
            hoverEffectSpriteRenderer.sprite = editorSprite;
        }

        switch (visualizationState)
        {
            case VisualizationState.MARKED_FOR_MOVE:
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
            if (!ownGridTile.CardInstance.IsDead())
                AnimateSpawn();
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
            groundStatusSpriteRenderer.sprite =
                fieldMaterials.FirstOrDefault((x) => x.FieldType == SpecialFieldType.NONE)?.Sprite;
            return;
        }

        if (ownGridTile.CardInstance != null)
        {
            bool condition = false;
            if (ownGridTile.SpecialField == null)
            {
                Debug.Log("Own Grid Tile has no Special Field");
                groundStatusSpriteRenderer.sprite = fieldPlantedSprite;
                return;
            }
            Debug.Log("Own Grid Tile has Special Field");

            groundStatusSpriteRenderer.sprite = fieldPlantedMPSprite;
            return;
        }
    

    //Set Ground Sprite
    groundStatusSpriteRenderer.sprite = fieldMaterials.First((x) => x.FieldType == ownGridTile.FieldType).Sprite;
}

public void OnPointerClick(PointerEventData eventData)
{
    Debug.Log($"Clicked {name}");
}
}