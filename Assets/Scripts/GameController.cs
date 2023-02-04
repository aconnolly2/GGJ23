using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    GUIManager GUIM;

    GameObject PlanterParent;
    List<Planter> planters = new List<Planter>();

    private int potatoCount = 30;

    private int currentSeason = 3;
    private int currentYear = 0;

    private int totalSeasons = 12;

    // 15 seconds to advance a season.
    private float seasonSpeed = 10f;

    private float seasonTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        GUIM = GetComponent<GUIManager>();
        GUIM.UpdatePotatoCount(potatoCount);
        GUIM.UpdateSeason(currentSeason, currentYear);

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
        UpdateSeason();
    }

    void UpdateSeason()
    {
        seasonTime += Time.deltaTime;
        if (seasonTime >= seasonSpeed)
        {
            advanceSeason();
            eatPotato();
            seasonTime = 0;
        }

    }
    public bool PlantPotato()
    {
        if (potatoCount >= 0 && currentSeason >= 3 && currentSeason <= 5)
        {
            potatoCount -= 1;
            GUIM.UpdatePotatoCount(potatoCount);
            return true;
        }
        return false;
    }

    public void CollectPotatoes(int count)
    {
        potatoCount += count;
        GUIM.UpdatePotatoCount(potatoCount);
    }

    public void UpdateTool(string toolName)
    {
        toolName = char.ToUpper(toolName[0]) + toolName.Substring(1);
        GUIM.UpdateCurrentTool(toolName);
    }

    public int GetSeason()
    {
        return currentSeason;
    }

    void eatPotato()
    {
        if (potatoCount <= 0)
        {
            // Game Over!
        }
        potatoCount -= 1; // * playerCount
        GUIM.UpdatePotatoCount(potatoCount);
    }

    void advanceSeason()
    {
        currentSeason = (currentSeason + 1) % totalSeasons;
        if (currentSeason == 0)
        {
            currentYear += 1;
        }
        GUIM.UpdateSeason(currentSeason, currentYear);
        foreach(Planter p in planters)
        {
            p.UpdateSeason(currentSeason);
        }
    }
}
