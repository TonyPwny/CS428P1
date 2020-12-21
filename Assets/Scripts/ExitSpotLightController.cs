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
        if (ProtagonistController.hasKey)
        {
            spotlight.color = Color.green;
        }
        else
        {
            spotlight.color = Color.red;
        }
    }
}
