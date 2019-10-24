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
    public float movementLength;
    float previousMovementLength;
    Vector2 startPos;
    Vector2 endPos;
    [HideInInspector] public Transform movingSaw;
    [HideInInspector] public Transform groove; // assigned from a hidden public insert

    void Start()
    {
        float grooveLength = (groove.localScale.x / 2) - (movingSaw.GetComponent<SpriteRenderer>().bounds.extents.x);

        startPos = new Vector2(-grooveLength, -0.4f);
        endPos= new Vector2(grooveLength, -0.4f);
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

    void OnDrawGizmos()
    {
        if(previousMovementLength != movementLength)
        {
            groove.localScale = new Vector3(movementLength * transform.localScale.x , groove.localScale.y , groove.localScale.z);
            previousMovementLength = movementLength;
        }
    }
}
