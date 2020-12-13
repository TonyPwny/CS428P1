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
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Text authorshipText;
    public Text promptText;
    public Text gameRulesText;
    public Text controlsP1Text;
    public Text controlsP2Text;
    public Text announcementsText;
    public Text gameOverText;
    public PlayerController playerOne;
    public PlayerController playerTwo;
    public int timeLimit;
    public static bool inPlay = false;

    private int timeLimitSeconds;
    private static bool announcementActive = false;
    private bool gameOver = false;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        authorshipText.text = "RU SAS CS448 B0 Anthony Tiongson (ast119) Fall 2020";
        timeLimitSeconds = timeLimit * 60;
        gameRulesText.text = "Game Rules" + "\n" +
            "The time limit is set to " + timeLimit + " min. (" + timeLimitSeconds + " sec.)\n" +
            "Golden cubes are worth points and will respawn after 10 seconds.\n" +
            "Every 10 seconds, golden cubes gain an additional point of value.\n" +
            "Colliding with walls will deduct a point.\n" +
            "Colliding with the opponent at a higher height reduces their points!\n" +
            "The higher you hit your opponent, the more points you knock off!\n" + 
            "Falling off is an instant disqualification!"; 
        promptText.text = "Press Enter/Return to Begin";
        controlsP1Text.text = "P1 Uses W, S, A, D to Move\nSpacebar to Jump";
        controlsP2Text.text = "P2 Uses Arrow Keys to Move\nRight Shift to Jump";
        announcementsText.text = "Roll a Ball Deluxe";
        gameOverText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Submit") && !inPlay && !gameOver)
        {
            authorshipText.text = "";
            promptText.text = "Press Esc to Start Over";
            gameRulesText.text = "";
            controlsP1Text.text = "";
            controlsP2Text.text = "";
            playerOne.SetScoreText();
            playerTwo.SetScoreText();
            TimeController.instance.BeginTimer();
            MakeAnnouncement("Begin!", 5);
            inPlay = true;
        }

        if (Input.GetButton("Cancel"))
        {
            inPlay = false;
            gameOver = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (playerOne.transform.position.y < -5 && playerTwo.transform.position.y >= -5)
        {
            inPlay = false;
            gameOver = true;
            MakeAnnouncement("Guess P1 had a falling out?", 0);
            gameOverText.text = "P1 has fallen!" + "\nP2 wins!";
        }
        else if (playerTwo.transform.position.y < -5 && playerOne.transform.position.y >= -5)
        {
            inPlay = false;
            gameOver = true;
            MakeAnnouncement("P2 was enjoying that view a little too much!", 0);
            gameOverText.text = "P2 has fallen!" + "\nP1 wins!";
        }

        if (((float)(TimeController.instance.ElapsedSeconds()) / (float)timeLimitSeconds)  == 0.5)
        {
            MakeAnnouncement("Time limit halfway reached!", 10);
        }

        if (((float)(TimeController.instance.ElapsedSeconds()) / (float)timeLimitSeconds) == 0.75)
        {
            MakeAnnouncement("Time is almost up!", 10);
        }

        if ((timeLimitSeconds - TimeController.instance.ElapsedSeconds()) == 10)
        {
            MakeAnnouncement("TEN SECONDS LEFT!", 2);
        }

        if ((timeLimitSeconds - TimeController.instance.ElapsedSeconds()) == 5)
        {
            MakeAnnouncement("FIVE SECONDS LEFT!", 2);
        }

        if (TimeController.instance.Minutes() >= timeLimit)
        {
            inPlay = false;
            gameOver = true;

            if (playerOne.MyScore() > playerTwo.MyScore())
            {
                gameOverText.text = "P1 is victorious!";
                MakeAnnouncement("Guess the 1 in P1 is for 1st place ;)", 0);
            }
            else if (playerOne.MyScore() < playerTwo.MyScore())
            {
                gameOverText.text = "P2 is victorious!";
                MakeAnnouncement("Guess P2 is just 2 good ;)", 0);
            }
            else
            {
                gameOverText.text = "Stalemate!" + "\n" + "Rematch?";
                MakeAnnouncement("How exciting!! Or boring??", 0);
            }
        }
    }

    public void MakeAnnouncement(string announcement, int killTime)
    {
        if (!announcementActive)
        {
            if (killTime == 0)
            {
                announcementsText.text = announcement;
                announcementActive = true;
            }
            else
            {
                announcementsText.text = announcement;
                announcementActive = true;
                StartCoroutine("KillAnnouncement", killTime);
            }
        }
        else
        {
            KillAnnouncementNow(announcement, killTime);
        }
    }

    private void KillAnnouncementNow(string announcement, int killTime)
    {
        StopCoroutine("KillAnnouncement");
        announcementsText.text = "";
        announcementActive = false;
        MakeAnnouncement(announcement, killTime);
    }

    IEnumerator KillAnnouncement(int killTime)
    {
        yield return new WaitForSeconds(killTime);
        announcementsText.text = "";
        announcementActive = false;
    }
}
