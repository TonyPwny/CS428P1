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
        if (GamePlusController.inPlay)
        {
            behaviorAgent.StartBehavior();
        }
        else
        {
            behaviorAgent.StopBehavior();
        }
    }

    protected Node Patrol() // Antagonists' Patrol routine.
    {
        return new LeafInvoke(() =>
        {
            bool timeLimitReached = GamePlusController.timeLimitReached; // true if the time limit is reached.

            foreach (Transform antagonist in antagonists)
            {
                antagonist.GetComponent<AntagonistController>().ComputeForce();
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

    protected Node Retrieve()
    {
        return new LeafInvoke(() =>
        {
            print("Retrieve");
        });
    }

    protected Node Defend()
    {
        return new LeafInvoke(() =>
        {
            print("Defend");
        });
    }

    protected Node AntagonistAttack()
    {
        return new LeafInvoke(() =>
        {
            print("Antagonist Attack");
        });
    }

    protected Node Pursue()
    {
        return new LeafInvoke(() =>
        {
            print("Pursue");
        });
    }

    protected Node SelP_AntagonistEvaluate() // Evaluate antagonists' current situation.
    {
        Node antagonistEvaluate = new SelectorParallel(
            new DecoratorLoop(
                this.Retrieve()
                ),
            new DecoratorLoop(
                this.Defend()
                ),
            new DecoratorLoop(
                this.Attack()
                ),
            new DecoratorLoop(
                this.Pursue()
                ));

        return antagonistEvaluate;
    }
    protected Node GetKey()
    {
        return new LeafInvoke(() =>
        {
            print("Get Key");
        });
    }

    protected Node GoToExit()
    {
        return new LeafInvoke(() =>
        {
            print("Go To Exit");
        });
    }
    protected Node Evade()
    {
        return new LeafInvoke(() =>
        {
            print("Evade");
        });
    }

    protected Node Attack()
    {
        return new LeafInvoke(() =>
        {
            print("Attack");
        });
    }

    protected Node SeqT_Objective()
    {
        Node objective = new Sequence(
            new DecoratorLoopSuccess(
                this.GetKey()
                ),
            new DecoratorLoopSuccess(
                this.GoToExit()
            ));

        return objective;
    }

    protected Node SelP_Evaluate()
    {
        Node evaluate = new SelectorParallel(
            new DecoratorLoop(
                this.Evade()
                ),
            new DecoratorLoop(
                this.Attack()
                ));

        return evaluate;
    }

    protected Node SeqP_ProtagonistRoot()
    {
        Node protagonistIBT = new SequenceParallel(
            new DecoratorLoopSuccess(
                this.SeqT_Objective()
                ),
            new DecoratorLoop(
                this.SelP_Evaluate()
                ));

        return protagonistIBT;
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
        Node gameIBT = new SelectorParallel(
            new DecoratorLoopSuccess(
                this.SeqP_ProtagonistRoot()),
            new DecoratorLoopSuccess(
                this.SeqP_AntagonistsRoot()
                ));

        return gameIBT;
    }
}
