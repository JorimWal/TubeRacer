using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartRow : MonoBehaviour
{
    PlayerController player;
    Image[] images;
    Color dead, alive;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        images = GetComponentsInChildren<Image>();
        dead = new Color(130 / 255f, 130 / 255f, 130 / 255f);
        alive = new Color(198 / 255f, 13 / 255f, 35 / 255f);
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            for (int i = 0; i < images.Length; i++)
                images[i].color = dead;
            for (int i = 0; i < player.Lives; i++)
                images[i].color = alive;
        }
    }
}
