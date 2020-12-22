using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Protagonist") && ProtagonistController.hasKey && (Vector3.Distance(collision.transform.position, transform.position) < 1))
        {
            GamePlusController.instance.ProtagonistWins();
        }
    }
}
