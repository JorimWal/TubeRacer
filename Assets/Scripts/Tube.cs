using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Tube : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;

    [Range(3, 36)]
    public int tilesPerTube = 12;
    public float tubeDiameter = 10;
    public int tubeDepth = 12;

    //debug variables
    int lastTilesPerTube = 12;
    float lastTubeDiameter = 10;
    int lastTubeDepth = 12;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        GenerateInitialMesh(meshFilter.mesh);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug code to regenerate the mesh when we change inspector values
        if(lastTubeDiameter != tubeDiameter || lastTilesPerTube != tilesPerTube || lastTubeDepth != tubeDepth)
        {
            GenerateInitialMesh(meshFilter.mesh);
            lastTubeDiameter = tubeDiameter;
            lastTilesPerTube = tilesPerTube;
            lastTubeDepth = tubeDepth;
        }
    }

    void GenerateInitialMesh(Mesh mesh)
    {
        mesh.Clear();
        //Create a new mesh
        mesh.name = "TubeMesh";


        //Generate a circle with amount of points equal to tilesPerTube
        Vector3[] circle = new Vector3[tilesPerTube];
        float angle = (2 * Mathf.PI) / tilesPerTube;
        for(int i = 0; i<tilesPerTube; i++)
        {
            circle[i] = new Vector3(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0) * (tubeDiameter /2f);
        }

        float tileWidth = (circle[0] - circle[1]).magnitude;

        //Instantiate an array of vertices
        //There are as many vertices as tiles at every depth level
        Vector3[] vertices = new Vector3[circle.Length * (tubeDepth + 1)];
        //Instantiate an array of triangle indices
        //Each tile has two triangles, which each have three indices
        int[] triangles = new int[circle.Length * tubeDepth * 2 * 3];

        //Generate a single ring of vertices to attach the squares to
        Vector3 tubeStartCenter = new Vector3(0, 0, 0);
        for (int i = 0; i < circle.Length; i++)
        {
            vertices[i] = tubeStartCenter + circle[i];
        }

        for (int z = 1; z < tubeDepth + 1; z++)
        {
            //Pick a center for this circle
            //TODO: Shift tubecenter to curve the tube
            Vector3 tubeCenter = new Vector3(0, 0, z * tileWidth);

            for (int i = 0; i < circle.Length; i++)
            {
                vertices[(z*circle.Length) + i] = tubeCenter + circle[i];
            }

            for (int i = 0; i < circle.Length; i++)
            {
                int triangleIndex = ((z-1) * circle.Length * 2 * 3) + (i * 2 * 3);
                //Triangle 012
                triangles[triangleIndex + 0] = (z * circle.Length) + i;
                triangles[triangleIndex + 1] = ((z - 1) * circle.Length) + ((i + 1) % circle.Length);
                triangles[triangleIndex + 2] = ((z - 1) * circle.Length) + i;
                //Triangle 213
                triangles[triangleIndex + 3] = (z * circle.Length) + i;
                triangles[triangleIndex + 4] = (z * circle.Length) + ((i + 1) % circle.Length);
                triangles[triangleIndex + 5] = ((z - 1) * circle.Length) + ((i + 1) % circle.Length);
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
