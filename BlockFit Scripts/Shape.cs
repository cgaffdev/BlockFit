using System.Collections.Generic;
using UnityEngine;


/// Essenetially, Tilerow and Tileshape allow you to make custom shapes within the inspector by using a grid.
#region Tile Shape Constructor
[System.Serializable]
public class TileRow
{
    public List<bool> cells = new List<bool>(); // each cell = true (cube exists) or false (empty)
}

[CreateAssetMenu(fileName = "NewTile", menuName = "Tiles/TileShape")]
public class TileShape : ScriptableObject
{
    public List<TileRow> shapeData = new List<TileRow>();
}
#endregion


/// <summary>
/// Responsible for shape behavior
/// </summary>
public class Shape : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform spriteTransform;

    [Header("Grab Settings")]
    
    [Header("Colliders")]
    public List<BoxCollider2D> childColliders = new List<BoxCollider2D>();
    [SerializeField] public List<TileRow> shapeData = new List<TileRow>();
    List<TileRow> originalShapeData = new List<TileRow>();

    private List<Transform> previewTiles = new List<Transform>();

    private Transform grabPointTransform;
    public Transform homeParent;
    private Camera mainCamera;

    private Vector3 originalPosition;
    private Vector3 originalScale;

    private float grabZDepth;
    private bool isDragging;

    Color shapeColor;
    

    
    void Awake(){
        originalShapeData = shapeData; // Store original shape data for rotation resets
        if(grabPoints != null && grabPoints.Count > 0){        
            grabPoint = grabPoints[0];
        }
        
        if (spriteTransform == null) {
            spriteTransform = transform;
        }

        grabPointTransform = transform.Find("GrabPoint");
        mainCamera = Camera.main;

        originalPosition = spriteTransform.position;
        originalScale = spriteTransform.localScale;
        homeParent = transform.parent;

        shapeColor = TileManager.instance.GetRandomColor();

        // Collect all child and their child BoxColliders
        //GetColliders();
        //CreateShape();

    }

    [Header("Shape Creation")]
    [SerializeField] private int shapeSpacing = 11;
    private Vector2 grabPoint; // Translate to tile coordinates
    [SerializeField] private List<Vector2> grabPoints; // List of potential grab points in tile coordinates

    [SerializeField] private Vector2 cursorOffsetXYZ; // This is for cursor Offset
    
    [SerializeField] private Transform grabPointPrefab;
    [SerializeField] private GameObject blankTilePrefab;


    /// <summary>
    /// Creates the shape based on the shapeData.
    /// </summary>
    private void CreateShape(){
        int xOffset = 0;
        int yOffset = 0;
        List<Transform> createdTiles = new List<Transform>();

        // Loop through the shape data
        for (int i = 0; i < shapeData.Count; i++){
            for (int j = 0; j < shapeData[i].cells.Count; j++){
                // If the cell is true, create a tile
                if (shapeData[i].cells[j]){
                    Transform cube = Instantiate(blankTilePrefab, transform).transform;
                    cube.localPosition = new Vector3(xOffset, yOffset, 0);
                    createdTiles.Add(cube);
                }

                // move over to next cell
                xOffset += shapeSpacing;
            }

            // move down to next row
            xOffset = 0;
            yOffset -= shapeSpacing;
            SetColor();

        }

        // Create grab point
        Transform point = Instantiate(grabPointPrefab, transform);
        point.localPosition = new Vector2(-grabPoint.x * shapeSpacing, grabPoint.y * shapeSpacing);

        // Parent tiles to grab point
        foreach (Transform tile in createdTiles){
            tile.SetParent(point, false);
        }

        // calculate bounds of the shape
        if (createdTiles.Count > 0){
            // Since tiles are now children of "point",
            // use world positions then convert back to this object's local space
            Vector2 min = transform.InverseTransformPoint(createdTiles[0].position);
            Vector2 max = min;

            foreach (Transform tile in createdTiles){
                Vector2 localPos = transform.InverseTransformPoint(tile.position);

                min = Vector2.Min(min, localPos);
                max = Vector2.Max(max, localPos);
            }

            // Add tile size padding
            Vector2 size = max - min + Vector2.one * shapeSpacing;
            Vector2 center = (min + max) / 2f;

            // creates a collider around the shape
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider == null)
                collider = gameObject.AddComponent<BoxCollider2D>();

            collider.size = size;
            collider.offset = center;
        }
    }


    #region Mouse Behavior

    // Grabs the shape when clicked
    void OnMouseDown(){
        isDragging = true;

        if (CursorManager.instance != null)
            CursorManager.instance.StartDragging();

        grabZDepth = mainCamera.WorldToScreenPoint(spriteTransform.position).z;

        transform.SetParent(null);
        spriteTransform.localScale = Vector3.one;

        // set the current shape in the tile manager
        TileManager.instance.SetShape(this);
        SoundManager.instance.PlayPickSound();
        ClearPreview(); // Removes hover preview
        SnapToMouse(); // snaps mouse to center of shape
    }

    // Drags the shape
    void OnMouseDrag(){
        if (!isDragging) return;
        
        SnapToMouse(); // snaps mouse to center of shape
        ClearPreview(); // Removes hover preview
        if(CheckPlace()) HoverPreview(); // Shows hover preview if can place
    }

    // Releases the shape
    void OnMouseUp(){
        isDragging = false;

        // Change position and size
        if(!TileHolder.instance.CheckHold()){
            ReturnToContainer();
        }

        // Set the current shape in the tile manager
        TileManager.instance.SetShape(null);
        SoundManager.instance.PlayPlaceSound();
        ClearPreview(); // Removes hover preview
        if(CheckPlace()) PlaceShape(); // Places shape if can place
    }

    // returns shape to container
    public void ReturnToContainer(){
        transform.SetParent(homeParent);
        spriteTransform.localPosition = new Vector3(0,0,-1);
        spriteTransform.localScale = originalScale;
    }

    // Snaps the shape to the mouse position
    private void SnapToMouse(){
        Vector3 mouseWorld = GetMouseWorldPosition();

        if (grabPointTransform == null){
            spriteTransform.position = mouseWorld + (Vector3)cursorOffsetXYZ;
            return;
        }

        // Offset to keep grabPointTransform aligned to cursor
        Vector3 grabAlignmentOffset = spriteTransform.position - grabPointTransform.position;

        // Final position with custom XY offset
        spriteTransform.position = mouseWorld + grabAlignmentOffset + (Vector3)cursorOffsetXYZ;
    }

    // Gets the mouse world position
    private Vector3 GetMouseWorldPosition(){
        Vector3 mouse = Input.mousePosition;
        mouse.z = grabZDepth;
        return mainCamera.ScreenToWorldPoint(mouse);
    }

    #endregion

    // Sets the color of the shape
    private void SetColor(Transform trans = null, Color color = default(Color)){
        if(trans == null){
            trans = transform;
        }

        if(color == default(Color)){
            color = shapeColor;
        }

        GetBottomNode(trans, color);
    }

    // Gets the bottom node of the shape
    private void GetBottomNode(Transform parent, Color color){
        foreach (Transform child in parent){
            // If this child has no children, it's a leaf (end of a branch)
            if (child.childCount == 0){
                child.GetComponent<SpriteRenderer>().color = color;
            }
            else{
                // Otherwise, keep going deeper
                GetBottomNode(child, color);
            }
        }
    }

    // Clears the shape
    private void ClearShape(){
        if(transform.childCount == 0) return;

        foreach (Transform child in transform){
            Destroy(child.gameObject);
        }
    }

    #region Shape Data Rotation - Contains CreateShape()

    private int currentRotation = 0; // 0,1,2,3

    // Sets the rotation of the shape
    public void SetRotation(int zRotation){
        currentRotation = zRotation % 4;

        // Reset to original shape first
        shapeData = originalShapeData;

        for (int i = 0; i < currentRotation; i++){
            shapeData = Rotate90(shapeData);
        }

        RotatePivot();
        CreateShape();
    }

    // Rotates the shape data
    public void RotateData(){
        currentRotation = (currentRotation + 1) % 4;

        shapeData = Rotate90(shapeData);
        RotatePivot();
    }

    // Rotates the shape data by 90 degrees
    private List<TileRow> Rotate90(List<TileRow> original){
        int rows = original.Count;
        int cols = original[0].cells.Count;

        List<TileRow> rotated = new List<TileRow>();
        
        // rotates the tile data
        for (int c = 0; c < cols; c++){
            TileRow newRow = new TileRow();

            for (int r = rows - 1; r >= 0; r--){
                newRow.cells.Add(original[r].cells[c]);
            }

            rotated.Add(newRow);
        }

        return rotated;
    }

    // Rotates the pivot point of the shape
    private void RotatePivot(){
        if (grabPoints == null || grabPoints.Count == 0){
            Debug.LogError("No grab points set for shape: " + name);
            return;
        }

        grabPoint = grabPoints[currentRotation % grabPoints.Count];

        Debug.Log("Rotation: " + currentRotation + " Grab Point: " + grabPoint);
    }
    #endregion
    

    // Checks if the shape can be placed at the current grid position
    private bool CheckPlace(){
        if(TileManager.instance.GetGridPosition() == new Vector2Int(-1, -1)) return false;

        Vector2Int loc = TileManager.instance.GetGridPosition();
        Vector2Int offset = new Vector2Int((int)this.grabPoint.x, (int)this.grabPoint.y);

        Debug.Log("Pos: " + loc + " Offset: " + offset);

        if(CanPlaceShape(TileManager.instance.grid, shapeData, loc.y - offset.y, loc.x - offset.x)){
            Debug.Log("Can place shape here");
            return true;
        }
        else{
           Debug.Log("Can't place shape here");
           return false;

        }
    }

    // Clears the hover preview
    private void ClearPreview(){
        if(previewTiles.Count <= 0) return;

        foreach(Transform tile in previewTiles){
            SetColor(tile, Color.white);
        }
        previewTiles.Clear();
    }    

    

    // Shows a hover preview of the shape
    private void HoverPreview(){   
        if(TileManager.instance.GetGridPosition() == new Vector2Int(-1, -1)){
            ClearPreview();
            return;

        }

        int startRow = TileManager.instance.GetGridPosition().y - (int)grabPoint.y;
        int startCol = TileManager.instance.GetGridPosition().x - (int)grabPoint.x;

    
        for (int r = 0; r < shapeData.Count; r++){
            for (int c = 0; c < shapeData[r].cells.Count; c++){
                if (shapeData[r].cells[c]){
                    // Replace the tile at location with colored tile
                    Transform tile = TileManager.instance.tileGameObjects[startRow + r][startCol + c].transform;

                    previewTiles.Add(tile);

                    SetColor(tile);
                }
            }
        }
    }

    // Checks if the shape can be placed at the current grid position
    private bool CanPlaceShape(List<TileRow> grid, List<TileRow> shape, int startRow, int startCol){
        int gridSize = grid.Count;

        for (int r = 0; r < shape.Count; r++){
            for (int c = 0; c < shape[r].cells.Count; c++){
                if (!shape[r].cells[c]) continue;

                int gridRow = startRow + r;
                int gridCol = startCol + c;

                // Bounds check
                if (gridRow < 0 || gridRow >= gridSize || gridCol < 0 || gridCol >= gridSize)
                    return false;

                // Collision check
                if (grid[gridRow].cells[gridCol])
                    return false;
            }
        }

        return true;
    }


    // Places the shape on the grid
    private void PlaceShape(){
        ClearPreview();


        int startRow = TileManager.instance.GetGridPosition().y - (int)grabPoint.y;
        int startCol = TileManager.instance.GetGridPosition().x - (int)grabPoint.x;

        for (int r = 0; r < shapeData.Count; r++){
            for (int c = 0; c < shapeData[r].cells.Count; c++){
                if (shapeData[r].cells[c]){
                    TileManager.instance.grid[startRow + r].cells[startCol + c] = true;
                    // Replace the tile at location with colored tile
                    SetColor(TileManager.instance.tileGameObjects[startRow + r][startCol + c].transform);
                }
            }
        }
        Destroy(gameObject);
    }

    
}