using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Text timeLabel;
    [SerializeField] private Text scoreLabel;

    private MazeConstructor generator;
    private DateTime startTime;
    private int timeLimit;
    private int reduceLimitBy;
    private int score;
    private bool goalReached;
    private int sizeRows;
    private int sizeColms;

    public Button startBtn;

    void Start()
    {
        generator = GetComponent<MazeConstructor>();
        //StartNewGame();
        sizeRows = 13;
        sizeColms = 15;
    }

    public void StartNewGame()
    {
        timeLimit = 80;
        reduceLimitBy = 5;
        startTime = DateTime.Now;

        score = 0;
        scoreLabel.text = score.ToString();
        startBtn.gameObject.SetActive(false);

        StartNewMaze();
    }

    private void StartNewMaze()
    {
        generator.GenerateNewMaze(sizeRows, sizeColms, OnStartTrigger, OnGoalTrigger, OnHealthTrigger);

        float x = generator.startCol * generator.hallWidth;
        float y = player.transform.position.y;
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
            //player.enabled = false;

            //Invoke("StartNewGame", 2);
        }       
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
            sizeRows += 2;
            sizeColms += 2;

            Invoke("StartNewMaze", 2);
        }
    }

    private void OnHealthTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("Picked Up health");
        goalReached = true;

        score += 1;
        scoreLabel.text = score.ToString();

        Destroy(trigger);
    }
}
