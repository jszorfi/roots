using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteRandomizer : MonoBehaviour
{

    public List<Sprite> possibleImages;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().sprite = possibleImages[Random.Range(0, possibleImages.Count - 1)];

    }

}
