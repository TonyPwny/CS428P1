// Anthony Tiongson (ast119)
// List of resources used:
//      
//      

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(50, 30, 45) * Time.deltaTime);
    }

    public void KeyDropped(Vector3 position)
    {
        transform.GetComponent<BoxCollider>().isTrigger = false;
        transform.parent.gameObject.transform.position = position;
        transform.position += new Vector3(0f, 0.75f, 0f);
        transform.GetComponent<Rigidbody>().isKinematic = false;
        transform.parent.gameObject.SetActive(true);
        transform.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 10f, 0f));
        if (transform.position.y <= 0.5)
        {
            transform.GetComponent<Rigidbody>().isKinematic = true;
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        }
    }

    private void KeyPickUp()
    {
        transform.parent.gameObject.SetActive(false);
    }

    private void KeyRetrieval()
    {
        transform.parent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (GamePlusController.inPlay && collision.gameObject.CompareTag("Protagonist") && (Vector3.Distance(collision.transform.position, transform.position) < 5))
        {
            KeyPickUp();
            collision.GetComponent<ProtagonistController>().HasKey(this);
        }

        if (GamePlusController.inPlay && collision.gameObject.CompareTag("Antagonist") && (Vector3.Distance(collision.transform.position, transform.position) < 5))
        {
            KeyRetrieval();
        }
    }
}
