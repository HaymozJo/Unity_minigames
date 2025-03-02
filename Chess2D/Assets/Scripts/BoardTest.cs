using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.U2D.Aseprite;

public class BoardTest : MonoBehaviour
{
    public Tilemap boardTilemap;

    public Tilemap pawnTilemap;
    public TileBase selectedTile;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
            Vector3Int cellPosition = boardTilemap.WorldToCell(worldPosition);

            foreach (Vector3Int cell in AvailableCells(cellPosition)){
                pawnTilemap.SetTile(cell, selectedTile);
            }

            if (boardTilemap.HasTile(cellPosition)) // Check if the tile exists
            {
                pawnTilemap.SetTile(cellPosition, selectedTile);
            }
        }
    }

    public bool checkCell(Vector3Int cellPosition){
        bool boardTile = boardTilemap.HasTile(cellPosition);
        bool free = !pawnTilemap.HasTile(cellPosition);
        return boardTile && free;
    }

    public List<Vector3Int> AvailableCells(Vector3Int cellPosition){
        List<Vector3Int> availables = new List<Vector3Int>();
        //case green, we go up on y. case red we go down on y
        // Assumption test 1: we green, 2 we are a pawn
        for (int x = cellPosition.x -1; x <= cellPosition.x +1; x++){
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