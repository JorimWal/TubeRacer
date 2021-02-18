using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeSegment : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;

    public float ringDistance = 1;
    int tubeIncrement = 12;
    float minorRadius = 1;
    public Torus Torus { get; private set; }

    public Transform pivot;
    public Transform connector;

    public int Segments { get; private set; } = 6;

    public int CurveSegments { get; private set; } = 12;

    public float MajorRadius { get; private set; } = 4;

    public float RadiansCovered { get; private set; }

    public float Rotation { get; private set; }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTorus(int Segments, int CurveIncrement, int TubeIncrement, float MajorRadius, float MinorRadius, float Rotation)
    {
        this.Segments = Segments + 1;
        CurveSegments = CurveIncrement;
        tubeIncrement = TubeIncrement;
        this.MajorRadius = MajorRadius;
        minorRadius = MinorRadius;
        this.Rotation = Rotation;

        GenerateMesh();

        //Set this tubesegment as a child to it's pivot, so it can be turned from a pivot not centered on the object
        pivot = transform.parent;
        //Allign this tube segment so the pivot is in the center of the opening of the tube
        transform.localPosition -= new Vector3(MajorRadius, 0, 0);
        connector = new GameObject("Connector").transform;
        connector.SetParent(transform);
        float curveAngle = 2f * Mathf.PI / (float)CurveSegments;
        //Set the connector object at the center of the end pipe
        connector.localPosition = Torus.PointOnCurve(curveAngle * Segments);
        //Turn the connector to face out the pipe
        connector.localRotation = Quaternion.Euler(0, 0, (Segments / (float)CurveSegments) * 360f);
        RadiansCovered = Segments * curveAngle;
    }

    void GenerateMesh()
    {
        //Create a new mesh
        Mesh mesh = new Mesh();
        mesh.name = "TubeMesh";
        //Pass the mesh to this object's meshFilter
        meshFilter.mesh = mesh;

        List<Vector3> vertices = new List<Vector3>();
        Torus = new Torus { MajorRadius = MajorRadius, MinorRadius = minorRadius };
        float curveAngle = 2f * Mathf.PI / (float)CurveSegments;
        float pipeAngle = 2f * Mathf.PI / (float)tubeIncrement;
        for (int i = 0; i < Segments; i++)
            for (int j = 0; j < tubeIncrement; j++)
                vertices.Add(Torus.PointOnTorus(i * curveAngle, j * pipeAngle));

        mesh.vertices = vertices.ToArray();
        List<int> triangles = new List<int>();

        for (int i = 1; i < Segments; i++)
        {
            for(int j = 0; j < tubeIncrement; j++)
            {
                int row = i * tubeIncrement;
                int lastRow = (i - 1) * tubeIncrement;
                int column = j;
                int nextColumn = (j + 1) % tubeIncrement;
                //Make the triangles in reverse order to invert the normals
                //Make a triangle with the vertex one row behind this one and the vertex one row behind and one column over
                triangles.Add(lastRow + column);
                triangles.Add(lastRow + nextColumn);
                triangles.Add(row + column);
                //Make a triangle with the vertex one column over and the vertex one row behind and one column over
                triangles.Add(lastRow + nextColumn);
                triangles.Add(row + nextColumn);
                triangles.Add(row + column);
            }
        }

        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }


}

public struct Torus
{
    public float MajorRadius, MinorRadius;

    public float Diameter {
        get { return 2 * (MajorRadius - MinorRadius); }
    }

    public Vector3 PointOnTorus(float angle1, float angle2)
    {
        float r = (MajorRadius + MinorRadius * Mathf.Cos(angle2));
        Vector3 output = Vector3.zero;
        output.x = r * Mathf.Cos(angle1);
        output.y = r * Mathf.Sin(angle1);
        output.z = MinorRadius * Mathf.Sin(angle2);
        return output;
    }

    public Vector3 PointOnSegmentedTorus(float angle1, float angle2, float segments1, float segments2)
    {
        Vector3 output = Vector3.zero;

        //Calculate the radians per Curve Segment
        float radiansPerOuterSegment = (2 * Mathf.PI) / segments1;
        //Calculate the radians per InnerCircle Segment
        float radiansPerInnerSegment = (2 * Mathf.PI) / segments2;

        //Find the closest segmented angle on either side of the precise angle
        float outerSegments = angle1 / radiansPerOuterSegment;
        float lastOuterSegmentRadius = Mathf.Floor(outerSegments) * radiansPerOuterSegment;
        float nextOuterSegmentRadius = Mathf.Ceil(outerSegments) * radiansPerOuterSegment;
        float outerDifference = outerSegments % 1;

        //Find the closest segmented angle on either side of the precise inner angle
        float innerSegments = angle2 / radiansPerInnerSegment;
        float lastInnerSegmentRadius = Mathf.Floor(innerSegments) * radiansPerInnerSegment;
        float nextInnerSegmentRadius = Mathf.Ceil(innerSegments) * radiansPerInnerSegment;
        float innerDifference = innerSegments % 1;

        Vector3 lastSegmentedPoint = PointOnTorus(lastOuterSegmentRadius, angle2);
        Vector3 nextSegmentedPoint = PointOnTorus(nextOuterSegmentRadius, angle2);

        output.x = Mathf.Lerp(lastSegmentedPoint.x, nextSegmentedPoint.x, outerDifference);
        output.y = Mathf.Lerp(lastSegmentedPoint.y, nextSegmentedPoint.y, outerDifference);
        output.z = Mathf.Lerp(lastSegmentedPoint.z, nextSegmentedPoint.z, innerDifference);
        return output;
    }

    public Vector3 PointOnCurve(float angle1)
    {
        float r = MajorRadius;
        Vector3 output = Vector3.zero;
        output.x = r * Mathf.Cos(angle1);
        output.y = r * Mathf.Sin(angle1);
        output.z = 0;
        return output;
    }
}
