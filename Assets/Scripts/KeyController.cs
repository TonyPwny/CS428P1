// Anthony Tiongson (ast119)
// List of resources used:
//      
//      

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    private Vector3 startingLocation;

    // Start is called before the first frame update
    void Start()
    {
        startingLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(50, 30, 45) * Time.deltaTime);

        if (!this.GetComponent<Rigidbody>().isKinematic && (transform.localPosition.y <= 0.6))
        {
            this.GetComponent<Rigidbody>().isKinematic = true;
            transform.localPosition = new Vector3(transform.localPosition.x, 0.5f, transform.localPosition.z);
        }
    }

    public void KeyDropped(KeyController key, Vector3 position)
    {
        key.transform.position = position + new Vector3(0f, 1f, 0f);
        key.gameObject.SetActive(true);
        key.GetComponent<Rigidbody>().isKinematic = false;
        key.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 400f, 0f));
    }

    private void KeyPickedUp(KeyController key)
    {
        key.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (GamePlusController.inPlay && collision.gameObject.CompareTag("Protagonist") && (Vector3.Distance(collision.transform.position, transform.position) < 1))
        {
            KeyPickedUp(this);
            collision.GetComponent<ProtagonistController>().HasKey(this);
        }

        if (GamePlusController.inPlay && !(transform.position == startingLocation) && collision.gameObject.CompareTag("Antagonist") && (Vector3.Distance(collision.transform.position, transform.position) < 1))
        {
            print("key position: " + transform.position);
            print("key starting position: " + startingLocation);
            KeyPickedUp(this);
        }
    }
}
