// Anthony Tiongson (ast119)
// List of resources used:
// https://answers.unity.com/questions/1417767/c-how-to-check-if-player-is-grounded.html
// https://answers.unity.com/questions/1261937/creating-a-restart-button.html
// https://answers.unity.com/questions/1275232/disable-all-inputs.html
// https://answers.unity.com/questions/1400361/how-to-create-a-border-around-2d-text.html
// https://answers.unity.com/questions/885849/how-to-respawn-with-a-simple-delay.html
// https://gamedev.stackexchange.com/questions/151670/unity-how-to-detect-collision-occuring-on-child-object-from-a-parent-script
// https://learn.unity.com/tutorial/fps-mod-customize-the-sky
// https://learn.unity.com/project/roll-a-ball-tutorial
// https://answers.unity.com/questions/406479/stopping-an-ienumerator-function.html
// https://www.youtube.com/watch?v=qc7J0iei3BU

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerOne;
    public GameObject playerTwo;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - ((playerOne.transform.position + playerTwo.transform.position)/2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // LateUpdate is called once per frame but is guaranteed to run after all items have been processed in Update
    void LateUpdate()
    {
        transform.position = ((playerOne.transform.position + playerTwo.transform.position) / 2) + offset;

        if ((playerOne.transform.position.y < -5) || (playerTwo.transform.position.y < -5))
        {
            transform.position = new Vector3 (0, 8, -12);
        }
    }
}
