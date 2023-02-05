using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Phase { Build, PlayerTurn, EnemyTurn }
public enum UnitType { Field, Shed, Woodmill, Potato, Carrot, Radish }

public class GameController : MonoBehaviour
{
    public Phase phase = Phase.Build;

    private MapController mapController;
    private CanvasController canvasController;
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int> {
        { ResourceType.wood, 6 },
        { ResourceType.plough, 6 },
        { ResourceType.fertilizer, 6 },
        { ResourceType.potato, 6 },
        { ResourceType.carrot, 6 },
        { ResourceType.radish, 6 },
    };

    public Dictionary<UnitType, Tuple<ResourceType, int>> unitCosts = new Dictionary<UnitType, Tuple<ResourceType, int>> {
        {UnitType.Field, new Tuple<ResourceType, int>( ResourceType.plough, 2 )},
        {UnitType.Shed, new Tuple<ResourceType, int>( ResourceType.wood, 5 )},
        {UnitType.Woodmill, new Tuple<ResourceType, int>( ResourceType.wood, 5 )},
        {UnitType.Carrot, new Tuple<ResourceType, int>( ResourceType.carrot, 5 )},
        {UnitType.Radish, new Tuple<ResourceType, int>( ResourceType.radish, 5 )},
        {UnitType.Potato, new Tuple<ResourceType, int>( ResourceType.potato, 5 )}
    };
    public Dictionary<ResourceType, int> Resources
    {
        get => resources;
    }

    public void placeUnit(UnitType type)
    {
        resources[unitCosts[type].Item1] -= unitCosts[type].Item2;
        canvasController.updateResources();
        mapController.placeUnit(type);
    }
    private void Start()
    {
        mapController = GameObject.Find("Tilemap").GetComponent<MapController>();
        canvasController = GameObject.Find("Canvas").GetComponent<CanvasController>();
    }

    private void EndBuild()
    {
        phase = Phase.PlayerTurn;
        if (canvasController.displayedGroup)
        {
            foreach (Transform s in canvasController.displayedGroup.gameObject.transform)
            {
                canvasController.changeButtonState(s.gameObject.GetComponent<Button>(), true);
            }
        }
    }

    public void FinishTurn()
    {
        if (phase == Phase.Build)
        {
            EndBuild();
        }
        else if (phase == Phase.PlayerTurn) { EndPlayerTurn(); }
    }
    private void EndPlayerTurn()
    {
        foreach (var ally in mapController.characters)
        {
            ally.refresh();
        }
        if (mapController.enemies.Count == 0)
        {
            EndFight();
        }
        else { EnemyTurn(); }
    }

    private void EnemyTurn()
    {
        phase = Phase.EnemyTurn;
        foreach (var enemy in mapController.enemies)
        {
            enemy.Turn();
        }
        if (mapController.enemies.Count == 0)
        {
            phase = Phase.Build;
        }
    }

    public void Update()
    {
        if (phase == Phase.EnemyTurn)
        {
            foreach (var enemy in mapController.enemies)
            {
                if (enemy.isBusy()) return;
            }

            phase = Phase.PlayerTurn;
            foreach (var enemy in mapController.enemies)
            {
                enemy.refresh();
            }
        }
    }
    private void EndFight()
    {
        foreach (var resoruceCreator in mapController.resCreators)
        {
            var r = resoruceCreator.collectResources();
            resources[r.Item1] += r.Item2;
            resoruceCreator.refresh();
        }
        canvasController.updateResources();
        phase = Phase.Build;
        if (canvasController.displayedGroup)
        {
            foreach (Transform s in canvasController.displayedGroup.gameObject.transform)
            {
                canvasController.changeButtonState(s.gameObject.GetComponent<Button>(), false);
            }
        }
    }
    public bool haveEnoughResourceForUnit(UnitType unit)
    {
        if (Resources[unitCosts[unit].Item1] < unitCosts[unit].Item2)
            return false;
        return true;
    }

}
