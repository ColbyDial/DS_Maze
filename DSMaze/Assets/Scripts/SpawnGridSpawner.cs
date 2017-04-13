using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpawnGridSpawner : MonoBehaviour {
    public int mazesToSpawn = 1;
    public GameObject _gridSpawner;
    public GameObject _itemToSpawn;
    public GameObject mazeRunner;
    public GameObject startFinishManager;
    public CreateGrid[] _createGrid = new CreateGrid[5];
    public GameObject[] maze = new GameObject[5];
    public bool allMazesGenerated = false;
    public bool allMazesStarted = false;
    private int gateKeeper = 0;
    private CellCords startCell = new CellCords();
    private int currentGrid = 0;
    private int finalPass = 0;

	// Use this for initialization
	void Start () {
        //create and set up the grids for all the mazes
        for (int i = 0; i < mazesToSpawn; i++)
        {
            maze[i] = new GameObject();
            maze[i] = Instantiate(_gridSpawner);
            // maze[i].transform.position = new Vector3((-12 * i), (-25 * i), (-12 * i));
            maze[i].transform.position = new Vector3(0, (-25 * i), 0);
            _createGrid[i] = maze[i].GetComponent<CreateGrid>();
            //   _createGrid[i].worldWidthCells = (12 * (i + 1));
            //    _createGrid[i].worldHeightCells = (12 * (i + 1));
            _createGrid[i].worldWidthCells = (12);
            _createGrid[i].worldHeightCells = (12);

            _createGrid[i].cellSize = 2;
            _createGrid[i].ItemToSpawn = _itemToSpawn;
        }

        //generate a random place for the first grid to start generating its maze from
        startCell = genRandCellCords(0);

        //set the start position of the maze
        _createGrid[0]._startCellCoords = startCell;

        //call createGrid on the first maze
        _createGrid[0].startGeneratingGrid();
    }

    void Update()
    {
        if (!allMazesGenerated)
        {
            if (!allMazesStarted)
            {
                //I need to look at the current value of current Grid, if it is more than 
                //the number of grids we need to spawn then we need to not do this next section of code

                if (_createGrid[currentGrid].allDone)
                {
                    //set the start cell of the new grid to be the end cell of the last grid
                    startCell = _createGrid[currentGrid]._endCellCoords;     //should be the 0 grid
                   // startCell.x += 6;   //offset since the grids are not lined up along left and bottom edge
                    //startCell.z += 6;   // ^^^^^

                    //go to the next grid
                    if(!((currentGrid + 1) == mazesToSpawn))
                    {
                        currentGrid++;

                        //assign the startCell to the grids startCell
                        _createGrid[currentGrid]._startCellCoords = startCell;        //should be the 1 grid
                        _createGrid[currentGrid].startGeneratingGrid();               //begin!
                    }
                    else
                    {
                        allMazesStarted = true;
                    }        
                }

                //was this maze the last one to be started?
                if (currentGrid >= (mazesToSpawn))
                {
                    allMazesStarted = true;
                }


            }

            if (allMazesStarted)
            {
                if (_createGrid[currentGrid].allDone)
                {
                    allMazesGenerated = true;
                }
            }
        }

        else
        {
            if(finalPass == 0)
            {
                finalPass++;
                Debug.Log("All Mazes are completed! Time to play!");
                //spawn mazeRunner
                var block = Instantiate(mazeRunner);
                block.transform.position = new Vector3(0, 1, 0);
            }
        }
        
    }


    private CellCords genRandCellCords(int i)
    {
        CellCords _randCell = new CellCords();

        //select a random place in the grid to start destroying walls
        //between 1 and the max row/col of cells

        int x = UnityEngine.Random.Range(1, _createGrid[i].worldWidthCells);
        int z = UnityEngine.Random.Range(1, _createGrid[i].worldHeightCells);

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
        Debug.Log("The Random cell is (" + x + ", " + z + ")");

        _randCell.x = x;
        _randCell.z = z;

        return _randCell;
    }


}



