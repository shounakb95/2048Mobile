using UnityEngine;
using System.Collections.Generic;


public class TileBoard : MonoBehaviour
{
    public TIle tilePrefab;
    public TileState[] tileStates;
    private TileGrid grid;
    private List<TIle> tiles;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
    private bool waiting=false;




    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<TIle>(16);
        
    }
    private void Start()
    {
        CreateTile();
        CreateTile();



    }

    private void CreateTile()
    {
        TIle tile= Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[1], 4);
        tile.spawnAtTheCell(grid.getRandomEmptyCell());
        tiles.Add(tile);

        //Debug.Log("here");


    }

    private void Update()
    {
        if (!waiting)
        {
            if (Swipe() == "up")
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Swipe() == "down")
            {
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
            }
            else if (Swipe() == "left")
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Swipe() == "right")
            {
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }
        
    }
    private void MoveTiles(Vector2Int direction,int startx,int incrementx, int starty, int incrementy)
    {
        for(int x=startx;x>=0 && x<grid.width;x+=incrementx)
        {
            for(int y=starty;y>=0 && y<grid.height;y+=incrementy)
            {
               TileCell cell= grid.GetCell(x, y);
                if(cell.occupied)
                {
                    MoveTile(cell.tile,direction);
                }
            }
        }
    }
    public void MoveTile(TIle tile,Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdvacentCell(tile.cell, direction);

        while(adjacent!=null)
        {
            if(adjacent.occupied)
            {
                //merge
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdvacentCell(adjacent, direction);
        }

        if(newCell!= null)
        {
           // waiting = true;
            tile.MoveToTheCell(newCell);
           // waiting = false;
            
        }
    }
    public string Swipe()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                //save began touch 2d point
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }
            if (t.phase == TouchPhase.Ended)
            {
                //save ended touch 2d point
                secondPressPos = new Vector2(t.position.x, t.position.y);

                //create vector from the two points
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                //normalize the 2d vector
                currentSwipe.Normalize();

                //swipe upwards
                if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
             {
                    return "up";
                }
                //swipe down
                if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
             {
                    return "down";
                }
                //swipe left
                if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
             {
                    return "left";
                }
                //swipe right
                if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
             {
                    return "right";
                }
            }
        }
        return null;
    }
}
