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

    public GameObject Player1;
    public GameObject Player2;
    private bool multiplayer = false;
    private bool arcadeMode = false;

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
        GUIM.UpdatePotatoCount(potatoQ, currentYear, currentSeason, potatoLifespan);
        GUIM.UpdateYear(currentYear);
        GUIM.UpdateCash(cash, cashGoal, arcadeMode);
        GUIM.UpdateCurrentTool("Planter");

        // Initialize player(s)
        Player1.GetComponent<PlayerInput>().Init(this);
        Player2.GetComponent<PlayerInput>().Init(this);



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
        GUIM.UpdatePotatoCount(potatoQ, currentYear, currentSeason, potatoLifespan);
    }

    public void SellPotatoes()
    {
        // 5 potatoes for 20 cash
        if (potatoQ.Count >= 5)
        {
            RemovePotatoes(5);
            cash += 20;
        }

        GUIM.UpdatePotatoCount(potatoQ, currentYear, currentSeason, potatoLifespan);
        GUIM.UpdateCash(cash, cashGoal, arcadeMode);

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
            GUIM.UpdatePotatoCount(potatoQ, currentYear, currentSeason, potatoLifespan);
            return true;
        }
        return false;
    }
    public void RestartGame()
    {
        if (multiplayer)
        {
            cashGoal = 600;
        }
        else
        {
            cashGoal = 300;
        }
        cash = 0;
        currentSeason = 3;
        currentYear = 0;
        potatoQ.Clear();
        AddPotatoes(30);
        GUIM.ClearEndText();
        GUIM.UpdatePotatoCount(potatoQ, currentYear, currentSeason, potatoLifespan);
        GUIM.UpdateCursorSprite(currentSeason, seasonTimer, seasonSpeed);
        GUIM.UpdateYear(currentYear);
        GUIM.UpdateCash(cash, cashGoal, arcadeMode);
        foreach (Planter p in planters)
        {
            p.Reset();
        }
    }
    
    public void StartGame()
    {
        currentGameState = GameState.Playing;
    }

    public void ReturnToMenu()
    {
        currentGameState = GameState.Menu;
        GUIM.ShowMenu();
    }

    void Win()
    {
        if (arcadeMode)
            return;

        currentGameState = GameState.Win;
        GUIM.ShowWin();

        // Check high score
        int bestSeason = PlayerPrefs.GetInt("StorySeason", -1);
        int bestYear = PlayerPrefs.GetInt("StoryYear", -1);

        if (bestSeason == -1)
        {
            PlayerPrefs.SetInt("StorySeason", currentSeason);
            PlayerPrefs.SetInt("StoryYear", currentYear);
        }
        else if (bestYear * 12 + bestSeason > currentYear * 12 + currentSeason)
        {
            PlayerPrefs.SetInt("StorySeason", currentSeason);
            PlayerPrefs.SetInt("StoryYear", currentYear);
        }
    }

    void Lose()
    {
        currentGameState = GameState.Lose;
        GUIM.ShowLose();

        if (arcadeMode)
        {
            // Check high scores
            int bestSeason = PlayerPrefs.GetInt("ArcadeSeason", 0);
            int bestYear = PlayerPrefs.GetInt("ArcadeYear", 0);
            int bestMoney = PlayerPrefs.GetInt("ArcadeMoney", 0);

            if (bestYear * 12 + bestSeason < currentYear * 12 + currentSeason)
            {
                PlayerPrefs.SetInt("ArcadeSeason", currentSeason);
                PlayerPrefs.SetInt("ArcadeYear", currentYear);
            }
            if(bestMoney < cash)
            {
                PlayerPrefs.SetInt("ArcadeMoney", cash);
            }
        }
    }

    public void UpdateHighScores()
    {
        GUIM.UpdateHighScores();
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
        int potatoFeast = 1;
        if (multiplayer)
        {
            potatoFeast = 2;
        }

        if (potatoQ.Count < potatoFeast)
        {
            Lose();
            return;
        }
        RemovePotatoes(potatoFeast); // * playerCount
        GUIM.UpdatePotatoCount(potatoQ, currentYear, currentSeason, potatoLifespan);
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
            p.UpdateSeason(currentSeason, multiplayer);
        }
        eatPotato();

        // Debug.Log(potatoQ.Peek().PlantedSeason.ToString() + " " + ((currentYear * 12 + currentSeason) - potatoLifespan).ToString());
        // Expire Potatoes
        while(potatoQ.Count > 0 && potatoQ.Peek().PlantedSeason <= (currentYear * 12 + currentSeason) - potatoLifespan)
        {
            potatoQ.Dequeue();
        }
        GUIM.UpdatePotatoCount(potatoQ, currentYear, currentSeason, potatoLifespan);
    }

    public void SinglePlayer()
    {
        multiplayer = false;
    }
    public void Multiplayer()
    {
        multiplayer = true;
    }
    public void StoryMode()
    {
        arcadeMode = false;
    }
    public void ArcadeMode()
    {
        arcadeMode = true;
    }
}
