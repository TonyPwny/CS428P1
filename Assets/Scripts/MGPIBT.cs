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
        Vector3 desiredDirection;

        return new LeafInvoke(() =>
        {
            foreach (Transform antagonist in antagonists) {
                desiredDirection = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
                antagonist.GetComponent<Rigidbody>().AddForce(desiredDirection * DESIRED_SPEED);
            }
        });
    }

    protected Node Pursue(Transform target)
    {
        Vector3 desiredDirection;

        return new LeafInvoke(() =>
        {
            print("Pursue");
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

    protected Node ST_ProtagonistRoot(Transform key, Transform exit, Transform enemies)
    {
        Node antagonistsIBT = new Sequence(
            // should be LoopSuccess, using Loop for debugging
            new DecoratorLoop(
                this.GetKey(key)
                ),
            new DecoratorLoop(
                this.GoToExit(exit)
                ));

        return antagonistsIBT;
    }

    protected Node ST_AntagonistsRoot(Transform protagonist)
    {
        Node antagonistsIBT = new Sequence(
            new DecoratorLoop(
                this.Patrol()
                ),
            new DecoratorLoop(
                this.Pursue(protagonist)
                ));

        return antagonistsIBT;
    }

    protected Node BuildTreeRoot()
    {
        Node gameIBT = new SelectorParallel(
                            new DecoratorLoopSuccess(
                                this.ST_ProtagonistRoot(key, exit, antagonists.transform)),
                            new DecoratorLoopSuccess(
                                this.ST_AntagonistsRoot(protagonist.transform)
                                ));
        return gameIBT;
    }
}
