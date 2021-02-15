using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    GameObject tunnelLight;
    GameObject obstacle;
    // Start is called before the first frame update
    void Start()
    {
        tunnelLight = Resources.Load<GameObject>("Prefabs/TunnelLight");
        obstacle = Resources.Load<GameObject>("Prefabs/Obstacle");

        StartCoroutine(SpawnObstacles());
        StartCoroutine(SpawnLights());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnObstacles()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            SpawnObject(obstacle, 25, 0);
        }
    }

    IEnumerator SpawnLights()
    {
        while (true)
        {
            GameObject light = SpawnObject(tunnelLight, 25, Tube.Instance.tubeDiameter / 2);
            light.AddComponent<DimLight>();
            yield return new WaitForSeconds(3);
        }
    }

    GameObject SpawnObject(GameObject prefab, float depth, float height)
    {
        int tile = Random.Range(0, Tube.Instance.tilesPerTube);
        GameObject instance = GameObject.Instantiate(prefab);
        TubeObject scriptInstance = instance.AddComponent<TubeObject>();
        scriptInstance.tile = tile;
        scriptInstance.depth = depth;
        scriptInstance.height = height;
        Tube.Instance.TranslateLocation(instance.transform, scriptInstance.depth, tile, scriptInstance.height);
        return instance;
    }
}
