using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;
    private MazeDataGenerator dataGenerator;

    public GameObject wall;
    public GameObject floor;
    public GameObject enemy;

    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;
    [SerializeField] private Material healthMat;

    public int[,] data
    {
        get; private set;
    }

    public float hallWidth
    {
        get; private set;
    }
    public float hallHeight
    {
        get; private set;
    }

    public int startRow
    {
        get; private set;
    }
    public int startCol
    {
        get; private set;
    }

    public int goalRow
    {
        get; private set;
    }
    public int goalCol
    {
        get; private set;
    }
    public int healthRow
    {
        get; private set;
    }
    public int healthCol
    {
        get; private set;
    }

    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        // default to walls surrounding a single empty cell
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols,
        TriggerEventHandler startCallback = null, TriggerEventHandler goalCallback = null, TriggerEventHandler healthCallback = null)
    {
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogError("Odd numbers work better for dungeon size.");
        }

        DisposeOldMaze();

        data = dataGenerator.FromDimensions(sizeRows, sizeCols);

        FindStartPosition();
        FindGoalPosition();
        FindHealthPosition();

        // store values used to generate this mesh
        hallWidth = 1;
        hallHeight = 1;

        DisplayMaze();

        PlaceStartTrigger(startCallback);
        PlaceGoalTrigger(goalCallback);
        PlaceHealthTrigger(healthCallback);

        FindAndPlaceEnemies();
    }

    void OnGUI()
    {
        if (!showDebug)
        {
            return;
        }

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0); // 0 for # of rows
        int cMax = maze.GetUpperBound(1); // 1 for # of columns

        string msg = "";

        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += "....";
                }
                else
                {
                    msg += "==";
                }
            }
            msg += "\n";
        }

        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

    private void DisplayMaze()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] != 0)
                {
                    GameObject go = Instantiate(wall);
                    go.transform.position = Vector3.zero;
                    go.name = "Procedural Maze";
                    go.tag = "Generated";
                    go.transform.position = new Vector3(i, 0, j);
                }
            }
        }
    }

    public void DisposeOldMaze()
    {
        //GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        List<GameObject> objects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Generated"));
        objects.Add(GameObject.FindGameObjectWithTag("Treasure"));
        objects.Add(GameObject.FindGameObjectWithTag("Health"));

        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
    }

    private void FindStartPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    startRow = i;
                    startCol = j;
                    return;
                }
            }
        }
    }

    private void FindGoalPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        // loop top to bottom, right to left
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = cMax; j >= 0; j--)
            {
                if (maze[i, j] == 0)
                {
                    goalRow = j;
                    goalCol = i;
                    return;
                }
            }
        }
    }

    private void FindHealthPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        // loop bottom to top, right to left
        for (int i = rMax; i > 0; i--)
        {
            for (int j = 0; j < cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    healthRow = j;
                    healthCol = i;
                    return;
                }
            }
        }
    }

    private void FindAndPlaceEnemies()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for(int j = 0; j <= cMax; j++)
        {
            if (maze[Mathf.RoundToInt(rMax/2), j] == 0)
            {
                //Debug.Log("Enemy row: " + Mathf.RoundToInt(rMax / 2) + ", column: " + j);
                Instantiate(enemy, new Vector3(Mathf.RoundToInt(rMax / 2), 0, j), Quaternion.identity);
            }
        }
    }

    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(startCol * hallWidth, -0.5f, startRow * hallWidth);
        go.name = "Start Trigger";
        go.tag = "Generated";
        go.transform.localScale = new Vector3(1f, 0.5f, 1f);

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    private void PlaceGoalTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(goalCol * hallWidth, -0.5f, goalRow * hallWidth);
        go.name = "Treasure";
        go.tag = "Treasure";
        go.transform.localScale = new Vector3(1f, 0.5f, 1f);

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    private void PlaceHealthTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(healthCol * hallWidth, -0.5f, healthRow * hallWidth);
        go.name = "Health";
        go.tag = "Health";
        go.transform.localScale = new Vector3(1f, 0.5f, 1f);

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = healthMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }
}

