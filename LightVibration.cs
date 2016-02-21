using UnityEngine;
using System.Collections;

public class LightVibration : MonoBehaviour
{

    private Light light;

    float startRange;
    float endRange;
    float oscilationRange;
    float oscilationOffset;

    void Start()
    {
        light = GetComponent<Light>();
        oscilationRange = (endRange - startRange) / 2;
        oscilationOffset = oscilationRange + startRange;
    }

    void Update()
    {
        float result = oscilationOffset + Mathf.Sin(Time.time * timeScale) * oscilationRange;
    }
}
