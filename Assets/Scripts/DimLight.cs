using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class DimLight : MonoBehaviour
{
    Light light;
    float startingIntensity;
    private void Start()
    {
        light = GetComponent<Light>();
        startingIntensity = light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.z < -1)
        {
            light.intensity -= startingIntensity * 5 * Time.deltaTime;
        }
    }
}
