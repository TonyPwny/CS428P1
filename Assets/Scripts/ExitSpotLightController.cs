using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpotLightController : MonoBehaviour
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

        if (ProtagonistController.hasKey)
        {
            spotlight.color = Color.Lerp(Color.green, Color.clear, time);
        }
        else
        {
            spotlight.color = Color.Lerp(Color.red, Color.clear, time);
        }
    }
}
