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
    [Tooltip("The minimum radius of the tube.")]
    public int MinimumMajorRadius = 25;
    [Tooltip("The maximum radius of the tube")]
    public int MaximumMajorRadius = 45;

    GameObject tubeSegmentPrefab, obstaclePrefab, scorePickupPrefab, heartPickupPrefab;

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
        //Load the prefabs from resources
        tubeSegmentPrefab = Resources.Load<GameObject>("Prefabs/TubeSegment");
        obstaclePrefab = Resources.Load<GameObject>("Prefabs/Obstacle");
        scorePickupPrefab = Resources.Load<GameObject>("Prefabs/ScorePickup");
        heartPickupPrefab = Resources.Load<GameObject>("Prefabs/HeartPickup");

        for(int i = 0; i < SegmentsCapacity; i++)
        {
            AddSegment();
        }

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
        float radius = Random.Range(MinimumMajorRadius, MaximumMajorRadius);
        //Get a random range for the amount of segments in the torus
        //Fewer segments lead to more frequent direction changes in the turning of the tube
        int segments = Random.Range(4, 9);
        //Pass the tube variables along so the created segment can create a mesh
        tubeScript.SetTorus(segments, CurveSegments, TubeSegments, radius + (Diameter / 2), (Diameter / 2), randomRotation);
        tubes.Add(tubeScript);

        //Parent the newly created tube to the existing tube system
        if (tubes.Count > 1)
        {
            AllignSegments(tubes[tubes.Count - 2], tubes[tubes.Count - 1]);
        }

        //Spawn random obstacles in the tube
        int ObstacleCount = Random.Range(0, 8);
        for(int i = 0; i < ObstacleCount; i++)
            SpawnObject(tubeScript, obstaclePrefab);

        //Spawn hearts when the player is hurt
        int heartChance = Random.Range(0, 10);
        if(heartChance == 0 && PlayerController.Instance.Lives < 3)
            SpawnObject(tubeScript, heartPickupPrefab);

        //Spawn score pickup if the score multiplier is not yet maxed out
        int scorePickupChance = Random.Range(0, 10);
        if (scorePickupChance == 0 && PlayerController.Instance.ScoreMultiplier < 8)
            SpawnObject(tubeScript, scorePickupPrefab);
    }

    void SpawnObject(TubeSegment tubeScript, GameObject prefab)
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        float curveAngle = Random.Range(0, tubeScript.RadiansCovered);
        GameObject obstacle = Instantiate(prefab);
        obstacle.transform.SetParent(tubeScript.transform);
        Vector3 newPosition = tubeScript.transform.TransformPoint(tubeScript.Torus.PointOnTorus(curveAngle, angle));
        Vector3 newCenter = tubeScript.transform.TransformPoint(tubeScript.Torus.PointOnCurve(curveAngle));
        obstacle.transform.position = newPosition;
        obstacle.transform.up = (newCenter - newPosition).normalized;
    }

    void AllignSegments(TubeSegment OriginTube, TubeSegment AlligningTube)
    {
        AlligningTube.pivot.SetParent(OriginTube.connector, false);
    }
}
