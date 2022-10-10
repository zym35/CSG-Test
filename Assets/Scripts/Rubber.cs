using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubber : MonoBehaviour
{
    public bool active;
    public float step;
    public Transform lightSource;
    public GameObject cuttingObject;

    private float _stepTimer;

    private void Start()
    {
        var lightAngles = lightSource.eulerAngles;
        cuttingObject.transform.eulerAngles = 
            new Vector3(lightAngles.x - 90, lightAngles.y, 0);
    }

    private void Update()
    {
        if (!active)
            return;
        
        _stepTimer += Time.deltaTime;
        if (_stepTimer >= step)
        {
            Perform();
            _stepTimer = 0;
        }
    }

    private void Perform()
    {
        var objs = FindObjectsOfType<CSGTest>();
        foreach (var csgTest in objs)
        {
            cuttingObject.GetComponent<MeshRenderer>().sharedMaterial =
                csgTest.GetComponent<MeshRenderer>().sharedMaterial;
            csgTest.Perform(cuttingObject);
        }
    }

    private void CreateCuttingObject()
    {
        
    }
}
