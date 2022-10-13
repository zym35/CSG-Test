using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubber : MonoBehaviour
{
    public bool debug;
    public float distThreshold;
    public Transform lightSource;
    public GameObject cuttingObject;
    public GameObject debugCuttingObject;
    public float width;
    public Transform debug0, debug1, debug2, debug3;

    private Vector3 _lastPos;
    private Vector3 _vLastLeft;
    private Vector3 _vLastRight;
    private Vector3 _lightVec;

    private MeshFilter _cuttingMeshFilter;
    private MeshRenderer _cuttingMeshRenderer;

    private const float CUT_LENGTH = 4;

    private void Start()
    {
        _lastPos = transform.position;
        _vLastLeft = Vector3.zero;
        _vLastRight = Vector3.zero;

        _cuttingMeshFilter = cuttingObject.GetComponent<MeshFilter>();
        _cuttingMeshRenderer = cuttingObject.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _lastPos) > distThreshold)
        {
            if (Physics.Raycast(transform.position, lightSource.position - transform.position, CUT_LENGTH))
            {
                CreateCuttingObject();
                Perform();
            }
            
            _lastPos = transform.position;
        }
    }

    private void Perform()
    {
        var objs = FindObjectsOfType<CSGTest>();
        foreach (var csgTest in objs)
        {
            //_cuttingMeshRenderer.sharedMaterial = 
                //csgTest.GetComponent<MeshRenderer>().sharedMaterial;
            
            if (debug)
                csgTest.Perform(debugCuttingObject);
            else 
                csgTest.Perform(cuttingObject);
        }
    }
    
    private void CreateCuttingObject()
    {
        Vector3 pos = transform.position;
        Vector3 forward = pos - _lastPos;
        Vector3 dirLeft = Vector3.Cross(Vector3.up, forward).normalized;

        // calc vertex positions
        Vector3 vLeft = pos + dirLeft * width / 2f;
        Vector3 vRight = pos - dirLeft * width / 2f;
        if (_vLastLeft == Vector3.zero)
        {
            _vLastLeft = _lastPos + dirLeft * width / 2f;
            _vLastRight = _lastPos - dirLeft * width / 2f;
        }

        // create mesh
        List<Vector3> vertexList = new (){
            _vLastLeft, vLeft, vRight, _vLastRight,
            CalculateFarVertex(_vLastLeft), CalculateFarVertex(vLeft),
            CalculateFarVertex(vRight), CalculateFarVertex(_vLastRight)
        };

        List<Vector3> vertices = new ();
        List<int> tris = new ();
        List<Vector3> normals = new ();

        int[] triangleIndexes = {
            0, 3, 4, 7, 4, 3,
            3, 2, 7, 2, 6, 7,
            2, 1, 6, 1, 5, 6,
            1, 0, 5, 0, 4, 5,
            4, 6, 5, 4, 7, 6,
            0, 1, 2, 0, 2, 3
        };
        for (int i = 0; i <= 33; i += 3)
            AddTriangle(triangleIndexes[i], triangleIndexes[i+1], triangleIndexes[i+2]);
        
        var mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(tris, 0);
        mesh.SetNormals(normals);
        
        _cuttingMeshFilter.sharedMesh = mesh;
        
        void AddTriangle(int v0, int v1, int v2)
        {
            vertices.Add(vertexList[v0]);
            tris.Add(vertices.Count - 1);
            vertices.Add(vertexList[v1]);
            tris.Add(vertices.Count - 1);
            vertices.Add(vertexList[v2]);
            tris.Add(vertices.Count - 1);

            var normal = Vector3.Cross(
                vertexList[v1] - vertexList[v0], vertexList[v2] - vertexList[v0]);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
        }
        
        _vLastLeft = vLeft;
        _vLastRight = vRight;
    }

    private Vector3 CalculateFarVertex(Vector3 nearVert)
    {
        var lightVec = (lightSource.position - nearVert).normalized * CUT_LENGTH;
        return nearVert + lightVec;
    }
    
}