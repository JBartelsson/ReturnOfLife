using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialFieldsGenerator : MonoBehaviour
{
    
    
    public static void GenerateSpecialFields(GridManager gridManager, EnemiesSO currentEnemy)
    {
        
        List<LevelSO> allowedLayouts = new List<LevelSO>(currentEnemy.AllowedSpecialFieldLayouts);
        List<SpecialFieldType> allowedFields = new List<SpecialFieldType>(currentEnemy.AllowedSpecialFields);
        // foreach (SpecialFieldsLayoutSO.Index index in currentEnemy.LevelLayout.GetRandomlyMirroredSelectedFields())
        // {
        //     SpecialFieldsLayoutSO randomLayout = allowedLayouts[Random.Range(0, allowedLayouts.Count)];
        //     SpecialFieldType randomSpecialFieldType = allowedFields[Random.Range(0, allowedFields.Count)];
        //     SpecialFieldsLayoutSO.Index offset = randomLayout.GetCenterField() - index; 
        //     foreach (SpecialFieldsLayoutSO.Index specialFieldIndex in randomLayout.GetRandomlyMirroredSelectedFields())
        //     {
        //         gridManager.Grid.AddSpecialField(specialFieldIndex, offset, randomSpecialFieldType, currentEnemy);
        //         allowedLayouts.Remove(randomLayout);
        //         allowedFields.Remove(randomSpecialFieldType);
        //     }
        // }
    }
}

public enum SpecialFieldType
{
    SHOP = 0, CARD_REMOVE = 1, CARD_ADD = 2, RETRIGGER = 3, DUPLICATE = 4, MANA = 5, ESSENCE = 6, UNLOCK_PLANT = 7, HALF_ECO = 8, TIME_PLAY= 9, MULTIPLY = 10, NORMAL_FIELD = 11, NONE = 12, CENTER = 13
}
