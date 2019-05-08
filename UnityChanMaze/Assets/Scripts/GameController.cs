/* 
 * written by Ninoslav Kjireski 05/2019
 * parts of this project were provided by Joseph Hocking 2017 and
 * the Unity-Chan Asset Package 05/2019 from the Unity Asset Store.
 * Written for DTT as an application test
 * released under MIT license (https://opensource.org/licenses/MIT)
 */

using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviour
{
    
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Text timeLabel;
    [SerializeField] private Text scoreLabel;
    [SerializeField] private InputField heightInput;
    [SerializeField] private InputField widthInput;
    [SerializeField] private Button genNewMaze;
    [SerializeField] private Camera orthCam;

    private MazeConstructor generator;

    
    private DateTime startTime;
    private int timeLimit;
    private int reduceLimitBy;

    private int score;
    private bool goalReached;

    private int mazeHeight = 25;
    private int mazeWidth = 25;

    private bool newMaze = false;


    void Start()
    {
        generator = GetComponent<MazeConstructor>();
        StartNewGame();
    }

   
    private void StartNewGame()
    {
        timeLimit = 80;
        reduceLimitBy = 5;
        startTime = DateTime.Now;

        score = 0;
        scoreLabel.text = score.ToString();

        StartNewMaze();
    }

   
    private void StartNewMaze()
    {
        generator.GenerateNewMaze(mazeHeight, mazeWidth, OnStartTrigger, OnGoalTrigger);
        //set camera so the whole maze is visible
        float temp = Mathf.Max(mazeHeight, mazeWidth);
        orthCam.orthographicSize = temp*2;

        if(mazeHeight == mazeWidth)
        {
            orthCam.transform.position = new Vector3(temp*2 - 5, 8, temp*2 - 3);
        }
        else if (mazeHeight > mazeWidth)
        {
            orthCam.transform.position = new Vector3(20, 8, 46);
        }
        else if (mazeHeight < mazeWidth)
        {
            orthCam.transform.position = new Vector3(46, 8, 20);
        }


        //generator.transform.Rotate(-90, 0, 0);
        // player spawn
        float x = generator.startCol * generator.hallWidth;
        float y = 1;
        float z = generator.startRow * generator.hallWidth;
        player.transform.position = new Vector3(x, y, z);

        goalReached = false;
        player.enabled = true;

        // restart timer
        timeLimit -= reduceLimitBy;
        startTime = DateTime.Now;
    }

    void Update()
    {
        genNewMaze.onClick.AddListener(OnClick);
        if (!player.enabled)
        {
            return;
        }

        int timeUsed = (int)(DateTime.Now - startTime).TotalSeconds;
        int timeLeft = timeLimit - timeUsed;

        if (timeLeft > 0)
        {
            timeLabel.text = timeLeft.ToString();
        }
        else
        {
            timeLabel.text = "TIME UP";
            player.enabled = false;

            Invoke("StartNewGame", 4);
        }
    }

    //new maze
    private void OnClick()
    {
        int tempTime = timeLimit;
        mazeHeight = int.Parse(heightInput.text);
        mazeWidth = int.Parse(widthInput.text);
        StartNewMaze();
        timeLimit = tempTime;
    }

    
    private void OnGoalTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("Goal!");
        goalReached = true;

        score += 1;
        scoreLabel.text = score.ToString();

        Destroy(trigger);
    }

    private void OnStartTrigger(GameObject trigger, GameObject other)
    {
        if (goalReached)
        {
            Debug.Log("Finish!");
            player.enabled = false;

            Invoke("StartNewMaze", 4);
        }
    }
}
