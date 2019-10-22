using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sawbladeTranslate : MonoBehaviour
{
    public bool activated = true;
    public float moveTime = 4f;
    public float bladeDistance = 5f;
    public float spinSpeed = 200f;
    public bool smoothMove = true;
    float moveTimer;
    bool forwards = true;
    public Vector2 startPos;
    public Vector2 endPos;
    Transform movingSaw;

    void Start()
    {
        movingSaw = transform.Find("MovingSawBlade");
    }

    void Update()
    {
        moveTimer += Time.fixedDeltaTime / moveTime;
        moveTimer = Mathf.Clamp(moveTimer , 0 , 1);
        if(forwards)
        {
            if(smoothMove) movingSaw.localPosition = new Vector2(Mathf.SmoothStep(startPos.x , endPos.x , moveTimer) , Mathf.SmoothStep(startPos.y , endPos.y , moveTimer));
            else movingSaw.localPosition = Vector2.Lerp(startPos , endPos, moveTimer);
        }
        else
        {
            if(smoothMove) movingSaw.localPosition = new Vector2(Mathf.SmoothStep(endPos.x , startPos.x , moveTimer) , Mathf.SmoothStep(endPos.y , startPos.y , moveTimer));
            else movingSaw.localPosition = Vector2.Lerp(endPos , startPos, moveTimer);
        }

        if(moveTimer >= 1)
        {
            forwards = !forwards;
            moveTimer = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        float locationIdentifier = 0.3f;

        Vector2 sPos = startPos * transform.localScale.x + (Vector2)transform.position;
        Vector2 ePos = endPos * transform.localScale.x + (Vector2)transform.position;

        if(!Application.isPlaying)
        {
            Gizmos.DrawLine(sPos - Vector2.up * locationIdentifier , sPos + Vector2.up * locationIdentifier);
            Gizmos.DrawLine(sPos - Vector2.left * locationIdentifier , sPos + Vector2.left * locationIdentifier);
            Gizmos.DrawLine(ePos - Vector2.up * locationIdentifier , ePos + Vector2.up * locationIdentifier);
            Gizmos.DrawLine(ePos - Vector2.left * locationIdentifier , ePos + Vector2.left * locationIdentifier);
            Gizmos.DrawLine(sPos, ePos);
        }
    }
}
