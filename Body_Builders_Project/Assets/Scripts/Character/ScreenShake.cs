using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float shakeLimit = 7f;
    Transform transformPos;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.7f;
    Vector3 initialPosition;

    void Awake()
    {
        if (transformPos == null)
        {
            transformPos = gameObject.transform;
        }
    }

    void OnEnable()
    {
        initialPosition = transformPos.localPosition;
    }

    void Update()
    {
        transformPos = gameObject.transform;
        if (shakeDuration > 0)
        {
            transformPos.localPosition = transformPos.position + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime;
            shakeMagnitude -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0f;
            transformPos.localPosition = transformPos.position;
        }
    }

    public void TriggerShake(float shakeAmount)
    {
        if(shakeAmount > shakeLimit)
        {
            shakeAmount = shakeLimit;
        }
        shakeMagnitude = shakeAmount;
        shakeDuration = Mathf.Clamp(shakeAmount/3f , 0f , 0.5f);
    }
}
