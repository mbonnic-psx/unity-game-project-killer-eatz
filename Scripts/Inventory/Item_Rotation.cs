using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Rotation : MonoBehaviour
{
    //public float bobSpeed = 2f;
    //public float bobHeight = 0.2f;
    public Vector3 rotationSpeed = new Vector3(0, 50, 0);
    //private Vector3 startPosition;
    private bool shouldRotate = false;

    void Start()
    {
        shouldRotate = true;
        //startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldRotate)
        {

            // float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            // transform.position = new Vector3(startPosition.x, newY, startPosition.z);

            transform.Rotate(rotationSpeed * Time.deltaTime);
        }

    }

    public void ActivateRotation()
    {
        shouldRotate = true;
    }
}
