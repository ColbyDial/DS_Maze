using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mazeRunner : MonoBehaviour {
    public float runSpeed = 2f;
    public GameObject mazeRunnerModel;
    public GameObject mapGenerator;
    public GameObject camera;

    private SpawnGridSpawner _sgs = new SpawnGridSpawner();
    private CellCords currentCell = new CellCords();
    private CellCords peek = new CellCords();
    private CellCords pop = new CellCords();
    private bool endFound = false;
    private bool pathways = true;

    private bool allMazesRan = false;
    private int thisMaze = 0;

    private Stack<CellCords> stack = new Stack<CellCords>();

    private List<CellCords> mazeRunnerCellList = new List<CellCords>();

    
    // Use this for initialization
    void Start () {
        camera = GameObject.Find("Main Camera");
        mapGenerator = GameObject.Find("MapGenerator");
        _sgs = mapGenerator.GetComponent<SpawnGridSpawner>();
        _sgs._createGrid[0].spawnMazeRunnerAtCell(_sgs._createGrid[0]._startCellCoords, mazeRunnerModel);
        mazeRunnerModel = GameObject.Find("fbKnight(Clone)");

        mazeRunnerCellList = _sgs._createGrid[0].getAllVisitedCells();
        StartCoroutine(RunMaze(_sgs._createGrid[0]));
        camera.gameObject.transform.position = new Vector3(12, 5, 12);
    }

    // Update is called once per frame
    void Update ()
    {
        //while running all mazes have not been completed
        if (!allMazesRan)
        {
            //if we have completed running the current maze
            if (_sgs._createGrid[thisMaze].foundEnd)
            {
                Debug.Log("Finished runing maze " + thisMaze + " Time to start running the next one!");
                thisMaze++;//look to the next maze to run
                if(thisMaze == _sgs.mazesToSpawn)//if there is not a next maze to run
                {
                    Debug.Log("We ran all the mazes!");
                    allMazesRan = true; // then we have run all of the mazes
                }
                else//if there is a next maze to run
                {
                    Debug.Log("Gonna start the next maze now!");
                    endFound = false;
                    mazeRunnerCellList = _sgs._createGrid[thisMaze].getAllVisitedCells();
                    StartCoroutine(RunMaze(_sgs._createGrid[thisMaze]));//run that maze!
                    camera.gameObject.transform.position = camera.gameObject.transform.position - new Vector3(0, 25, 0);

                }
            }
        }

    }

    public IEnumerator RunMaze(CreateGrid currentMaze)
    {
        yield return new WaitForSeconds(runSpeed);

        currentCell = currentMaze._startCellCoords;
        currentCell.visited = true;
        setCellVisit(currentCell, true);

        Debug.Log("The Max Cells are " + currentMaze.worldWidthCells + " " + currentMaze.worldHeightCells);
        Debug.Log("The Start cell is (" + currentCell.x + ", " + currentCell.z + ")");
        Debug.Log("The End cell is (" + currentMaze._endCellCoords.x + ", " + currentMaze._endCellCoords.z + ")");

        stack.Push(currentCell);


        //while there is a cell not visited from current cell, continue
        //this needs to become a function called create arm or something
        Debug.Log("Starting to explore pathways");
        while (!endFound)          //while the end is not completed
        {
            Debug.Log("End not found");
            while (pathways && !endFound)            //while there are still pathways available from current cell
            {
                Debug.Log("Moving forward");
                yield return new WaitForSeconds(runSpeed);
                if (stack.Count > 0)    //as long as there is still something on the stack
                {
                    peek = stack.Peek(); //look sat the cell that is on top of the stack
                    currentCell = peek; // rferences
                    Debug.Log("current cell: " + currentCell.x + " " + currentCell.z);

                    // if ((currentCell.x == (currentMaze._endCellCoords.x - 6)) && (currentCell.z == (currentMaze._endCellCoords.z - 6)))
                    if ((currentCell.x == (currentMaze._endCellCoords.x )) && (currentCell.z == (currentMaze._endCellCoords.z)))
                    {
                        Debug.Log("End has been found!");
                        endFound = true;
                        currentMaze.foundEnd = true;
                    }

                    if (!endFound)
                    {
                        //check the surrounding cells of the cell on top of the stack
                        checkSurroundingCells(currentCell, currentMaze);
                    }

                }

                //if ((currentCell.x == (currentMaze._endCellCoords.x - 6)) && (currentCell.z == (currentMaze._endCellCoords.z - 6)))
                if ((currentCell.x == (currentMaze._endCellCoords.x)) && (currentCell.z == (currentMaze._endCellCoords.z)))
                {
                    Debug.Log("End has been found!");
                    endFound = true;
                }
            }

            //while there are no pathways from the current cell, but we have not visited
            //all the existing cells
            while (!pathways && !endFound)
            {
                //and as long as there is still something on the stack

                if (stack.Count > 0)
                {
                   
                    Debug.Log("Backtracking!");
                    //then pop a cell of the stack, and see if there are any cells we can go to from here
                    pop = stack.Pop();
                    Debug.Log("first pop: " + pop.x + ", " + pop.z);


                    //which direction should I move the mazeRunner
                    int xDif, zDif = 0;
                    Debug.Log("(" + currentCell.x + ", " + currentCell.z + ")  (" + pop.x + ", " + pop.z + ")"); 
                    xDif = currentCell.x - pop.x;
                    zDif = currentCell.z - pop.z;
                    Debug.Log("Just checked the difference in cell ref");
                    Debug.Log("The difference is; x: " + xDif + " z: " + zDif);
                    if(xDif == 0 && zDif == -1)
                    {//north
                        mazeRunnerModel.transform.position += new Vector3(0, 0, currentMaze.cellSize);
                        yield return new WaitForSeconds(runSpeed);

                    }
                    if (xDif == 0 && zDif == 1)
                    {//south
                        mazeRunnerModel.transform.position -= new Vector3(0, 0, currentMaze.cellSize);
                        yield return new WaitForSeconds(runSpeed);

                    }
                    if (xDif == -1 && zDif == 0)
                    {//east
                        mazeRunnerModel.transform.position += new Vector3(currentMaze.cellSize, 0, 0);
                        yield return new WaitForSeconds(runSpeed);

                    }
                    if (xDif == 1 && zDif == 0)
                    {//west
                        mazeRunnerModel.transform.position -= new Vector3(currentMaze.cellSize, 0, 0);
                        yield return new WaitForSeconds(runSpeed);

                    }

                    currentCell = pop;
                    checkSurroundingCells(pop, currentMaze);
                    Debug.Log("current cell: " + currentCell.x + " " + currentCell.z);
                
                }

               // yield return new WaitForSeconds(runSpeed);
            }

        }

        Debug.Log("While loop exited");
    }

    public void checkSurroundingCells(CellCords cell, CreateGrid currentMaze)
    {
        CellCords cellToPush = new CellCords();
        cellToPush = cell;  //This is a reference
        bool found = false;

        int direction = 0; // an int that represents which direction from the cell we will go
        int[] numbers = shuffleArray(4);
      

        //we will check to see if there are any places to go from here
        for (int i = 0; i < 4; i++)
        {
            //if we have not already located the next cell available
            if (!found)
            {
                //if it is the 4th time that we are checking for an open cell, then we know
                //that all the surrounding cells have already been visited, so there
                //are no more pathways open
                if (i == 3)
                {
                    pathways = false;
                }

                direction = numbers[i]; //look at the shuffled array and get a random direction

                //what direction are we checking for?
                switch (direction)
                {

                    //Case 0 is the cell north of the current cell
                    case 0:
                        found = attemptToOccupyNorthCell(cell, currentMaze);
                        break;

                    //case 1 is the cell south of the current cell
                    case 1:
                        found = attemptToOccupySouthCell(cell, currentMaze);
                        break;

                    //case 2 is the cell east of the current cell
                    case 2:
                        found = attemptToOccupyEastCell(cell, currentMaze);
                        break;

                    //case 3 is the cell west of the current cell
                    case 3:
                        found = attemptToOccupyWestCell(cell, currentMaze);
                        break;
                }
            }
        }
    }

    public void setCellVisit(CellCords cell, bool visited)
    {
        int _xCellRef;
        int _zCellRef;
        for (int i = 0; i < mazeRunnerCellList.Count; i++)
        {
            _xCellRef = mazeRunnerCellList[i].getx();
            _zCellRef = mazeRunnerCellList[i].getz();
            if (_xCellRef == cell.getx() && _zCellRef == cell.getz())
            {
                Debug.Log("Set the cell " + cell.getx() + " " + cell.getz() + " as visited" );
                mazeRunnerCellList[i].setVisited(true);
                Debug.Log(mazeRunnerCellList[i].getVisited().ToString());
            }
        }
    }

    private bool hasCellBeenVisitedByRunner(CellCords cell)
    {
        int _xCellRef;
        int _zCellRef;
        bool visited = false;
        for (int i = 0; i < mazeRunnerCellList.Count; i++)
        {
            _xCellRef = mazeRunnerCellList[i].x;
            _zCellRef = mazeRunnerCellList[i].z;
            if (_xCellRef == cell.x && _zCellRef == cell.z)
            {

                visited = mazeRunnerCellList[i].visited;
            }
        }

        return visited;
    }

    private bool hasCellBeenVisitedByRunner(int x, int z)
    {
        int _xCellRef;
        int _zCellRef;
        bool visited = true;
        for (int i = 0; i < mazeRunnerCellList.Count; i++)
        {
            _xCellRef = mazeRunnerCellList[i].getx();
            _zCellRef = mazeRunnerCellList[i].getz();
            if (_xCellRef == x && _zCellRef == z)
            {

                visited = mazeRunnerCellList[i].getVisited();
            }
        }

        return visited;
    }

    public bool isCellInList(CellCords cell)
    {
        int _xCellRef;
        int _zCellRef;
        bool cellInList = false;
        for (int i = 0; i < mazeRunnerCellList.Count; i++)
        {
            _xCellRef = mazeRunnerCellList[i].x;
            _zCellRef = mazeRunnerCellList[i].z;
            if (_xCellRef == cell.x && _zCellRef == cell.z)
            {
                cellInList = true;
            }
        }

        return cellInList;
    }

    public bool isCellInList(int x, int z)
    {
        int _xCellRef;
        int _zCellRef;
        bool cellInList = false;
        for (int i = 0; i < mazeRunnerCellList.Count; i++)
        {
            _xCellRef = mazeRunnerCellList[i].x;
            _zCellRef = mazeRunnerCellList[i].z;
            if (_xCellRef == x && _zCellRef == z)
            {

                cellInList = true;
            }
        }

        return cellInList;
    }

    public int[] shuffleArray(int max)
    {
        int[] numbers = new int[4];
        int count = 0;
        int x;
        int temp;

        for (int i = 0; i < max; i++)
        {
            numbers[i] = i;
        }
        //this for loop shuffles up the order of the array to be randomly unordered
        for (int i = 0; i < max; i++)
        {
            x = Random.Range(count, max);
            temp = numbers[x];
            numbers[x] = numbers[count];
            numbers[count] = temp;
            count++;
        }

        return numbers;
    }

    public bool attemptToOccupyNorthCell(CellCords cell, CreateGrid currentMaze)
    {
        bool found = false;
        bool onList = false;
        bool runnerVisited = false;
        CellCords northCell = new CellCords();
        northCell.x = cell.x;
        northCell.z = cell.z;
        northCell.z += 1; //look at the cell north of here
        onList = isCellInList(northCell);
        runnerVisited = hasCellBeenVisitedByRunner(northCell);

        if (onList && !runnerVisited)
        {
            occupyCell(northCell);
            moveMazeRunner("north", currentMaze);
            found = true;
            pathways = true;
        }

        return found;
    }

    public bool attemptToOccupySouthCell(CellCords cell, CreateGrid currentMaze)
    {
        bool found = false;
        bool onList = false;
        bool runnerVisited = false;
        CellCords southCell = new CellCords();
        southCell.x = cell.x;
        southCell.z = cell.z;
        southCell.z -= 1; //look at the cell south of here
        onList = isCellInList(southCell);
        runnerVisited = hasCellBeenVisitedByRunner(southCell);

        if (onList && !runnerVisited)
        {
            occupyCell(southCell);
            moveMazeRunner("south", currentMaze);
            found = true;
            pathways = true;
        }

        return found;
    }

    public bool attemptToOccupyEastCell(CellCords cell, CreateGrid currentMaze)
    {
        bool found = false;
        bool onList = false;
        bool runnerVisited = false;
        CellCords eastCell = new CellCords();
        eastCell.x = cell.x;
        eastCell.z = cell.z;
        eastCell.x += 1; //look at the cell East of here
        onList = isCellInList(eastCell);
        runnerVisited = hasCellBeenVisitedByRunner(eastCell);

        if (onList && !runnerVisited)
        {
            occupyCell(eastCell);
            moveMazeRunner("east", currentMaze);
            found = true;
            pathways = true;
        }

        return found;
    }

    public bool attemptToOccupyWestCell(CellCords cell, CreateGrid currentMaze)
    {
        bool found = false;
        bool onList = false;
        bool runnerVisited = false;
        CellCords westCell = new CellCords();
        westCell.x = cell.x;
        westCell.z = cell.z;
        westCell.x -= 1; //look at the cell west of here
        onList = isCellInList(westCell);
        runnerVisited = hasCellBeenVisitedByRunner(westCell);

        if (onList && !runnerVisited)
        {
            occupyCell(westCell);
            moveMazeRunner("west", currentMaze);
            found = true;
            pathways = true;
        }

        return found;
    }


    public void occupyCell(CellCords cell)
    {
        setCellVisit(cell, true);
        cell.visited = true;
        stack.Push(cell);
    }

    public void moveMazeRunner(string direction, CreateGrid currentMaze)
    {
        switch (direction)
        {
            case "north":
                mazeRunnerModel.transform.position += new Vector3(0, 0, currentMaze.cellSize);
                break;

            case "south":
                mazeRunnerModel.transform.position -= new Vector3(0, 0, currentMaze.cellSize);
                break;

            case "east":
                mazeRunnerModel.transform.position += new Vector3(currentMaze.cellSize, 0, 0);
                break;

            case "west":
                mazeRunnerModel.transform.position -= new Vector3(currentMaze.cellSize, 0, 0);
                break;
        }
    }
}
