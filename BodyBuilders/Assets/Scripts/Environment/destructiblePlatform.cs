using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destructiblePlatform : MonoBehaviour
{
    List<SpriteRenderer> partSprites = new List<SpriteRenderer>();
    public float forceLimit = 10f;
    bool breaking = false;
    float frameCount;
    [Tooltip("The duration that the fractured pieces take to turn invisible")] public float fadeDuration = 1f;
    float fadeTimer = 1f;
    [Tooltip("Use this while running the game to test the effect, assuming a central groundbreaker collision")] public bool callGroundBreak;
    [Tooltip("Each of the fragments")] public List<GameObject> parts = new List<GameObject>();

    public void Groundbreak(Vector2 colPos)
    {
        if(!breaking)
        {
            breaking = true;
            Destroy(GetComponent<BoxCollider2D>());
            if(GetComponent<SpriteRenderer>() != null) Destroy(GetComponent<SpriteRenderer>());
            foreach (GameObject part in parts)
            {
                part.transform.parent = null;
                part.SetActive(true);
                Vector2 colVec = (Vector2)part.transform.position - colPos;
                float torque = Mathf.Clamp(colVec.magnitude * Mathf.Sign(colVec.x) * 3, 1 , 5);
                part.GetComponent<Rigidbody2D>().AddTorque(torque , ForceMode2D.Impulse);
                part.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Clamp(colVec.x / 2f , 0 , forceLimit) , Mathf.Clamp(Mathf.Clamp(colVec.y, 1 , Mathf.Infinity) * 4f , 0 , forceLimit)) , ForceMode2D.Impulse);
                partSprites.Add(part.GetComponent<SpriteRenderer>());
            }
        }
    }

    void Update()
    {
        if(breaking)
        {
            frameCount ++;
            fadeTimer -= Time.deltaTime / fadeDuration;

            if(frameCount % 5 == 0)
            {
                foreach (SpriteRenderer render in partSprites)
                {
                    render.color = new Color(1 , 1 , 1 , fadeTimer);
                }
                if(fadeTimer <= 0)
                {
                    foreach (SpriteRenderer render in partSprites)
                    {
                        Destroy(render.gameObject);
                    }
                    Destroy(gameObject);
                }
            }
        }
        else if(callGroundBreak)
        {
            Groundbreak(transform.position);
        }
    }
}
