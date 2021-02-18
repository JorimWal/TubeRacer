using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinning : MonoBehaviour
{
    public Vector3 Speed = new Vector3(1,1,1);

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Speed * Time.deltaTime);
    }
}
