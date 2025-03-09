using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum GameMode
{
    idle,
    playing,
    levelEnd
}
public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S; // a private singleton

    [Header("Inscribed")]
    public TextMeshProUGUI uitLevel; // UIText_Level text
    public TextMeshProUGUI uitShots; // UIText_Shots text
    public Vector3 castlePos; // place to put castles
    public GameObject[] castles; // array of castles

    [Header("Dynamic")]
    public int level; // current level
    public int levelMax; // num of levels
    public int shotsTaken;
    public GameObject castle; // current castle
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot"; // followCam mode

    void Start()
    {
        S = this; // define the singleton 

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel()
    {
        // get rid of old castle if exists
        if(castle != null)
        {
            Destroy(castle);
        }
        // destroy old projectiles if they exist
        Projectile.DESTROY_PROJECTILES();

        // instantiate the new castle
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        // reset the goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;

        // zooms out to show both
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI()
    {
        // show the data in the GUITexts
        uitLevel.text = "Level: "+(level+1)+" of "+levelMax;
        uitShots.text = "Shots Taken: "+shotsTaken;
    }

    void Update()
    {
        UpdateGUI();

        // check for level end
        if((mode == GameMode.playing) && Goal.goalMet)
        {
            // change mode to stop checking for level end
            mode = GameMode.levelEnd;

            // zoom out to show both
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);

            // start the next level in 2 seconds
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel()
    {
        level++;
        if(level == levelMax)
        {
            level = 0;
            shotsTaken = 0;
        }
        StartLevel();
    }

    // static method that allows code anywhere to increment shotsTaken
    static public void SHOT_FIRED()
    {
        S.shotsTaken++;
    }

    // static method that allows code anywhere to get a reference to S.castle
    static public GameObject GET_CASTLE()
    {
        return S.castle;
    }
}
