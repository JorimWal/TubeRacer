using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeSystem : MonoBehaviour
{
    public static TubeSystem Instance;

    GameObject prefab;
    public List<TubeSegment> tubes = new List<TubeSegment>();

    public float Diameter = 15;
    public int TubeSegments = 16;

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
        prefab = Resources.Load<GameObject>("Prefabs/TubeSegment");

        for(int i = 0; i < 10; i++)
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
        //Reset the system to position 0, so the player stays near the origin
        //nextTube.transform.localPosition = Vector3.zero;
        AddSegment();
    }

    public void AddSegment()
    {
        GameObject pivot = new GameObject("Pivot");
        GameObject instance = GameObject.Instantiate(prefab);
        TubeSegment tubeScript = instance.GetComponent<TubeSegment>();
        instance.transform.SetParent(pivot.transform, false);
        pivot.transform.SetParent(transform, false);
        float randomRotation = Random.Range(0, TubeSegments) * (360 / (float)TubeSegments);
        pivot.transform.localRotation = Quaternion.Euler(0, randomRotation, 0);

        float radius = Random.Range(25, 45);
        int segments = Random.Range(4, 9);
        tubeScript.SetTorus(segments, 50, TubeSegments, radius + (Diameter / 2), (Diameter / 2));
        tubes.Add(tubeScript);

        if (tubes.Count > 1)
        {
            AllignSegments(tubes[tubes.Count - 2], tubes[tubes.Count - 1]);
        }
    }

    void AllignSegments(TubeSegment OriginTube, TubeSegment AlligningTube)
    {
        AlligningTube.pivot.SetParent(OriginTube.connector, false);
        //AlligningTube.pivot.SetParent(transform);
    }
}
