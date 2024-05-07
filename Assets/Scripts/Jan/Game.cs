using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int turn = 0;

    IEnumerator Start()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(2.0f);
            AdvanceTurn();
        }
    }

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
}
