using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] private Player player;
    [SerializeField] private Text timeLabel;
    [SerializeField] private Text scoreLabel;
    [SerializeField] private Text healthLabel;
    [SerializeField] private Text spinLabel;
    [SerializeField] private Text chargeLabel;
    [SerializeField] private Text throwLabel;

    private MazeConstructor generator;
    private DateTime startTime;
    private int timeLimit;
    private int score;
    private bool goalReached;
    private int sizeRows;
    private int sizeColms;

    public Button startBtn;
    public GameObject titleLabel;
    public Button retryBtn;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        generator = GetComponent<MazeConstructor>();
        sizeRows = 13;
        sizeColms = 15;
    }

    public void StartNewGame()
    {
        timeLimit = 90;
        startTime = DateTime.Now;

        score = 0;
        scoreLabel.text = "SCORE: " + score;
        startBtn.gameObject.SetActive(false);
        titleLabel.SetActive(false);

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
            timeLabel.text = "TIME: " + timeLeft;
        }
        if (!Player.Instance.gameOn)
        {
            timeLabel.text = "TIME UP";
            GameOver();
        }     
    }

    private void OnGoalTrigger(GameObject trigger, GameObject other)
    {
        //Debug.Log("Goal!");
        goalReached = true;

        score += 1;
        scoreLabel.text = "SCORE: " + score;

        Destroy(trigger);
        Destroy(GameObject.FindGameObjectWithTag("Health"));

        if (score == 4)
        {
            Player.Instance.spinUnlocked = true;
            spinLabel.text = "SPIN: Z";
        }

        if (score == 8)
        {
            Player.Instance.chargeUnlocked = true;
            chargeLabel.text = "CHARGE: C";
        }

        if (score == 12)
        {
            Player.Instance.throwUnlocked = true;
            throwLabel.text = "THROW: X";
        }
    }

    private void OnStartTrigger(GameObject trigger, GameObject other)
    {
        if (goalReached)
        {
            //Debug.Log("Finish!");
            player.enabled = false;
            sizeRows += 2;
            sizeColms += 2;

            Invoke("StartNewMaze", 2);
        }
    }

    private void OnHealthTrigger(GameObject trigger, GameObject other)
    {
        //Debug.Log("Picked Up health");
        goalReached = true;

        Player.Instance.playerHealth++;
        UpdateHealthLabel();

        Destroy(trigger);
        Destroy(GameObject.FindGameObjectWithTag("Treasure"));
    }

    public void UpdateHealthLabel()
    {
        healthLabel.text = "HEALTH: " + Player.Instance.playerHealth;
    }

    public void GameOver()
    {
        player.enabled = false;
        retryBtn.gameObject.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene("Scene");
    }
}
