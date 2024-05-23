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
<<<<<<< HEAD
        nextTurn = GameObject.Find("Next Turn").GetComponent<Button>();
        nextTurn.onClick.AddListener(AdvanceTurn);
        clearMission = GameObject.Find("Clear Mission").GetComponent<Button>();
        clearMission.onClick.AddListener(ClearMission);
=======
       
>>>>>>> b20a9e888c1acbddf9638a767d278801821b4c09
    }

    private void ClearMission()
    {
        int tempMissionNumber = GameObject.Find("Mission Number").GetComponent<TMP_Dropdown>().value;
<<<<<<< HEAD
        EventManager.Game.OnMissionCompleted?.Invoke(new EventManager.GameEvents.MissionCompletedArgs()
=======
        EventManager.Game.Level.OnMissionCompleted?.Invoke(new EventManager.GameEvents.LevelEvents.MissionCompletedArgs()
>>>>>>> b20a9e888c1acbddf9638a767d278801821b4c09
        {
            sender = this,
            missionNumber = tempMissionNumber,
            missionText = "Blub"
        });
    }

<<<<<<< HEAD
    public void AdvanceTurn()
    {
        if (turn < 3)
        {
            turn++;
            EventManager.Game.OnTurnChanged?.Invoke(new EventManager.GameEvents.TurnChangedArgs() { 
                sender = this,
                turnNumber = turn
            });
        }
    }
=======
    
>>>>>>> b20a9e888c1acbddf9638a767d278801821b4c09
}
