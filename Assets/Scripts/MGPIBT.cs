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
    public GameObject antagonists;

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

    protected Node BuildAntagonistsRoot(Transform target)
    {
        Val<Vector3> position = Val.V(() => target.position);

        return new Selector();
    }

    protected Node BuildTreeRoot()
    {
        Node gameIBT = new SelectorParallel(
                            new DecoratorLoopSuccess(
                                new Selector(
                                    )),
                            new DecoratorLoopSuccess(
                                this.BuildAntagonistsRoot(protagonist.transform)
                                ));
        return gameIBT;
    }
}
