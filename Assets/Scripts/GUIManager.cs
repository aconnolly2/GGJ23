using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIManager : MonoBehaviour
{

    public TextMeshProUGUI PotatoCounter;
    public TextMeshProUGUI SeasonCounter;

    public void UpdatePotatoCount(int potatoCount)
    {
        PotatoCounter.text = "Potatoes: " + potatoCount.ToString();
    }

    public void UpdateSeason(int season, int year)
    {
        SeasonCounter.text = "Year: " + year.ToString() + " Season: " + season.ToString();
    }
}
