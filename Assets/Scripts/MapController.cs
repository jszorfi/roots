using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    public Tile tile;
    public Tile tile2;
    public Tile tile3;
    private Tilemap t;
    private Vector3Int oldHighlight;
    private bool oldHighlightSet = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(0, 0, 0);
        t = gameObject.GetComponent<Tilemap>();
        t.origin = new Vector3Int(0, 0, 0);
        t.size = new Vector3Int(20, 15, 0);
        t.FloodFill(t.origin, tile2);
        t.ResizeBounds();

        //t.SetTile(t.origin, tile3);
        //t.SetTile()
    }

    // Update is called once per frame
    void Update()
    {

        Vector3Int newHighlight;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    
        if(mousePos.x > t.origin.x && mousePos.y > t.origin.y && mousePos.x < t.origin.x + t.size.x && mousePos.y < t.origin.y + t.size.y)
        {
            newHighlight = new Vector3Int((int)mousePos.x, (int)mousePos.y, 1);
            t.SetTile(newHighlight, tile);

            if(!oldHighlightSet)
            {
                oldHighlight = newHighlight;
                oldHighlightSet = true;
            }

            if(oldHighlightSet && newHighlight != oldHighlight)
            {
                t.SetTile(oldHighlight, null);
                oldHighlight = newHighlight;
            }
        }
        else
        {
            if(oldHighlightSet)
            {
                t.SetTile(oldHighlight, null);
                oldHighlightSet = false;
            }
        }
    }
}
