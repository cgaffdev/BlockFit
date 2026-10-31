using UnityEngine;


/// <summary>
/// Manages the cursor based on what the mouse is hovering over.
/// </summary>
public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    [Header("Default")]
    [SerializeField] private Texture2D cursorDefault;

    [Header("Grab")]
    [SerializeField] private Texture2D cursorHoverGrab;
    [SerializeField] private Texture2D cursorGrab;

    [Header("Click")]
    [SerializeField] private Texture2D cursorHoverClick;
    [SerializeField] private Texture2D cursorClick;

    private bool isDraggingShape;
    private Texture2D currentCursor;

    void Awake(){
        instance = this;
        SetCursor(cursorDefault);
    }


    void Update(){
        // if dragging AND mouse still held → never change cursor
        if (isDraggingShape && Input.GetMouseButton(0)){
            SetCursor(cursorGrab);
            return;
        }

        // If mouse released, clear dragging state
        if (!Input.GetMouseButton(0)){
            isDraggingShape = false;
        }


        bool isHovering = false;
        bool isClickHovering = false;

        // Raycast to check what the mouse is hovering over
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        // Checks if the mouse is hovering over a tag
        if (hit.collider != null){
            if (hit.collider.CompareTag("PlaceableShape")){
                isHovering = true;
            }else if (hit.collider.CompareTag("ClickTarget")){
                isClickHovering = true;
            }
        }

        Texture2D nextCursor = cursorDefault;

        // Changes cursor based on if hovering over a placeable shape or click target
        if (isHovering){
            nextCursor = Input.GetMouseButton(0) ? cursorGrab : cursorHoverGrab;
        }
        else if (isClickHovering){
            nextCursor = Input.GetMouseButton(0) ? cursorClick : cursorHoverClick;
        }

        SetCursor(nextCursor);
    }



    /// Sets the cursor to the new cursor.
    private void SetCursor(Texture2D newCursor){
        if (currentCursor == newCursor) return;

        currentCursor = newCursor;
        Cursor.SetCursor(currentCursor, Vector2.zero, CursorMode.Auto);
    }

    /// Starts the dragging process.
    public void StartDragging(){
        isDraggingShape = true;
    }
}