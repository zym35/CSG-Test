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
    public float width;
    public Transform debug0, debug1, debug2, debug3;

    private Vector3 _lastPos;
    private Vector3 _vLastLeft;
    private Vector3 _vLastRight;

    private void Start()
    {
        var lightAngles = lightSource.eulerAngles;
        cuttingObject.transform.eulerAngles =
            new Vector3(lightAngles.x - 90, lightAngles.y, 0);

        _lastPos = transform.position;
        _vLastLeft = Vector3.zero;
        _vLastRight = Vector3.zero;
    }

    private void Update()
    {
        //Ray ray = new Ray(transform.position, transform.up)
        active = Physics.Raycast(transform.position, cuttingObject.transform.up, 100);

        //if (!active) return;

        if (Vector3.Distance(transform.position, _lastPos) > distThreshold)
        {
            //Perform();
            CreateCuttingObject();
            _lastPos = transform.position;
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
        Vector3 pos = transform.position;
        Vector3 forward = pos - _lastPos;
        Vector3 dirLeft = Vector3.Cross(Vector3.up, forward).normalized;
        Vector3 lightAngles = lightSource.eulerAngles;
        Vector3 lightDir = new Vector3(lightAngles.x - 90, lightAngles.y, 0).normalized;

        Vector3 vLeft = pos + dirLeft * width / 2f;
        Vector3 vRight = pos - dirLeft * width / 2f;
        if (_vLastLeft == Vector3.zero)
        {
            _vLastLeft = _lastPos + dirLeft * width / 2f;
            _vLastRight = _lastPos - dirLeft * width / 2f;
        }

        debug0.position = _vLastLeft + lightDir * 2;
        debug1.position = _vLastRight + lightDir * 2;
        debug2.position = vLeft + lightDir * 2;
        debug3.position = vRight + lightDir * 2;

        Vector3[] vertices =
        {
            _vLastLeft, vLeft, vRight, _vLastRight
        };

        _vLastLeft = vLeft;
        _vLastRight = vRight;
    }
}