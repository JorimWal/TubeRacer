using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube2 : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;
    LineRenderer lineRenderer;

    List<Torus> toruses = new List<Torus>();

    public float curveIncrement = 12;
    public float pipeIncrement = 12;
    public Vector3 Origin = new Vector3(0, 0, 0);
    public float MajorRadius = 4;
    public float MinorRadius = 1;

    Torus t;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        t = new Torus { MajorRadius = MajorRadius, Origin = Origin, MinorRadius = MinorRadius };
        float curveAngle = 2 * Mathf.PI / curveIncrement;
        float pipeAngle = 2 * Mathf.PI / pipeIncrement;
        for(int i = 0; i<curveIncrement; i++)
            for(int j = 0; j < pipeIncrement; j++)
                Gizmos.DrawSphere(t.PointOnTorus(i*curveAngle, j*pipeAngle), 0.1f);
    }
}

struct Torus
{
    public Vector3 Origin;
    public float MajorRadius, MinorRadius;

    public Vector3 PointOnTorus(float angle1, float angle2)
    {
        float r = (MajorRadius + MinorRadius * Mathf.Cos(angle2));
        Vector3 output = Vector3.zero;
        output.x = r * Mathf.Cos(angle1);
        output.y = r * Mathf.Sin(angle1);
        output.z = MinorRadius * Mathf.Sin(angle2);
        return output + Origin;
    }
}
