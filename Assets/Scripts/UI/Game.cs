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
        // nextTurn = GameObject.Find("Next Turn").GetComponent<Button>();
        // nextTurn.onClick.AddListener(AdvanceTurn);
        // clearMission = GameObject.Find("Clear Mission").GetComponent<Button>();
        // clearMission.onClick.AddListener(ClearMission);
        //
    }

    private void ClearMission()
    {
        int tempMissionNumber = GameObject.Find("Mission Number").GetComponent<TMP_Dropdown>().value;

        
    }

}
