using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static CardData;

public class GridTile
{
    private GridTile topNeighbor, bottomNeighbor, leftNeighbor, rightNeighbor = null;
    private List<CardInstance> objects = new();
    private int x;
    private int y;
    private Grid grid;
    private SpecialField specialField;


    private SpecialFieldType fieldType = SpecialFieldType.NONE;
    private List<FieldModifier> fieldModifiers = new();

    public SpecialFieldType FieldType => fieldType;

    public SpecialField SpecialField
    {
        get => specialField;
        set => specialField = value;
    }

    public event UnityAction<OnContentUpdatedArgs> OnContentUpdated;
    
    public class OnContentUpdatedArgs : EventManager.GameEvents.Args
    {
        public GridTile GridTile;
        public CardInstance CardInstance;
    }

    public GridTile(Grid grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public GridTile TopNeighbor
    {
        get => topNeighbor;
        set => topNeighbor = value;
    }

    public GridTile BottomNeighbor
    {
        get => bottomNeighbor;
        set => bottomNeighbor = value;
    }

    public GridTile LeftNeighbor
    {
        get => leftNeighbor;
        set => leftNeighbor = value;
    }

    public GridTile RightNeighbor
    {
        get => rightNeighbor;
        set => rightNeighbor = value;
    }

    public List<CardInstance> Content
    {
        get => objects;
        set => objects = value;
    }

    public CardInstance CardInstance
    {
        get
        {
            if (objects.Count == 0) return null;
            return objects[0];
        }
    }

    public int X
    {
        get => x;
        set => x = value;
    }

    public int Y
    {
        get => y;
        set => y = value;
    }

   
    
    public List<FieldModifier> FieldModifiers
    {
        get => fieldModifiers;
        set => fieldModifiers = value;
    }

    public void AddObject(CallerArgs callerArgs)
    {
        if (ContainsAnyPlant())
        {
            EventManager.Game.Level.OnPlantSacrificed?.Invoke(new EventManager.GameEvents.LevelEvents.PlantSacrificedArgs()
            {
                SacrificeCallerArgs = callerArgs
            });
        }
        objects.Add(callerArgs.CallingCardInstance);
        OnContentUpdated?.Invoke(new OnContentUpdatedArgs()
        {
            GridTile = this,
            CardInstance = callerArgs.CallingCardInstance
        });
        grid.ChangeGridContent(x, y, this);
    }

    public void KillObject(CallerArgs callerArgs)
    {
        if (!ContainsAnyPlant()) return;
        CardInstance.KillLifeform(callerArgs);
        ClearSubscribers();
        grid.ChangeGridContent(x, y, this);

    }
    public void TryReviveLifeform(CallerArgs callerArgs)
    {
        if (CardInstance == null) return;
        if (!CardInstance.IsDead()) return;
        CallerArgs reviveCallerArgs = callerArgs.ReturnShallowCopy();
        reviveCallerArgs.playedTile = this;
        CardInstance oldCardInstance = CardInstance;
        oldCardInstance.TryReviveLifeform(reviveCallerArgs);
    }

    public void ClearTile()
    {
        objects.Clear();
    }
    

    public void ChangeFieldType(SpecialFieldType newFieldType, bool invokeEvent = true)
    {
        fieldType = newFieldType;
        switch (newFieldType)
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
                break;
            case SpecialFieldType.ESSENCE:
                break;
            case SpecialFieldType.UNLOCK_PLANT:
                break;
            case SpecialFieldType.HALF_ECO:
                break;
            case SpecialFieldType.TIME_PLAY:
                break;
            case SpecialFieldType.MULTIPLY:
                fieldModifiers.Add(new FieldModifier(FieldModifier.ModifierType.Multiplier, Constants.MULTIPLICATION_FIELD_MULTIPLIER));
                break;
            case SpecialFieldType.NORMAL_FIELD:
                break;
            case SpecialFieldType.NONE:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newFieldType), newFieldType, null);
        }
        if (invokeEvent)
        grid.ChangeGridContent(x,y, this);
    }
    
    


    public override string ToString()
    {
        CardInstance card = objects.FirstOrDefault();
        if (card != null)
        {
            return $"{x}, {y}: {card.CardData.name}, {fieldType}";
        }
        else
        {
            return $"{x}, {y}: No Plant, {fieldType}";
        }
    }

    public bool OnSameAxisAs(GridTile other)
    {
        if (this.x == other.x || this.y == other.y)
        {
            return true;
        }

        return false;
    }

    public float DistanceTo(GridTile other)
    {
        float xDistanceSqr = Mathf.Pow(other.x - this.x, 2);
        float yDistanceSqr = Mathf.Pow(other.y - this.y, 2);
        return Mathf.Sqrt(xDistanceSqr + yDistanceSqr);
    }

    public bool ContainsAnyPlant(bool deadIncluded = true)
    {
        if (this.Content.Count != 0)
        {
            if (CardInstance.IsDead()) return deadIncluded;
            return true;
        }
        return false;
    }

    public bool ContainsLivingPlant()
    {
        return ContainsAnyPlant(false);
    }
    

    public void ForEachNeighbor(Action<GridTile> action)
    {
        ForTopAndBottomNeighbor(action);
        ForLeftAndRightNeighbor(action);
    }

    public void ForTopAndBottomNeighbor(Action<GridTile> action)
    {
        if (this.TopNeighbor != null)
            action(this.TopNeighbor);
        if (this.BottomNeighbor != null)
            action(this.BottomNeighbor);
    }

    public void ForLeftAndRightNeighbor(Action<GridTile> action)
    {
        if (this.LeftNeighbor != null)
            action(this.LeftNeighbor);
        if (this.RightNeighbor != null)
            action(this.RightNeighbor);
    }

    public void ForEachAdjacentTile(Action<GridTile> action)
    {
        ForEachNeighbor(action);
        RightNeighbor?.ForTopAndBottomNeighbor(action);
        LeftNeighbor?.ForTopAndBottomNeighbor(action);
    }

    public bool IsAccessible(CallerArgs callerArgs, bool emptyAreAccessible = false)
    {
        //If no plant is there, the Card Can Execute Function must decide if the card is placable
        if (objects.Count == 0) return emptyAreAccessible;
        //The first plant determines if the field is accessible, this needs to be a bit more structured as it can cause problems later on maybe
        return objects[0].CanBeBePlantedOn(callerArgs);

    }

    public bool HasNeighboredPlant()
    {
        bool hasNeighboredPlant = false;
        this.ForEachNeighbor((gridTile) =>
        {
            if (gridTile.ContainsAnyPlant())
            {
                hasNeighboredPlant = true;
            }
        });
        return hasNeighboredPlant;
    }

    public void Reset()
    {
        objects.Clear();
        fieldType = SpecialFieldType.NONE;
        //empty the event
        ClearSubscribers();
        fieldModifiers = new();
        grid.ChangeGridContent(x,y, this);
    }

    public void ClearSubscribers()
    {
        OnContentUpdated = delegate { };

    }

    public void ResetSubscriptions()
    {
        OnContentUpdated = delegate { };
    }

    public void ForPattern(PatternSO pattern, Action<GridTile> action, bool includeCenter = false)
    {
        pattern.ForEachNormalFieldRelative((field, relativeX, relativeY) =>
        {
            GridTile affectedTile = grid.GetGridObject(x + relativeX, y + relativeY);
            if (affectedTile != null)
            {
                action(affectedTile);
            }
        }, includeCenter);
    }

    public bool IsLava()
    {
        return fieldType == SpecialFieldType.NONE;
    }
}