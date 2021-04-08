using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathGraph
{
    public PathGrid<PathNode> pathGrid;

    private List<PathNode> openList;
    private List<PathNode> closedList;
    private Tilemap wallsTilemap;
    
    public PathGraph (int width, int height, float cellSize, Vector3 origin, Tilemap wallsTilemap) 
    {
        this.pathGrid = new PathGrid<PathNode>(width, height, cellSize, origin, (PathGrid<PathNode> g, int x, int y) => new PathNode(g, x, y));
        this.wallsTilemap = wallsTilemap;
    }

    public PathGrid<PathNode> GetPathGrid()
    {
        return pathGrid;
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = pathGrid.GetGridObject(startX, startY);
        PathNode endNode = pathGrid.GetGridObject(endX, endY);

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < pathGrid.GetWidth(); x++)
        {
            for (int y = 0; y < pathGrid.GetHeight(); y++) 
            {
                PathNode pathNode = pathGrid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCost(openList);
            if (currentNode == endNode)
            {
                //End reached
                return CalculatePath(endNode);
            } 

            openList.Remove(currentNode); //current node already searched
            closedList.Add(currentNode);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    Debug.DrawLine(pathGrid.GetTilePosition(currentNode.x, currentNode.y) + 0.45f * Vector3.one, pathGrid.GetTilePosition(neighbourNode.x, neighbourNode.y) + 0.45f * Vector3.one, Color.yellow, 200f, false);
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode); // PROBLEM IF MULTIPLE SOLUTIONS???
                    }
                }
            }
        }

        // out of node in the open list
        return null;
    
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        int x = currentNode.x;
        int y = currentNode.y;

        if (x - 1 >= 0 && !wallsTilemap.HasTile(pathGrid.GetTilePosition(x - 1, y)))
        {
            neighbourList.Add(GetNode(x - 1, y)); //Left
        }
        if (x + 1 < pathGrid.GetWidth() && !wallsTilemap.HasTile(pathGrid.GetTilePosition(x + 1, y))) 
        {
            neighbourList.Add(GetNode(x + 1, y)); //Right
        }
        if (y - 1 >= 0 && !wallsTilemap.HasTile(pathGrid.GetTilePosition(x, y - 1))) 
        {
            neighbourList.Add(GetNode(x, y - 1)); //Down
        }
        if (y + 1 < pathGrid.GetHeight() && !wallsTilemap.HasTile(pathGrid.GetTilePosition(x, y + 1)))
        {
            neighbourList.Add(GetNode(x, y + 1)); //Up
        } 

        return neighbourList;
    }

    private PathNode GetNode(int x, int y)
    {
        return pathGrid.GetGridObject(x, y);
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;

        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        
        }

        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost (PathNode a, PathNode b) 
    {
        return (int) pathGrid.GetCellSize()*(Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));
    }

    private PathNode GetLowestFCost(List<PathNode>  list)
    {
        PathNode lowestFCostNode = list[0];
        for (int i = 1; i < list.Count ; i++) 
        {
            if (list[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = list[i];
            }
        }

        return lowestFCostNode;
    }
}
