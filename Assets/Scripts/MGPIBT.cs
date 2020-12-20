// Anthony Tiongson (ast119)
// List of resources used:
//      
//      

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;

public class MGPIBT : MonoBehaviour
{
    public GameObject protagonist;
    public Transform antagonists, exit, key, key_starting_location;
    public static float DESIRED_SPEED = 10f;
    public static float DESIRED_JUMP_POWER = 10f;

    private BehaviorAgent behaviorAgent;

    // Start is called before the first frame update
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected Node Patrol()
    {
        Vector3 movement;

        return new LeafInvoke(() =>
        {
            print("Patrol");

            bool detection = false; // true if protagonist is "in view" or key is "in view" and not at key_starting_location

            foreach (Transform antagonist in antagonists) {
                if (antagonist.GetComponent<Rigidbody>().velocity == Vector3.zero)
                {
                    int moveHorizontal = Random.Range(-1, 2);
                    int moveVertical = Random.Range(-1, 2);
                    movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * DESIRED_SPEED;
                    antagonist.GetComponent<Rigidbody>().AddForce(movement);
                }
                else
                {
                    movement = antagonist.GetComponent<Rigidbody>().velocity * DESIRED_SPEED;
                    antagonist.GetComponent<Rigidbody>().AddForce(Vector3.ClampMagnitude(movement, 10f));
                }
            }

            if (!detection) // fail (loop again) if protagonist is not "in view" or key is not "in view" and not at key_starting_location, otherwise succeed (break)
            {
                return RunStatus.Failure;
            }
            else // successfully patrolled (break)
            {
                return RunStatus.Success;
            }
        });
    }

    protected Node Pursue(Transform target)
    {
        Vector3 desiredDirection;

        bool detection = true; // true if protagonist or key is "in view"

        return new LeafInvoke(() =>
        {
            print("Pursue");

            foreach (Transform antagonist in antagonists)
            {
                desiredDirection = target.transform.position - antagonist.transform.position;
                antagonist.GetComponent<Rigidbody>().AddForce(desiredDirection * DESIRED_SPEED);
            }

            if (detection)
            {
                if (false)
                {
                    return RunStatus.Success;
                }
                else
                {
                    return RunStatus.Failure;
                }
            }
            else
            {
                return RunStatus.Failure;
            }
        });
    }

    protected Node Evade(Transform target)
    {
        Vector3 desiredDirection;

        return new LeafInvoke(() =>
        {
            print("Evade");
        });
    }
    protected Node GetKey(Transform key)
    {
        Vector3 desiredDirection;

        return new LeafInvoke(() =>
        {
            print("GetKey");
        });
    }

    protected Node GoToExit(Transform exit)
    {
        Vector3 desiredDirection;

        return new LeafInvoke(() =>
        {
            print("GoToExit");
        });
    }

    protected Node ST_ProtagonistRoot(Transform key, Transform exit, Transform antagonists)
    {
        Node protagonistIBT = new Sequence(
            // should be LoopSuccess, using Loop for debugging
            new DecoratorLoop(
                this.GetKey(key)
                ),
            new DecoratorLoop(
                this.GoToExit(exit)
                ));

        return protagonistIBT;
    }

    protected Node ST_AntagonistsRoot(Transform protagonist, Transform key, Transform key_starting_location)
    {
        Transform target = null; // assign target to either protagnoist or key
        Node antagonistsIBT = new Sequence(
            // loop until protagonist is "in view" or key is "in view" and not at key_starting_location
            new DecoratorLoopSuccess(
                this.Patrol()
                ),
            // if protagonist is "in view" and key is either at key_starting_location or not "in view", otherwise fail/break
            new DecoratorLoopSuccess(
                this.Pursue(target)
                ));

        return antagonistsIBT;
    }

    protected Node BuildTreeRoot()
    {
        Node gameIBT = new SelectorParallel(
                            new DecoratorLoopSuccess(
                                this.ST_ProtagonistRoot(key, exit, antagonists)),
                            new DecoratorLoopSuccess(
                                this.ST_AntagonistsRoot(protagonist.transform, key, key_starting_location)
                                ));
        return gameIBT;
    }
}
