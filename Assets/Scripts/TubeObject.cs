using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeObject : MonoBehaviour
{
    public int tile;
    public float height = 0;
    public float depth = 25;
    public float speedModifier = 4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        depth -= speedModifier * Tube.Instance.speed * Time.deltaTime;
        //Let the tube translate the object to match the curve
        Tube.Instance.TranslateLocation(transform, depth, tile, height);
        if (transform.position.z < -1)
            Destroy(gameObject, 5);
    }
}
