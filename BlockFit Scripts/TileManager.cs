using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


/// <summary>
/// Manages the grid and spawns shapes
/// </summary>
public class TileManager : MonoBehaviour
{
    public static TileManager instance;
    private GridLayoutGroup gridLayout;

    [SerializeField] private GameObject blankTilePrefab;
    public List<List<GameObject>> tileGameObjects = new List<List<GameObject>>();
    
    [SerializeField] private GameObject[] shapePrefabs;

    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] Color[] shapeColors;

    public Shape currentShape;

    public List<TileRow> grid = new List<TileRow>(); // <-- NEW
    public int gridSize;
    Vector2Int currentGridPos;

    void Awake(){
        gridLayout = gridRect.GetComponent<GridLayoutGroup>();
        instance = this;

        int rowCount = gridLayout.constraintCount;

        // Create logical grid
        for (int y = 0; y < rowCount; y++){
            tileGameObjects.Add(new List<GameObject>());
            TileRow row = new TileRow();
            for (int x = 0; x < rowCount; x++){
                row.cells.Add(false);
                // Instantiate(blankTilePrefab, gridLayout.transform).AddComponent<TileTracker>().gridPos = new Vector2Int(x, y);
                GameObject tile = Instantiate(blankTilePrefab, gridLayout.transform);
                tile.AddComponent<TileTracker>().gridPos = new Vector2Int(x, y);
                tileGameObjects[y].Add(tile);
            }
            grid.Add(row);
        }

        
    }
    
    private void LateUpdate(){
        if(CheckEmpty() && currentShape == null){
            SpawnShape();
        }
    }

    private bool CheckEmpty(){
        // Check if ANY spawn point is occupied
        for (int i = 0; i < spawnPoints.Length; i++){
            if (spawnPoints[i].childCount > 0){
              //  anyOccupied = true;
                //break;
                return false;
            }
        }
        return true;

    }

    public void SpawnShape(){
        if(!CheckEmpty()) return;

        // Spawn into ALL empty points
        for (int i = 0; i < spawnPoints.Length; i++){
            int shapeIndex = Random.Range(0, shapePrefabs.Length);
            GameObject shapeInstance = Instantiate(shapePrefabs[shapeIndex], spawnPoints[i]);
            shapeInstance.transform.localPosition=Vector2.zero;
            // Rotate the shape's data and transform 

            // randon int from 1-4
            int randRot = Random.Range(0, 4);
            
            // shapeInstance.transform.Rotate(0, 0, randRot * -90);
            shapeInstance.GetComponent<Shape>().SetRotation(randRot);
        }
    }


    /// Gets a random color from the shape colors array.
    public Color GetRandomColor(){
        return shapeColors[Random.Range(0, shapeColors.Length)];
    }



    public void SetShape(Shape shape){
        currentShape = shape;
    }

    [SerializeField] private RectTransform gridRect;

    // Gets current grid position 
    public Vector2Int GetGridPosition(){
        return currentGridPos;
    }

    // Sets grid position
    public void SetGridPosition(Vector2Int newPos){
        currentGridPos = newPos;
    }

    /// Rotates the current shape
    public void RotateShape(InputAction.CallbackContext context){
        // Make sure the function can only be called once per key press
        if(!context.performed || currentShape == null) return;
        currentShape.transform.Rotate(0, 0, -90);
        currentShape.RotateData();
        SoundManager.instance.PlayRotateSound();
    }
}