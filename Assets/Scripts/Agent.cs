// Anthony Tiongson (ast119)
// Used previous project, lecture, and recitation for reference as well as:
// https://answers.unity.com/questions/697830/how-to-calculate-direction-between-2-objects.html

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Agent : MonoBehaviour
{
    public static float DESIRED_SPEED = 1.5f;
    public float radius;
    public float mass;
    public float perceptionRadius;

    private List<Vector3> path;
    private NavMeshAgent nma;
    private Rigidbody rb;
    private int explosion;

    private HashSet<GameObject> perceivedNeighbors = new HashSet<GameObject>();
    private HashSet<GameObject> adjacentWalls = new HashSet<GameObject>();

    void Start()
    {
        explosion = 0;
        path = new List<Vector3>();
        nma = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        gameObject.transform.localScale = new Vector3(2 * radius, 1, 2 * radius);
        nma.radius = radius;
        rb.mass = mass;
        GetComponent<SphereCollider>().radius = perceptionRadius / 2;
        
    }

    private void Update()
    {
        if (path.Count > 1 && Vector3.Distance(transform.position, path[0]) < 1.1f)
        {
            path.RemoveAt(0);
        } else if (path.Count == 1 && Vector3.Distance(transform.position, path[0]) < 2f)
        {
            path.RemoveAt(0);

            if (path.Count == 0)
            {
                gameObject.SetActive(false);
                AgentManager.RemoveAgent(gameObject);
            }
        }

        HashSet<GameObject> rem = new HashSet<GameObject>();

        foreach (var neighbor in perceivedNeighbors)
        {
            if(!neighbor.activeSelf){
                rem.Add(neighbor);     
			}
        }

        foreach(var neighbor in rem)
        {
            perceivedNeighbors.Remove(neighbor);  
		}

        #region Visualization

        if (false)
        {
            if (path.Count > 0)
            {
                Debug.DrawLine(transform.position, path[0], Color.green);
            }
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.yellow);
            }
        }

        if (false)
        {
            foreach (var neighbor in perceivedNeighbors)
            {
                Debug.DrawLine(transform.position, neighbor.transform.position, Color.yellow);
            }
        }
        
        explosion += 1;
        #endregion
    }

    #region Public Functions

    public void ComputePath(Vector3 destination)
    {
        nma.enabled = true;
        var nmPath = new NavMeshPath();
        nma.CalculatePath(destination, nmPath);
        path = nmPath.corners.Skip(1).ToList();
        //path = new List<Vector3>() { destination };
        //nma.SetDestination(destination);
        nma.enabled = false;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    #endregion

    #region Incomplete Functions

    // Recitation Parameters
    // float DESIRED_SPEED = 1.5f;
    // float T = 0.5f;
    // float A = 2000f;
    // float B = 0.08f;
    // float K = 1.2f * 100000f;
    // float KAPPA = 2.4f * 100000f;
    // float WALL_A = 2000f;
    // float WALL_B = 0.08f;
    // float WALL_K = 1.2f * 100000f;
    // float WALL_KAPPA = 2.4f * 100000f;
    // float MAX_ACCEL = 10000f;

    private Vector3 ComputeForce()
    {
        var force = CalculateGoalForce() + CalculateAgentForce() + CalculateWallForce();

        if (force != Vector3.zero)
        {
            return force.normalized * Mathf.Min(force.magnitude, Parameters.maxSpeed);
        }
        else
        {
            return Vector3.zero;
        }
    }
    
    private Vector3 CalculateGoalForce()
    {
        if (path.Any())
        {
            Vector3 desiredDirection = new Vector3();
            desiredDirection = (path[0] - transform.position);
            desiredDirection.Normalize();
            var force = rb.mass * (((DESIRED_SPEED * desiredDirection) - GetVelocity()) / Parameters.T);
            return force;
        }
        else
        {
            return Vector3.zero;
        }
        //testing
        /*
        if (path.Count == 0)
        {
            return Vector3.zero;
        }
        var desiredDir = path[0] - transform.position;
        var desiredVel = desiredDir.normalized * DESIRED_SPEED;
        var actualVel = rb.velocity;
        return mass * (desiredVel - actualVel) / Parameters.T;*/
    }

    private Vector3 CalculateAgentForce()
    {
        if(explosion < 10)
            return Vector3.zero;

        var force = Vector3.zero;
        /*
        foreach(var agent in perceivedNeighbors)
        {
            if (!AgentManager.IsAgent(agent))
            {
                continue;
            }
                var neighbor = AgentManager.agentsObjs[agent];
                var dir = (transform.position - neighbor.transform.position).normalized;
                var overlap = (radius + neighbor.radius) - Vector3.Distance(transform.position, agent.transform.position);
                var tangent = Vector3.Cross(Vector3.up, dir);

                force += Parameters.A * Mathf.Exp(overlap / Parameters.B) * dir; //proximityForce
                force += Parameters.k * Mathf.Max(overlap, 0) * dir; //repultionForce
                force += Parameters.Kappa * Mathf.Max(overlap, 0) * Vector3.Dot(rb.velocity - neighbor.GetVelocity(), tangent) * tangent; //slidingForce
        }
        */
        int g;
        Vector3 n = new Vector3();
        Vector3 t = new Vector3();

        foreach (var agent in perceivedNeighbors)
        {
            if (!AgentManager.IsAgent(agent))
            {
                continue;
            }

            n = transform.position - agent.transform.position;
            n.Normalize();
            t = Vector3.Cross(Vector3.up, n);

            if (Vector3.Distance(transform.position, agent.transform.position) <= (nma.radius + agent.GetComponent<NavMeshAgent>().radius))
            {
                g = 1;
            }
            else
            {
                g = 0;
            }
            force += ((Parameters.A * Mathf.Exp(((nma.radius + agent.GetComponent<NavMeshAgent>().radius) - Vector3.Distance(transform.position, agent.transform.position)) / Parameters.B) + (Parameters.k * g)) * n); //proximityForce + repulsionForce
            force +=(Parameters.Kappa * g * Vector3.Dot((agent.GetComponent<Rigidbody>().velocity - GetVelocity()), t) * t); //slidingForce
        }
        
        return force;
    }

    private Vector3 CalculateWallForce() //causing random push to agent

    {
        if(explosion < 10)
            return Vector3.zero;

        var force = Vector3.zero;
        /*
        foreach (var wall in adjacentWalls)
        {
           
            var dir = (transform.position - wall.transform.position).normalized;
            dir.y = 0;
            if(Mathf.Abs(dir.x) > Mathf.Abs(dir.z)){
                dir.z = 0;
            }
            else
            {
                dir.x = 0;
            }

            var overlap = radius - dir.magnitude - 0.5f;
            var tangent = Vector3.Cross(Vector3.up, dir);

            force += Parameters.WALL_A * Mathf.Exp(overlap / Parameters.WALL_B) * dir; //proximityForce
            force += Parameters.WALL_k * Mathf.Max(overlap, 0) * dir; //repultionForce
            force -= Parameters.WALL_Kappa * Mathf.Max(overlap, 0) * Vector3.Dot(rb.velocity, tangent) * tangent; //slidingForce
        }
       */
        
        int g;
        Vector3 d = new Vector3();
        Vector3 n = new Vector3();
        Vector3 t = new Vector3();

        foreach (var wall in adjacentWalls)
        {
            d = wall.transform.position - transform.position;
            RaycastHit hit;
            Ray wallRay = new Ray(transform.position, d);
            if (Physics.Raycast(wallRay, out hit))
            {
                n = hit.normal;
                n.Normalize();
                t = Vector3.Cross(Vector3.up, n);
                //Vector3 wallpoint = wall.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                //float wallRad = Vector3.Distance(wall.transform.position, wallpoint);

                if (hit.distance <= nma.radius + 0.25)
                //if (Vector3.Distance(transform.position, wall.transform.position) <= nma.radius + wallRad)
                {
                    g = 1;
                }
                else
                {
                    g = 0;
                }
                force += ((Parameters.WALL_A * Mathf.Exp(((nma.radius - hit.distance) - Vector3.Distance(transform.position, wall.transform.position)) / Parameters.WALL_B) + (Parameters.WALL_k/50000 * g)) * n) + (Parameters.WALL_Kappa * g * Vector3.Dot(nma.velocity, t) * t);
                //force += ((Parameters.WALL_A * Mathf.Exp((nma.radius + wallRad - Vector3.Distance(transform.position, wall.transform.position)) / Parameters.WALL_B) + (Parameters.WALL_k * g)) * n); //proximityForce + repulsionForce
                //force -= (Parameters.WALL_Kappa * g * Vector3.Dot(nma.velocity, t) * t); //slidingForce
            }

        }
        
        return force;
    }

    public void ApplyForce()
    {
        var force = ComputeForce();
        force.y = 0;

        rb.AddForce(force * 10, ForceMode.Force);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (AgentManager.IsAgent(other.gameObject))
        {
            perceivedNeighbors.Add(other.gameObject);
        }
        if (WallManager.IsWall(other.gameObject))
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

    public void OnCollisionEnter(Collision collision)
    {
        
    }

    public void OnCollisionExit(Collision collision)
    {
        
    }

    
    #endregion
}
