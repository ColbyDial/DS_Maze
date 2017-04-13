using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateStartFinish : MonoBehaviour {
    public GameObject GridMaker;
    private CreateGrid _grid;
    private int timesThrough = 0;
    public GameObject finishFloor;
    public GameObject startFloor;
    public GameObject StartFinishManager;
    private FromFinishtoStart _ffts;
    CellWorldLocation _cwl = new CellWorldLocation();
   
    //This script should look at the maze to see where it should spawn its start
    //then it can generate its own finish from a visited 

    // Use this for initialization
	void Start () {
        _grid = GridMaker.GetComponent<CreateGrid>();
        StartFinishManager = GameObject.Find("StartFinishManager");
        _cwl = StartFinishManager.GetComponent<CellWorldLocation>();
        _ffts = finishFloor.GetComponent<FromFinishtoStart>();
     
	}
	
	// Update is called once per frame
	void Update () {

        if (_grid.IsMazeGenerated() && timesThrough < 1)
        {
            timesThrough++;
            CellCords _coordsStart = new CellCords();
            CellCords _coordsFinish = new CellCords();
            Debug.Log("The maze has been generated!!!");

            //_coordsStart = getRandVisitedCell(_grid.worldWidthCells, _grid.worldHeightCells);
            _coordsStart =_grid._startCellCoords;
  
            Debug.Log("The Start point is cell (" + _coordsStart.x + ", " + _coordsStart.z + ")");
            _grid.spawnObjectAtCell(_coordsStart, startFloor, "wall");
            Vector3 startLocation = _grid.GetCellWorldLocation(_coordsStart);
            _cwl.startLocationList.Add(startLocation);

            _coordsFinish = getRandVisitedCell(_grid.worldWidthCells, _grid.worldHeightCells, _coordsStart);
            _grid._endCellCoords = _coordsFinish;

            Debug.Log("The Finish point is cell (" + _coordsFinish.x + ", " + _coordsFinish.z + ")");
            _grid.destroyCellFloor(_coordsFinish.x, _coordsFinish.z);
            _grid.spawnObjectAtCell(_coordsFinish, finishFloor, "floor");
            Vector3 finishLocation = _grid.GetCellWorldLocation(_coordsFinish);
            _cwl.finishLocationList.Add(finishLocation);

            _grid.startEndCellsSet = true;
            _grid.allDone = true;

        }
    }

    public void setTeleporLocationOfFinish(Vector3 newLocation)
    {
        _ffts.changeTeleportLocation(newLocation);
    }

    public CellCords getRandVisitedCell(int worldWidthCells, int worldHeightCells)
    {
         CellCords _coords = new CellCords();
        _coords = getRandCell(worldWidthCells, worldHeightCells);
        while (!_grid.hasCellBeenVisited(_coords.x, _coords.z))
        {
            _coords = getRandCell(worldWidthCells, worldHeightCells);
        }
        Debug.Log("The Randomly selected VISITED cell was(" + _coords.x + ", " + _coords.z + ")");
        return _coords;
    }

    public CellCords getRandVisitedCell(int worldWidthCells, int worldHeightCells, CellCords matchCoords)
    {
        CellCords _coords = new CellCords();
        _coords = getRandCell(worldWidthCells, worldHeightCells);
        while (!_grid.hasCellBeenVisited(_coords.x, _coords.z) || (_coords.x == matchCoords.x && _coords.z == matchCoords.z))
        {
            _coords = getRandCell(worldWidthCells, worldHeightCells);
        }
        Debug.Log("The Randomly selected VISITED cell was(" + _coords.x + ", " + _coords.z + ")");
        return _coords;
    }

    public CellCords getRandCell(int worldWidthCells, int worldHeightCells)
    {
         CellCords _coords = new CellCords();
        _coords.x = Random.Range(1, worldWidthCells);
        _coords.z = Random.Range(1, worldHeightCells);

        //This bit of code makes sure that the selected cell will be a cell that
        //the maze pathways would be on
        //(maze pathways are only guarunteed to be on odd cells)
        if ((_coords.x % 2) == 0)//if its even, make it odd
        {
            _coords.x--;
            if (_coords.x < 0)  //and if its now below 0, put it back on the map
            {
                _coords.x += 2;
            }
        }

        if ((_coords.z % 2) == 0)//if its even, make it odd
        {
            _coords.z--;
            if (_coords.z < 0)
            {
                _coords.z += 2; //and if its now below 0, put it back on the map
            }
        }

        Debug.Log("The Max Cells are " + worldWidthCells + " " + worldHeightCells);
       
        Debug.Log("The Randomly selected cell was (" + _coords.x + ", " + _coords.z + ")");
        return _coords;
    }
}
