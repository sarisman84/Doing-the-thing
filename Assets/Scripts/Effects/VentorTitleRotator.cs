using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentorTitleRotator : MonoBehaviour
{
    public float rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed);
    }
}