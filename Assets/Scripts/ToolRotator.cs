using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolRotator : MonoBehaviour
{
    float rotationSpeed = 30f;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, Time.time * rotationSpeed % 360, 0));
    }
}
