using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planter : MonoBehaviour
{
    public Material planterMat;
    public Material blightMat;
    MeshRenderer meshRenderer;

    GameController gCon;
    List<GameObject> inactivePotatoMeshes = new List<GameObject>();
    List<GameObject> activePotatoMeshes = new List<GameObject>();

    List<Planter> neighborPlots = new List<Planter>();

    int plantedSeason;
    int potatoYield = 0;

    public void Init(GameController g)
    {
        gCon = g;
        meshRenderer = GetComponent<MeshRenderer>();
        // Populate Potato Meshes
        Transform t = transform.Find("Potatoes");
        foreach (Transform p in t)
        {
            inactivePotatoMeshes.Add(p.gameObject);
        }
        findNeighbors();
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
        transform.Find("BlightParticle").GetComponent<ParticleSystem>().Stop();
        transform.Find("FlameParticle").GetComponent<ParticleSystem>().Play();
        pS = planterState.empty;
        potatoYield = 0;
        hasBlight = false;
        transform.Find("Seedling").gameObject.SetActive(false);
        transform.Find("PotatoPlant").gameObject.SetActive(false);
        meshRenderer.material = planterMat;
        clearPotatoes();
    }

    public void UpdateSeason(int newSeason)
    {

        // Blight Check
        if (pS != planterState.empty)
        {
            // Blight Spread Check
            float blightRisk = -1f;
            float blightThresh = Random.Range(0f, 1f);
            // + 50% if neighbor has blight.
            int blightedNeighbors = 0;
            foreach (Planter neighbor in neighborPlots)
            {
                if (neighbor.BlightCheck() && neighbor.isPlanted())
                {
                    blightedNeighbors += 1;
                }
            }

            if (blightedNeighbors > 0)
            {
                blightRisk = 0.5f;
            }

            if (blightThresh <= blightRisk)
            {
                blighted();
            }

            // Base blight risk = 0.5% * year + 1
            blightRisk = 0.005f * (gCon.GetYear() + 1);
            blightThresh = Random.Range(0f, 1f);

            if (blightThresh <= blightRisk)
            {
                blighted();
            }
        }

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

    public bool BlightCheck()
    {
        return hasBlight;
    }

    public bool isPlanted()
    {
        if (pS == planterState.empty)
        {
            return false;
        }
        return true;
    }

    void blighted()
    {
        hasBlight = true;
        meshRenderer.material = blightMat;
        transform.Find("BlightParticle").GetComponent<ParticleSystem>().Play();
    }

    void findNeighbors()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + new Vector3(5, 1f, 0), Vector3.down, out hit);
        if (hit.transform.tag == "planter")
        {
            neighborPlots.Add(hit.transform.GetComponent<Planter>());
        }
        Physics.Raycast(transform.position + new Vector3(-5, 1f, 0), Vector3.down, out hit);
        if (hit.transform.tag == "planter")
        {
            neighborPlots.Add(hit.transform.GetComponent<Planter>());
        }
        Physics.Raycast(transform.position + new Vector3(0, 1f, 5), Vector3.down, out hit);
        if (hit.transform.tag == "planter")
        {
            neighborPlots.Add(hit.transform.GetComponent<Planter>());
        }
        Physics.Raycast(transform.position + new Vector3(0, 1f, -5), Vector3.down, out hit);
        if (hit.transform.tag == "planter")
        {
            neighborPlots.Add(hit.transform.GetComponent<Planter>());
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
        else if (potatoIndex > 35)
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
