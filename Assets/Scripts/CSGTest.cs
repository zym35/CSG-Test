using System;
using System.Collections;
using System.Collections.Generic;
using Parabox.CSG;
using UnityEngine;

public class CSGTest : MonoBehaviour
{
    [ContextMenu("Perform")]
    public void Perform(GameObject other)
    {
        var result = CSG.Subtract(gameObject, other);
        CreateNewGameObject(result);
        Destroy(gameObject);
    }

    private void CreateNewGameObject(Model model)
    {
        var result = new GameObject(name);
        result.AddComponent<MeshFilter>().sharedMesh = model.mesh;
        result.AddComponent<MeshRenderer>().sharedMaterials = model.materials.ToArray();
        result.AddComponent<CSGTest>();
    }
}
