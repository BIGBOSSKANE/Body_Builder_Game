using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class edgelord : MonoBehaviour
{
    public float outerBladeRadius = 3f;
    public float spinSpeed = 3f;
    public float innerBladeSpinSpeed = 6f;
    public float outerBladeSpinSpeed = 3f;
    public Transform[] outerBlades;
    [HideInInspector] public List<Vector2> bladePositions;
    [HideInInspector] public List<Transform> blades;
    int outerBladeCount;
    GameObject core;
    Transform coreBlade;

    void Start()
    {
        coreBlade = gameObject.transform.Find("Core").gameObject.transform.Find("blade");
        UpdateOuterBlades();
        core = gameObject.transform.Find("Core").gameObject;
    }

    void UpdateOuterBlades()
    {
        outerBladeCount = outerBlades.Length;

        int oddCount = (outerBladeCount%2 == 0)? 0 : 1;

        float radialDivisions = (outerBladeCount != 0)? (360 / outerBladeCount) : 0;

        for (int i = 0; i < outerBlades.Length; i++)
        {
        
            Vector2 direction = ((Vector2)(Quaternion.Euler(0 , 0 , i * radialDivisions - (90f * oddCount)) * Vector2.right)).normalized;
            bladePositions.Add((Vector2)transform.position + (direction * outerBladeRadius));
            outerBlades[i].transform.position = bladePositions[i];
            outerBlades[i].transform.up = direction;
            blades.Add(outerBlades[i].transform.Find("blade"));
        } 
    }

    void Update()
    {

        transform.Rotate(0f , 0f , -spinSpeed * Time.deltaTime); // rotate full body

        core.transform.up = Vector2.up; // make sure the centre doesn't rotate
        coreBlade.Rotate(0f , 0f , -innerBladeSpinSpeed * Time.deltaTime); // spin central blade

        foreach (Transform blade in blades)
        {
            blade.Rotate(0f , 0f , -outerBladeSpinSpeed * Time.deltaTime); // spin outer blades
        }
    }
}
