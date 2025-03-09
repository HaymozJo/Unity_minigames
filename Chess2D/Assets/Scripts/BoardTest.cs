using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.U2D.Aseprite;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph;
using System.Linq;
using UnityEngine.Rendering;
using TMPro;
using Unity.Mathematics;
using Unity.Collections;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TextCore.LowLevel;

public class BoardTest : MonoBehaviour
{
    public Tilemap boardTilemap;
    public Tilemap availableTilemap;
    public Tilemap greenTilemap;
    public Tilemap redTilemap;
    public Tilemap takenTilemap;
    public TileBase blue;
    public TileBase white;
    public TileBase blueHL;
    public TileBase whiteHL;
    //All the pieces:
    public TileBase GreenPawn;
    public TileBase RedPawn;
    public TileBase GreenBishop;
    public TileBase RedBishop;
    public TileBase GreenHorse;
    public TileBase RedHorse;
    public TileBase GreenTower;
    public TileBase RedTower;
    public TileBase GreenQueen;
    public TileBase RedQueen;
    public TileBase GreenKing;
    public TileBase RedKing;
    public TextMeshProUGUI player;
    public GameObject GameOver;
    public AudioManager audioManager;

    private enum Piece {Pawn, Bishop, Horse, Tower, Queen, King, bug};
    private enum TeamPossibility {Green, Red, Free, Out};
    private TeamPossibility myTeam;
    private TeamPossibility otherTeam;
    private bool click;
    private Vector3Int lastClickedCell;
    private List<Vector3Int> lastAvailables;
    private Vector3Int FAROUTSIDE = new Vector3Int(20, 20, 0);
    private bool paused;
    private Vector3Int pos_red_taken;
    private Vector3Int pos_green_taken;

    private Boolean mateState;
    void Start()
    {
        paused = false;
        click = false;
        mateState = false;
        lastClickedCell = new();
        lastAvailables = new();
        myTeam = TeamPossibility.Green;
        otherTeam = TeamPossibility.Red;
        player.text = "Green";
        pos_red_taken = new Vector3Int(-1, 7,0);
        pos_green_taken = new Vector3Int(8, 0,0);
        takenTilemap.ClearAllTiles();

    }

    void Update()
    {
        if (!paused){
            if (Input.GetMouseButtonDown(0)){
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
                Vector3Int cellPosition = boardTilemap.WorldToCell(worldPosition);
                
                TeamPossibility cellColor = checkTileUse(cellPosition);
                // Case where we want new availables. Note that the case where cellPos is in availables is dealt with
                // within the availibility rules. A stronger bool can be added to force no cannibalism if the availables fail
                if (cellColor == myTeam && cellPosition != lastClickedCell){
                    //set, we still clearclicked in case we already clicked, if not it changes nothing
                    ClearClicked();
                    //then we get availables and update the variables:
                    //we also need the direction we are looking at, going up (green) or down (red)
                    int dir = (cellColor == TeamPossibility.Green)? 1 : -1;
                    lastAvailables = AvailableCells(cellPosition, dir);
                    lastClickedCell = cellPosition;
                    click = true;
                    //then we show the highligths
                    HighlightCells(lastAvailables);
                }else{
                    //already clicked, otherwise we just clear all, the myTeam case is already put above on other cell already put above
                    if (click && lastAvailables.Contains(cellPosition)){    
                        Debug.Log("We should definitly move here eh");
                        //Move logic: if free, don't care
                        // if red, overthrow!!, we clean: all on the new pos, our tile in the old pos and add our new tile!
                        MovePiece(lastClickedCell, cellPosition);
                        
                        //then we can clear all 
                        ClearClicked();
                    }else{
                        ClearClicked();
                    }
                }
            }
        }
       
    }

