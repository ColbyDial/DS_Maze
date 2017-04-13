using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour {

    public GameObject ItemToSpawn;
    private Cell peek;
    private Cell pop;
    public int worldWidthCells = 10;        //x cells desired
    public int worldHeightCells = 10;       //y cells desired
    private int worldWidth;                 //physical size of the maze width
    private int worldHeight;                //physical size of the maze height
    private int totalCells;                 //total number of cells in grid
    public float spawnSpeed = .5f;          //how long we waiat till we spawn the next cell
    private GameObject temp;                
    public int cellSize = 1;                //physical size of the cell, used to calculate physical size of grid
    private int xCellRef = -1;              //cell reference for locating where a cell is (x location, z location)
    private int zCellRef = -1;              // ^^^^^
    private Vector3 newV;
    private bool gridGenerated1 = false;     //is the grid still generating itself
    private bool mazeGenerating1 = false;    //is the maze still generating itself
    private bool mazeGenerated1 = false;     //has the maze been completely generated
    private bool pathways = true;           // are there any cells that we can go to from this cell?
    private int numCellsVisited = 0;
    public bool startEndCellsSet = false;
    private CellCords _coords;
    public CellCords _startCellCoords;
    public CellCords _endCellCoords;
    public bool allDone = false;
    public bool foundEnd = false;


    private Stack<Cell> stack = new Stack<Cell>();

    public List<Cell> cellList = new List<Cell>();
 

   public class Cell
    {
        // Attributes----------------------------------------------
        public GameObject wall;
        public GameObject floor;

        public int xCellRef;
        public int zCellRef;

        public bool cellVisited = false;

        //Manipulators---------------------------------------------
        public void DeactivateWall()
        {
            wall.SetActive(false);
        }

        public void DeactivateFloor()
        {
            floor.SetActive(false);
        }

        //Setters--------------------------------------------------
        public void setWall(GameObject newWall)
        {
            this.wall = newWall;
        }

        public void setFloor(GameObject newFloor)
        {
            this.floor = newFloor;
        }

        public void setXCellRef(int cellRef)
        {
            this.xCellRef = cellRef;
        }

        public void setZCellRef(int cellRef)
        {
            this.zCellRef = cellRef;
        }

        public void setCellVisited(bool visit)
        {
            this.cellVisited = visit;
        }

        //Getters-----------------------------------------------------
        public GameObject getWall()
        {
            return this.wall;
        }

        public GameObject getFloor()
        {
            return this.floor;
        }

        public int getXCellRef()
        {
            return this.xCellRef;
        }

        public int getZCellRef()
        {
            return this.zCellRef;
        }

        public bool getCellVisited()
        {
            return this.cellVisited;
        }
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame

    void Update()
    {
        if (!mazeGenerated1)
        {
            if (gridGenerated1 && !mazeGenerating1)
            {
                Debug.Log("Time to start destroying cell walls");
                StartCoroutine(GenerateMaze());
                mazeGenerating1 = true;
            }
        }
    }

    public void startGeneratingGrid()
    {
        worldWidth = 1 + (worldWidthCells * cellSize);
        worldHeight = 1 + (worldHeightCells * cellSize);
        StartCoroutine(GenerateGrid());
    }

    //Generates the grid that we will "carve" the maze out of
    public IEnumerator GenerateGrid()
    {
        for (int x = 0; x < this.worldWidth; x += this.cellSize)
        {
            xCellRef++;
            zCellRef = -1;

            for (int z = 0; z < worldHeight; z += cellSize)
            {
                if((z%(10 * cellSize) == 0))
                {
                   yield return new WaitForSeconds(spawnSpeed);
                }
                zCellRef++;

                var block = Instantiate(ItemToSpawn);
                block.transform.position = new Vector3(this.transform.position.x + x, this.transform.position.y,
                    this.transform.position.z + z);


                Cell tempCell = new Cell();
                temp = block.gameObject.transform.GetChild(0).gameObject;
                tempCell.setWall(temp);

                temp = block.gameObject.transform.GetChild(1).gameObject;
                tempCell.setFloor(temp);

                //set the cell reference for the grid location
                tempCell.setXCellRef(xCellRef);
                tempCell.setZCellRef(zCellRef);

                cellList.Add(tempCell);

             //   Debug.Log("spawned a cell");


                //Is this the last cell to be generated?
                newV = getWallPosition(this.worldWidthCells, this.worldHeightCells);

                //if so then declare that the whole grid has been generated
                if (newV != new Vector3(0, 0, 0))
                {
                    Debug.Log(" The Grid has been generated!");
                    this.gridGenerated1 = true;
                }

            }
        }
    }

    //------------------------------------------------------------------------------------------------
    //Generates the maze by checking surrounding cells and destroying respective walls
    //------------------------------------------------------------------------------------------------

    //this method takes the startCellCoords attribute and starts generating the maze at that location
    public IEnumerator GenerateMaze()
    {
        yield return new WaitForSeconds(spawnSpeed);

        int x = _startCellCoords.x;
        int z = _startCellCoords.z;


        Debug.Log("The Max Cells are " + worldWidthCells + " " + worldHeightCells);
        Debug.Log("The Start cell is (" + x + ", " + z + ")");

        pushCellOnStack(x, z);

        //make sure that we have succesfully put the cell on top of the stack
        peek = stack.Peek();
       // Debug.Log("The cell on top of the stack is " + (peek.getXCellRef()) + ", " + (peek.getZCellRef()));

        //while there is a cell not visited from current cell, continue
        //this needs to become a function called create arm or something
        Debug.Log("Starting to generate pathways");
        while (!mazeGenerated1)          //while the maze is not completed
        {
            while (pathways)            //while there are still pathways available from current cell
            {
                yield return new WaitForSeconds(spawnSpeed);
                if (stack.Count > 0)    //as long as there is still something on the stack
                {
                    peek = stack.Peek(); //look sat the cell that is on top of the stack
                   // Debug.Log("The cell on top of the stack is " + (peek.getXCellRef()) + ", " + (peek.getZCellRef()));
                   
                    //check the surrounding cells of the cell on top of the stack
                    checkSurroundingCells(peek.getXCellRef(), peek.getZCellRef()); 
                }
                if (numCellsVisited >= ((worldHeightCells * worldWidthCells)))
                {
                    mazeGenerated1 = true;
                }

            }
            
            //while there are no pathways from the current cell, but we have not visited
            //all the existing cells
            while (!pathways && !mazeGenerated1)
            {
                //and as long as there is still something on the stack
                
                if (stack.Count > 1)
                {
                    //then pop a cell of the stack, and see if there are any cells we can go to from here
                    pop = stack.Pop();
                    if (stack.Count == 1)
                    {
                        mazeGenerated1 = true;
                        Debug.Log("mazeGenerated1 has been set to true");
                    }
                  //  Debug.Log("Reverse: The cell on top of the stack is " + (pop.getXCellRef()) + ", " + (pop.getZCellRef()) + " Is maze genreated? " + (mazeGenerated1.ToString()) + "Stack count: " + (stack.Count.ToString()));
                    //Debug.Log("Stack count: " + stack.Count.ToString());
                    
                    checkSurroundingCells(pop.getXCellRef(), pop.getZCellRef());
                }
              
                yield return new WaitForSeconds(spawnSpeed);
            }

        }
    }

    //this method creates its own coordinates to start generating from
    public IEnumerator GenerateMazeRandomStart()
    {
        mazeGenerating1 = true;
        yield return new WaitForSeconds(spawnSpeed);

        //select a random place in the grid to start destroying walls
        //between 1 and the max row/col of cells
        int x = Random.Range(1, worldWidthCells);
        int z = Random.Range(1, worldHeightCells);

        //This bit of code makes sure that the wall of the maze is only 1 cell thick
        //by ensuring that the starting cell will maximaze the total amount of 
        //space available to it
        if ((x % 2) == 0)//if its even, make it odd
        {
            x--;
            if (x < 0)  //and if its now below 0, put it back on the map
            {
                x += 2;
            }
        }

        if ((z % 2) == 0)//if its even, make it odd
        {
            z--;
            if (z < 0)
            {
                z += 2; //and if its now below 0, put it back on the map
            }
        }

        Debug.Log("The Max Cells are " + worldWidthCells + " " + worldHeightCells);
        Debug.Log("The Randomly selected cell was (" + x + ", " + z + ")");

        pushCellOnStack(x, z);

        //make sure that we have succesfully put the cell on top of the stack
        peek = stack.Peek();
        Debug.Log("The cell on top of the stack is " + (peek.getXCellRef()) + ", " + (peek.getZCellRef()));

        //while there is a cell not visited from current cell, continue
        //this needs to become a function called create arm or something
        Debug.Log("Starting to generate pathways");
        while (!mazeGenerated1)          //while the maze is not completed
        {
            while (pathways)            //while there are still pathways available from current cell
            {
                yield return new WaitForSeconds(spawnSpeed);
                if (stack.Count > 0)    //as long as there is still something on the stack
                {
                    peek = stack.Peek(); //look sat the cell that is on top of the stack
                    Debug.Log("The cell on top of the stack is " + (peek.getXCellRef()) + ", " + (peek.getZCellRef()));

                    //check the surrounding cells of the cell on top of the stack
                    checkSurroundingCells(peek.getXCellRef(), peek.getZCellRef());
                }
                if (numCellsVisited >= ((worldHeightCells * worldWidthCells)))
                {
                    mazeGenerated1 = true;
                }

            }

            //while there are no pathways from the current cell, but we have not visited
            //all the existing cells
            while (!pathways && !mazeGenerated1)
            {
                //and as long as there is still something on the stack

                if (stack.Count > 1)
                {
                    //then pop a cell of the stack, and see if there are any cells we can go to from here
                    pop = stack.Pop();
                    if (stack.Count == 1)
                    {
                        mazeGenerated1 = true;
                        mazeGenerating1 = false;
                        Debug.Log("mazeGenerated1 has been set to true");
                    }
                    Debug.Log("Reverse: The cell on top of the stack is " + (pop.getXCellRef()) + ", " + (pop.getZCellRef()) + " Is maze genreated? " + (mazeGenerated1.ToString()) + "Stack count: " + (stack.Count.ToString()));
                    Debug.Log("Stack count: " + stack.Count.ToString());

                    checkSurroundingCells(pop.getXCellRef(), pop.getZCellRef());
                }

                yield return new WaitForSeconds(spawnSpeed);
            }

        }
    }


    //------------------------------------------------------------------------------------------------
    // Functions that search the list for a specific attribute of a Cell()
    //------------------------------------------------------------------------------------------------

    public void checkSurroundingCells(int xRef, int zRef)
    {
        bool visited = true;
        bool found = false;
        int max = 4;
        int count = 0;
        int temp;
        int direction = 0; // an int that represents which direction from the cell we will go
        int[] numbers = new int[4];
        int x;

        //This section randomly generates an order of 0 - 3--------------------------
        //this for loop populates the array in order i.e. 0, 1, 2, 3, ....
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
        //sanity check, printing the current order of the array to console
        //should display the mow shuffled array
        //for (int r = 0; r < max; r++)
        //{
        //    Debug.Log(numbers[r]);
        //}

        //are there pathways left for us to go to from here?
       // Debug.Log("Pathways = " + pathways);

        //we will check to see if there are any places to go from here
        for (int i = 0; i < max; i++)
        {
            //if we have not already located the next cell available
            if (!found)
            {
                //how many of the surrounding cells have we already checked
               // Debug.Log("checked " + i + " Surrounding cells.");
                
                direction = numbers[i]; //look at the shuffled array and get a random direction

                //if it is the 4th time that we are checking for an open cell, then we know
                //that all the surrounding cells have already been visited, so there
                //are no more pathways open
                if (i == 3)
                {
                    pathways = false;
                  //  Debug.Log("There are not pathways left to visit. pathways set to false.");
                }

                //what direction are we checking for?
                switch (direction)
                {

                    //Case 1 is the cell north of the current cell
                    case 0:
                      //  Debug.Log("Checking case 0");
                        //visited = hasCellBeenVisited(xRef, (zRef + 1));
                        visited = hasCellBeenVisited(xRef, (zRef + 2));

                        if (!visited)
                        {
                            pathways = true;
                            found = true;
                            numCellsVisited+=2;
                         //   Debug.Log("Cell " + xRef + ", " + (zRef + 1) + "Has now been visited");
                            //     destroyCellWall(xRef, zRef, "North");
                            destroyCellWall(xRef, (zRef + 1));
                            setCellVisit(xRef, (zRef + 1), true);
                            //--- this is new ---
                            destroyCellWall(xRef, (zRef + 2));
                            setCellVisit(xRef, (zRef + 2), true);
                            pushCellOnStack(xRef, (zRef + 2));
                            //pushCellOnStack(xRef, (zRef + 1));
                          
                        }
                        break;

                    //case 2 is the cell south of the current cell
                    case 1:
                       // Debug.Log("Checking case 1");

                        //visited = hasCellBeenVisited(xRef, (zRef - 1));
                        visited = hasCellBeenVisited(xRef, (zRef - 2));

                        if (!visited)
                        {
                            pathways = true;
                            found = true;

                            numCellsVisited+=2;
                         //   Debug.Log("Cell " + xRef + ", " + (zRef - 1) + "Has now been visited");
                            //     destroyCellWall(xRef, (zRef - 1), "North");
                            destroyCellWall(xRef, (zRef - 1));
                            setCellVisit(xRef, (zRef - 1), true);
                            //--- this is new ---
                            destroyCellWall(xRef, (zRef - 2));
                            setCellVisit(xRef, (zRef - 2), true);
                            pushCellOnStack(xRef, (zRef - 2));
                            //pushCellOnStack(xRef, (zRef - 1));
                        }
                        break;

                    //case 3 is the cell east of the current cell
                    case 2:
                       // Debug.Log("Checking case 2");

                        //visited = hasCellBeenVisited((xRef + 1), zRef);
                        visited = hasCellBeenVisited((xRef + 2), zRef);

                        if (!visited)
                        {
                            pathways = true;
                            found = true;

                            numCellsVisited+=2;
                          //  Debug.Log("Cell " + (xRef + 1) + ", " + zRef + "Has now been visited");
                            //   destroyCellWall(xRef, zRef, "East");
                            destroyCellWall((xRef + 1), zRef);
                            setCellVisit((xRef + 1), zRef, true);
                            // -- this is new ---
                            destroyCellWall((xRef + 2), zRef);
                            setCellVisit((xRef + 2), zRef, true);
                            pushCellOnStack((xRef + 2), zRef);
                            //pushCellOnStack((xRef + 1), zRef);
                        }
                        break;

                    //case 4 is teh cell west of teh current cell
                    case 3:
                       // Debug.Log("Checking case 3");

                        //visited = hasCellBeenVisited((xRef - 1), zRef);
                        visited = hasCellBeenVisited((xRef - 2), zRef);

                        if (!visited)
                        {
                            pathways = true;
                            found = true;

                            numCellsVisited+=2;
                          //  Debug.Log("Cell " + (xRef - 1) + ", " + zRef + "Has now been visited");
                            //  destroyCellWall((xRef - 1), zRef, "East");
                            destroyCellWall((xRef - 1), zRef);
                            setCellVisit((xRef - 1), zRef, true);
                            // -- this is new ---
                            destroyCellWall((xRef - 2), zRef);
                            setCellVisit((xRef - 2), zRef, true);
                            pushCellOnStack((xRef - 2), zRef);
                            // pushCellOnStack((xRef - 1), zRef);
                        }
                        break;
                }
            }
        }


    }

    public void pushCellOnStack(int xRef, int zRef)
    {
        int _xCellRef;
        int _zCellRef;
 
        //search the list as long as there are still cells left in the list
        for (int i = 0; i < cellList.Count; i++)
        {
            //grab this cells cell references
            _xCellRef = cellList[i].getXCellRef();
            _zCellRef = cellList[i].getZCellRef();

            //if the cell references match up then we have found our cell
            if (_xCellRef == xRef && _zCellRef == zRef)
            {
                //put this cell on the stack
                stack.Push(cellList[i]);              
            }
        }
    }

    public void destroyCellWall(int xRef, int zRef)
    {
        int _xCellRef;
        int _zCellRef;

        for (int i = 0; i < cellList.Count; i++)
        {
            _xCellRef = cellList[i].getXCellRef();
            _zCellRef = cellList[i].getZCellRef();
            if (_xCellRef == xRef && _zCellRef == zRef)
            {

                cellList[i].DeactivateWall();
            }
        }
    }

    public void destroyCellFloor(int xRef, int zRef)
    {
        int _xCellRef;
        int _zCellRef;

        for (int i = 0; i < cellList.Count; i++)
        {
            _xCellRef = cellList[i].getXCellRef();
            _zCellRef = cellList[i].getZCellRef();
            if (_xCellRef == xRef && _zCellRef == zRef)
            {

                cellList[i].DeactivateFloor();
            }
        }
    }

    public void setCellVisit(int xRef, int zRef, bool visited)
    {
        int _xCellRef;
        int _zCellRef;
        for (int i = 0; i < cellList.Count; i++)
        {
            _xCellRef = cellList[i].getXCellRef();
            _zCellRef = cellList[i].getZCellRef();
            if (_xCellRef == xRef && _zCellRef == zRef)
            {

                cellList[i].setCellVisited(visited);
            }
        }
    }

    public bool hasCellBeenVisited(int xRef, int zRef)
    {
        int _xCellRef;
        int _zCellRef;
        bool visited = false;

        //as long as there is cells left in list, grab the next cells cel references
        for (int i = 0; i < cellList.Count; i++)
        {
            _xCellRef = cellList[i].getXCellRef();
            _zCellRef = cellList[i].getZCellRef();
            //if the cell references match up, we have found the cell we are looking for
            if (_xCellRef == xRef && _zCellRef == zRef)
            {
                //grab that cells visited flag
                visited = cellList[i].getCellVisited();
            }
        }
        //if it is a wall cell, then it has been "visited"
        //(dont want to break out of the walls)


        if (xRef >= worldWidthCells || zRef >= worldHeightCells || xRef <= 0 || zRef <= 0)
        {
            visited = true;
            numCellsVisited++;
        }
        if (xRef == (worldWidthCells + 1) || zRef == (worldHeightCells + 1) || xRef == -1 || zRef == -1)
        {
            visited = true;
            numCellsVisited++;
        }

        return visited;
    }

    public Vector3 getWallPosition(int xRef, int zRef)
    {
        int _xCellRef;
        int _zCellRef;
        GameObject walls;
        Vector3 location;
        location = new Vector3(0, 0, 0);
        for (int i = 0; i < cellList.Count; i++)
        {
            _xCellRef = cellList[i].getXCellRef();
            _zCellRef = cellList[i].getZCellRef();
            if (_xCellRef == xRef && _zCellRef == zRef)
            {
                walls = cellList[i].getWall();
                location = walls.transform.position;
            }
        }
        return location;
    }

    public Vector3 getFloorPosition(int xRef, int zRef)
    {
        int _xCellRef;
        int _zCellRef;
        GameObject walls;
        Vector3 location;
        location = new Vector3(0, 0, 0);
        for (int i = 0; i < cellList.Count; i++)
        {
            _xCellRef = cellList[i].getXCellRef();
            _zCellRef = cellList[i].getZCellRef();
            if (_xCellRef == xRef && _zCellRef == zRef)
            {
               
                walls = cellList[i].getFloor();
                location = walls.transform.position;
            }
        }
        return location;
    }

    public void setCellFloor(int xRef, int zRef, GameObject newFloor)
    {
        int _xCellRef;
        int _zCellRef;
        GameObject walls;
        Vector3 location;
        location = new Vector3(0, 0, 0);
        for (int i = 0; i < cellList.Count; i++)
        {
            _xCellRef = cellList[i].getXCellRef();
            _zCellRef = cellList[i].getZCellRef();
            if (_xCellRef == xRef && _zCellRef == zRef)
            {

                cellList[i].setFloor(newFloor);
            }
        }
    }

    public List<CellCords> getAllVisitedCells()
    {
        Debug.Log("List of all open cells");
        List<CellCords> visitedCells = new List<CellCords>();
        foreach(Cell cell in cellList)
        {
            CellCords cellCoords = new CellCords();
            if (cell.cellVisited)
            {
                cellCoords.x = cell.xCellRef;
                cellCoords.z = cell.zCellRef;
                //this may seem counterIntuitive, but this represents whether it has
                //been visited by the maze runner yet
                cellCoords.visited = false; 
                visitedCells.Add(cellCoords);
                Debug.Log("(" + cellCoords.x + ", " + cellCoords.z + ")");
            }
        }
        Debug.Log("EOF List of Open Cells");
        return visitedCells;
    }

    //------------------------------------------------------------------------------------------------
    // Functions that return an attribute of the GridMaker
    //------------------------------------------------------------------------------------------------

    public int getWorldWidthCells()
    {
        return worldWidthCells;
    }

    public int getWorldHeightCells()
    {
        int cellHeight = worldHeightCells;
        return cellHeight;
    }

    public float GetWidth()
    {
        int width = worldWidth - cellSize;
        return width;
    }

    public float GetHeight()
    {
        int height = worldHeight - cellSize;
        return height;
    }

    public float GetOffset()
    {
        return cellSize;
    }

    public List<Cell> getCellList()
    {
        return this.cellList;
    }

    public bool IsMazeGenerated()
    {
        return this.mazeGenerated1;
    }

    public bool isMazeGenerating()
    {
        return this.mazeGenerating1;
    }

    public bool isGridGenerated()
    {
        return this.gridGenerated1;
    }

    public void spawnObjectAtCell(CellCords _coords, GameObject itemToSpawn, string position)
    {
        if(position == "floor")
        {
        Vector3 spawnSpot = getFloorPosition(_coords.x, _coords.z);
            var block = Instantiate(itemToSpawn);
            block.transform.position = new Vector3(((spawnSpot.x * cellSize) / 2), spawnSpot.y,
               ((spawnSpot.z * cellSize) / 2));
            GetCellWorldLocation(_coords);
        }
        if (position == "wall")
        {
            Vector3 spawnSpot = getWallPosition(_coords.x, _coords.z);
            var block = Instantiate(itemToSpawn);
            block.transform.position = new Vector3(((spawnSpot.x * cellSize) / 2), (spawnSpot.y - .5f),
               ((spawnSpot.z * cellSize) / 2));
            GetCellWorldLocation(_coords);

        }

    }

    public void spawnMazeRunnerAtCell(CellCords _coords, GameObject itemToSpawn)
    {
        Vector3 spawnSpot = getWallPosition(_coords.x, _coords.z);
        var block = Instantiate(itemToSpawn);
        block.transform.position = new Vector3(((spawnSpot.x * cellSize) / 2), (spawnSpot.y + .5f),
           ((spawnSpot.z * cellSize) / 2));
        GetCellWorldLocation(_coords);
    }

    public Vector3 GetCellWorldLocation(CellCords _coords)
    {
        Vector3 spawnSpot = getWallPosition(_coords.x, _coords.z);
        Vector3 worldLocation = new Vector3(((spawnSpot.x * cellSize) / 2), (spawnSpot.y - .5f),
            ((spawnSpot.z * cellSize) / 2));

        Debug.Log("The Location of the cell is " + worldLocation.ToString());
        return worldLocation;
    }
}
