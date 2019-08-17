using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineAnimator : MonoBehaviour
{
    public float frameDuration = 0.2f;
    float frameTimer;
    public int spriteSheetColumns;
    public int spriteSheetRows;
    public int singleSpriteSize;
    public int totalFrames;
    int currentFrame;

    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
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

        if(currentFrame % spriteSheetColumns == 0 || currentFrame == 0)
        {
            float offsetX = 0f;
            float offsetY = singleSpriteSize * ((currentFrame / spriteSheetColumns) / spriteSheetRows);
            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
            currentFrame ++;
            frameTimer = 0f;
        }
        else
        {
            float offsetX = spriteSheetColumns * singleSpriteSize * (currentFrame / spriteSheetRows);
            float offsetY = spriteSheetRows * singleSpriteSize * (currentFrame / spriteSheetColumns);
            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
            currentFrame ++;
            frameTimer = 0f;
        }
    }
}