    //Goal is just to reduce code in main function, highligth the available cells
    private void HighlightCells(List<Vector3Int> availables){
        foreach (Vector3Int cell in availables){   
            TileBase checkTileColor = boardTilemap.GetTile(cell);
            if (checkTileColor == white){
                availableTilemap.SetTile(cell, whiteHL);
            }else if (checkTileColor == blue){
                availableTilemap.SetTile(cell, blueHL);
            }
        }
    }

    //to stop redundant code, clear the 3 variables with information about the click, also clear highligths tilemap
    private void ClearClicked(){
        availableTilemap.ClearAllTiles();
        lastAvailables.Clear();
        lastClickedCell = new();
        click = false;
    }


    // to reduce redundant code:
    private void MovePiece(Vector3Int oldLoc, Vector3Int newLoc){
        //Sets some variable given the color string chosen:
        bool green = myTeam == TeamPossibility.Green;
        TileBase king = green? RedKing: GreenKing;
        string text = green? "Green": "Red";
        Tilemap myTilemap = green? greenTilemap: redTilemap;
        Tilemap otherTilemap = green? redTilemap: greenTilemap;

        //first check if we are eating the king, if so-> gameover
        if (otherTilemap.HasTile(newLoc)){
            //check if king -> win boy
            TileBase otherPiece = otherTilemap.GetTile(newLoc);
            if (otherPiece == king){
                paused = true;
                GameOver.SetActive(true);
            }else{
                audioManager.PlaySFX(audioManager.chessTake);
                AddToTaken(otherPiece);
            }
        }else{
            audioManager.PlaySFX(audioManager.chessMove);
        }

        //move piece and clean other tiles
        TileBase piece = myTilemap.GetTile(oldLoc);
        myTilemap.SetTile(oldLoc, null);
        myTilemap.SetTile(newLoc, piece);
        otherTilemap.SetTile(newLoc, null);
        //set new text (in a barbarian way but easy)
        player.text = text;
        //change player
        (otherTeam, myTeam) = (myTeam, otherTeam);
    }

    //add to taken tilemap
    private void AddToTaken(TileBase piece){          
        //Kinad obligated to do it manually otherwise complicated to change the variable through another variable
        // in a really "clean and understandable" way
        if (myTeam ==  TeamPossibility.Green){
            takenTilemap.SetTile(pos_green_taken, piece);
            if (pos_green_taken.y < 3){ pos_green_taken.y += 1;}
            else{
                pos_green_taken.y = 0;
                pos_green_taken.x +=1;
            }
        }else{
            takenTilemap.SetTile(pos_red_taken, piece);
            if (pos_red_taken.y > 4){pos_red_taken.y -= 1;}
            else{
                pos_red_taken.y = 7;
                pos_red_taken.x -=1;
            }
    }
       

                     
    }

    private TeamPossibility checkTileUse(Vector3Int cellPosition){
        if (boardTilemap.HasTile(cellPosition)){
            if (greenTilemap.HasTile(cellPosition)){
                return TeamPossibility.Green;
            }else if(redTilemap.HasTile(cellPosition)){
                return TeamPossibility.Red;
            }else{
                return TeamPossibility.Free;
            }
        }else{
            return TeamPossibility.Out;
        }
        
    }

    // to facilitate usage of the piece, similar to checkTileUse, we want to know what tile is and what piece is on to do the choices
    private Piece checkPiece(TileBase pieceTile){
        if (pieceTile == GreenPawn || pieceTile == RedPawn){return Piece.Pawn;}
        else if (pieceTile == GreenBishop || pieceTile == RedBishop){
return Piece.Bishop;}
        else if (pieceTile == GreenHorse || pieceTile == RedHorse){return Piece.Horse;}
        else if (pieceTile == GreenTower || pieceTile == RedTower){return Piece.Tower;}
        else if (pieceTile == GreenQueen || pieceTile == RedQueen){return Piece.Queen;}
        else if (pieceTile == GreenKing || pieceTile == RedKing){return Piece.King;}else{
            return Piece.bug;
        }
    }



