using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potato
{
    int plantedSeason;
    public int PlantedSeason { get => plantedSeason; set => plantedSeason = value; }

    public Potato(int plantedSeason)
    {
        this.plantedSeason = plantedSeason;
    }

}
