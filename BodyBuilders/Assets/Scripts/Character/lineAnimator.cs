using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineAnimator : MonoBehaviour
{
     int spriteSheetColumns = 4; // number of horizontal sprite frames
     int spriteSheetRows = 6; // number of vertical sprite frames
     int emptyTiles = 0; // number of empty sprite frames
     int index; // current frame
     Vector2 size; // size of each sprite
     
     [Range (0 , 200)]public int framesPerSecond = 100; // speed through which frames cycle
     LineRenderer lineRenderer; // the renderer we are changing the material for
     
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        size = new Vector2(1.0f / spriteSheetColumns , 1.0f / spriteSheetRows);
    }

     void Update()
    {
        index = (int)(Time.time * framesPerSecond);
        index = index % (spriteSheetColumns * spriteSheetRows - emptyTiles);
        int uIndex = index % spriteSheetColumns;
        int vIndex = index % spriteSheetRows; 

        Vector2 offset = new Vector2(uIndex * size.x, 1.0f - size.y - vIndex * size.y);
     
        lineRenderer.material.SetTextureOffset("_MainTex", offset);
        lineRenderer.material.SetTextureScale("_MainTex", size);
    }
}
