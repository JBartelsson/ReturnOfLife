using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyDosisOfSports : MonoBehaviour
{
    //public string[] ClubDerZehn = { "Adrian", "Anna", "Chris", "Jan", "Jana", "Johann" };
    public string[] ClubDerZehn = { "Anna", "Jan", "Jana", "Johann" };
    private TMP_Text contestantName;
    // Start is called before the first frame update
    void Start()
    {
        contestantName = this.gameObject.GetComponent<TMP_Text>();
        Random.seed = System.DateTime.Now.Millisecond;
        int contestant = Random.Range(0, ClubDerZehn.Length);
        contestantName.text = ClubDerZehn[contestant];
    }

}
