using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineAnimator : MonoBehaviour
{
    public float frameDuration = 0.2f;
    float frameTimer;
    public int spriteSheetColumns = 1;
    public int spriteSheetRows = 1;
    public int singleSpriteSize = 64;
    public int totalFrames = 1;
    int currentFrame = 1;

    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if(spriteSheetColumns == 0)
        {
            spriteSheetColumns = 1;
        }
        
        if(spriteSheetRows == 0)
        {
            spriteSheetColumns = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        frameTimer += Time.unscaledDeltaTime;
        if(frameTimer >= frameDuration)
        {
            if(currentFrame == totalFrames)
            {
                currentFrame = 0;
            }
            else
            {
                currentFrame ++;
            }

        }

        if(currentFrame == 0) // isolate the 0 frame, so you don't get errors from dividing by 0
        {
            float offsetX = 0f;
            float offsetY = 0f;
            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
            currentFrame ++;
            frameTimer = 0f;
        }
        else if((currentFrame % spriteSheetColumns) == 0)
        {
            float offsetX = 0f;
            float offsetY = singleSpriteSize * (currentFrame / spriteSheetColumns);
            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
            currentFrame ++;
            frameTimer = 0f;           
        }
        else
        {
            float offsetX = singleSpriteSize * (currentFrame / spriteSheetRows);
            float offsetY = singleSpriteSize * (currentFrame / spriteSheetColumns);
            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
            currentFrame ++;
            frameTimer = 0f;
        }
    }
}
