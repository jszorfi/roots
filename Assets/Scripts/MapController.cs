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
        gameObject.transform.position = new Vector3(0, 0, 0);
        var t = gameObject.GetComponent<Tilemap>();
        t.origin = new Vector3Int(0, 0, 0);
        t.size = new Vector3Int(2, 4, 0);
        t.FloodFill(t.origin, tile2);
        t.ResizeBounds();

        //t.SetTile(t.origin, tile3);
        //t.SetTile()
    }

    // Update is called once per frame
    void Update()
    {

    }
}
