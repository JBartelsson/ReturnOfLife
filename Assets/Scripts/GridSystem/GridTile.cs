using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CardData;

public class GridTile
{
    private GridTile topNeighbor, bottomNeighbor, leftNeighbor, rightNeighbor = null;
    private List<CardInstance> objects = new();
    private int x;
    private int y;
    private Grid grid;
    private SpecialFieldType fieldType = SpecialFieldType.NONE;
    private List<FieldModifier> fieldModifiers = new();

    public SpecialFieldType FieldType => fieldType;


    public event EventHandler<CardInstance> OnContentUpdated;

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
        if (!IsAccessible(callerArgs)) return;
        if (ContainsPlant())
        {
            EventManager.Game.Level.OnPlantSacrificed?.Invoke(new EventManager.GameEvents.LevelEvents.PlantSacrificedArgs()
            {
                SacrificeCallerArgs = callerArgs
            });
        }
        objects.Add(callerArgs.CallingCardInstance);
        OnContentUpdated?.Invoke(this, callerArgs.CallingCardInstance);
        grid.UpdateGridContent(x, y, this);
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
        grid.UpdateGridContent(x,y, this);
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

    public bool ContainsPlant()
    {
        if (this.Content.Count != 0)
        {
            return true;
        }

        return false;
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

    public bool IsAccessible(CallerArgs callerArgs)
    {
        if (objects.Count == 0) return true;
        //The first plant determines if the field is accessible, this needs to be a bit more structured as it can cause problems later on maybe
        return objects[0].CanBePlayedWith(callerArgs);

    }

    public bool HasNeighboredPlant()
    {
        bool hasNeighboredPlant = false;
        this.ForEachNeighbor((gridTile) =>
        {
            if (gridTile.ContainsPlant())
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
        OnContentUpdated = delegate { };
        fieldModifiers = new();
        grid.UpdateGridContent(x,y, this);
    }

    public void ResetSubscriptions()
    {
        OnContentUpdated = delegate { };
    }

    public void ForPattern(PatternSO pattern, Action<GridTile> action)
    {
        pattern.ForEachNormalFieldRelative((field, relativeX, relativeY) =>
        {
            GridTile affectedTile = grid.GetGridObject(x + relativeX, y + relativeY);
            if (affectedTile != null)
            {
                action(affectedTile);
            }
        });
    }
}