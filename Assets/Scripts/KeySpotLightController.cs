﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpotLightController : MonoBehaviour
{
    Light spotlight;

    // Start is called before the first frame update
    void Start()
    {
        spotlight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        float time = Mathf.PingPong(Time.time, 1.0f) / 1.0f;
        spotlight.color = Color.Lerp(Color.white, Color.yellow, time);

        if (ProtagonistController.hasKey)
        {
            spotlight.gameObject.SetActive(false);
        }
        else
        {
            spotlight.gameObject.SetActive(true);
        }
    }
}