    public List<Vector3Int> AvailableCells(Vector3Int cellPosition, int dir){
        List<Vector3Int> availables = new List<Vector3Int>();
        //first we want to check if the piece is in our team or not, if not, no check for available
        //It also helps us set the dir (go up (+1) for green, go down (-1) for red)
        if (greenTilemap.HasTile(cellPosition) && myTeam== TeamPossibility.Green){
            dir = 1;
            //now we check which piece is on this cell
            Piece piece = checkPiece(greenTilemap.GetTile(cellPosition));
            return piece switch
            {
                Piece.Pawn => AvailableCellsPawn(cellPosition, dir),
                Piece.Bishop => AvailableCellsDiag(cellPosition),
                Piece.Tower => AvailableCellsLine(cellPosition),
                Piece.Queen => AvailableCellsQueen(cellPosition),
                Piece.King => AvailableCellsKing(cellPosition),
                Piece.Horse => AvailableCellsHorse(cellPosition),
                _ => availables,
            };
        }
        else if (redTilemap.HasTile(cellPosition) && myTeam== TeamPossibility.Red){
            dir = -1;
            //now we check which piece is on this cell
            Piece piece = checkPiece(redTilemap.GetTile(cellPosition));
            return piece switch
            {
                Piece.Pawn => AvailableCellsPawn(cellPosition, dir),
                Piece.Bishop => AvailableCellsDiag(cellPosition),
                Piece.Tower => AvailableCellsLine(cellPosition),
                Piece.Queen => AvailableCellsQueen(cellPosition),
                Piece.King => AvailableCellsKing(cellPosition),
                Piece.Horse => AvailableCellsHorse(cellPosition),
                _ => availables,
            };
        }else{
                    return availables;
        }
    } 

    //case pawn, we also have to know which player is playing, we can maybe change the cell to color for easy =
    //We have to check 3 cases: front is free or not to move, diagonals are occupied by enemy pieces to take
    // Note: En passant not put in place for now, we also do not look at the king like any different, it will just game over when taken
    // Note2: we do it manually as it has different behaviours and looping for 2 makes no sense
    public List<Vector3Int> AvailableCellsPawn(Vector3Int cellPosition, int dir){
        List<Vector3Int> availables = new List<Vector3Int>();
        // number of y iteration => 2 if pawn is at either y = 1 or y = 6
        //Note: as we only have one more in case y = 1 for red and y= 6 for green, we still just iterate through both knowing the +2 won't be reachable
        int max_step = (cellPosition.y == 1 || cellPosition.y == 6) ? 2: 1;
        //front:
        for (int y_add = 1; y_add<=max_step; y_add++){

            Vector3Int front = new Vector3Int(cellPosition.x, cellPosition.y+(dir*y_add), 0);
            if (checkTileUse(front)==TeamPossibility.Free){availables.Add(front);};
        }
        //diagonals:
        Vector3Int diagLeft = new Vector3Int(cellPosition.x-1, cellPosition.y+dir, 0);
        Vector3Int diagRight = new Vector3Int(cellPosition.x+1, cellPosition.y+dir, 0);
        if (checkTileUse(diagLeft)==otherTeam){availables.Add(diagLeft);};
        if (checkTileUse(diagRight)==otherTeam){availables.Add(diagRight);};
        return availables;
        
    } 

