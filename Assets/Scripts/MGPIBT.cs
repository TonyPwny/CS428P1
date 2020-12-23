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
    public Transform antagonists, key, exit;

    private BehaviorAgent behaviorAgent;

    // Start is called before the first frame update
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartBehavior()
    {
        behaviorAgent.StartBehavior();
    }

    public void StopBehavior()
    {
        behaviorAgent.StopBehavior();
    }

    protected Node Patrol() // Antagonists' Patrol routine.
    {
        return new LeafInvoke(() =>
        {
            bool timeLimitReached = GamePlusController.timeLimitReached; // true if the time limit is reached.

            foreach (Transform antagonist in antagonists)
            {
                if (!antagonist.GetComponent<AntagonistController>().targetAcquired)
                {
                    antagonist.GetComponent<AntagonistController>().ComputeForce();
                }
            }
            
            if (!timeLimitReached) // Fail (loop again) if time limit has not been reached.
            {
                return RunStatus.Failure;
            }
            else // Successfully patrolled until the end of the game; antagonists win.
            {
                return RunStatus.Success;
            }
        });
    }

    protected Node Pursue()
    {
        return new LeafInvoke(() =>
        {
            Transform target;

            foreach (Transform antagonist in antagonists)
            {
                if (antagonist.GetComponent<AntagonistController>().targetAcquired)
                {
                    target = antagonist.GetComponent<AntagonistController>().Target();
                    antagonist.GetComponent<AntagonistController>().ComputeForce(target);
                }
            }
        });
    }

    protected Node AntagonistAttack()
    {
        return new LeafInvoke(() =>
        {
            Transform target;

            foreach (Transform antagonist in antagonists)
            {
                if (antagonist.GetComponent<AntagonistController>().targetAcquired)
                {
                    target = antagonist.GetComponent<AntagonistController>().Target();

                    if (target.CompareTag("Protagonist") && (Vector3.Distance(antagonist.position, target.position) <= 4) && ProtagonistController.hasKey && antagonist.GetComponent<AntagonistController>().isGrounded)
                    {
                        antagonist.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 100f, 0f));
                    }
                }
            }
        });
    }

    protected Node SelP_AntagonistEvaluate() // Evaluate antagonists' current situation.
    {
        Node antagonistEvaluate = new SelectorParallel(
            new DecoratorLoop(
                this.Pursue()
                ),
            new DecoratorLoop(
                this.AntagonistAttack()
                ));

        return antagonistEvaluate;
    }

    protected Node SeqP_AntagonistsRoot()
    {
        Node antagonistsIBT = new SequenceParallel(
            // Patrol until the time limit is reached to succeed.
            new DecoratorLoopSuccess(
                this.Patrol()
                ),
            // Continually evaluate your surroundings, returning success.
            new DecoratorLoop(
                this.SelP_AntagonistEvaluate()
                ));

        return antagonistsIBT;
    }

    protected Node BuildTreeRoot()
    {
        Node gameIBT = new DecoratorLoopSuccess(
                this.SeqP_AntagonistsRoot()
                );

        return gameIBT;
    }
}
