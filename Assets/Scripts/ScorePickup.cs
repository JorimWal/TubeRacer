using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePickup : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (PlayerController.Instance.ScoreMultiplier >= 8)
            Destroy(gameObject);
    }
}
