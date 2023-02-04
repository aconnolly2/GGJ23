using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    GameController gCon;
    enum tools
    {
        planter,
        harvester,
        flamethrower,
        none
    };

    tools currentTool = tools.planter;
    private float speed = 5f;
    private Vector3 direction = new Vector3();
    private CharacterController CC;

    public void Init(GameController g)
    {
        gCon = g;
        gCon.UpdateTool(currentTool.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        CC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = Input.GetAxis("Horizontal");
        direction.z = Input.GetAxis("Vertical");
        Vector3.Normalize(direction);
        direction.y = -9f;
        CC.Move(direction * speed * Time.deltaTime);
        if (Input.GetButtonDown("Jump"))
        {
            Interact();
        }    
    }

    void switchTool(string toolName)
    {
        switch(toolName)
        {
            case "planter":
                currentTool = tools.planter;
                break;
            case "harvester":
                currentTool = tools.harvester;
                break;
            case "flamethrower":
                currentTool = tools.flamethrower;
                break;
            default:
                currentTool = tools.none;
                break;
        }
        gCon.UpdateTool(currentTool.ToString());
    }

    void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.transform.tag == "planter")
            {
                if (currentTool == tools.planter)
                {
                    hit.transform.GetComponent<Planter>().PlantPotato();
                }
                if (currentTool == tools.harvester)
                {
                    hit.transform.GetComponent<Planter>().HarvestPotatoes();
                }
                if (currentTool == tools.flamethrower)
                {
                    hit.transform.GetComponent<Planter>().BurnField();
                }
            }
            else if (hit.transform.tag == "tool")
            {
                switchTool(hit.transform.name);
            }
            else if (hit.transform.tag == "vendor")
            {
                gCon.SellPotatoes();
            }
        }
    }
}
