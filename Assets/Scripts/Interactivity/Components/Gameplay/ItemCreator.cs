using System;
using System.Collections;
using System.Collections.Generic;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;

public class ItemCreator : MonoBehaviour
{
 
    private float _startingTime;

    private void Awake()
    {
       
        _startingTime = Time.time;
    }


    public void CreateItem(GameObject objToSpawn, float spawnRate)
    {
        if (Time.time - _startingTime >= spawnRate)
        {
            GameObject obj = ObjectManager.DynamicInstantiate(objToSpawn);
            obj.transform.position = transform.position;
            obj.SetActive(true);
            _startingTime = Time.time;
        }
    }

    private void Update()
    {
    }
}