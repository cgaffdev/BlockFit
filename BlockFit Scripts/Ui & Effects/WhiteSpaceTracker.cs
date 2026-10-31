using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects when the mouse is no longer hovering over a tile.
/// </summary>
public class WhiteSpaceTracker : MonoBehaviour
{
    // If Mouse is No longer Hovering
    void OnMouseExit(){
         TileManager.instance.SetGridPosition(new Vector2Int(-1,-1)); // Essentially tells the TileManager invalid position
    }
    
}
