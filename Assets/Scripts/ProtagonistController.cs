// Anthony Tiongson (ast119)
// List of resources used:
//      
//      

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProtagonistController : MonoBehaviour
{
    public float speed = 10f;
    public float jumpPower = 10f;
    public static bool hasKey = false;

    private NavMeshAgent nma;
    private Rigidbody rb;
    private KeyController key;
    private bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasKey && (key != null))
        {
            key = null;
        }

        if (transform.position.y < -10)
        {
            GamePlusController.instance.ProtagonistFell();
        }
    }

    // FixedUpdate is called just before performing any physics calculations
    private void FixedUpdate()
    {
        if (GamePlusController.inPlay && GamePlusController.userControlled)
        {
            float moveHorizontal = Input.GetAxis("1_Horizontal");
            float moveVertical = Input.GetAxis("1_Vertical");


            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            if (Input.GetButton("1_Jump") && isGrounded)
            {
                movement.Set(moveHorizontal, jumpPower, moveVertical);
            }

            rb.AddForce(movement * speed);
        }
    }

    public void HasKey(KeyController key)
    {
        this.key = key;
        hasKey = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Antagonist"))
        {
            if (hasKey && (transform.position.y < (collision.transform.position.y - 0.01f)))
            {
                key.KeyDropped(key, transform.TransformPoint(Vector3.zero));
                hasKey = false;
            }
        }
    }
}
