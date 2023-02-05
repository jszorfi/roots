using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    private MapController mapController;
    private Vector2Int pos;


    public void setPos(Vector2Int p)
    {
        pos = p;
    }

    void Start()
    {
       mapController = GameObject.Find("Tilemap").GetComponent<MapController>();

       pos = mapController.clipVect3Int(mapController.gameObject.GetComponent<Tilemap>().WorldToCell(gameObject.transform.position));
    }
    

}
