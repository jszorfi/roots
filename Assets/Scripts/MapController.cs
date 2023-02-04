using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    public Tile tile;
    public Tile tile2;
    public Tile tile3;

    // Start is called before the first frame update
    void Start()
    {
        var t = gameObject.GetComponent<Tilemap>();
        t.size = new Vector3Int(1, 1, 0);
        t.FloodFill(new Vector3Int(0, 0, 1), tile2);
        t.SetTile(t.origin, tile3);
        //t.SetTile()
    }

    // Update is called once per frame
    void Update()
    {

    }
}
