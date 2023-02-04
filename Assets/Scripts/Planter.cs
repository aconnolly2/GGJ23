using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planter : MonoBehaviour
{
    GameController gCon;
    List<GameObject> inactivePotatoMeshes = new List<GameObject>();
    List<GameObject> activePotatoMeshes = new List<GameObject>();

    int plantedSeason;
    int potatoYield = 0;

    public void Init(GameController g)
    {
        gCon = g;
        // Populate Potato Meshes
        Transform t = transform.Find("Potatoes");
        foreach (Transform p in t)
        {
            inactivePotatoMeshes.Add(p.gameObject);
        }
    }

    enum planterState
    {
        empty,
        seedling,
        producing
    };

    bool hasBlight = false;
    planterState pS = planterState.empty;

    public void PlantPotato()
    {
        if (pS == planterState.empty)
        {
            if (gCon.PlantPotato())
            {
                pS = planterState.seedling;
                plantedSeason = gCon.GetSeason();
                transform.Find("Seedling").gameObject.SetActive(true);
            }
        }
    }

    public void HarvestPotatoes()
    {
        pS = planterState.empty;
        gCon.CollectPotatoes(potatoYield);
        potatoYield = 0;
        transform.Find("Seedling").gameObject.SetActive(false);
        transform.Find("PotatoPlant").gameObject.SetActive(false);
        clearPotatoes();
    }

    public void BurnField()
    {
        // Play Fire Particle Effect
        pS = planterState.empty;
        potatoYield = 0;
        hasBlight = false;
        transform.Find("Seedling").gameObject.SetActive(false);
        transform.Find("PotatoPlant").gameObject.SetActive(false);
        clearPotatoes();
    }

    public void UpdateSeason(int newSeason)
    {
        // Blight Check

        // Growth
        if (pS == planterState.seedling)
        {
            if (newSeason >= 6 && (newSeason - plantedSeason) > 2 && !hasBlight)
            {
                growPlant();
            }
        }
        else if (pS == planterState.producing)
        {
            if (newSeason >= 6 && newSeason <= 10 && !hasBlight)
            {
                growPotatoes();
            }
        }
    }    

    void growPlant()
    {
        pS = planterState.producing;
        transform.Find("Seedling").gameObject.SetActive(false);
        transform.Find("PotatoPlant").gameObject.SetActive(true);
    }

    void growPotatoes()
    {
        // 50% chance to grow 1 potato/season
        // 10% chance to grow 2 potatoes/season
        // Max 8 potatoes/season possible. Very unlikely.
        int potatoIndex = Random.Range(1, 101);
        if (potatoIndex > 90)
        {
            potatoYield += 2;
            activatePotato();
            activatePotato();
        }
        else if (potatoIndex > 50)
        {
            potatoYield += 1;
            activatePotato();
        }
    }

    void activatePotato()
    {
        int randomPotato = Random.Range(0, inactivePotatoMeshes.Count);

        GameObject potato = inactivePotatoMeshes[randomPotato];
        potato.SetActive(true);
        activePotatoMeshes.Add(potato);
        inactivePotatoMeshes.RemoveAt(randomPotato);
    }

    void clearPotatoes()
    {
        foreach(GameObject p in activePotatoMeshes)
        {
            inactivePotatoMeshes.Add(p);
            p.SetActive(false);
        }
        activePotatoMeshes.Clear();
    }
}
