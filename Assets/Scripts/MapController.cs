using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapNode
{
    private int origCost;
    private int cost;
    public int Cost
    {
        get { return cost; }
        set { cost = value; origCost = value; }
    }

    public MapNode(int c)
    {
        origCost = c;
        cost = origCost;
    }

    public bool Occupy()
    {
        if (cost >= 0)
        {
            cost = -1;
            return true;
        }
        else 
        {
            return false;
        }
    }

    public bool Leave()
    {
        if (cost < 0)
        {
            cost = origCost;
            return true;
        }
        else
        {
            return false;
        }
    }



}

public class Map2D
{
    private MapNode[,] map;
    private Vector2Int size;
    
    public Map2D(int sx, int sy)
    {
        init(sx, sy);
    }

    public Map2D(Vector3Int v3)
    {
        init(v3.x, v3.y);
    }

    public void init(int sx, int sy)
    {
        map = new MapNode[sx, sy];
        size.x = sx; size.y = sy;


        for (int i = 0; i < sx; i++)
        {
            for (int j = 0; j < sy; j++)
            {
                map[i, j] = new MapNode(0);
            }
        }
    }

    public Vector2Int getSize()
    {
        return size;
    }

    public MapNode getNode(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < size.x && y < size.y)
        {
            return map[x, y];
        }

        else return null;
    }
}

public class MapController : MonoBehaviour
{
    public Tile         tile;
    public Tile         highlightTile;
    public Vector3Int   tilemapSizeHalf;
    public Map2D          map;
    private Vector3Int  bottomLeftBounds;
    private Vector3Int  topRightBounds;
    private Tilemap     tilemap;
    private Vector3Int  oldHighlight;
    private bool        oldHighlightSet = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(0, 0, 0);
        tilemap = gameObject.GetComponent<Tilemap>();
        tilemap.origin = new Vector3Int(0, 0, 0);
        tilemap.size = tilemapSizeHalf*2 + new Vector3Int(1,1,0);

        //The cooridnates given here are to be interpreted as the whole tile. So the topright bound 3,3 means the topright bound of tile 3,3, which
        //in wordspace coordiantes means 4,4, as the coordiante of the tile in wordspace is the bottomleft corner

        bottomLeftBounds = new Vector3Int(tilemap.origin.x - tilemapSizeHalf.x, tilemap.origin.y - tilemapSizeHalf.y, 0);
        topRightBounds = new Vector3Int(tilemap.origin.x + tilemapSizeHalf.x, tilemap.origin.y + tilemapSizeHalf.y, 0);
        tilemap.BoxFill(bottomLeftBounds, tile, bottomLeftBounds.x, bottomLeftBounds.y, topRightBounds.x, topRightBounds.y);
        tilemap.ResizeBounds();

        map = new Map2D(tilemap.size);
    }

    // Update is called once per frame
    void Update()
    {
        //Highlight handling
        Vector3Int newHighlight;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // The + 1 is to compensate for the the fact that the coordinates of a tile in its bottom left corner
        if (mousePos.x >= bottomLeftBounds.x && mousePos.y >= bottomLeftBounds.y && mousePos.x <= topRightBounds.x + 1 && mousePos.y <= topRightBounds.y + 1)
        {

            //The offsets are so when the negative coordinates are considered the bound calculations still work.
            int xoffset = 0;
            int yoffset = 0;

            if (mousePos.x < 0)
            {
                xoffset--;
            }
            if (mousePos.y < 0)
            {
                yoffset--;
            }

            newHighlight = new Vector3Int((int)mousePos.x + xoffset, (int)mousePos.y + yoffset, 1);
            tilemap.SetTile(newHighlight, highlightTile);

            if (!oldHighlightSet)
            {
                oldHighlight = newHighlight;
                oldHighlightSet = true;
            }
            if (newHighlight != oldHighlight)
            {
                tilemap.SetTile(oldHighlight, null);
                oldHighlight = newHighlight;
            }
        }
        else if(oldHighlightSet)
        {
            tilemap.SetTile(oldHighlight, null);
            oldHighlightSet = false;
        }
        
    }

    public List<PathFinding.PathNode> getPathNodeList()
    {
        List<PathFinding.PathNode> pathList = new List<PathFinding.PathNode>();

        Vector2Int size = map.getSize();

        for(int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                MapNode n = map.getNode(i, j);

                if (n.Cost >= 0)
                {
                    pathList.Add(new PathFinding.PathNode(new Vector2Int(i,j)));
                }
            }
        }

        return pathList;
    }
}
