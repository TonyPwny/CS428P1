// Anthony Tiongson (ast119)
// List of resources used:
//      
//      

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePlusController : MonoBehaviour
{
    public static GamePlusController instance;
    public Text authorshipText;
    public Text promptText;
    public Text gameRulesText;
    public Text controlsText;
    public Text announcementsText;
    public Text gameOverText;
    public int timeLimitInMinutes = 2;
    public static bool inPlay = false;
    public static bool timeLimitReached = false;

    private int timeLimitSeconds;
    private static bool announcementActive = false;
    private bool gameOver = true;
    private bool protagonistFell = false;
    private bool protagonistWins = false;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        timeLimitSeconds = timeLimitInMinutes * 60;
        authorshipText.text = "RU SAS CS428 P1 Fall 2020 Group 13\n" +
            "Anthony Tiongson (ast119)\n" +
            "Saikiran Nakka (vn150)\n" +
            "Paul Chung (paulchun)";
        gameRulesText.text = "Game Rules" + "\n" +
            "The time limit is set to " + timeLimitInMinutes + " min. (" + timeLimitSeconds + " sec.)\n" +
            "The golden cube is a KEY item in the game and is REQUIRED to win:\n" +
            "TAKE the KEY to the GOAL AREA BEFORE the TIME LIMIT\n" +
            "TO WIN THE GAME.\n" +
            "If an enemy JUMPS INTO YOU and is HIGHER THAN YOU at impact,\n" +
            "you will be STRUCK AWAY then UPWARDS,\n" +
            "and you will DROP THE KEY if you are holding it.\n" +
            "An enemy that retrieves a dropped key\n" +
            "will immediately RETURN it to its ORIGINAL LOCATION.\n" +
            "FALLING OFF is an INSTANT LOSS, so be careful of attacks!\n" +
            "You can DEFEND yourself by JUMPING INTO ENEMIES to STUN them,\n" +
            "but only if you are HIGHER THAN THEM at the point of impact.";
        promptText.text = "Press Enter/Return to Begin";
        controlsText.text = "Use W, S, A, D to Move\n" +
            "Spacebar to Jump";
        announcementsText.text = "Super Roll a Ball Deluxe";
        gameOverText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Submit") && !inPlay && gameOver)
        {
            authorshipText.text = "";
            promptText.text = "Press Esc to Start Over";
            gameRulesText.text = "";
            controlsText.text = "";
            TimeControllerPlus.instance.BeginTimer();
            MakeAnnouncement("Begin!", 5);
            inPlay = true;
        }

        if (Input.GetButton("Cancel"))
        {
            inPlay = false;
            gameOver = true;
            timeLimitReached = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (((float)(TimeControllerPlus.instance.ElapsedSeconds()) / (float)timeLimitSeconds) == 0.5)
        {
            MakeAnnouncement("Time limit halfway reached!", 10);
        }

        if (((float)(TimeControllerPlus.instance.ElapsedSeconds()) / (float)timeLimitSeconds) == 0.75)
        {
            MakeAnnouncement("Time is almost up!", 10);
        }

        if ((timeLimitSeconds - TimeControllerPlus.instance.ElapsedSeconds()) == 10)
        {
            MakeAnnouncement("TEN SECONDS LEFT!", 2);
        }

        if ((timeLimitSeconds - TimeControllerPlus.instance.ElapsedSeconds()) == 5)
        {
            MakeAnnouncement("FIVE SECONDS LEFT!", 2);
        }

        if (TimeControllerPlus.instance.Minutes() >= timeLimitInMinutes)
        {
            inPlay = false;
            gameOver = true;
            timeLimitReached = true;
            gameOverText.text = "Time is up!\n" +
                "GAME OVER\n" +
                "Retry?\n" +
                "(Press ESC)";
            MakeAnnouncement("You have been defated.", 0);
        }

        if (protagonistFell)
        {
            inPlay = false;
            gameOver = true;
            gameOverText.text = "You fell!\n" +
                "GAME OVER\n" +
                "Retry?\n" +
                "(Press ESC)";
            MakeAnnouncement("Be careful next time...", 0);
        }

        if (protagonistWins)
        {
            inPlay = false;
            gameOver = true;
            gameOverText.text = "You are a true\n" +
                "WINNER\n" +
                "Roll again?\n" +
                "(Press ESC)";
            MakeAnnouncement("Victory is yours!", 0);
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
                StartCoroutine(nameof(KillAnnouncement), killTime);
            }
        }
        else
        {
            KillAnnouncementNow(announcement, killTime);
        }
    }

    public void ProtagonistFell()
    {
        protagonistFell = true;
    }

    public void ProtagonistWins()
    {
        protagonistWins = true;
    }

    private void KillAnnouncementNow(string announcement, int killTime)
    {
        StopCoroutine(nameof(KillAnnouncement));
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
