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

    enum SkillRange : int
    {
        CloseQuarters = 1,
        Ranged = 2,
        All = 3
    }

    enum SelectionState
    {
        Building,
        Movement,
        Attack,
        Skill,
        Repair
    }

    enum CurrentSelection
    {
        Nothing,
        Position,
        Unit,
        Error
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
    private bool currentCursoBasic = true;
    private Dictionary<UnitType, GameObject> unitTypeToPrefab;




    private SelectionState selectionState = SelectionState.Building;


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
    public GameObject       cauldronPrefab;
    public GameObject       cowpenPrefab;
    public Sprite           carrotFarm;
    public Sprite           potatoFarm;
    public Sprite           raddishFarm;
    public Texture2D        buildCursor;
    public Texture2D        basicCursor;
    public List<Tree> trees;

    [HideInInspector]
    public List<Enemy> enemies;
    public List<Character> characters;
    public List<ResourceCreator> resCreators;
    public List<Building> buildings;
    public GameObject theCauldron;
    public GameObject cows;

    //If a new unit list is created that is disjoint from the previous ones, add it here.
    public List<Unit> getAllUnits()
    {
        List<Unit> allUnits = new List<Unit>();

        allUnits.AddRange(characters);
        allUnits.AddRange(buildings);
        allUnits.AddRange(enemies);

        return allUnits;
    }

    private void fillUnitTypePrefabDictionary()
    {
        unitTypeToPrefab = new Dictionary<UnitType, GameObject>();
        unitTypeToPrefab.Add(UnitType.Carrot, carrotPrefab);
        unitTypeToPrefab.Add(UnitType.Radish, radishPrefab);
        unitTypeToPrefab.Add(UnitType.Potato, potatoPrefab);
        unitTypeToPrefab.Add(UnitType.Field, fieldPrefab);
        unitTypeToPrefab.Add(UnitType.Shed, shedPrefab);
        unitTypeToPrefab.Add(UnitType.Woodmill, woodmillPrefab);
    }

    private bool unitTypeIsBuilding(UnitType ut)
    {
        if (ut == UnitType.Field || ut == UnitType.Shed || ut == UnitType.Woodmill) return true;

        return false;
    }

    private bool unitTypeIsResGen(UnitType ut)
    {
        if (ut == UnitType.Field || ut == UnitType.Shed || ut == UnitType.Woodmill) return true;

        return false;
    }

    private bool unitTypePlaysBuildSound(UnitType ut)
    {
        if (ut == UnitType.Field || ut == UnitType.Shed || ut == UnitType.Woodmill) return true;

        return false;
    }
    private bool unitTypeIsCharacter(UnitType ut)
    {
        if (ut == UnitType.Carrot || ut == UnitType.Radish || ut == UnitType.Potato) return true;

        return false;
    }

    public bool worldPosOnMap(Vector3 pos)
    {
        return pos.x >= bottomLeftBounds.x && pos.y >= bottomLeftBounds.y && pos.x <= topRightBounds.x + 1 && pos.y <= topRightBounds.y + 1;
    }

    private CurrentSelection currentlySelected()
    {
        if (selectedPosition == null && selectedUnit == null) return CurrentSelection.Nothing;

        if (selectedUnit == null && selectedPosition != null) return CurrentSelection.Position;

        if (selectedPosition != null && selectedPosition != null) return CurrentSelection.Unit;

        return CurrentSelection.Error;
    }



    void Awake()
    {
        fixedHighlights = new List<Vector3Int>();
        fillUnitTypePrefabDictionary();

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

        theCauldron = Instantiate(cauldronPrefab, new Vector3(0.5f, 0.5f, -2.0f), Quaternion.identity);
        map.getNode(0, 0).Occupy(theCauldron.GetComponent<Cauldron>());
        theCauldron.GetComponent<Cauldron>().pos = new Vector2Int(0, 0);
        buildings.Add(theCauldron.GetComponent<Cauldron>());

        cows = Instantiate(cowpenPrefab, new Vector3(1.5f, 0.5f, -2.0f), Quaternion.identity);
        map.getNode(1, 0).Occupy(cows.GetComponent<Cowpen>());
        cows.GetComponent<Cowpen>().pos = new Vector2Int(1, 0);
        buildings.Add(cows.GetComponent<Cowpen>());
        resCreators.Add(cows.GetComponent<Cowpen>());
    }

    // Start is called before the first frame update
    void Start()
    {
        canvasController = GameObject.Find("Canvas").GetComponent<CanvasController>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        makeTrees();
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
        if (worldPosOnMap(mousePos) && !clickedOnUI)
        {
            /*-----------------------
            * Hover Highlight and Cursor handling
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

            MapNode hoverNode = map.getNode(mouseTileMapCoords);

#pragma warning disable
            if (hoverNode.Occupant == null)
            {
                if(currentCursoBasic)
                {
                    Cursor.SetCursor(buildCursor, Vector2.zero, CursorMode.ForceSoftware);
                    currentCursoBasic = false;
                }
            }
            else
            {
                if (!currentCursoBasic)
                {
                    Cursor.SetCursor(basicCursor, Vector2.zero, CursorMode.ForceSoftware);
                    currentCursoBasic = true;
                }
            }
#pragma warning restore

            /*-----------------------
            * Click handling with fix highlight handling
            * ----------------------*/

            if (Input.GetMouseButtonDown(0))
            {
                handleClick(mouseTileMapCoords);
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

    private void handleClick(Vector3Int clickedMapCoords)
    {
        MapNode clickedNode = map.getNode(clickedMapCoords);
        Character chara = selectedUnit as Character;

        switch (currentlySelected())
        {
            case CurrentSelection.Nothing:

                selectMapNode(clipVect3Int(clickedMapCoords));

                break;
            case CurrentSelection.Position:

                deselect();
                selectMapNode(clipVect3Int(clickedMapCoords));

                break;
            case CurrentSelection.Unit:

                int hamDist = HamiltonianDistance(selectedPosition.pos2D, clipVect3Int(clickedMapCoords));

                //TODO: Selection State should be renamed, as it is not really accurate
                switch (selectionState)
                {
                    case SelectionState.Building:
                        break;

                    case SelectionState.Movement:

                        MapNode previousNode = map.getNode(selectedUnit.pos);

                        if (clickedNode.Occupant == null && selectedUnit is Character && !(selectedUnit is Enemy) && gameController.phase != Phase.EnemyTurn)
                        {

                            previousNode.Leave(selectedUnit);

                            if (gameController.phase == Phase.Build)
                            {
                                chara.move(clipVect3Int(clickedMapCoords));
                                clickedNode.Occupy(selectedUnit);
                            }
                            else if (gameController.phase == Phase.PlayerTurn)
                            {
                                List<PathFinding.PathNode> path = PathFinding.FindPath(map.generatePathNodeList(), selectedUnit.pos, clipVect3Int(clickedMapCoords));

                                if (path.Count == 0 || chara.currentMovement == 0)
                                {
                                    //If we can't go anywhere, occupy the tile we just left
                                    previousNode.Occupy(selectedUnit);
                                }
                                else
                                {
                                    Vector2Int target = path[path.Count - 1].position;

                                    if (path.Count > chara.currentMovement)
                                    {
                                        target = path[chara.currentMovement - 1].position;
                                        chara.currentMovement = 0;
                                    }
                                    else
                                    {
                                        chara.currentMovement -= path.Count;
                                    }

                                    chara.move(target);
                                    map.getNode(target).Occupy(selectedUnit);
                                }
                            }
                        }

                        deselect();

                        break;
                    case SelectionState.Attack:
                        //We can (hopefully) safely assume that the selected unit is a character

                        if (clickedNode.Occupant != null)
                        {

                            if ((chara.skillkRange == (int)SkillRange.CloseQuarters || chara.skillkRange == (int)SkillRange.Ranged) && hamDist == chara.skillkRange)
                            {
                                chara.targetedSkill(clickedNode.Occupant);
                            }
                            else if (chara.skillkRange == (int)SkillRange.All && hamDist <= chara.skillkRange)
                            {
                                chara.targetedSkill(clickedNode.Occupant);
                            }

                        }

                        deselect();

                        break;

                    case SelectionState.Skill:
                        //We can (hopefully) safely assume that the selected unit is a character
                        chara = selectedUnit as Character;
                        System.Predicate<Unit> distancePredicate = null;

                        //TODO: Simplyfy, the ranges could be simplified if we mainained it with an integer list perhaps? So say, all units 1 distance, 2 distance, etc
                        switch (chara.skillkRange)
                        {
                            case (int)SkillRange.CloseQuarters:
                                if (hamDist == chara.skillkRange)
                                {
                                    distancePredicate = x => { int hamiltDist = HamiltonianDistance(x.pos, selectedPosition.pos2D); return (hamiltDist == chara.skillkRange); };
                                }
                                break;
                            case (int)SkillRange.Ranged:
                                if (hamDist == chara.skillkRange)
                                {
                                    distancePredicate = x => { int hamiltDist = HamiltonianDistance(x.pos, selectedPosition.pos2D); return (hamiltDist == chara.skillkRange); };
                                }
                                break;
                            case (int)(int)SkillRange.All:
                                if (hamDist <= chara.skillkRange)
                                {
                                    distancePredicate = x => { int hamiltDist = HamiltonianDistance(x.pos, selectedPosition.pos2D); return (hamiltDist == 2 || hamiltDist == 1); };
                                }
                                break;
                            default:
                                break;
                        }

                        if (distancePredicate != null)
                        {
                            chara.areaSkill(allUnitsInANeighbourhoodOf(distancePredicate));
                        }

                        deselect();

                        break;
                    case SelectionState.Repair:

                        //If we clicked where there is no unit, we can stop the desection
                        if (clickedNode.Occupant != null && HamiltonianDistance(selectedPosition.pos2D, clipVect3Int(clickedMapCoords)) <= 2 && clickedNode.Occupant is Building)
                        {
                            chara.repair(clickedNode.Occupant as Building);
                        }

                        deselect();

                        break;
                    default:
                        break;
                }

                break;
            case CurrentSelection.Error:
                Debug.LogError("Current Selection Is Error");
                break;
        }

    }

    private void selectMapNode(Vector2Int clickedPosition)
    {
        MapNode clickedNode = map.getNode(clickedPosition);
        Character chara;

        if (clickedNode.Occupant != null)
        {
            //Currently nothing is selected, and we clicked on a non empty position => we select whatever we clicked on
            //TODO: Maybe put a call to movementshadow into the unit onclicked;

            selectedUnit = clickedNode.Occupant;
            selectedUnit.onClicked();

            //If we click on a character, we can select it and display it's movement range (the movement range only in fight mode), even if it is an enemy
            if (clickedNode.Occupant is Character)
            {
                chara = clickedNode.Occupant as Character;
                selectionState = SelectionState.Movement;

                if (chara is Carrot || chara is Potato || chara is Radish)
                {
                    chara.gameObject.GetComponent<AudioPlayer>().PlayAudioByName("Select");
                }
            }
        }
        else
        {
            //Currently nothing is selected, and we clicked on an empty position

            if (gameController.phase == Phase.Build)
            {
                //We can build in build phase
                canvasController.displayBuilderOptions();
            }
        }

        selectPos(clickedPosition);
    }

    //Below 2 functions could probably be merged, with a public interfaces to the outside
    public void displayMovementShadow(Vector2Int center, int movementRange)
    {
        //brute force, check for every tile in a box neightbourhood if it is available. I am sure I could think of a way to calculate which are needed but fuck that.
        Vector2Int movementShadowBottomLeft = new Vector2Int(Mathf.Max(center.x - movementRange, bottomLeftBounds.x), Mathf.Max(center.y - movementRange, bottomLeftBounds.y));
        Vector2Int movementShadowTopRight = new Vector2Int(Mathf.Min(center.x + movementRange, topRightBounds.x), Mathf.Min(center.y + movementRange, topRightBounds.y));

        //Manually add the starting pos, as it is currently impassable(as the char is standing on it)
        Vector2Int currPos = center;

        List<PathFinding.PathNode> fullPathGraph = map.generatePathNodeList();
        fullPathGraph.Add(new PathFinding.PathNode(currPos));

        List<Vector2Int> positionsToCheck = new List<Vector2Int>();

        for (int i = movementShadowBottomLeft.x; i <= movementShadowTopRight.x; i++)
        {
            for (int j = movementShadowBottomLeft.y; j <= movementShadowTopRight.y; j++)
            {
                Vector2Int posToCheck = new Vector2Int(i, j);

                //As a tile cost is at minimum 1, we cannot possibly move more than this
                if (HamiltonianDistance(posToCheck, currPos) <= movementRange)
                {
                    positionsToCheck.Add(posToCheck);
                }
            }
        }

        int index = 0;
        //Movement shadow is never going to be bigger than a 1000 squares, hopefully....
        for (int i = 0; i < 1000; i++)
        {
            if (index >= positionsToCheck.Count)
            {
                break;
            }

            Vector2Int p = positionsToCheck[index];

            //Maybe copying instead of recreating the list will boost speed;
            List<PathFinding.PathNode> fullPathCopy = CopyPNList(fullPathGraph);
            List<PathFinding.PathNode> path = PathFinding.FindPath(fullPathCopy, currPos, p);

            for (int j = 0; j < Mathf.Min(movementRange, path.Count); j++)
            {
                fixHighlightTile(path[j].position, purpleHighlightTile);
                positionsToCheck.Remove(path[j].position);
            }

            //if p was unreachable, it wasn't removed, so it should be
            positionsToCheck.Remove(p);
        }
    }

    private void highlightRedFromCenter(Vector2Int center, SkillRange sr)
    {
        //This could be made prettier, I am sure, but right now don't care
        //brute force, check for every tile in a box neightbourhood if it is available. I am sure I could think of a way to calculate which are needed but fuck that.
        int radius = 10;

        switch (sr)
        {
            case SkillRange.CloseQuarters:
                radius = 1;
                break;
            case SkillRange.Ranged:
            case SkillRange.All:
                radius = 2;
                break;
        }


        Vector2Int selectShadowBottomLeft = new Vector2Int(Mathf.Max(center.x - radius, bottomLeftBounds.x), Mathf.Max(center.y - radius, bottomLeftBounds.y));
        Vector2Int selectShadowTopRight = new Vector2Int(Mathf.Min(center.x + radius, topRightBounds.x), Mathf.Min(center.y + radius, topRightBounds.y));

        for (int i = selectShadowBottomLeft.x; i <= selectShadowTopRight.x; i++)
        {
            for (int j = selectShadowBottomLeft.y; j <= selectShadowTopRight.y; j++)
            {
                Vector2Int posToCheck = new Vector2Int(i, j);
                int hamDist = HamiltonianDistance(posToCheck, center);

                if ((sr == SkillRange.CloseQuarters || sr == SkillRange.Ranged) && hamDist == radius)
                {
                    fixHighlightTile(posToCheck, redHighlightTile);
                }
                else if (sr == SkillRange.All && hamDist <= radius)
                {
                    fixHighlightTile(posToCheck, redHighlightTile);
                }

            }
        }

    }

    public void Attack()
    {
        selectionState = SelectionState.Attack;
        Character chara = selectedUnit as Character;
        highlightRedFromCenter(selectedUnit.pos, (SkillRange)chara.skillkRange);
    }

    public void Skill()
    {
        selectionState = SelectionState.Skill;
        Character chara = selectedUnit as Character;
        highlightRedFromCenter(selectedUnit.pos, (SkillRange)chara.skillkRange);
    }

    public void Repair()
    {
        selectionState = SelectionState.Repair;
        Character chara = selectedUnit as Character;
        highlightRedFromCenter(selectedUnit.pos, SkillRange.All);
    }

    //TODO, check if you can substitute this with reflection, what I mean is replacing this with actual typeChecks
    public void placeUnit(UnitType unitType)
    {
        if (selectedPosition == null) { return; /*oof*/ }

        //The instantiation uses world coordinates, so we need to translate the object to the center of the cell;
        Vector3 actualPosition = new Vector3((float)selectedPosition.pos2D.x + 0.5f, (float)selectedPosition.pos2D.y + 0.5f, -2.0f);
        GameObject prefabToInstantiate = unitTypeToPrefab[unitType];
        GameObject inst = Instantiate(prefabToInstantiate, actualPosition, Quaternion.identity);
        Unit u = inst.GetComponent<Unit>();
        u.pos = selectedPosition.pos2D;
        map.getNode(selectedPosition.pos2D).Occupy(u);

        if (unitTypeIsBuilding(unitType))
        {
            buildings.Add(resCreators[resCreators.Count - 1]);
        }
        if (unitTypeIsResGen(unitType))
        {
            resCreators.Add(inst.GetComponent<ResourceCreator>());
        }
        if(unitTypeIsCharacter(unitType))
        {
            characters.Add(inst.GetComponent<Character>());
        }

        if (unitTypePlaysBuildSound(unitType))
        {
            inst.GetComponent<AudioPlayer>().PlayAudioByName("Build");
        }

        deselect();
    }

    public List<PathFinding.PathNode> getPathNodeList()
    {
        return map.generatePathNodeList();
    }

    private void selectPos(Vector2Int v)
    {
        selectedPosition = new MapPos2D();
        selectedPosition.pos2D = v;
        fixedHighlights.Add(new Vector3Int(v.x, v.y, (int)TilemapLayers.FixHiglight));
        tilemap.SetTile(fixedHighlights[fixedHighlights.Count - 1], blueHighlightTile);
    }

    private void selectPos(Vector3Int v)
    {
        selectedPosition = new MapPos2D();
        selectedPosition.pos2D = clipVect3Int(v);
        fixedHighlights.Add(new Vector3Int(v.x, v.y, (int)TilemapLayers.FixHiglight));
        tilemap.SetTile(fixedHighlights[fixedHighlights.Count - 1], blueHighlightTile);
    }

    public void deselect()
    {
        selectionState = SelectionState.Building;
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
        map.getNode(u.pos).Leave(u);

        var enem = u as Enemy;
        if (enem != null)
        {
            enemies.Remove(enem);
            return;
        }

        var chara = u as Character;
        if (chara != null)
        {
            characters.Remove(chara);
            return;
        }

        var build = u as Building;
        if (build != null)
        {
            buildings.Remove(build);
        }

        var res = u as ResourceCreator;
        if (res != null)
        {
            resCreators.Remove(res);
        }

    }
    public bool isOnSkillPanel(Vector3 mouseScreenPos)
    {
        //We are on the next turn button
        if (RectTransformUtility.RectangleContainsScreenPoint(canvasController.finishTurn.gameObject.GetComponent<RectTransform>(), new Vector2(mouseScreenPos.x, mouseScreenPos.y)))
        {
            return true;
        }

        //If not, we may be on the currently displayed skillbar
        if (canvasController.displayedGroup == null)
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
        tilemap.SetTile(fixedHighlights[fixedHighlights.Count - 1], tile);
    }

    public int HamiltonianDistance(Vector2Int v1, Vector2Int v2)
    {
        return (Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y));
    }

    public List<PathFinding.PathNode> CopyPNList(List<PathFinding.PathNode> pl)
    {
        var plr = new List<PathFinding.PathNode>();

        foreach (var i in pl)
        {
            plr.Add(new PathFinding.PathNode(i.position));
        }

        return plr;
    }

    public void plantCarrot()
    {
        plant(SeedType.carrot, carrotFarm);
    }

    public void plantRadish()
    {
        plant(SeedType.radish, raddishFarm);
    }

    public void plantPotato()
    {
        plant(SeedType.potato, potatoFarm);
    }

    public void fertilize()
    {
        Field f = selectedUnit as Field;
        f.fertilize();
        f.GetComponent<AudioPlayer>().PlayAudioByName("Fertilize");
        deselect();
    }
    public void moveEnemy(Enemy e, Vector2Int target)
    {
        //Since the target is always a unit or building, we will need to add back it to the list.
        List<PathFinding.PathNode> pathGraph = map.generatePathNodeList();
        pathGraph.Add(new PathFinding.PathNode(e.pos));
        pathGraph.Add(new PathFinding.PathNode(target));

        List<PathFinding.PathNode> path = PathFinding.FindPath(pathGraph, e.pos, target);

        if (path.Count == 0)
        {
            return;
        }
        else
        {
            //If we can, we try to move to the goal, but we may be able to move less than we liked
            int index = Mathf.Min(path.Count, e.currentMovement) - 1;

            if (index <= 0) return;
            e.currentMovement = 0;

            Vector2Int moveTo = path[index].position;

            //if we would arrive at the target, which is occupied, stop at the last stop instead, which is right next to it, and must
            //be free, as we did not trick the pathing algorithm to think it is.
            if (moveTo == target)
            {
                moveTo = path[index - 1].position;
            }

            map.getNode(e.pos).Leave(e);
            e.move(moveTo);
            map.getNode(moveTo).Occupy(e);
        }
    }

    public List<Unit> neighbours4(Vector2Int pos)
    {
        List<Unit> r = new List<Unit>();
        List<Unit> allUnits = new List<Unit>();

        allUnits.AddRange(characters);
        allUnits.AddRange(buildings);

        foreach (var item in allUnits)
        {
            if (HamiltonianDistance(item.pos, pos) == 1)
            {
                r.Add(item);
            }
        }

        return r;
    }

    /*
     * Get units in the neighbourhood of pos defined by isInDistance predicate
     */
    public List<Unit> allUnitsInANeighbourhoodOf(System.Predicate<Unit> isInDistance)
    {
        List<Unit> allUnits = getAllUnits();
        return allUnits.FindAll(isInDistance);
    }


    private void plant(SeedType s, Sprite spr)
    {
        Field f = selectedUnit as Field;
        f.planted = s;

        f.gameObject.GetComponent<SpriteRenderer>().sprite = spr;
        f.gameObject.GetComponent<AudioPlayer>().PlayAudioByName("Seed");

        deselect();
    }

    public bool SpawnEnemy(Vector2Int pos)
    {
        if (map.getNode(pos).Occupant != null)
        {
            return false;
        }

        enemies.Add(Instantiate(bunnyPrefab, new Vector3((float)pos.x + 0.5f, (float)pos.y + 0.5f, -2.0f), Quaternion.identity).GetComponent<Enemy>());

        map.getNode(pos).Occupy(enemies[enemies.Count-1].GetComponent<Enemy>());

        enemies[enemies.Count - 1].GetComponent<Enemy>().pos = pos;
        return true;

    }

    public void makeTrees()
    {
        foreach(var t in trees)
        {
            map.makeImpassable(t.pos);
        }
    }

    public Vector2Int clipVect3Int(Vector3Int v)
    {
        return new Vector2Int(v.x, v.y);
    }
}
