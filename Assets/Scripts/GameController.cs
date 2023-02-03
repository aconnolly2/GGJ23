using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    GUIManager GUIM;

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

    void eatPotato()
    {
        potatoCount -= 1; // * playerCount
        GUIM.UpdatePotatoCount(potatoCount);
    }

    void advanceSeason()
    {
        currentSeason = currentSeason + 1 % totalSeasons;
        if (currentSeason == 0)
        {
            currentYear += 1;
        }
        GUIM.UpdateSeason(currentSeason, currentYear);
    }
}
