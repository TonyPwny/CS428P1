// Anthony Tiongson (ast119)
// List of resources used:
//      
//      

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    public Light spotLight;
    public static bool keyDropped = false;

    private Vector3 startingLocation;
    private Vector3 spotlightStartingLocation;

    // Start is called before the first frame update
    void Start()
    {
        startingLocation = transform.position;
        spotlightStartingLocation = spotLight.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.Rotate(new Vector3(50, 30, 45) * Time.deltaTime);

            if (!this.GetComponent<Rigidbody>().isKinematic && (transform.localPosition.y <= 0.6))
            {
                this.GetComponent<Rigidbody>().isKinematic = true;
                transform.localPosition = new Vector3(transform.localPosition.x, 0.5f, transform.localPosition.z);
            }
        }
    }

    public Vector3 StartingLocation()
    {
        return startingLocation;
    }

    public void KeyDropped(KeyController key, Vector3 position)
    {
        keyDropped = true;
        key.GetComponentInParent<Transform>().position = position;
        key.transform.localPosition += new Vector3(0f, 1.5f, 0f);
        key.gameObject.SetActive(true);
        key.GetComponent<Rigidbody>().isKinematic = false;
        key.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 400f, 0f));
        spotLight.gameObject.SetActive(true);
    }

    private void KeyPickedUp(GameObject protagonist, KeyController key)
    {
        keyDropped = false;
        protagonist.GetComponent<ProtagonistController>().HasKey(key);
        key.gameObject.SetActive(false);
        spotLight.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (GamePlusController.inPlay && collision.gameObject.CompareTag("Protagonist") && (Vector3.Distance(collision.transform.position, transform.position) < 1))
        {
            KeyPickedUp(collision.gameObject, this);
        }
    }
}
