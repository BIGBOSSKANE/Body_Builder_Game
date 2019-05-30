using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    private Dictionary<GameObject, Transform> _holding;

    // Start is called before the first frame update
    void Start()
    {
        _holding = new Dictionary<GameObject, Transform>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        Debug.Log(c.gameObject.name);
        if (!_holding.ContainsKey(c.gameObject) )
        {
            //store old parent.
            _holding.Add(c.gameObject, c.gameObject.transform.parent);
            ReparentToPlatform(c.gameObject);
        }
        
    }

    void ReparentToPlatform(GameObject g)
    {
        g.transform.SetParent(transform);
        //turn off physics if present
        EnablePhysics(g, false);
    }

    void EnablePhysics(GameObject g, bool val)
    {
        Rigidbody2D _rb2d = g.GetComponent<Rigidbody2D>();
        if (_rb2d)
        {
            if (val)
            {
                _rb2d.bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                _rb2d.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }
}


