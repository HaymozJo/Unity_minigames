using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.U2D.Aseprite;
using UnityEditor.Experimental.GraphView;

public class BoardTest : MonoBehaviour
{
    public Tilemap boardTilemap;
    public Tilemap availableTilemap;
    public Tilemap greenTilemap;
    public TileBase blue;
    public TileBase white;
    public TileBase blueHL;
    public TileBase whiteHL;
    private enum Cell {Green, Red, Free};
    private bool click;
    private Vector3Int lastClickedCell;

    void Start()
    {
        click = false;

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
            Vector3Int cellPosition = boardTilemap.WorldToCell(worldPosition);
            
            Cell cellColor = checkCaseUse(cellPosition);
            if (cellColor != Cell.Free){
                if (lastClickedCell != cellPosition || click == false){
                    if (click){
                        availableTilemap.ClearAllTiles();
                    }

                    //Now we can check for the new cell
                    //iterates through available cells (see AvailableCells fct)

                    int dir = (cellColor == Cell.Green)? 1 : -1;
                    foreach (Vector3Int cell in AvailableCells(cellPosition, dir)){
                        TileBase checkTileColor = boardTilemap.GetTile(cell);
                        if (checkTileColor == white){
                            availableTilemap.SetTile(cell, whiteHL);
                        }else if (checkTileColor == blue){
                            availableTilemap.SetTile(cell, blueHL);
                        }
                    }
                    lastClickedCell = cellPosition;
                    click = true;
                }else{
                    availableTilemap.ClearAllTiles();
                    click = false;
                }
            
            }else{
                    availableTilemap.ClearAllTiles();
                    click = false;
            }
        }
    }

    //TODO: Add redTileMap once checked for green
    private Cell checkCaseUse(Vector3Int cellPosition){
        if (boardTilemap.HasTile(cellPosition)){
            if (greenTilemap.HasTile(cellPosition)){
                return Cell.Green;
            }else{
                return Cell.Free;
            }
        }else{
            return Cell.Free;
        }
        
    }
    public bool checkCell(Vector3Int cellPosition){
        bool boardTile = boardTilemap.HasTile(cellPosition);
        bool free = !greenTilemap.HasTile(cellPosition);
        return boardTile && free;
    }

    public List<Vector3Int> AvailableCells(Vector3Int cellPosition, int dir){
        List<Vector3Int> availables = new List<Vector3Int>();
        //case green, we go up on y. case red we go down on y
        // Assumption test 1: we green, 2 we are a pawn
        // Should use enum to do case by case and take out checkCell for now, but only when needed
        for (int x = cellPosition.x - dir; x <= cellPosition.x + dir; x++){
            Vector3Int toCheck = new Vector3Int(x, cellPosition.y +1, 0);
            bool free = checkCell(toCheck);
            Debug.Log("....................");
            if (free){
                availables.Add(toCheck);
            }
        }
        return availables;
    } 
}