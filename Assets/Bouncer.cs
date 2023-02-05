using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bouncer : MonoBehaviour
{

    public float bounceHeight = 1.0f;
    public float bounceHeightDiff = 0.5f;
    public float bounceTime = 1.0f;
    public float bounceTimeDiff = 0.1f;

    public List<Sprite> possibleImages;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().sprite = possibleImages[Random.Range(0, possibleImages.Count-1)];
        bounceHeight = bounceHeight + Random.Range(-bounceHeightDiff, bounceHeightDiff);
        bounceTime = bounceTime + Random.Range(-bounceTimeDiff, bounceTimeDiff);
        BounceUp();
    }

    void BounceUp ()
    {
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "position", this.transform.position + new Vector3(0.0f, bounceHeight, 0.0f),
            "onComplete", "BounceDown",
            "time", bounceTime,
            "delay", Random.Range(0.0f, 1.0f),
            "easetype", "easeOutSine"));
    }

    void BounceDown()
    {
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "position", this.transform.position - new Vector3(0.0f, bounceHeight, 0.0f),
            "onComplete", "BounceUp",
            "time", bounceTime,
            "delay", 0.0f,
            "easetype", "easeInSine"));
    }

}
