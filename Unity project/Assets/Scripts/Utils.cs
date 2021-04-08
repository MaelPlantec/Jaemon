using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public const float E = 0.005f; // spatial error

    // Get Mouse Position in World with Z = 0f
    public static Vector3 GetMouseWorldPosition() {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    public static Vector3 GetMouseWorldPositionWithZ() {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera) {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    public static bool Equals(Vector3 a, Vector3 b)
    {
        return (a.x > b.x - E && a.x < b.x + E) && (a.y > b.y - E && a.y < b.y + E); 
    }

    public static bool Equals(Vector3 a, Vector3 b, float error)
    {
        return (a.x > b.x - error && a.x < b.x + error) && (a.y > b.y - error && a.y < b.y + error); 
    }

    public static Vector3 PixelPos(Vector3 pos, Camera camera) 
    {
        Vector3 PixelPos = camera.WorldToScreenPoint(pos);

        Debug.Log( camera.ScreenToWorldPoint(new Vector3(32*Mathf.FloorToInt(PixelPos.x/32), 32*Mathf.FloorToInt(PixelPos.y/32), 32*Mathf.FloorToInt(PixelPos.z/32))));
        
        return camera.ScreenToWorldPoint(new Vector3(32*Mathf.FloorToInt(PixelPos.x/32), 32*Mathf.FloorToInt(PixelPos.y/32), 32*Mathf.FloorToInt(PixelPos.z/32)));
    }
}
