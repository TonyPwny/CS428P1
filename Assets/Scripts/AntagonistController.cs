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
    private Transform target;
    private bool isGrounded = true;
    private static bool targetAcquired = false;
    private static readonly HashSet<GameObject> hashSets = new HashSet<GameObject>();
    private HashSet<GameObject> perceivedNeighbors = hashSets;
    private HashSet<GameObject> adjacentWalls = hashSets;

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

    public void ComputePath(Vector3 destination)
    {
        nma.enabled = true;
        var nmPath = new NavMeshPath();
        nma.CalculatePath(destination, nmPath);
        nma.enabled = false;
    }

    public void ComputeForce(Transform target = null)
    {
        var force = CalculateGoalForce(target) + CalculateAntagonistForce() + CalculateWallForce();
        rb.AddForce(Vector3.ClampMagnitude(force, 10f));
    }

    private Vector3 CalculateGoalForce(Transform target)
    {
        Vector3 desiredDirection, desiredDirectionSide;
        float moveHorizontal, moveVertical;
        var force = Vector3.zero;

        if (target == null) // Patrol
        {
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
                force += desiredDirection * speed;
            }

            if (targetAcquired) // Pursue target
            {

            }
            else // No target in range, patrol
            {
                Ray wallRay = new Ray(transform.position, desiredDirection);

                if (Physics.Raycast(wallRay, out RaycastHit hit))
                {
                    if (hit.distance <= 5 && !(hit.transform.CompareTag("Protagonist") || hit.transform.CompareTag("Key")))
                    {
                        force -= (desiredDirection * speed);
                        Ray wallRayLeft = new Ray(transform.position, Vector3.Cross(desiredDirection, Vector3.up));
                        Ray wallRayRight = new Ray(transform.position, Vector3.Cross(desiredDirection, Vector3.down));
                        Physics.Raycast(wallRayLeft, out RaycastHit hitLeft);
                        Physics.Raycast(wallRayRight, out RaycastHit hitRight);

                        if (hitLeft.distance <= hitRight.distance)
                        {
                            desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.down);
                        }
                        else
                        {
                            desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.up);
                        }

                        force += new Vector3(desiredDirectionSide.x, 0.0f, desiredDirectionSide.z) * speed;
                    }
                    else
                    {
                        force += desiredDirection * speed;
                    }
                }
            }
        }

        return force;
    }

    private Vector3 CalculateAntagonistForce()
    {
        var force = Vector3.zero;

        return force;
    }

    private Vector3 CalculateWallForce()
    {
        var force = Vector3.zero;

        return force;
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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key") || other.CompareTag("Protagonist") || other.CompareTag("Antagonist"))
        {
            perceivedNeighbors.Add(other.gameObject);

            if (other.CompareTag("Protagonist") && !target.CompareTag("Key"))
            {
                target = other.gameObject.transform;
            }

            if (other.CompareTag("Key"))
            {
                target = other.gameObject.transform;
            }
        }
        if (other.CompareTag("Wall"))
        {
            adjacentWalls.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (perceivedNeighbors.Contains(other.gameObject))
        {
            perceivedNeighbors.Remove(other.gameObject);
        }
        if (adjacentWalls.Contains(other.gameObject))
        {
            adjacentWalls.Remove(other.gameObject);
        }
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
