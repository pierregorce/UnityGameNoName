﻿using UnityEngine;
using System.Collections;

public class LightVibration : MonoBehaviour
{

    private Light light;

    float startRange;
    float endRange;
    float oscilationRange;
    float oscilationOffset;
    float timeScale;

    void Start()
    {
        light = GetComponent<Light>();
        timeScale = Random.Range(0.3f, 0.5f);
        startRange = 1.1f;
        endRange = 5.5f;
        oscilationRange = (endRange - startRange) / 2;
        oscilationOffset = oscilationRange + startRange;
    }

    void Update()
    {
        float result = oscilationOffset + Mathf.Sin(Time.time * timeScale) * oscilationRange;
        light.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -result);
    }
}
