using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public Vector3 Amount = Vector3.up;
    public float speed = 0.25f;
    Vector3 startPosition;
    float angle = 0;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        angle += 2 * Mathf.PI * Time.deltaTime * speed;
        angle = angle % (2 * Mathf.PI);
        transform.localPosition = startPosition + (Amount * Mathf.Sin(angle));
    }
}
