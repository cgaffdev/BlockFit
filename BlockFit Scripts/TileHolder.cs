using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Block Tile holder slot
/// </summary>
public class TileHolder : MonoBehaviour
{
    public static TileHolder instance;
    private Camera camera;
    bool mouseHovering = false;

    [SerializeField] Transform tileHolder;

    void Awake(){
        if(TileHolder.instance!=null){
            Destroy(this);
        }

        instance=this;
    }

    void Start(){
        camera = Camera.main;

        if(tileHolder==null){
            tileHolder=transform;
        }
    }
    
    private void OnMouseEnter(){
        mouseHovering = true;
    }

    private void OnMouseExit(){
        mouseHovering = false;
    }

    /// <summary>
    /// Checks if the mouse is hovering over the tile holder and if the shape can be held.
    /// </summary>
    public bool CheckHold(){
        if(!mouseHovering) return false;

        Shape currentShape = TileManager.instance.currentShape;
        Debug.Log(currentShape.transform.name);
        if(currentShape!=null && tileHolder.childCount==0){
            currentShape.homeParent = transform;
            currentShape.ReturnToContainer();
            return true;
        }
        return false;
    }

}
