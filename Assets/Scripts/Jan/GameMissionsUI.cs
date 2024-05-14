using UnityEngine;
using TMPro;
using static EventManager.GameEvents;
using UnityEngine.UI;

public class GameMissionsUI : MonoBehaviour
{
    private TMP_Text mission_header;
    private TMP_Text[] missions;

    private void Awake()
    {
        mission_header = GetComponent<TMP_Text>();
        TMP_Text[] missions_tmp = mission_header.GetComponentsInChildren<TMP_Text>();
        missions = new TMP_Text[missions_tmp.Length - 1];
        for ( int i = 1; i < missions_tmp.Length; i++ )
        {
            missions[i - 1] = missions_tmp[i];
        }
    }

    private void OnEnable()
    {
        EventManager.Game.OnMissionCompleted += UpdateMission;
    }

    private void Game_OnTest(object sender, System.EventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OnDisable()
    {
        EventManager.Game.OnMissionCompleted -= UpdateMission;
    }

    private void UpdateMission(MissionCompletedArgs args)
    {
        missions[args.missionNumber].SetText(args.missionText.ToString());
        missions[args.missionNumber].GetComponentInChildren<Toggle>().isOn = true;
    }
}
