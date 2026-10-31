using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple sprite dragger with mouse compatibility 
/// </summary>
public class SpriteDragger : MonoBehaviour
{
    [SerializeField] Transform SpriteTransform;
    private Vector2 mouseOffset;
    private bool isDragging = false;
    Camera mainCamera;

    Vector3 originalPos;
    void Awake(){
        originalPos = SpriteTransform.position;
    }

    void OnMouseDown(){
        mainCamera = Camera.main;

        // Moves the sprite to the mouse location
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            mainCamera.WorldToScreenPoint(SpriteTransform.position).z)
        );

        mouseOffset = SpriteTransform.position - mouseWorldPosition;

        isMouseDown = true;
        isDragging = true;

    }


    void OnMouseDrag(){
        // Constantly moves the sprite to the mouse location while dragging
        if (isDragging){
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                mainCamera.WorldToScreenPoint(SpriteTransform.position).z)
            );


            SpriteTransform.position = mouseWorldPosition + (Vector3)mouseOffset;
        }
    }

    bool isMouseDown = false;

    void OnMouseUp(){
        // Returns sprite to it's original position
        isMouseDown = false;
        isDragging = false;

        SpriteTransform.position = originalPos;
    }


}