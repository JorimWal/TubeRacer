using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (PlayerController.Instance.Lives >= 3)
            Destroy(gameObject);
    }
}
