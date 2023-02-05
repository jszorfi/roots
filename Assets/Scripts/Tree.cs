using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tree : MonoBehaviour
{
    private MapController mapController;

    void Start()
    {
        mapController = GameObject.Find("Tilemap").GetComponent<MapController>();

        Vector2Int pos = mapController.clipVect3Int(mapController.gameObject.GetComponent<Tilemap>().LocalToCell(gameObject.transform.position));

        mapController.Treeeeeee(pos);
    }
}
