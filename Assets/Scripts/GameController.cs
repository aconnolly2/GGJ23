using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    Playing,
    Win,
    Lose
}

public class GameController : MonoBehaviour
{
    GameState currentGameState = GameState.Menu; 
    
    GUIManager GUIM;

    GameObject PlanterParent;
    List<Planter> planters = new List<Planter>();
    private Queue<Potato> potatoQ = new Queue<Potato>();

    private int potatoLifespan = 9;
    private int cash = 0;
    private int cashGoal = 300;

    const int TOTAL_SEASONS = 12;
    // 10 seconds to advance a season.
    private float seasonSpeed = 10f;
    private float seasonTimer = 0;
    private int currentSeason = 3;
    private int currentYear = 0;
    public int GetSeason()
    {
        return currentSeason;
    }
    public int GetYear()
    {
        return currentYear;
    }


    // Start is called before the first frame update
    void Start()
    {
        GUIM = GetComponent<GUIManager>();
        GUIM.UpdatePotatoCount(potatoQ.Count);
        GUIM.UpdateYear(currentYear);
        GUIM.UpdateCash(cash, cashGoal);

        // Initialize player(s)
        PlayerInput PI = GameObject.Find("Player").GetComponent<PlayerInput>();
        PI.Init(this);
           

        // Populate Planter List
        PlanterParent = GameObject.Find("Planters");
        foreach(Transform p in PlanterParent.transform)
        {
            if (p.tag == "planter")
            {
                Planter planter = p.GetComponent<Planter>();
                planter.Init(this);
                planters.Add(planter);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.Playing)
        {
            updateSeason();
        }
    }

    public GameState GetCurrentGameState()
    {
        return currentGameState;
    }

    public void CollectPotatoes(int count)
    {
        AddPotatoes(count);
        GUIM.UpdatePotatoCount(potatoQ.Count);
    }

    public void SellPotatoes()
    {
        // 5 potatoes for 20 cash
        if (potatoQ.Count >= 5)
        {
            RemovePotatoes(5);
            cash += 20;
        }

        GUIM.UpdatePotatoCount(potatoQ.Count);
        GUIM.UpdateCash(cash, cashGoal);

        if (cash >= cashGoal)
        {
            Win();
        }
    }

    public void UpdateTool(string toolName)
    {
        toolName = char.ToUpper(toolName[0]) + toolName.Substring(1);
        GUIM.UpdateCurrentTool(toolName);
    }
    public bool PlantPotato()
    {
        if (potatoQ.Count > 0 && currentSeason >= 3 && currentSeason <= 5)
        {
            RemovePotatoes(1);
            GUIM.UpdatePotatoCount(potatoQ.Count);
            return true;
        }
        return false;
    }
    public void RestartGame()
    {
        cash = 0;
        currentSeason = 3;
        currentYear = 0;
        AddPotatoes(30);
        GUIM.ClearEndText();
        GUIM.UpdatePotatoCount(potatoQ.Count);
        GUIM.UpdateCursorSprite(currentSeason, seasonTimer, seasonSpeed);
        GUIM.UpdateYear(currentYear);
        GUIM.UpdateCash(cash, cashGoal);
        foreach (Planter p in planters)
        {
            p.Reset();
        }
        currentGameState = GameState.Playing;
    }

    void Win()
    {
        currentGameState = GameState.Win;
        GUIM.ShowWin();
    }

    void Lose()
    {
        currentGameState = GameState.Lose;
        GUIM.ShowLose();
    }

    void AddPotatoes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            potatoQ.Enqueue(new Potato((currentYear * 12) + currentSeason));
        }
    }

    void RemovePotatoes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            potatoQ.Dequeue();
        }
    }

    void eatPotato()
    {
        if (potatoQ.Count <= 0)
        {
            Lose();
            return;
        }
        RemovePotatoes(1); // * playerCount
        GUIM.UpdatePotatoCount(potatoQ.Count);
    }

    void updateSeason()
    {
        seasonTimer += Time.deltaTime;
        if (seasonTimer >= seasonSpeed)
        {
            advanceSeason();
            seasonTimer = 0;
        }
        GUIM.UpdateCursorSprite(currentSeason, seasonTimer, seasonSpeed);
    }
    void advanceSeason()
    {
        currentSeason = (currentSeason + 1) % TOTAL_SEASONS;
        if (currentSeason == 0)
        {
            currentSeason = 12;
        }
        else if (currentSeason == 1)
        {
            currentYear += 1;
            GUIM.UpdateYear(currentYear);
        }
        foreach(Planter p in planters)
        {
            p.UpdateSeason(currentSeason);
        }
        eatPotato();

        // Debug.Log(potatoQ.Peek().PlantedSeason.ToString() + " " + ((currentYear * 12 + currentSeason) - potatoLifespan).ToString());
        // Expire Potatoes
        while(potatoQ.Count > 0 && potatoQ.Peek().PlantedSeason <= (currentYear * 12 + currentSeason) - potatoLifespan)
        {
            potatoQ.Dequeue();
        }
        GUIM.UpdatePotatoCount(potatoQ.Count);
    }
}
