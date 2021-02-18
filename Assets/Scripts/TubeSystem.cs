using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeSystem : MonoBehaviour
{
    //Singleton instance
    public static TubeSystem Instance;

    [HideInInspector]
    public List<TubeSegment> tubes = new List<TubeSegment>();

    [Tooltip("The diameter of the inner tubes created by the system")]
    public float Diameter = 15;
    [Tooltip("The amount of faces of the outer circle")]
    public int CurveSegments = 50;
    [Tooltip("The amount of faces of the inner tubes")]
    public int TubeSegments = 16;
    [Tooltip("The amount of segments the system keeps loaded simultaneously")]
    public int SegmentsCapacity = 5;

    GameObject tubeSegmentPrefab;
    GameObject obstaclePrefab;

    private void Awake()
    {
        //Singleton Instance
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Load the segment prefab from resources
        tubeSegmentPrefab = Resources.Load<GameObject>("Prefabs/TubeSegment");
        //Load the obstacle prefab from resources
        obstaclePrefab = Resources.Load<GameObject>("Prefabs/Obstacle");

        for(int i = 0; i < SegmentsCapacity; i++)
        {
            AddSegment();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveSegment()
    {
        //Get the current first tube
        TubeSegment tube = tubes[0];
        //Remove the first tube from the system's list
        tubes.RemoveAt(0);
        //Get the new first tube
        Transform nextTube = tube.connector.GetChild(0);
        //Destroy the last pipe
        Destroy(tube.transform.parent.gameObject);
        //Set the nextTube as a child of the system
        nextTube.SetParent(transform);
        nextTube.localRotation = Quaternion.identity;
        //Reset the system to position 0, so the player stays near the origin
        nextTube.transform.localPosition = Vector3.zero;
        AddSegment();
    }

    public void AddSegment()
    {
        GameObject pivot = new GameObject("Pivot");
        GameObject instance = GameObject.Instantiate(tubeSegmentPrefab);
        TubeSegment tubeScript = instance.GetComponent<TubeSegment>();

        instance.transform.SetParent(pivot.transform, false);
        pivot.transform.SetParent(transform, false);
        float randomRotation = Random.Range(0, TubeSegments) * (360 / (float)TubeSegments);
        //float randomRotation = 0;
        pivot.transform.localRotation = Quaternion.Euler(0, randomRotation, 0);

        //Get a random range for the Major radius
        //The smaller the radius, the sharper the corners
        float radius = Random.Range(25, 45);
        //Get a random range for the amount of segments in the torus
        //Fewer segments lead to more frequent direction changes in the turning of the tube
        int segments = Random.Range(4, 9);
        tubeScript.SetTorus(segments, CurveSegments, TubeSegments, radius + (Diameter / 2), (Diameter / 2), randomRotation);
        tubes.Add(tubeScript);

        if (tubes.Count > 1)
        {
            AllignSegments(tubes[tubes.Count - 2], tubes[tubes.Count - 1]);
        }

        int ObstacleCount = Random.Range(0, 8);
        for(int i = 0; i < ObstacleCount; i++)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            float curveAngle = Random.Range(0, tubeScript.RadiansCovered);
            GameObject obstacle = Instantiate(obstaclePrefab);
            obstacle.transform.SetParent(tubeScript.transform);
            Vector3 newPosition = tubeScript.transform.TransformPoint(tubeScript.Torus.PointOnTorus(curveAngle, angle));
            Vector3 newCenter = tubeScript.transform.TransformPoint(tubeScript.Torus.PointOnCurve(curveAngle));
            obstacle.transform.position = newPosition;
            obstacle.transform.up = (newCenter - newPosition).normalized;
        }
    }

    void AllignSegments(TubeSegment OriginTube, TubeSegment AlligningTube)
    {
        AlligningTube.pivot.SetParent(OriginTube.connector, false);
        //AlligningTube.transform.SetParent(transform);
    }
}
