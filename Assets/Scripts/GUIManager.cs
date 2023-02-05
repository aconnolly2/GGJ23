using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{

    public TextMeshProUGUI PotatoCounter;
    public TextMeshProUGUI YearNumText;
    public TextMeshProUGUI CashCounter;

    public TextMeshProUGUI CurrentTool;
    public GameObject PlanterImg;
    public GameObject HarvesterImg;
    public GameObject FlamethrowerImg;

    public TextMeshProUGUI WinText;
    public TextMeshProUGUI LoseText;
    public TextMeshProUGUI StartGame;

    public GameObject SeasonCursor;

    public GameObject PotatoSprite;
    public GameObject PotatoSack;

    //public void UpdatePotatoCount(int potatoCount)
    //{
    //    PotatoCounter.text = "Potatoes: " + potatoCount.ToString();
    //}
    public void UpdatePotatoCount(Queue<Potato> potatoQ, int year, int season, int lifespan)
    {
        int potatoCount = potatoQ.Count;
        PotatoCounter.text = "Potatoes: " + potatoQ.Count.ToString();
        // Pooling would be better here, but this is what we have for now.
        foreach(Transform t in PotatoSack.transform)
        {
            Destroy(t.gameObject);
        }
        Queue<Potato> copyPotatoes = new Queue<Potato>(potatoQ);
        while (copyPotatoes.Count > 0)
        {
            Potato p = copyPotatoes.Dequeue();
            GameObject potato = Instantiate(PotatoSprite, Vector3.zero, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))), PotatoSack.transform);
            if (p.PlantedSeason <= (year * 12 + season) - lifespan + 1)
            {
                potato.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);                
            }
            else if (p.PlantedSeason <= (year * 12 + season) - lifespan + 3)
            {
                potato.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f);
            }

        }
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
        PlanterImg.SetActive(false);
        HarvesterImg.SetActive(false);
        FlamethrowerImg.SetActive(false);
        CurrentTool.text = toolName;

        switch (toolName)
        {
            case "Planter":
                PlanterImg.SetActive(true);
                break;
            case "Harvester":
                HarvesterImg.SetActive(true);
                break;
            case "Flamethrower":
                FlamethrowerImg.SetActive(true);
                break;
        }

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
