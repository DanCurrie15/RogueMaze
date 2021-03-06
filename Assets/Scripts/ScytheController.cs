﻿using UnityEngine;

public class ScytheController : MonoBehaviour
{
    public bool activated;

    public float rotationSpeed;

    void Update()
    {

        if (activated)
        {
            transform.localEulerAngles += Vector3.up * rotationSpeed * Time.deltaTime;
        }

    }
}
