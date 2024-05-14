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
        EventManager.Game.OnTurnChanged += UpdateTurn;
    }

    private void Game_OnTest(object sender, System.EventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OnDisable()
    {
        EventManager.Game.OnTurnChanged -= UpdateTurn;
    }

    private void UpdateTurn(TurnChangedArgs args)
    {
        label.SetText("Turn: \n" + args.turnNumber.ToString() + "/3");
    }
}