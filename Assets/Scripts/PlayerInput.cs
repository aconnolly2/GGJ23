using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private float speed = 5f;
    private Vector3 direction = new Vector3();
    private CharacterController CC;
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
        if (Input.GetButton("Jump"))
        {
            Interact();
        }    
    }

    void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Debug.Log(hit.transform.tag);
            if (hit.transform.tag == "planter")
            {
                hit.transform.GetComponent<Planter>().PlantPotato();
            }
        }
    }
}
