using UnityEngine;
using UnityEngine.Tilemaps;

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
            Debug.Log("World Position: " + worldPosition);
            
            Vector3Int cellPosition = boardTilemap.WorldToCell(worldPosition);
            Debug.Log("Cell Position: " + cellPosition);

            if (boardTilemap.HasTile(cellPosition)) // Check if the tile exists
            {
                boardTilemap.SetColor(cellPosition, Color.white);
                pawnTilemap.SetTile(cellPosition, selectedTile);
            }
        }
    }
}