    //For the bishop, we look at a diagonal in every dir
    //So we look at -+1*x and -+1*y up for every iteration
    //We get the diagonals that meet at our bishop and then clear ones not of use:
    //  - bishop itself, - green pieces (and each after) - red pieces after firs
    public List<Vector3Int> AvailableCellsDiag(Vector3Int cellPosition){

        //The logic is such that (left to right logic):
        // for the "going up" diag, all cells with the same x-y are on the diag
        // for the "going down" diag, all the cells with the same x+y are on the diag
        int diffBishop = cellPosition.x - cellPosition.y;
        int sumBishop = cellPosition.x + cellPosition.y;
        //we do it in "brute force", need to be optimized for bigger board, but 8x8 is chill
        //We keep a piece to check if we already have seen a piece on this diagonal
        List<Vector3Int> availablesDownLeft = new();
        List<Vector3Int> availablesupLeft = new();
        List<Vector3Int> availablesdownRight = new();
        List<Vector3Int> availablesupRight = new();
        Vector3Int downLeftPiece = FAROUTSIDE;
        Vector3Int upLeftPiece = FAROUTSIDE;
        Vector3Int downRightPiece = FAROUTSIDE;
        Vector3Int upRightPiece = FAROUTSIDE;
        for (int x=0; x < 8; x++){
            for (int y=0; y<8; y++){
                Vector3Int pos = new Vector3Int(x,y,0);
                //takes out the bishop itself
                if (pos != cellPosition){
                    if (x-y== diffBishop) {
                        //case downleft: y < bishop
                        if (y < cellPosition.y){
                            (downLeftPiece, availablesDownLeft) = helperLine(cellPosition, pos, downLeftPiece, availablesDownLeft);
                        //case upright: y > bishop
                        }else if (y > cellPosition.y){
                             (upRightPiece, availablesupRight) = helperLine(cellPosition, pos, upRightPiece, availablesupRight, clear:false);
                        }                    
                    }
                    if (x+y == sumBishop){
                        //case downright: y < bishop
                        if (y < cellPosition.y){
                            (downRightPiece, availablesdownRight) = helperLine(cellPosition, pos, downRightPiece, availablesdownRight, clear:false);
                        //case upleft: y > bishop
                        }else if (y > cellPosition.y){
                             (upLeftPiece, availablesupLeft) = helperLine(cellPosition, pos, upLeftPiece, availablesupLeft);
                        }
                    }
                }
            }
                

        }
        List<Vector3Int> availables = new List<Vector3Int>();
        availables.AddRange(availablesDownLeft);
        availables.AddRange(availablesdownRight);
        availables.AddRange(availablesupLeft);
        availables.AddRange(availablesupRight);
        foreach (Vector3Int cell in availables){
            Debug.Log(cell);
        }
        return availables;
    }


     public List<Vector3Int> AvailableCellsLine(Vector3Int cellPosition){

        //The logic is such that (left to right logic):
        // if x or y is the same, they are on the same line
        List<Vector3Int> availablesLeft = new();
        List<Vector3Int> availablesUp = new();
        List<Vector3Int> availablesRight = new();
        List<Vector3Int> availablesDown = new();
        Vector3Int leftPiece = FAROUTSIDE;
        Vector3Int upPiece = FAROUTSIDE;
        Vector3Int rightPiece = FAROUTSIDE;
        Vector3Int downPiece = FAROUTSIDE;
        for (int x=0; x < 8; x++){
            for (int y=0; y<8; y++){
                Vector3Int pos = new Vector3Int(x,y,0);
                //takes out the tower itself
                if (pos != cellPosition){
                    if (y== cellPosition.y) {
                        //case left: x < tower
                        if (x < cellPosition.x){
                            (leftPiece, availablesLeft) = helperLine(cellPosition, pos, leftPiece, availablesLeft);
                        //case right: x > tower
                        }else if (x > cellPosition.x){
                             (rightPiece, availablesRight) = helperLine(cellPosition, pos, rightPiece, availablesRight, clear:false);
                        }                    
                    }
                    if (x == cellPosition.x) {
                        //case down: y < tower
                        if (y < cellPosition.y){
                            (downPiece, availablesDown) = helperLine(cellPosition, pos, downPiece, availablesDown);
                        //case up: y > tower
                        }else if (y > cellPosition.y){
                             (upPiece, availablesUp) = helperLine(cellPosition, pos, upPiece, availablesUp, clear:false);
                        }                    
                    }
                }
            }
                

        }
        List<Vector3Int> availables = new List<Vector3Int>();
        availables.AddRange(availablesLeft);
        availables.AddRange(availablesRight);
        availables.AddRange(availablesUp);
        availables.AddRange(availablesDown);
        foreach (Vector3Int cell in availables){
            Debug.Log(cell);
        }
        return availables;
    }

