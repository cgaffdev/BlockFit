using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    
    // Update is called once per frame
    void Update(){
        CheckAllPatterns(TileManager.instance.grid);
    }

    // Checks grid patterns for clearable rows/columns
    private void CheckAllPatterns(List<TileRow> grid){
        bool cleared = false;
        // Lists to keep track of what to clear
        List<int> rowsToClear = new();
        List<int> columnsToClear = new();

        // Checks all rows and columns
        for (int x = 0; x < 9; x++){
            // Checks if row is full
            if (IsRowFull(grid, x)){
                Debug.Log("Full row: " + x);
                rowsToClear.Add(x);
                cleared = true;
            }
            // Checks if column is full
            if (IsColumnFull(grid, x)){
                Debug.Log("Full column: " + x);
                columnsToClear.Add(x); 
                cleared = true;
            }
        }

        // Calculates points from cleared rows/columns
        int columsCleared = columnsToClear.Count+rowsToClear.Count;
        if(columsCleared>0){
            Scoreboard.instance.UpdateScoreboard(columsCleared);
        }
        

        // Clears rows
        foreach(int r in rowsToClear){
            ClearRow(grid, r);
        }
        // Clears columns
        foreach(int c in columnsToClear){
            ClearColumn(grid, c);
        }
 
        // Plays clear sound if any rows/columns were cleared
        if(cleared) SoundManager.instance.PlayClearSound();
    }

    #region Check Patterns
    // Checks if a row is full
    private bool IsRowFull(List<TileRow> grid, int row){
        for (int col = 0; col < grid[row].cells.Count; col++){
            if (!grid[row].cells[col])
                return false;
        }
        return true;
    }

    // Checks if a column is full
    private bool IsColumnFull(List<TileRow> grid, int col){
        for (int row = 0; row < grid.Count; row++){
            if (!grid[row].cells[col])
                return false;
        }
        return true;
    }

    // Checks if a 3x3 box is full
    private bool IsBoxFull(List<TileRow> grid, int startRow, int startCol){
        for (int r = 0; r < 3; r++){
            for (int c = 0; c < 3; c++){
                if (!grid[startRow + r].cells[startCol + c])
                    return false;
            }
        }
        return true;
    }

    #endregion

    #region Clear Patterns

    // Clears a row
    private void ClearRow(List<TileRow> grid, int row){
        for (int col = 0; col < 9; col++){
            grid[row].cells[col] = false;
            ClearTileVisual(row, col);
        }
    }

    // Clears a column
    private void ClearColumn(List<TileRow> grid, int col){
        for (int row = 0; row < 9; row++){
            grid[row].cells[col] = false;
            ClearTileVisual(row, col);
        }
    }

    // Clears a 3x3 box
    // Decided to not use this for now.
    /*
    void ClearBox(List<TileRow> grid, int startRow, int startCol)
    {
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                grid[startRow + r].cells[startCol + c] = false;
                ClearTileVisual(startRow + r, startCol + c);
            }
        }
    }
    */

    // Resets tile visuals
    private void ClearTileVisual(int row, int col){
        GameObject tile = TileManager.instance.tileGameObjects[row][col];
        GetBottomNode(tile.transform, Color.white);
    }

    // Helper function to find the bottom node of a tile
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

    #endregion

}
