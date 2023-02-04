using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIManager : MonoBehaviour
{

    public TextMeshProUGUI PotatoCounter;
    public TextMeshProUGUI SeasonCounter;
    public TextMeshProUGUI CurrentTool;
    public TextMeshProUGUI CashCounter;

    public TextMeshProUGUI WinText;
    public TextMeshProUGUI LoseText;
    public TextMeshProUGUI StartGame;

    public void UpdatePotatoCount(int potatoCount)
    {
        PotatoCounter.text = "Potatoes: " + potatoCount.ToString();
    }

    public void UpdateSeason(int season, int year)
    {
        SeasonCounter.text = "Year: " + year.ToString() + " Season: " + season.ToString();
    }

    public void UpdateCurrentTool(string toolName)
    {
        CurrentTool.text = "Current Tool: " + toolName;
    }

    public void UpdateCash(int cash, int cashGoal)
    {
        CashCounter.text = "$" + cash.ToString() + " / $" + cashGoal.ToString();
    }

    public void ShowWin()
    {
        WinText.gameObject.SetActive(true);
        StartGame.gameObject.SetActive(true);
    }

    public void ShowLose()
    {
        LoseText.gameObject.SetActive(true);
        StartGame.gameObject.SetActive(true);
    }

    public void ClearEndText()
    {
        WinText.gameObject.SetActive(false);
        LoseText.gameObject.SetActive(false);
        StartGame.gameObject.SetActive(false);
    }
}
