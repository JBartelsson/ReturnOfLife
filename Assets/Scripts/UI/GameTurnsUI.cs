using UnityEngine;
using TMPro;
using static EventManager.GameEvents;

public class GameTurnsUI : MonoBehaviour
{
    private TMP_Text label;

    private void Awake()
    {
        label = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        EventManager.Game.Level.OnTurnChanged += UpdateTurn;
    }
    

    private void OnDisable()
    {
        EventManager.Game.Level.OnTurnChanged -= UpdateTurn;
    }

    private void UpdateTurn(LevelEvents.TurnChangedArgs args)
    {
        label.SetText("Turn: \n" + args.TurnNumber.ToString() + "/3");
    }
}