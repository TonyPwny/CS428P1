﻿// Anthony Tiongson (ast119)
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
    public Transform antagonists;
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

    protected Node ST_Patrol()
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

    protected Node ST_Pursue(Transform target)
    {
        Vector3 desiredDirection;

        return new LeafInvoke(() =>
        {
            foreach (Transform antagonist in antagonists)
            {
                desiredDirection = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
                antagonist.GetComponent<Rigidbody>().AddForce(desiredDirection * DESIRED_SPEED);
            }
        });
    }

    protected Node ST_Evade(Transform target)
    {
        Vector3 desiredDirection;

        return new LeafInvoke(() =>
        {
            foreach (Transform antagonist in antagonists)
            {
                desiredDirection = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
                antagonist.GetComponent<Rigidbody>().AddForce(desiredDirection * DESIRED_SPEED);
            }
        });
    }

    protected Node BuildAntagonistsRoot(Transform target)
    {
        Node antagonistsIBT = new Sequence(
            new DecoratorLoop(
                this.ST_Patrol()
                ),
            new DecoratorLoop(
                this.ST_Pursue(target)
                ));

        return antagonistsIBT;
    }

    protected Node BuildTreeRoot()
    {
        Node gameIBT = new SelectorParallel(
                            new DecoratorLoopSuccess(
                                new Selector(
                                    new DecoratorLoop(
                                        new Selector(

                                            )))),
                            new DecoratorLoopSuccess(
                                this.BuildAntagonistsRoot(protagonist.transform)
                                ));
        return gameIBT;
    }
}
