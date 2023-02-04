using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    /*
     * Tile layer
     * 0 - Map
     * 1 - Hover Highlight
     * 2 - Fix Highlight
     * 3 - Characters and buildings
     */

    enum TilemapLayers : int
    {
        Map             = 0,
        HoverHighlight  = 1,
        FixHiglight     = 2,
        Buildings       = 3
    }

    //Private
    private Vector3Int  bottomLeftBounds;
    private Vector3Int  topRightBounds;
    private Tilemap     tilemap;
    private Vector3Int  oldHighlightCoords;
    private bool        oldHighlightSet = false;
    private Map2D map;
    private Unit selectedUnit;
    private CanvasController canvasController;

    //Public
    public Tile         tile;
    public Tile         highlightTile;
    public Vector3Int   tilemapSizeHalf;
    public GameObject   carrot;
    private GameObject  carrotInst;
    public GameObject   potato;
    private GameObject  potatoInst;

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

        map = new Map2D(tilemap.size, tilemapSizeHalf.x, tilemapSizeHalf.y);

        carrotInst = Instantiate(carrot, new Vector3(0.5f, 0.5f, -2.0f), Quaternion.identity);
        potatoInst = Instantiate(potato, new Vector3(0.5f, 1.5f, -2.0f), Quaternion.identity);
        
        map.getNode(0,0).Occupy(carrotInst.GetComponent<Carrot>());
        map.getNode(0,1).Occupy(potatoInst.GetComponent<Potato>());

        carrotInst.GetComponent<Carrot>().pos = new Vector2Int(0, 0);
        potatoInst.GetComponent<Potato>().pos = new Vector2Int(0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //The offsets are so when the negative coordinates are considered the bound calculations still work.
        int xoffset = 0;
        int yoffset = 0;

        if (mousePos.x < 0) { xoffset--; }
        if (mousePos.y < 0) { yoffset--; }

        Vector3Int mouseTileMapCoords = new Vector3Int((int)mousePos.x + xoffset, (int)mousePos.y + yoffset, (int)TilemapLayers.HoverHighlight);

        // The + 1 is to compensate for the the fact that the coordinates of a tile in its bottom left corner
        if (mousePos.x >= bottomLeftBounds.x && mousePos.y >= bottomLeftBounds.y && mousePos.x <= topRightBounds.x + 1 && mousePos.y <= topRightBounds.y + 1)
        {
            /*-----------------------
            * Highlight handling
            * ----------------------*/
            Vector3Int newHighlightCoords;

            newHighlightCoords = mouseTileMapCoords;
            tilemap.SetTile(newHighlightCoords, highlightTile);

            if (!oldHighlightSet)
            {
                oldHighlightCoords = newHighlightCoords;
                oldHighlightSet = true;
            }
            if (newHighlightCoords != oldHighlightCoords)
            {
                tilemap.SetTile(oldHighlightCoords, null);
                oldHighlightCoords = newHighlightCoords;
            }

            /*-----------------------
            * Click handling
            * ----------------------*/

            if (Input.GetMouseButtonDown(0))
            {
                MapNode clickedNode = map.getNode(mouseTileMapCoords);

                //If no unit is selected, we can only select a unit
                if (selectedUnit == null)
                {
                    if (clickedNode.Occupant != null)
                    {
                        Character chara = clickedNode.Occupant as Character;
                        if(chara != null && chara.isBusy())
                        {
                            return;
                        }

                        selectedUnit = clickedNode.Occupant;
                        selectedUnit.onClicked();
                 //       tilemap.SetTile(new Vector3Int(tileMapCoordinates.x, tileMapCoordinates.y, 2), highlightTile);
                    }
                }
                else
                {
                    MapNode previousNode = map.getNode(selectedUnit.pos);

                    if (clickedNode.Occupant == null && previousNode.Leave(selectedUnit))
                    {
                        Character c = selectedUnit as Character;
                        c.move(clipVect3Int(mouseTileMapCoords));
                        clickedNode.Occupy(selectedUnit);
                    }
                    deselectUnit();
                }

            }

        }
        else if (oldHighlightSet)
        {
            tilemap.SetTile(oldHighlightCoords, null);
            oldHighlightSet = false;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            deselectUnit();
        }


    }

    public List<PathFinding.PathNode> getPathNodeList()
    {
        return map.generatePathNodeList();
    }

    public Vector2Int clipVect3Int(Vector3Int v)
    {
        return new Vector2Int(v.x, v.y);
    }

    private void deselectUnit()
    {
     //   Vector2Int v = map2DToTileMapCoordinates(selectedUnit.pos.x, selectedUnit.pos.y);
     //   tilemap.SetTile(new Vector3Int( v.x, v.y, 2), null);
        selectedUnit = null;
        canvasController.clear();
    }

    public void Die(Unit u)
    {

    }

}
