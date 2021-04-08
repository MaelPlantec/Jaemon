/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class PathGrid<TPathGridObject> {

    public event EventHandler<OnPathGridObjectChangedEventArgs> OnPathGridObjectChanged;

    public class OnPathGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 origin;
    private TPathGridObject[,] pathGridArray;

    public PathGrid(int width, int height, float cellSize, Vector3 origin, Func<PathGrid<TPathGridObject>, int, int, TPathGridObject> createGridObject) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;

        pathGridArray = new TPathGridObject[width, height];

        for (int x = 0; x < pathGridArray.GetLength(0); x++) {
            for (int y = 0; y < pathGridArray.GetLength(1); y++) {
                pathGridArray[x, y] = createGridObject(this, x, y);
            }
        }

        bool showDebug = false;
        if (showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < pathGridArray.GetLength(0); x++) {
                for (int y = 0; y < pathGridArray.GetLength(1); y++) {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(pathGridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, (int) (5*cellSize), Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnPathGridObjectChanged += (object sender, OnPathGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = pathGridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x + 0.5f * cellSize, y + 0.5f * cellSize) * cellSize + origin;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - origin).y / cellSize);
    }

    public Vector3Int GetTilePosition(int x, int y) {
        return new Vector3Int(Mathf.FloorToInt(x + origin.x), Mathf.FloorToInt(y + origin.y), 0);
    }

    public Vector3 GetCellCenter(Vector3 position)
    {
        return new Vector3(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), Mathf.FloorToInt(position.z)) + 0.5f * Vector3.one * cellSize;
    }

    public void SetGridObject(int x, int y, TPathGridObject value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            pathGridArray[x, y] = value;
            if (OnPathGridObjectChanged != null) OnPathGridObjectChanged(this, new OnPathGridObjectChangedEventArgs { x = x, y = y });
        }
    }

    public void TriggerGridObjectChanged(int x, int y) {
        if (OnPathGridObjectChanged != null) OnPathGridObjectChanged(this, new OnPathGridObjectChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, TPathGridObject value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TPathGridObject GetGridObject(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return pathGridArray[x, y];
        } else {
            return default(TPathGridObject);
        }
    }

    public TPathGridObject GetGridObject(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

}
