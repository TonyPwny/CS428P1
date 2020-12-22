using System.Collections;
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
        if (spotlight.isActiveAndEnabled)
        {
            float time = Mathf.PingPong(Time.time, 1.0f) / 1.0f;
            spotlight.color = Color.Lerp(Color.yellow, Color.white, time);
        }
    }
}
