// Anthony Tiongson (ast119)
// List of resources used:
//      
//      

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtagonistCameraController : MonoBehaviour
{
    public GameObject protagonist;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - protagonist.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // LateUpdate is called once per frame but is guaranteed to run after all items have been processed in Update
    void LateUpdate()
    {
        transform.position = protagonist.transform.position + offset;

        if (protagonist.transform.position.y < -5)
        {
            transform.position = new Vector3(0, 8, -12);
        }
    }
}
