using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Test : MonoBehaviour
{
    private LineRenderer lineRenderer = new LineRenderer();
    private List<Vector2> newVertices = new List<Vector2>() {
        new Vector2 (0, 0),
        new Vector2 (0, 0)
    };

    private EdgeCollider2D edgeCol;

    public Transform origin;
    public Transform destin;

    void Awake()
    {
        lineRenderer = GameObject.Find("Line").GetComponent<LineRenderer>();
        edgeCol = GameObject.Find("Line").GetComponent<EdgeCollider2D>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDrag()
    {

        Vector2 screenPos = new Vector2();
        Camera.main.ScreenToWorldPoint(screenPos);

        lineRenderer.SetPosition(0, new Vector3(origin.position.x + (GetComponent<SpriteRenderer>().bounds.size.x) / 2, origin.position.y, origin.position.z));
        lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        newVertices[0] = new Vector2(origin.position.x + (GetComponent<SpriteRenderer>().bounds.size.x) / 2, origin.position.y);
        newVertices[1] = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        edgeCol.points = newVertices.ToArray();
    }
}