using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
