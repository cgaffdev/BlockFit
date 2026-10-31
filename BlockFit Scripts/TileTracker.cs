using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Tracks where the mouse is hovering over on the grid.
/// </summary>
public class TileTracker : MonoBehaviour
{

    public Vector2Int gridPos;

    void Update(){
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPos);

        
        foreach (var hit in hits){
            if(hit.gameObject == this.gameObject){
                MouseHovering();
            }   
        }
    }

    void MouseHovering(){
        TileManager.instance.SetGridPosition(gridPos);
    }

}
