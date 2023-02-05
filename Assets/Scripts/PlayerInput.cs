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

    int playerIndex;

    tools currentTool = tools.planter;

    GameObject planterMesh;
    GameObject harvesterMesh;
    GameObject flamethrowerMesh;

    private float speed = 5f;
    private float rotSpeed = 15f;
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
        planterMesh = transform.Find("Spade").gameObject;
        harvesterMesh = transform.Find("Hoe").gameObject;
        flamethrowerMesh = transform.Find("Flamethrower").gameObject;
        switchTool("planter");

        int.TryParse(gameObject.name[gameObject.name.Length - 1].ToString(), out playerIndex);
        Debug.Log(playerIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (gCon.GetCurrentGameState() == GameState.Playing)
        {
            if (playerIndex == 1)
            {
                direction.x = Input.GetAxis("HorizontalP1");
                direction.z = Input.GetAxis("VerticalP1");
                Vector3.Normalize(direction);
                if (Input.GetAxisRaw("HorizontalP1") != 0 || Input.GetAxisRaw("VerticalP1") != 0)
                {
                    Quaternion targetRot = Quaternion.Euler(new Vector3(0, Mathf.Atan2(direction.z, -direction.x) * Mathf.Rad2Deg - 90, 0));
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
                }
                if (Input.GetButtonDown("Fire1"))
                {
                    Interact();
                }
            }
            else if (playerIndex == 2)
            {
                direction.x = Input.GetAxis("HorizontalP2");
                direction.z = Input.GetAxis("VerticalP2");
                Vector3.Normalize(direction);
                if (Input.GetAxisRaw("HorizontalP2") != 0 || Input.GetAxisRaw("VerticalP2") != 0)
                {
                    Quaternion targetRot = Quaternion.Euler(new Vector3(0, Mathf.Atan2(direction.z, -direction.x) * Mathf.Rad2Deg - 90, 0));
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
                }
                if (Input.GetButtonDown("Fire2"))
                {
                    Interact();
                }
            }

            direction.y = -9f;
            CC.Move(direction * speed * Time.deltaTime);
        }
        if (Input.GetButtonDown("Jump"))
        {
            MenuInteract();
        }
    }

    void switchTool(string toolName)
    {
        hideMeshes();
        switch(toolName)
        {
            case "planter":
                planterMesh.SetActive(true);
                currentTool = tools.planter;
                break;
            case "harvester":
                harvesterMesh.SetActive(true);
                currentTool = tools.harvester;
                break;
            case "flamethrower":
                flamethrowerMesh.SetActive(true);
                currentTool = tools.flamethrower;
                break;
            default:
                currentTool = tools.none;
                break;
        }
        gCon.UpdateTool(currentTool.ToString());
    }

    void hideMeshes()
    {
        planterMesh.SetActive(false);
        harvesterMesh.SetActive(false);
        flamethrowerMesh.SetActive(false);
    }

    void MenuInteract()
    {
        if (gCon.GetCurrentGameState() == GameState.Menu)
        {
            gCon.RestartGame();
            gCon.StartGame();
        }
        else if (gCon.GetCurrentGameState() != GameState.Playing)
        {
            gCon.ReturnToMenu();
        }
    }

    void Interact()
    {
        if (gCon.GetCurrentGameState() == GameState.Playing)
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
}
