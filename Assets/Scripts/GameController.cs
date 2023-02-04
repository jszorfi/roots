using System.Collections.Generic;
using UnityEngine;

public enum Phase { Build, PlayerTurn, EnemyTurn }
public enum BuildingType { Field, Shed, Woodmill }

public class GameController : MonoBehaviour
{
    public Phase phase = Phase.Build;

    private MapController mapController;
    private CanvasController canvasController;
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int> {
        { ResourceType.wood, 0 },
        { ResourceType.plough, 0 },
        { ResourceType.fertilizer, 0 },
        { ResourceType.potato, 0 },
        { ResourceType.carrot, 0 },
        { ResourceType.radish, 0 },
    };
    public Dictionary<ResourceType, int> Resources
    {
        get => resources;
    }
    private void Start()
    {
        mapController = GameObject.Find("TileMap").GetComponent<MapController>();
        canvasController = GameObject.Find("Canvas").GetComponent<CanvasController>();
        canvasController.updateResources();
    }

    public void EndBuild()
    {

    }
    public void EndFight()
    {
        foreach (var resoruceCreator in mapController.resCreators)
        {
            var r = resoruceCreator.collectResources();
            resources[r.Item1] += r.Item2;
        }
        canvasController.updateResources();
    }
}
