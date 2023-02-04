using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SpriteAnimator : MonoBehaviour
{
    [System.Serializable]
    public class Animation
    {
        /** The name of the animation */
        public string Name;

        /** The list of sprites in the animation */
        public List<Sprite> Srites;

        /** The duration of each frame in the animation in seconds */
        public float FrameDuration;

        /** Whether the animation is repeating */
        public bool Repeating;
    }

    private int CurrentSpriteIndex = 0;
    private float TimeSpentInFrame = 0.0f;
    private Animation CurrentAnimation;

    /** The SriteRenderer of the GameObject */
    private SpriteRenderer Renderer;

    /** The List of animations for the GameObject */
    public List<Animation> Animations;

    // Start is called before the first frame update
    void Start()
    {
        // Getting the SpriteRenderer for the GameObject
        Renderer = gameObject.GetComponent<SpriteRenderer>();

        // Playing the first animation by default
        CurrentAnimation = Animations[0];
    }

    // Update is called once per frame
    void Update()
    {
        // Updating the time spent in the current frame
        TimeSpentInFrame += Time.deltaTime;

        // Checking whether we need to change frames or animations
        if(TimeSpentInFrame > CurrentAnimation.FrameDuration)
        {
            // We start the clock on a new frame
            TimeSpentInFrame = 0.0f;

            // We get the index of the next sprite to show
            if(CurrentAnimation.Repeating) {
                CurrentSpriteIndex = (CurrentSpriteIndex + 1) % CurrentAnimation.Srites.Count;
            } else {
                CurrentSpriteIndex = Math.Min(CurrentSpriteIndex + 1, CurrentAnimation.Srites.Count - 1);
            }
            
            // Updating the sprite to display
            Renderer.sprite = CurrentAnimation.Srites[CurrentSpriteIndex];
        }
    }

    /**
     * Sets the animation to be played by its name.
     */
    public void SetAnimationByName(string animationName)
    {
        foreach(Animation animation in Animations)
        {
            if(animation.Name == animationName)
            {
                CurrentSpriteIndex = 0;
                TimeSpentInFrame = 0.0f;
                CurrentAnimation = animation;
            }
        }
    }
}
