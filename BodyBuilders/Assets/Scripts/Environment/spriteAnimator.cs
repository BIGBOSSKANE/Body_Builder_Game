using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteAnimator : MonoBehaviour
{
    public bool animated = true;
    float animationTime;
    int currentFrame; // current frame
    int lastFrame; // the animation frame in the last update cycle;
    int frameCount; // the number of frames
    public SpriteRenderer render; // the spriteer we are changing the material for
    [HideInInspector] public bool reverse = false; // called by the conveyor belt
    [Tooltip("Framerate of animation (overriden by the converyorBelt script)")] [Range (0 , 200)] public float framesPerSecond = 100; // speed through which frames cycle
    public List<Sprite> animationFrames = new List<Sprite>();
    public bool randomStart = true;
    int randomStartTime;
     
    void Start()
    {
        frameCount = animationFrames.Count;
        if(randomStart) randomStartTime = Random.Range(0 , frameCount);
    }

    void Update()
    {
        if(animated)
        {
            animationTime = (Time.time * framesPerSecond + randomStartTime) % frameCount; // get the animation time by finding the remainder of the real time to the frame count
            currentFrame = (int)(animationTime); // cycle through frames over time
            if(reverse) currentFrame = frameCount - (currentFrame + 1); // reverse the order
            if(lastFrame != currentFrame) render.sprite = animationFrames[currentFrame]; // if the next frame to allocate isn't the same as the last one, play it
            lastFrame = currentFrame; // track the frame that just showed
        }
    }
}
