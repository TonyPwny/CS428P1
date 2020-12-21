// Anthony Tiongson (ast119)
// List of resources used:
//      
//      

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TreeSharpPlus;

public class AntagonistController : MonoBehaviour
{
    public float speed;
    public float jumpPower;

    private NavMeshAgent nma;
    private Rigidbody rb;
    private bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        SetSpeed();
        SetJumpPower();
        nma = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Patrol()
    {
        print("Patrol");
        Vector3 desiredDirection, desiredDirectionSide;
        float moveHorizontal, moveVertical;
        var force = Vector3.zero;

        if (GetVelocity() == Vector3.zero)
        {
            moveHorizontal = Random.Range(-1, 2);
            moveVertical = Random.Range(-1, 2);
            desiredDirection = new Vector3(moveHorizontal, 0.0f, moveVertical);
        }
        else
        {
            moveHorizontal = GetVelocityX();
            moveVertical = GetVelocityZ();
            desiredDirection = new Vector3(moveHorizontal, 0.0f, moveVertical);
            force = desiredDirection;
        }

        force += desiredDirection * speed;
        Ray wallRay = new Ray(transform.position, desiredDirection);

        if (Physics.Raycast(wallRay, out RaycastHit hit))
        {
            if (hit.distance <= 5 && !hit.transform.CompareTag("Protagonist"))
            {
                force -= desiredDirection * speed;
                Ray wallRayLeft = new Ray(transform.position, Vector3.Cross(desiredDirection, Vector3.up));
                Ray wallRayRight = new Ray(transform.position, Vector3.Cross(desiredDirection, Vector3.down));

                if (Physics.Raycast(wallRayLeft, out RaycastHit hitLeft) && hitLeft.distance <= 5)
                {
                    desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.up);
                    force -= new Vector3(desiredDirectionSide.x, 0.0f, desiredDirectionSide.z) * speed;
                }
                else if (Physics.Raycast(wallRayRight, out RaycastHit hitRight) && hitRight.distance <= 5)
                {
                    desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.down);
                    force -= new Vector3(desiredDirectionSide.x, 0.0f, desiredDirectionSide.z) * speed;
                }
                else
                {
                    int rand = Random.Range(0, 2);
                    if (rand == 0)
                    {
                        desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.up);
                        force -= new Vector3(desiredDirectionSide.x, 0.0f, desiredDirectionSide.z) * speed;
                    }
                    else
                    {
                        desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.down);
                        force -= new Vector3(desiredDirectionSide.x, 0.0f, desiredDirectionSide.z) * speed;
                    }
                }
            }
        }

        rb.AddForce(Vector3.ClampMagnitude(force, 10f));
    }

    private Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    private float GetVelocityX()
    {
        return rb.velocity.x;
    }

    private float GetVelocityZ()
    {
        return rb.velocity.z;
    }

    private void SetSpeed()
    {
        speed = GetComponentInParent<AntagonistsController>().GetSpeed();
    }

    private void SetJumpPower()
    {
        jumpPower = GetComponentInParent<AntagonistsController>().GetJumpPower();
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }
}
