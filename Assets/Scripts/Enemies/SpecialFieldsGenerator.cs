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
    SHOP, CARD_REMOVE, CARD_ADD, RETRIGGER, DUPLICATE, MANA, ESSENCE, UNLOCK_PLANT, HALF_ECO, TIME_PLAY, MULTIPLY, NORMAL_FIELD, NONE
}
