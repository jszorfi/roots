using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tree : MonoBehaviour
{
    private MapController mapController;
    public Vector2Int pos;

    void Start()
    {
        mapController = GameObject.Find("Tilemap").GetComponent<MapController>();
        pos = mapController.clipVect3Int(mapController.gameObject.GetComponent<Tilemap>().LocalToCell(gameObject.transform.position));
    }
}
