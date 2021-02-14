using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Tube : MonoBehaviour
{
    public static Tube Instance;

    Mesh mesh;
    MeshFilter meshFilter;

    [Tooltip("The amount of faces the tube has.")]
    [Range(3, 36)]
    public int tilesPerTube = 12;
    [Tooltip("Diameter of the tube")]
    public float tubeDiameter = 10;
    [Tooltip("How many squares deep the tube is")]
    public int renderDistance = 25;

    [HideInInspector]
    public float speed = 1;

    Vector3[] vertices;
    int[] triangles;
    Vector3[] circle;
    Vector3[] circleCenters;
    float tileSize;

    private void Awake()
    {
        //Singleton Instance
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        //Generate a circle with amount of points equal to tilesPerTube
        circle = new Vector3[tilesPerTube];
        circleCenters = new Vector3[tilesPerTube];
        float angle = (2 * Mathf.PI) / tilesPerTube;
        for (int i = 0; i < tilesPerTube; i++)
        {
            circle[i] = new Vector3(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0) * (tubeDiameter / 2f);
            circleCenters[i] = new Vector3(Mathf.Cos((angle * i) + (0.5f * angle)), Mathf.Sin((angle * i) + (0.5f * angle)), 0) * (tubeDiameter / 2f);
        }
        tileSize = (circle[0] - circle[1]).magnitude;
    }


    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        GenerateInitialMesh(meshFilter.mesh);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMesh();
    }

    void GenerateInitialMesh(Mesh mesh)
    {
        mesh.Clear();
        //Create a new mesh
        mesh.name = "TubeMesh";

        //Instantiate an array of vertices
        //There are as many vertices as tiles at every depth level
        vertices = new Vector3[circle.Length * (renderDistance + 1)];
        //Instantiate an array of triangle indices
        //Each tile has two triangles, which each have three indices
        triangles = new int[circle.Length * renderDistance * 2 * 3];

        //Generate a single ring of vertices to attach the squares to
        Vector3 lastCenter = new Vector3(0, 0, 0);
        Vector3 currentCenter = new Vector3(0, 0, 0);
        for (int i = 0; i < circle.Length; i++)
        {
            vertices[i] = currentCenter + circle[i];
        }

        for (int z = 1; z < renderDistance + 1; z++)
        {
            //Pick a center for this circle
            currentCenter = new Vector3(lastCenter.x, lastCenter.y, z * tileSize);

            for (int i = 0; i < circle.Length; i++)
            {
                vertices[(z*circle.Length) + i] = currentCenter + circle[i];
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

            lastCenter = currentCenter;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void UpdateMesh()
    {
        mesh.Clear();
        List<Vector3> newVertices = new List<Vector3>(vertices);
        //Move all vertices according to speed
        //Remove vertices that are behind the camera
        for(int i = newVertices.Count - 1; i >= 0; i--)
        {
            newVertices[i] -= new Vector3(0, 0, speed * Time.deltaTime);
            if (newVertices[i].z < -1)
            {
                newVertices.RemoveAt(i);
            }
        }

        int circlesAmount = vertices.Length / tilesPerTube;

        while (circlesAmount < renderDistance)
        {
            Vector3 newCircleCenter;
            if (newVertices.Count > 0)
                newCircleCenter = new Vector3(0, 0, newVertices[newVertices.Count - 1].z + tileSize);
            else
                newCircleCenter = Vector3.zero;

            for (int i = 0; i < circle.Length; i++)
            {
                newVertices.Add(newCircleCenter + circle[i]);
            }
            circlesAmount++;
        }

        //Remake the triangles
        vertices = newVertices.ToArray();
        //Two triangles per row of vertices (except the last), 3 vertices per triangle.
        int triangleamount = Mathf.Max((vertices.Length - tilesPerTube) * 6, 0);
        triangles = new int[triangleamount];
        circlesAmount = vertices.Length / tilesPerTube;
        for(int z = 1; z < circlesAmount; z++)
        {
            for (int i = 0; i < circle.Length; i++)
            {
                int triangleIndex = ((z - 1) * circle.Length * 2 * 3) + (i * 2 * 3);
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

        //Give the mesh the updated input
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public void TranslateLocation(Transform translated, float depth, int tile, float height)
    {
        //TODO: Take rotation of tube into account
        Vector3 newPosition = new Vector3(0, 0, depth * tileSize) + circleCenters[tile];
        Vector3 upVector = new Vector3(0, 0, depth * tileSize) - newPosition;
        newPosition += height * upVector.normalized;
        translated.position = newPosition;
        translated.up = upVector;
    }
}
