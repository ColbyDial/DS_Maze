using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCords{
    public int x;
    public int z;
    public bool visited;

    public void setVisited(bool hasbeenvisited)
    {
        this.visited = hasbeenvisited;
    }

    public bool getVisited()
    {
        return this.visited;
    }

    public int getx()
    {
        return this.x;
    }

    public int getz()
    {
        return this.z;
    }
}


