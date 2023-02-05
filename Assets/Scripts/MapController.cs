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
        Map = 0,
        HoverHighlight = 1,
        FixHiglight = 2,
        Buildings = 3
    }

    //Private
    private Vector3Int bottomLeftBounds;
    private Vector3Int topRightBounds;
    private Tilemap tilemap;
    private Vector3Int oldHighlightCoords;
    private bool oldHighlightSet = false;
    private Map2D map;

    //If selected unit is null, but selectedNode isn't, we have selected an empty node.
    private Unit selectedUnit;
    private MapPos2D selectedPosition;

    private CanvasController canvasController;
    private GameController gameController;

    private List<Vector3Int> fixedHighlights;

    //Public
    public Tile             tile;
    public Tile             blueHighlightTile;
    public Tile             redHighlightTile;
    public Tile             purpleHighlightTile;
    public Vector3Int       tilemapSizeHalf;
    public GameObject       carrotPrefab;
    public GameObject       potatoPrefab;
    public GameObject       radishPrefab;
    public GameObject       bunnyPrefab;
    public GameObject       shedPrefab;
    public GameObject       fieldPrefab;
    public GameObject       woodmillPrefab;
    private GameObject      carrotInst;
    private GameObject      potatoInst;

    [HideInInspector]
    public List<Enemy> enemies;
    public List<Character> characters;
    public List<ResourceCreator> resCreators;

    // Start is called before the first frame update
    void Start()
    {
        canvasController = GameObject.Find("Canvas").GetComponent<CanvasController>();
        gameController   = GameObject.Find("GameController").GetComponent<GameController>();
        fixedHighlights = new List<Vector3Int>();

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

        carrotInst = Instantiate(carrotPrefab, new Vector3(0.5f, 0.5f, -2.0f), Quaternion.identity);
        potatoInst = Instantiate(potatoPrefab, new Vector3(0.5f, 1.5f, -2.0f), Quaternion.identity);

        map.getNode(0, 0).Occupy(carrotInst.GetComponent<Carrot>());
        map.getNode(0, 1).Occupy(potatoInst.GetComponent<Potato>());

        carrotInst.GetComponent<Carrot>().pos = new Vector2Int(0, 0);
        potatoInst.GetComponent<Potato>().pos = new Vector2Int(0, 1);

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector3Int mouseTileMapCoords = tilemap.WorldToCell(mousePos);
        mouseTileMapCoords.z = (int)TilemapLayers.HoverHighlight;

        bool clickedOnUI = isOnSkillPanel(mouseScreen);
        
        // The + 1 is to compensate for the the fact that the coordinates of a tile in its bottom left corner, and not on the skills panel
        if (mousePos.x >= bottomLeftBounds.x && mousePos.y >= bottomLeftBounds.y && mousePos.x <= topRightBounds.x + 1 && mousePos.y <= topRightBounds.y + 1 && !clickedOnUI)
        {
            /*-----------------------
            * Hover Highlight handling
            * ----------------------*/
            Vector3Int newHighlightCoords;

            newHighlightCoords = mouseTileMapCoords;
            tilemap.SetTile(newHighlightCoords, blueHighlightTile);

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
            * Click handling with fix highlight handling
            * ----------------------*/

            if (Input.GetMouseButtonDown(0))
            {
                MapNode clickedNode = map.getNode(mouseTileMapCoords);

                //If no unit is selected, we can select a unit (building or character) or an empty fields
                if (selectedUnit == null)
                {
                    //There was already a selection (that wasn't a unit or we wouldn't be here), clear the selection before a new one.
                    if (selectedPosition != null)
                    {
                        deselect();
                    }

                    //If the node is occuped, it is either a Character, Enemy, or Building
                    if (clickedNode.Occupant != null)
                    {
                        Character chara = clickedNode.Occupant as Character;
                        if (chara != null && chara.isBusy())
                        {
                            return;
                        }

                        selectedUnit = clickedNode.Occupant;
                        selectedUnit.onClicked();
                        selectPos(mouseTileMapCoords);

                        //If we are in a fight phase, the characters movement shadow needs to be displyed
                        if (gameController.phase == Phase.PlayerTurn)
                        {
                            //brute force, check for every tile in a box neightbourhood if it is available. I am sure I could think of a way to calculate which are needed but fuck that.
                            Vector2Int movementShadowBottomRight = new Vector2Int(Mathf.Max(mouseTileMapCoords.x - chara.movementRange, bottomLeftBounds.x), Mathf.Max(mouseTileMapCoords.y - chara.movementRange, bottomLeftBounds.y));
                            Vector2Int movementShadowTopRight    = new Vector2Int(Mathf.Min(mouseTileMapCoords.x + chara.movementRange, topRightBounds.x  ), Mathf.Min(mouseTileMapCoords.y + chara.movementRange, topRightBounds.y  ));
                            
                            //Manually add the starting pos, as it is currently impassable(as the char is standing on it)
                            Vector2Int currPos = clipVect3Int(mouseTileMapCoords);
                            
                            List<PathFinding.PathNode> fullPathGraph = map.generatePathNodeList();
                            fullPathGraph.Add(new PathFinding.PathNode(currPos));

                            for (int i = movementShadowBottomRight.x; i <= movementShadowTopRight.x; i++)
                            {
                                for (int j = movementShadowBottomRight.y; j <= movementShadowTopRight.y; j++)
                                {
                                    Vector2Int posToCheck = new Vector2Int(i, j);
                                    if ( HamiltonianDistance(posToCheck, currPos) <= chara.movementRange)
                                    {
                                        //Maybe copying instead of recreating the list will boost speed;
                                        List<PathFinding.PathNode> fullPathCopy = CopyPNList(fullPathGraph);

                                        List <PathFinding.PathNode> path = PathFinding.FindPath(fullPathCopy, currPos, posToCheck);

                                        if(path.Count > 0 && path.Count <= chara.movementRange)
                                        {
                                            fixHighlightTile(posToCheck, purpleHighlightTile);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //If the node is not occupied, we are selecting an empty field.
                    else
                    {
                        // but empty field selection should only happen in buildphase.
                        if(gameController.phase == Phase.Build)
                        {
                            canvasController.displayBuilderOptions();
                            selectPos(mouseTileMapCoords);
                        }

                    }
                }
                else
                {
                    MapNode previousNode = map.getNode(selectedUnit.pos);

                    if (clickedNode.Occupant == null && previousNode.Leave(selectedUnit))
                    {
                        if(gameController.phase == Phase.Build)
                        {
                            Character c = selectedUnit as Character;
                            c.move(clipVect3Int(mouseTileMapCoords));
                            clickedNode.Occupy(selectedUnit);
                        }
                        else if(gameController.phase == Phase.PlayerTurn)
                        {
                            List<PathFinding.PathNode> path = PathFinding.FindPath(map.generatePathNodeList(), selectedUnit.pos, clipVect3Int(mouseTileMapCoords));
                            Character c = selectedUnit as Character;

                            if (path.Count == 0 || c.movementRange == 0)
                            {
                                //If we can't go anywhere, occupy the tile we just left
                                previousNode.Occupy(selectedUnit);
                            }
                            else
                            {
                                Vector2Int target = path[path.Count - 1].position;

                                if (path.Count > c.movementRange)
                                {
                                    target = path[c.movementRange - 1].position;
                                }

                                c.move(target);
                                map.getNode(target).Occupy(selectedUnit);

                                //TODO: DRAIN MOVEMENT POINTS
                            }


                        }

                    }
                    deselect();
                }

            }

        }
        else if (oldHighlightSet)
        {
            tilemap.SetTile(oldHighlightCoords, null);
            oldHighlightSet = false;
        }
        else if (Input.GetMouseButtonDown(0) && !clickedOnUI)
        {
            deselect();
        }


    }

    public void Attack()
    {
        selectedUnit.gameObject.GetComponent<SpriteAnimator>().SetAnimationByName("Cast Spell");
        deselect();
    }

    public void Skill()
    {
        selectedUnit.gameObject.GetComponent<SpriteAnimator>().SetAnimationByName("Cast Spell");
        deselect();
    }

    public void Repair()
    {
        selectedUnit.gameObject.GetComponent<SpriteAnimator>().SetAnimationByName("Cast Spell");
        deselect();
        

    }

    public void placeUnit(UnitType unitType)
    {
        if (selectedPosition == null) { return; /*oof*/ }

        switch (unitType)
        {
            case UnitType.Field:
                resCreators.Add(Instantiate(fieldPrefab, new Vector3((float)selectedPosition.pos2D.x + 0.5f, (float)selectedPosition.pos2D.y + 0.5f, -2.0f), Quaternion.identity).GetComponent<ResourceCreator>());
                map.getNode(selectedPosition.pos2D).Occupy(resCreators[resCreators.Count-1]);
                break;
            case UnitType.Shed:
                resCreators.Add(Instantiate(shedPrefab, new Vector3((float)selectedPosition.pos2D.x + 0.5f, (float)selectedPosition.pos2D.y + 0.5f, -2.0f), Quaternion.identity).GetComponent<ResourceCreator>());
                map.getNode(selectedPosition.pos2D).Occupy(resCreators[resCreators.Count - 1]);
                break;
            case UnitType.Woodmill:
                resCreators.Add(Instantiate(woodmillPrefab, new Vector3((float)selectedPosition.pos2D.x + 0.5f, (float)selectedPosition.pos2D.y + 0.5f, -2.0f), Quaternion.identity).GetComponent<ResourceCreator>());
                map.getNode(selectedPosition.pos2D).Occupy(resCreators[resCreators.Count-1]);
                break;
            case UnitType.Carrot:
                characters.Add(Instantiate(carrotPrefab, new Vector3((float)selectedPosition.pos2D.x + 0.5f, (float)selectedPosition.pos2D.y + 0.5f, -2.0f), Quaternion.identity).GetComponent<Character>());
                map.getNode(selectedPosition.pos2D).Occupy(characters[characters.Count-1]);
                break;
            case UnitType.Radish:
                characters.Add(Instantiate(radishPrefab, new Vector3((float)selectedPosition.pos2D.x + 0.5f, (float)selectedPosition.pos2D.y + 0.5f, -2.0f), Quaternion.identity).GetComponent<Character>());
                map.getNode(selectedPosition.pos2D).Occupy(characters[characters.Count-1]);
                break;
            case UnitType.Potato:
                characters.Add(Instantiate(potatoPrefab, new Vector3((float)selectedPosition.pos2D.x + 0.5f, (float)selectedPosition.pos2D.y + 0.5f, -2.0f), Quaternion.identity).GetComponent<Character>());
                map.getNode(selectedPosition.pos2D).Occupy(characters[characters.Count-1]);
                break;
            default:
                break; /*oof*/
        }

        deselect();
    }

    public List<PathFinding.PathNode> getPathNodeList()
    {
        return map.generatePathNodeList();
    }

    public Vector2Int clipVect3Int(Vector3Int v)
    {
        return new Vector2Int(v.x, v.y);
    }

    private void selectPos(Vector2Int v)
    {
        selectedPosition = new MapPos2D();
        selectedPosition.pos2D = v;
        fixedHighlights.Add(new Vector3Int(v.x, v.y, (int)TilemapLayers.FixHiglight));
        tilemap.SetTile(fixedHighlights[fixedHighlights.Count-1], blueHighlightTile);
    }

    private void selectPos(Vector3Int v)
    {
        selectedPosition = new MapPos2D();
        selectedPosition.pos2D = clipVect3Int(v);
        fixedHighlights.Add(new Vector3Int(v.x, v.y, (int)TilemapLayers.FixHiglight));
        tilemap.SetTile(fixedHighlights[fixedHighlights.Count - 1], blueHighlightTile);
    }

    private void deselect()
    {
        //   Vector2Int v = map2DToTileMapCoordinates(selectedUnit.pos.x, selectedUnit.pos.y);
        //   tilemap.SetTile(new Vector3Int( v.x, v.y, 2), null);
        selectedUnit = null;
        selectedPosition = null;
        canvasController.clear();
        clearFixHighlightLayer();
    }

    private void clearFixHighlightLayer()
    {
        foreach (var hl in fixedHighlights)
        {
            tilemap.SetTile(hl, null);
        }

        fixedHighlights.Clear();
    }

    public void Die(Unit u)
    {

    }

    public bool isOnSkillPanel(Vector3 mouseScreenPos)
    {
        if(canvasController.displayedGroup == null)
        {
            return false;
        }

        RectTransform canvasRect = canvasController.displayedGroup.GetComponent<RectTransform>();

        if (RectTransformUtility.RectangleContainsScreenPoint(canvasRect, new Vector2(mouseScreenPos.x, mouseScreenPos.y)))
        {
            return true;
        }

        return false;

    }

    public void fixHighlightTile(Vector2Int pos, Tile tile)
    {
        fixedHighlights.Add(new Vector3Int(pos.x, pos.y, (int)TilemapLayers.FixHiglight));
        tilemap.SetTile(fixedHighlights[fixedHighlights.Count-1], tile);
    }

    public int HamiltonianDistance(Vector2Int v1, Vector2Int v2)
    {
        return (Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y));
    }

    public List<PathFinding.PathNode> CopyPNList(List<PathFinding.PathNode> pl)
    {
        var plr = new List<PathFinding.PathNode>();

        foreach(var i in pl)
        {
            plr.Add(new PathFinding.PathNode(i.position));
        }

        return plr;
    }

}
