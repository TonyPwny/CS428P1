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
    private float jumpPower;
    public bool targetAcquired = false;
    public bool isGrounded = true;

    private NavMeshAgent nma;
    private Rigidbody rb;
    private Transform target;
    private HashSet<GameObject> perceivedNeighbors = new HashSet<GameObject>();
    private HashSet<GameObject> adjacentWalls = new HashSet<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SetSpeed();
        SetJumpPower();
        nma = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        target = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Transform Target()
    {
        return target;
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
        var force = CalculateGoalForce(target) + CalculateNeighborForce() + CalculateWallForce();
        rb.AddForce(new Vector3(Mathf.Clamp(force.x, -1, 1), 0.0f, Mathf.Clamp(force.z, -1, 1)) * speed);

    }

    private Vector3 CalculateGoalForce(Transform target)
    {
        Vector3 desiredDirection, desiredDirectionSide;
        float moveHorizontal, moveVertical;
        var force = Vector3.zero;

        if (!targetAcquired) // Patrol
        {
            if (Mathf.Approximately(GetVelocityX(), 0) && Mathf.Approximately(GetVelocityY(), 0) && Mathf.Approximately(GetVelocityZ(), 0))
            {
                moveHorizontal = Random.Range(-1f, 1f);
                moveVertical = Random.Range(-1f, 1f);
                desiredDirection = new Vector3(moveHorizontal, 0.0f, moveVertical);
            }
            else
            {
                moveHorizontal = GetVelocityX();
                moveVertical = GetVelocityZ();
                desiredDirection = new Vector3(moveHorizontal, 0.0f, moveVertical);
            }

            Ray wallRay = new Ray(transform.position, desiredDirection);

            if (Physics.Raycast(wallRay, out RaycastHit hit))
            {
                if (hit.distance <= 5 && hit.transform.CompareTag("Wall"))
                {
                    force -= desiredDirection;
                    Ray wallRayLeft = new Ray(transform.position, Vector3.Cross(desiredDirection, Vector3.up));
                    Ray wallRayRight = new Ray(transform.position, Vector3.Cross(desiredDirection, Vector3.down));
                    Physics.Raycast(wallRayLeft, out RaycastHit hitLeft);
                    Physics.Raycast(wallRayRight, out RaycastHit hitRight);

                    if (((hitLeft.distance <= hitRight.distance) && hitLeft.transform.CompareTag("Wall") && hitRight.transform.CompareTag("Wall")) || (hitLeft.transform.CompareTag("Wall") && !hitRight.transform.CompareTag("Wall")) || ((hitLeft.distance > hitRight.distance) && (!hitLeft.transform.CompareTag("Wall") && !hitRight.transform.CompareTag("Wall"))))
                    {
                        desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.down);
                    }
                    else if (((hitLeft.distance > hitRight.distance) && hitLeft.transform.CompareTag("Wall") && hitRight.transform.CompareTag("Wall")) || (!hitLeft.transform.CompareTag("Wall") && hitRight.transform.CompareTag("Wall")) || ((hitLeft.distance <= hitRight.distance) && (!hitLeft.transform.CompareTag("Wall") && !hitRight.transform.CompareTag("Wall"))))
                    {
                        desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.up);
                    }
                    else
                    {
                        desiredDirectionSide = Vector3.zero;
                    }
                    
                    force += new Vector3(desiredDirectionSide.x, 0.0f, desiredDirectionSide.z);
                }
                else
                {
                    force += desiredDirection;
                }
            }
        }
        else // Pursue or retrieve target.
        {
            desiredDirection = target.position - transform.position;

            if (Vector3.Distance(target.position, transform.position) >= 2f) {
                force += desiredDirection * (10f / Vector3.Distance(target.position, transform.position));
            }
            else
            {
                force += desiredDirection;
            }
        }

        return force;
    }

    private Vector3 CalculateNeighborForce()
    {
        Vector3 desiredDirection, desiredDirectionSide = Vector3.zero;
        var force = Vector3.zero;

        if (!targetAcquired) // Ignore other antagonists if in pursuit or recovery.
        {
            foreach (var antagonist in perceivedNeighbors)
            {
                desiredDirection = transform.position - antagonist.transform.position;
                force += desiredDirection;
                Ray wallRayLeft = new Ray(transform.position, Vector3.Cross(desiredDirection, Vector3.up));
                Ray wallRayRight = new Ray(transform.position, Vector3.Cross(desiredDirection, Vector3.down));
                Physics.Raycast(wallRayLeft, out RaycastHit hitLeft);
                Physics.Raycast(wallRayRight, out RaycastHit hitRight);


                if ((hitLeft.distance <= 3) && hitLeft.transform.CompareTag("Antagonist"))
                {
                    desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.down);
                }
                if ((hitRight.distance <= 3) && hitRight.transform.CompareTag("Antagonist"))
                {
                    desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.up);
                }

                force += new Vector3(desiredDirectionSide.x, 0.0f, desiredDirectionSide.z);
            }
        }

        return force;
    }

    private Vector3 CalculateWallForce()
    {
        Vector3 desiredDirection, desiredDirectionSide = Vector3.zero;
        var force = Vector3.zero;

        if (!targetAcquired) // Ignore walls if in pursuit or recovery.
        {
            foreach (var wall in adjacentWalls)
            {
                desiredDirection = transform.position - wall.transform.position;
                force += desiredDirection;
                Ray wallRayLeft = new Ray(transform.position, Vector3.Cross(desiredDirection, Vector3.up));
                Ray wallRayRight = new Ray(transform.position, Vector3.Cross(desiredDirection, Vector3.down));
                Physics.Raycast(wallRayLeft, out RaycastHit hitLeft);
                Physics.Raycast(wallRayRight, out RaycastHit hitRight);

                if ((hitLeft.distance <= 5) && hitLeft.transform.CompareTag("Wall"))
                {
                    desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.down);
                }
                if ((hitRight.distance <= 5) && hitRight.transform.CompareTag("Wall"))
                {
                    desiredDirectionSide = Vector3.Cross(desiredDirection, Vector3.up);
                }

                force += new Vector3(desiredDirectionSide.x, 0.0f, desiredDirectionSide.z);
            }
        }

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

    private float GetVelocityY()
    {
        return rb.velocity.y;
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
        if (other.CompareTag("Protagonist"))
        {
            target = other.gameObject.transform;
            targetAcquired = true;
            perceivedNeighbors.Add(other.gameObject);
        }

        if (other.CompareTag("Antagonist"))
        {
            perceivedNeighbors.Add(other.gameObject);
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
