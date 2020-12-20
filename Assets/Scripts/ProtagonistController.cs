// Anthony Tiongson (ast119)
// List of resources used:
//      
//      

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtagonistController : MonoBehaviour
{
    public float speed;
    public float jumpPower;

    private Rigidbody rb;
    private bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // FixedUpdate is called just before performing any physics calculations
    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("1_Horizontal");
        float moveVertical = Input.GetAxis("1_Vertical");


        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (Input.GetButton("1_Jump") && isGrounded)
        {
            movement.Set(moveHorizontal, jumpPower, moveVertical);
        }

        print("moveHorizontal: " + moveHorizontal);
        print("moveVertical: " + moveVertical);
        rb.AddForce(movement * speed);
        print("adding force: " + movement * speed);
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
    }
}
