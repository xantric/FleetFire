using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, killsText, DeathsText;

    public void Initialize(string name)
    {
        nameText.text = name;
        killsText.text = "0";
        DeathsText.text = "0";
    }

    public void SetKills(int kills)
    {
        killsText.text = kills.ToString();

    }

    public void SetDeaths(int deaths)
    {
        DeathsText.text = deaths.ToString();
    }

    public void SetName(string name)
    {
        nameText.text = name;
        killsText.text = "0";
        DeathsText.text = "0";
    }
}
