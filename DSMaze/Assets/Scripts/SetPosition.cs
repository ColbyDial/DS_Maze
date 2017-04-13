using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPosition : MonoBehaviour {
    private float tanVal;
    private float x;
    private float y;
    private float z;
    private float offset;
    private int cellSize;
    public GameObject gridMaker;
    private GameObject _grid;
    private bool camset = false;
    private GameObject other;
    private Camera _camera;


	// Use this for initialization
	void Start () {
        _camera = this.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

        _grid = gridMaker;
        CreateGrid createGrid = _grid.GetComponent<CreateGrid>();
        offset = createGrid.GetOffset();
        cellSize = createGrid.getWorldHeightCells();
        _camera.orthographicSize = cellSize + 2;

        x = createGrid.GetWidth();
        x = x / 2;

        z = createGrid.GetHeight();
        z = z / 2;
        


        //y = tanVal * x * (offset + (offset / 3));
        
        if (!camset)
        {
            this.transform.position = new Vector3(x + offset, y + offset, z);
            camset = true;
        }
    }
}

