/* 
 * written by Ninoslav Kjireski 05/2019
 * parts of this project were provided by Joseph Hocking 2017
 * and the Unity-Chan Asset Package 05/2019 from the Unity Asset Store.
 * Written for DTT as an application test
 * released under MIT license (https://opensource.org/licenses/MIT)
 */

using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    
    public bool showDebug;
    private MazeMeshGenerator meshGenerator;
    private MazeDataGenerator dataGenerator;
    private float objRotaSpeed = 2.5f;        // speed that defines how fast an object spins
    GameObject tempGo;

    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;


    public int[,] data
    {
        get; private set;
    }

    
    void Awake()
    {
        // default to walls surrounding a single empty cell
        meshGenerator = new MazeMeshGenerator();
        dataGenerator = new MazeDataGenerator();
        tempGo = new GameObject();
        data = new int[,]

        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
        //go.transform.Rotate(-90, 0, 0);
        go.name = "Procedural Maze";
        go.tag = "Generated";

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(data);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[2] { mazeMat1, mazeMat2 };
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols,
        TriggerEventHandler startCallback = null, TriggerEventHandler goalCallback = null)
    {
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogError("Odd numbers work better for dungeon size.");
        }

        DisposeOldMaze();

        data = dataGenerator.FromDimensions(sizeRows, sizeCols);

        FindStartPosition();
        FindGoalPosition();

        // store values used to generate this mesh
        hallWidth = meshGenerator.width;
        hallHeight = meshGenerator.height;

        DisplayMaze();

        PlaceStartTrigger(startCallback);
        PlaceGoalTrigger(goalCallback);
    }

    // properties to store sizes and coordinates
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


    // finds all objects with the 'Generated' tag and destroys them
    public void DisposeOldMaze()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
    }

    // Starts at 0,0 and iterates through the maze data until it finds an open space, then sets it as StartPosition
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

    // Same as above but with max values
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
                    goalRow = i;
                    goalCol = j;
                    return;
                }
            }
        }
    }

    // place trigger in the scene at the start
    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Collider goC = go.GetComponent<Collider>();
        GameObject go2 = GameObject.FindGameObjectWithTag("Player");
        Collider go2C = go2.GetComponent<Collider>();

        //sets start block
        go.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth); // new Vector3(startCol * hallWidth, startRow * hallWidth, -1f);
        go.name = "Start Trigger";
        go.tag = "Generated";

        //so that play and collider don't collide
        Physics.IgnoreCollision(goC, go2C);

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    // place trigger in the scene at the end
    private void PlaceGoalTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tempGo = go;

        go.transform.position = new Vector3(goalCol * hallWidth, .5f, goalRow * hallWidth); // new Vector3(goalCol * hallWidth, goalRow * hallWidth, -1f); //
        go.name = "Treasure";
        go.tag = "Generated";

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    private void Update()
    {
        tempGo.transform.Rotate(0, objRotaSpeed, 0);
    }

}
