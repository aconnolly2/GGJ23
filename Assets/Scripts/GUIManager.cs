using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{

    public TextMeshProUGUI PotatoCounter;
    public TextMeshProUGUI YearNumText;
    public TextMeshProUGUI CurrentTool;
    public TextMeshProUGUI CashCounter;

    public TextMeshProUGUI WinText;
    public TextMeshProUGUI LoseText;
    public TextMeshProUGUI StartGame;

    public GameObject SeasonCursor;

    public void UpdatePotatoCount(int potatoCount)
    {
        PotatoCounter.text = "Potatoes: " + potatoCount.ToString();
    }

    public void UpdateYear(int year)
    {
        YearNumText.text = year.ToString();
    }

    public void UpdateCursorSprite(int season, float seasonTimer, float seasonTime)
    {
        Image cursorSprite = SeasonCursor.GetComponent<Image>();
        cursorSprite.rectTransform.anchoredPosition = 
            new Vector2(120 + season * 150 - 150 + (seasonTimer / seasonTime) * 150, cursorSprite.rectTransform.anchoredPosition.y);
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
