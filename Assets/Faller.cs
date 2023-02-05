using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Faller : MonoBehaviour
{

    private Vector3 origin;

    public float fallDistance = 1.0f;
    public float fallTime = 1.0f;
    public float delay = 0.0f;

    public List<Sprite> possibleImages;


    // Start is called before the first frame update
    void Start()
    {
        origin = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Fall();
    }

    void Fall()
    {
        GetComponent<Image>().sprite = possibleImages[Random.Range(0, possibleImages.Count - 1)];
        transform.position = origin;
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "position", this.transform.position - new Vector3(0.0f, fallDistance, 0.0f),
            "onComplete", "Fall",
            "time", fallTime,
            "delay", delay + Random.Range(-delay, delay),
            "easetype", "easeInCubic"));
    }
}
