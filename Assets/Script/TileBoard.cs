using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;
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
    

    public  void CreateTile()
    {
        TIle tile= Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[1], 2);
        tile.spawnAtTheCell(grid.getRandomEmptyCell());
        tiles.Add(tile);

        //Debug.Log("here");


    }

    public void ClearBoard()
    {
        foreach(var cell in grid.cells)
        {
            cell.tile = null;
        }
        foreach(var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
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
        bool changed = false;
        for(int x=startx;x>=0 && x<grid.width;x+=incrementx)
        {
            for(int y=starty;y>=0 && y<grid.height;y+=incrementy)
            {
               TileCell cell= grid.GetCell(x, y);
                if(cell.occupied)
                {
                    changed|=MoveTile(cell.tile,direction);

                }
            }
        }
        if(changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }
    public bool MoveTile(TIle tile,Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdvacentCell(tile.cell, direction);

        while(adjacent!=null)
        {
            if(adjacent.occupied)
            {
                //merge
                if(CanMerge(tile,adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdvacentCell(adjacent, direction);
        }

        if(newCell!= null)
        {
           
            tile.MoveToTheCell(newCell);

            return true;
        }
        return false;
        
    }
    private bool CanMerge(TIle a, TIle b)
    {
        return a.number == b.number && !b.Locked;
    }

    private void Merge(TIle a, TIle b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;

        b.SetState(tileStates[index], number);
    }

    private int IndexOf(TileState state)
    {
        for(int i=0;i<tileStates.Length;i++)
        {
            if (state == tileStates[i])
                return i;
        }
        return -1;
    }
    private IEnumerator WaitForChanges()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;

        foreach(var tile in tiles)
        {
            tile.Locked = false;
        }

        if (tiles.Count != grid.size)
        {
            CreateTile();
        }
        if(CheckForGameOver())
        {
            gameManager.GameOver();
        }

        //check for GameOver

    }

    private bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }

        foreach(var tile in tiles)
        {
            TileCell up = grid.GetAdvacentCell(tile.cell,Vector2Int.up);
            TileCell Down = grid.GetAdvacentCell(tile.cell, Vector2Int.down);
            TileCell Left = grid.GetAdvacentCell(tile.cell, Vector2Int.left);
            TileCell Right = grid.GetAdvacentCell(tile.cell, Vector2Int.right);

            if(up!=null && CanMerge(tile,up.tile))
            {
                return false;
            }
            if (Down != null && CanMerge(tile, Down.tile))
            {
                return false;
            }
            if (Left != null && CanMerge(tile, Left.tile))
            {
                return false;
            }
            if (Right != null && CanMerge(tile, Right.tile))
            {
                return false;
            }
        }

        return true;
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