    //queen is diag + line
    private List<Vector3Int> AvailableCellsQueen(Vector3Int cellPosition){
        List<Vector3Int> line = AvailableCellsLine(cellPosition);
        List<Vector3Int> diag = AvailableCellsDiag(cellPosition);
        line.AddRange(diag);
        return line;
    }

    //we iterate through the neighbours and the king but do not set the king
    private List<Vector3Int> AvailableCellsKing(Vector3Int cellPosition){
        List<Vector3Int> availables = new();
        int minX = Math.Max(0, cellPosition.x-1);
        int maxX = Math.Min(7, cellPosition.x+1);
        int minY = Math.Max(0, cellPosition.y-1);
        int maxY = Math.Min(7, cellPosition.y+1);

        for (int x = minX; x<= maxX; x++){
            for (int y = minY; y <= maxY; y++){
                Vector3Int pos = new Vector3Int(x,y,0);
                TeamPossibility caseUse = checkTileUse(pos);
                if (pos!=cellPosition){
                    if (caseUse == TeamPossibility.Free || caseUse == otherTeam){
                        availables.Add(pos);
                    }
                } 
            }
        }
        return availables;
    }

    private List<Vector3Int> AvailableCellsHorse(Vector3Int cellPosition){
        List<Vector3Int> availables = new();
        int[,] potential = {
            {1,2}, {1, -2}, {-1, 2}, {-1, -2}, {2, 1}, {2, -1}, {-2, 1}, {-2, -1}
        };

        for (int i = 0; i <8; i++){
            int potentialX = cellPosition.x + potential[i,0];
            int potentialY = cellPosition.y + potential[i,1];
            Vector3Int pos = new Vector3Int(potentialX, potentialY, 0);

            if (potentialX>=0 && potentialX <8 && potentialY>=0 && potentialY<8){
                TeamPossibility caseUse = checkTileUse(pos);
                if (caseUse == TeamPossibility.Free || caseUse == otherTeam){
                        availables.Add(pos);
                }
            }
        }
        return availables;
    }
    


    //Put to avoid redundant code
    //For every piece that goes through a line (diagonal, horizontal or vertical)
    //It takes the position checked, a pos of the piece already on the line (None if none), and the availables list
    // We check if the tile is closer or not than the "lastPiece" occupied, at start and if none it is set as outside to ensure bigger dist
    // if LastPiece closer than our tile, we don't care for it
    // else we check the place:
    //  free -> add it to availables, no change to lastpiece
    //  myTeam -> clearavailabes, change lastPiece to this place
    //  otherTeam ->clear availables,  add it to availables and change lastPiece to this place
    private (Vector3Int, List<Vector3Int>) helperLine(Vector3Int cellPosition, Vector3Int cellChecked, 
        Vector3Int lastPiece, List<Vector3Int> availables, bool clear=true){
        float distCheck = Math.Max(Math.Abs(cellChecked.x - cellPosition.x), Math.Abs(cellChecked.y - cellPosition.y));
        float distLast = (lastPiece == FAROUTSIDE) ? float.MaxValue : // Ensure lastPiece is initialized
                     Math.Max(Math.Abs(lastPiece.x - cellPosition.x), Math.Abs(lastPiece.y - cellPosition.y));

        // Always update lastPiece for first valid tile
        if (distCheck < distLast) {  
            TeamPossibility caseUse = checkTileUse(cellChecked);

            if (caseUse == TeamPossibility.Free) {
                availables.Add(cellChecked);
            } 
            else if (caseUse == myTeam) {
                if (clear){availables.Clear();}
                lastPiece = cellChecked; // Update lastPiece
            } 
            else if (caseUse == otherTeam) {
                if (clear){availables.Clear();}
                availables.Add(cellChecked); 
                lastPiece = cellChecked; // Update lastPiece
            }
        }

        return (lastPiece, availables);
    }
}
        

