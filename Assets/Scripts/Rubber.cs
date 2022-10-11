using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubber : MonoBehaviour
{
    public bool active;
    public float distThreshold;
    public Transform lightSource;
    public GameObject cuttingObject;

    private Vector3 _lastPos;

    private void Start()
    {
        var lightAngles = lightSource.eulerAngles;
        cuttingObject.transform.eulerAngles = 
            new Vector3(lightAngles.x - 90, lightAngles.y, 0);

        _lastPos = transform.position;
    }

    private void Update()
    {
        //Ray ray = new Ray(transform.position, transform.up)
        active = Physics.Raycast(transform.position, cuttingObject.transform.up, 100);

        if (!active)
            return;

        if (Vector3.Distance(transform.position, _lastPos) > distThreshold)
        {
            _lastPos = transform.position;
            Perform();
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
        // TODO: generate mesh dynamically based on rubber movement direction and smooth
    }
}
