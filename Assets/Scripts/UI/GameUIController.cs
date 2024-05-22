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
    [SerializeField] private TextMeshProUGUI fieldsText;
    [SerializeField] private TextMeshProUGUI specialFieldsText;


    private void OnEnable()
    {
        EventManager.Game.Level.OnTurnChanged += UpdateTurn;
        EventManager.Game.Level.OnManaChanged += OnManaChanged;
        EventManager.Game.Level.OnScoreChanged += OnScoreChanged;
    }

    private void OnScoreChanged(EventManager.GameEvents.LevelEvents.ScoreChangedArgs args)
    {
        ecoText.SetText(args.newScore.EcoPoints.ToString());
        fieldsText.SetText(args.newScore.Fields.ToString());
        specialFieldsText.SetText(args.newScore.SpecialFields.ToString());
    }

    private void OnManaChanged(EventManager.GameEvents.LevelEvents.ManaChangedArgs args)
    {
        manaText.SetText(args.newMana.ToString());
    }


    private void OnDisable()
    {
        EventManager.Game.Level.OnTurnChanged -= UpdateTurn;
        EventManager.Game.Level.OnManaChanged -= OnManaChanged;
        EventManager.Game.Level.OnScoreChanged -= OnScoreChanged;


    }

    private void UpdateTurn(EventManager.GameEvents.LevelEvents.TurnChangedArgs args)
    {
        turnsText.SetText("Turn: \n" + args.turnNumber.ToString() + "/3");
    }
}
