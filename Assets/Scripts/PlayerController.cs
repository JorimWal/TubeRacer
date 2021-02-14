using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float height = 4.3f;
    float angleDelta = 0;
    float angle = (3*Mathf.PI)/2;
    const float turnAngle = Mathf.PI / 2;

    public Transform model;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: read height from Tube.cs
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        //Calculate where on the circle the player should be
        Vector3 newposition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * height;
        //Move the player to new position, but keep the old z coordinate
        transform.position = new Vector3(newposition.x, newposition.y, transform.position.z);
        //Rotate the ball so the center of the tube is up
        transform.up = Vector3.zero - new Vector3(transform.position.x, transform.position.y);
    }

    void HandleInput()
    {
        float x = Input.GetAxis("Horizontal");
        angle += (x * turnAngle * Time.deltaTime);
        angle = angle % (2 * Mathf.PI);

        model.Rotate(transform.right, 1);
    }
}
