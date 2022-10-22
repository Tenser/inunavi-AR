using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCreator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh arrow = new Mesh();
        arrow.vertices = new Vector3[] { new Vector3(0, 1, 1), new Vector3(0, 2, 0), new Vector3(0, 1.5f, 0), new Vector3(0, 1.5f, -1), new Vector3(0, 0.5f, -1), new Vector3(0, 0.5f, 0), new Vector3(0, 0, 0) };
        arrow.triangles = new int[] { 0, 1, 6, 2, 3, 5, 3, 4, 5 };
        GetComponent<MeshFilter>().mesh = arrow;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
