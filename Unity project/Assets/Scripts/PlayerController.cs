using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float idle1Timer = 0f;
    public Transform movePoint;
    public Tilemap backTilemap;
    public Tilemap wallsTilemap;
    public Animator animator;

    private PathGraph pathGraph;
    private List<PathNode> movePath = null;
    private Vector3Int p0;
    private float timer = 0f;
    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        p0 = new Vector3Int((int)(backTilemap.origin.x), (int)(wallsTilemap.origin.y), 0);
        pathGraph = new PathGraph(backTilemap.size.x, backTilemap.size.y, backTilemap.cellSize.x, p0, wallsTilemap);
        movePoint.parent = null; //prevent movepoint from moving as a child of player

        for (int x = 0; x < wallsTilemap.size.x; x++)
        {
            for (int y = 0; y < wallsTilemap.size.y; y++) 
            {
                if (wallsTilemap.HasTile(new Vector3Int(x+p0.x, y+p0.y, 0))) 
                {
                    //Debug.Log("INIT has tile " + new Vector3Int(x+p0.x, y+p0.y, 0));
                    Debug.DrawLine(new Vector3(x, y) + p0 + wallsTilemap.transform.position, new Vector3(x, y) + p0 + wallsTilemap.transform.position + 0.5f * Vector3.one, Color.blue, 200f, false);
                }
            }
        }
    }

    void Update()
    {
        // Watch for path request
        if (Input.GetMouseButton(0))
        {
            int startX, startY, endX, endY;
            List<PathNode> newMovePath;
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition(); 

            if (isMoving) // start pathfinding from next node
            {
                startX = movePath[1].x; 
                startY = movePath[1].y;
            } else { // start from current position
                pathGraph.pathGrid.GetXY(transform.position, out startX, out startY);
            }

            pathGraph.pathGrid.GetXY(mouseWorldPosition, out endX, out endY);
            newMovePath = pathGraph.FindPath(startX, startY, endX, endY);

            if (newMovePath != null) 
            {
                if (isMoving) //finish current node transition
                {
                    PathNode lastNode = movePath[0];
                    movePath = newMovePath;
                    movePath.Insert(0, lastNode);
                } else {
                    movePath = newMovePath;
                }

                movePoint.position = pathGraph.pathGrid.GetCellCenter(mouseWorldPosition);
            }
        }

        // If path is valid
        if (movePath != null && movePath.Count > 1)
        {
            MoveToNextPos();
        } else {
            isMoving = false;
        }
    }

    void FixedUpdate() 
    {
        float idle1Timer = animator.GetFloat("Idle1Timer") + Time.fixedDeltaTime;
        animator.SetFloat("Idle1Timer", idle1Timer);
    }

    private void MoveToNextPos()
    {
        isMoving = true;
        timer += Time.deltaTime * moveSpeed;
        Vector3 lastPosition = pathGraph.pathGrid.GetWorldPosition(movePath[0].x, movePath[0].y);
        Vector3 nextPosition = pathGraph.pathGrid.GetWorldPosition(movePath[1].x, movePath[1].y);
        transform.position = Vector3.Lerp(lastPosition, nextPosition, timer); // move
        Debug.DrawLine(transform.position, transform.position + 0.05f * Vector3.one, Color.magenta, 3f, false);
        //Debug.Log("move " + timer + "%");
        
        for (int i=0 ; i<movePath.Count - 1 ; i++)
        {
            //Debug.DrawLine(new Vector3(movePath[i].x, movePath[i].y) + p0 + 0.5f * Vector3.one, new Vector3(movePath[i+1].x, movePath[i+1].y) + p0 + 0.5f * Vector3.one, Color.red, 2f, false);
        }

        if (Utils.Equals(transform.position, nextPosition)) // If next pos now reached, update path
        {
            //Debug.Log("removing " + lastPosition);
            movePath.RemoveAt(0);
            timer = 0f;

            if (movePath.Count <= 1) isMoving = false; // end reached
        }
    }
}
