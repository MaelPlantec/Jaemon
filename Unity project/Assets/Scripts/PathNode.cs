using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    private PathGrid<PathNode> pathGrid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;
    public PathNode cameFromNode;

    public PathNode(PathGrid<PathNode> pathGrid, int x, int y)
    {
        this.pathGrid = pathGrid;
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString() 
    {
        return "{" + x +","+ y + "}";
    }
}
