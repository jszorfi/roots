using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Phase { Build, PlayerTurn, EnemyTurn }
public enum UnitType { Field, Shed, Woodmill, Potato, Carrot, Radish }


public class GameController : MonoBehaviour
{
    public Phase phase = Phase.Build;
    public TMP_Text buildPhaseText;

    //A turn consists of however many ints have been provided.
    public List<int> turn1;
    public List<int> turn2;
    public List<int> turn3;
    public List<int> turn4;
    public List<int> turn5;
    public List<EnemySpawner> enemySpawnerList;

    public int turn = 1;
    public int wave = 0;
    private List<List<int>> waves;

    private MapController mapController;
    private CanvasController canvasController;
    public Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int> {
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

        waves = new List<List<int>>();

        waves.Add(turn1);
        waves.Add(turn2);
        waves.Add(turn3);
        waves.Add(turn4);
        waves.Add(turn5);
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
        //canvasController.night.enabled = true;
        canvasController.finishTurnInactive.enabled = false;
        canvasController.finishTurnText.text = "Finish turn";
        buildPhaseText.text = "Player's turn";
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

        SpawnEnemiesForWave();

        if (mapController.enemies.Count == 0)
        {
            handleFightPhaseEnd();
        }
        else { EnemyTurn(); }
    }

    private void EnemyTurn()
    {
        canvasController.finishTurnInactive.enabled = true;
        canvasController.finishTurnText.text = "Enemy's turn";
        buildPhaseText.text = "Enemy's turn";
        phase = Phase.EnemyTurn;
        foreach (var enemy in mapController.enemies)
        {
            enemy.Turn();
        }
        if (mapController.enemies.Count == 0)
        {
            EndFight();
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
            wave++;
            foreach (var enemy in mapController.enemies)
            {
                enemy.refresh();
            }
            canvasController.finishTurnInactive.enabled = false;
            canvasController.finishTurnText.text = "Finish turn";
            buildPhaseText.text = "Player's turn";
        }

        bool moreSpawn = false;

        if (waves[turn - 1].Count > wave)
        {
            moreSpawn = true;
        }

        if (phase != Phase.Build && mapController.enemies.Count == 0 && !moreSpawn)
        {
            handleFightPhaseEnd();
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
        //canvasController.night.enabled = false;
        canvasController.finishTurnInactive.enabled = false;
        canvasController.finishTurnText.text = "Finish build";
        buildPhaseText.text = "Building phase";
    }
    public bool haveEnoughResourceForUnit(UnitType unit)
    {
        if (resources[unitCosts[unit].Item1] < unitCosts[unit].Item2)
            return false;
        return true;
    }

    private void SpawnEnemiesForWave()
    {
        List<int> currentWave = waves[turn-1];
        var enemyspawnercopy = enemySpawnerList;

        if (currentWave.Count == 0)
        {
            return;
        }

        if(wave >= currentWave.Count)
        {
            return;
        }

        int toSpawn = currentWave[wave];

        while (toSpawn > 0)
        {
            bool spawned = false;

            while (!spawned)
            {
                int es = UnityEngine.Random.Range(0, enemyspawnercopy.Count - 1);

                if (mapController.SpawnEnemy(enemyspawnercopy[es].pos))
                {
                    spawned = true;
                }
                else
                {
                    enemyspawnercopy.RemoveAt(es);

                    if (enemyspawnercopy.Count == 0)
                    {
                        return;
                    }
                }
            }

            toSpawn--;
        }


    }

    void handleFightPhaseEnd()
    {
        turn++;
        if (turn > 5)
        {
            gameObject.GetComponent<IntroController>().ToOutroScene();
        }
        else
        {
            mapController.theCauldron.GetComponent<Cauldron>().setPhase(turn-2);
            EndFight();
        }
    }

}
