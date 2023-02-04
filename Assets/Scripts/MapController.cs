using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    //Private
    private Vector3Int  bottomLeftBounds;
    private Vector3Int  topRightBounds;
    private Tilemap     tilemap;
    private Vector3Int  oldHighlight;
    private bool        oldHighlightSet = false;
    private Map2D map;
    private Unit selectedUnit;
    private CanvasController canvasController;

    //Public
    public Tile         tile;
    public Tile         highlightTile;
    public Vector3Int   tilemapSizeHalf;
    public GameObject  carrot;
    private GameObject carrotInst;
    public GameObject  potato;
    private GameObject potatoInst;

    [HideInInspector]
    public List<Enemy>              enemies;
    public List<Character>          characters;
    public List<ResoruceCreator>    resCreators;

    // Start is called before the first frame update
    void Start()
    {
        canvasController = GameObject.Find("Canvas").GetComponent<CanvasController>();

        gameObject.transform.position = new Vector3(0, 0, 0);
        tilemap = gameObject.GetComponent<Tilemap>();
        tilemap.origin = new Vector3Int(0, 0, 0);
        tilemap.size = tilemapSizeHalf * 2 + new Vector3Int(1, 1, 0);

        //The cooridnates given here are to be interpreted as the whole tile. So the topright bound 3,3 means the topright bound of tile 3,3, which
        //in wordspace coordiantes means 4,4, as the coordiante of the tile in wordspace is the bottomleft corner

        bottomLeftBounds = new Vector3Int(tilemap.origin.x - tilemapSizeHalf.x, tilemap.origin.y - tilemapSizeHalf.y, 0);
        topRightBounds = new Vector3Int(tilemap.origin.x + tilemapSizeHalf.x, tilemap.origin.y + tilemapSizeHalf.y, 0);
        tilemap.BoxFill(bottomLeftBounds, tile, bottomLeftBounds.x, bottomLeftBounds.y, topRightBounds.x, topRightBounds.y);
        tilemap.ResizeBounds();

        map = new Map2D(tilemap.size);

        carrotInst = Instantiate(carrot, new Vector3(0.5f, 0.5f, -2.0f), Quaternion.identity);
        potatoInst = Instantiate(potato, new Vector3(0.5f, 1.5f, -2.0f), Quaternion.identity);
        
        map.getNode( clipVect3Int(tileMapToMap2DCoordinates(0, 0)) ).Occupy(carrotInst.GetComponent<Carrot>());
        map.getNode( clipVect3Int(tileMapToMap2DCoordinates(0, 1))).Occupy(potatoInst.GetComponent<Potato>());

        carrotInst.GetComponent<Carrot>().pos = clipVect3Int(tileMapToMap2DCoordinates(0, 0));
        potatoInst.GetComponent<Potato>().pos = clipVect3Int(tileMapToMap2DCoordinates(0, 1));


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // The + 1 is to compensate for the the fact that the coordinates of a tile in its bottom left corner
        if (mousePos.x >= bottomLeftBounds.x && mousePos.y >= bottomLeftBounds.y && mousePos.x <= topRightBounds.x + 1 && mousePos.y <= topRightBounds.y + 1)
        {
            /*-----------------------
            * Highlight handling
            * ----------------------*/
            Vector3Int newHighlight;


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

            Vector3Int tileMapCoordinates = new Vector3Int((int)mousePos.x + xoffset, (int)mousePos.y + yoffset, 1);

            newHighlight = tileMapCoordinates;
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

            /*-----------------------
            * Click handling
            * ----------------------*/

            if (Input.GetMouseButtonDown(0))
            {
                Vector3Int actualMapCoordinates = tileMapToMap2DCoordinates(tileMapCoordinates);
                MapNode clickedNode = map.getNode(actualMapCoordinates);

                //If no unit is selected, we can only select a unit
                if (selectedUnit == null)
                {
                    if (clickedNode.Occupant != null)
                    {
                        selectedUnit = clickedNode.Occupant;
                        selectedUnit.onClicked();
                    }
                }
                else
                {
                    MapNode previousNode = map.getNode(selectedUnit.pos);

                    if (clickedNode.Occupant == null && previousNode.Leave(selectedUnit))
                    {
                        Character c = selectedUnit as Character;
                        c.move(clipVect3Int(actualMapCoordinates), clipVect3Int(tileMapCoordinates));
                        clickedNode.Occupy(selectedUnit);
                    }
                    unselectUnit();
                }

            }

        }
        else if (oldHighlightSet)
        {
            tilemap.SetTile(oldHighlight, null);
            oldHighlightSet = false;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            unselectUnit();
        }


    }

    public List<PathFinding.PathNode> getPathNodeList()
    {
        List<PathFinding.PathNode> pathList = new List<PathFinding.PathNode>();

        Vector2Int size = map.getSize();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                MapNode n = map.getNode(i, j);

                if (n.Cost >= 0)
                {
                    pathList.Add(new PathFinding.PathNode(map2DToTileMapCoordinates(i, j)));
                }
            }
        }

        return pathList;
    }

    public Vector3Int tileMapToMap2DCoordinates(Vector3Int v)
    {
        return new Vector3Int(v.x + tilemapSizeHalf.x, v.y + tilemapSizeHalf.y, 0);
    }

    public Vector3Int map2DToTileMapCoordinates(Vector3Int v)
    {
        return new Vector3Int(v.x - tilemapSizeHalf.x, v.y - tilemapSizeHalf.y, 0);
    }

    public Vector2Int tileMapToMap2DCoordinates(Vector2Int v)
    {
        return new Vector2Int(v.x + tilemapSizeHalf.x, v.y + tilemapSizeHalf.y);
    }

    public Vector2Int map2DToTileMapCoordinates(Vector2Int v)
    {
        return new Vector2Int(v.x - tilemapSizeHalf.x, v.y - tilemapSizeHalf.y);
    }

    public Vector3Int tileMapToMap2DCoordinates(int x, int y)
    {
        return new Vector3Int(x + tilemapSizeHalf.x, y + tilemapSizeHalf.y, 0);
    }

    public Vector2Int map2DToTileMapCoordinates(int x, int y)
    {
        return new Vector2Int(x - tilemapSizeHalf.x, y - tilemapSizeHalf.y);
    }

    public Vector2Int clipVect3Int(Vector3Int v)
    {
        return new Vector2Int(v.x, v.y);
    }

    private void unselectUnit()
    {
        selectedUnit = null;
        canvasController.clear();
    }

    public void Die(Unit u)
    {

    }

}
