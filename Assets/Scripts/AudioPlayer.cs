using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [Serializable]
    public class NamedClip
    {
        public string Name;
        public AudioClip Clip;
    }

    [SerializeField]
    public List<NamedClip> ambientClips;

    [SerializeField]
    public List<NamedClip> effectClips;
    public AudioSource audioSource;
    public float minimumIdleTime = 1.0f;
    public float maximumIdleTime = 5.0f;
    private float timePassed = 0.0f;
    private float timeToPass = 0.0f;

    static float RandomFloat(float min, float max)
    {
        System.Random random = new System.Random();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Checking whether a clip should be played
        if (timePassed > timeToPass)
        {
            // Playing the next audio clip
            int clipIndexToPlay = UnityEngine.Random.Range(0, ambientClips.Count);
            audioSource.clip = ambientClips[clipIndexToPlay].Clip;
            audioSource.Play();

            // Generating the next timeframe to wait 
            timeToPass = RandomFloat(minimumIdleTime, maximumIdleTime);
            timePassed = 0.0f;
        }

        // Updating time that has passed
        timePassed += Time.deltaTime;
    }

    public void PlayAudioByName(string name)
    {
        foreach (NamedClip namedClip in effectClips)
        {
            if (namedClip.Name == name)
            {
                audioSource.clip = namedClip.Clip;
                audioSource.Play();
            }
        }
    }
}
