using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Game : MonoBehaviour
{
    public int turn = 0;

    public Button nextTurn;

    public Button clearMission;

    private void Awake()
    {
       
    }

    private void ClearMission()
    {
        int tempMissionNumber = GameObject.Find("Mission Number").GetComponent<TMP_Dropdown>().value;
        EventManager.Game.Level.OnMissionCompleted?.Invoke(new EventManager.GameEvents.LevelEvents.MissionCompletedArgs()
        {
            sender = this,
            missionNumber = tempMissionNumber,
            missionText = "Blub"
        });
    }

    
}
