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
        [SerializeField] private List<EnemiesSO> allowedEnemies;

        public Stage Stage => stage;

        public List<EnemiesSO> AllowedEnemies => allowedEnemies;
    }

    [SerializeField] private List<StageEnemies> progression;

    public List<StageEnemies> Progression => progression;

    public EnemiesSO GetRandomEnemy(Stage stage)
    {
        List<EnemiesSO> enemiesList =
            progression.FirstOrDefault((stageEnemies) => stageEnemies.Stage == stage)?.AllowedEnemies;
        //Return random enemy of List
        if (enemiesList != null) return enemiesList[Random.Range(0, enemiesList.Count)];
        //default case
        return null;
    }
}
