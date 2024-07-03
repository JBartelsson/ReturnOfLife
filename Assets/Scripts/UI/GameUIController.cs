using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnsText;
    [SerializeField] private TextMeshProUGUI manaText;
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI ecoText;


    private void OnEnable()
    {
        EventManager.Game.Level.OnTurnChanged += UpdateTurn;
        EventManager.Game.Level.OnManaChanged += OnManaChanged;
        EventManager.Game.Level.OnScoreChanged += OnScoreChanged;
    }

    private void OnScoreChanged(EventManager.GameEvents.LevelEvents.ScoreChangedArgs args)
    {
        ecoText.SetText($"{args.NewScore.EcoPoints.ToString()}/{args.CurrentLevel.NeededEcoPoints}");
    }

    private void OnManaChanged(EventManager.GameEvents.LevelEvents.ManaChangedArgs args)
    {
        manaText.SetText(args.NewMana.ToString());
    }



    private void UpdateTurn(EventManager.GameEvents.LevelEvents.TurnChangedArgs args)
    {
        turnsText.SetText("Turn: \n" + args.TurnNumber.ToString() + "/3");
    }
}
