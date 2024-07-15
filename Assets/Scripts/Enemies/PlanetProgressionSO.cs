using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "PlanetProgression", menuName = "ScriptableObjects/Enemies/PlanetProgression")]
public class PlanetProgressionSO : ScriptableObject
{
    public enum Stage
    {
        STAGE1, STAGE2, STAGE3, STAGE4, BOSS
    }
[Serializable]
    public class StageEnemies
    {
        [SerializeField] private Stage stage;
        [SerializeField] private List<LevelSO> allowedEnemies;

        public Stage Stage => stage;

        public List<LevelSO> AllowedEnemies => allowedEnemies;
    }

    [SerializeField] private List<StageEnemies> progression;

    public List<StageEnemies> Progression => progression;

    public LevelSO GetRandomEnemy(int index)
    {
        if (index < 0 || index >= progression.Count) return null;
        List<LevelSO> enemiesList =
            progression[index].AllowedEnemies;
        //Return random enemy of List
        if (enemiesList != null) return enemiesList[Random.Range(0, enemiesList.Count)];
        //default case
        return null;
    }

    public bool IsBoss(int index)
    {
        return index == progression.Count - 1;
    }
}
