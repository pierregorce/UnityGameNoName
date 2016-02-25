using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;

public class CameraShake : MonoBehaviour
{
    //Simple random
     float duration = 0.5f;
     float magnitude = 0.1f;

    //For Perlin Noise
     bool withPerlinNoise = false;
     float speed = 1.0f;
    float randomStart = 0;
    float perlinNoiseProgress = 0;

    //Common
    Vector3 originalPosition;
    bool shaking = false;
    float elapsedTime = 0.0f;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ShakeThatBooty();
        }

        if (elapsedTime < duration && shaking)
        {
            elapsedTime += Time.deltaTime;
            float percentComplete = elapsedTime / duration; //Avancement
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 2.0f, 0.0f, 1.0f); //Amortissement à partir de 50% : 4/2
            float x = 0;
            float y = 0;

            // ------------------------------------- Simple Random map value to [-1, 1]
            if (!withPerlinNoise)
            {
                x = Random.value * 2.0f - 1.0f;
                y = Random.value * 2.0f - 1.0f;
            }

            // ------------------------------------- Perlin Noise
            if (withPerlinNoise)
            {
                // Calculate the noise parameter starting randomly and going as fast as speed allows
                perlinNoiseProgress += Time.deltaTime;
                float alpha = randomStart + speed * perlinNoiseProgress;

                // map noise to [-1, 1]
                x = Mathf.PerlinNoise(alpha, 0.0f) * 2.0f - 1.0f;
                y = Mathf.PerlinNoise(0.0f, alpha) * 2.0f - 1.0f;
            }

            x *= magnitude * damper;
            y *= magnitude * damper;
            transform.localPosition = originalPosition + new Vector3(x, y, originalPosition.z);
        }
        else
        {
            if (shaking)
            {
                transform.localPosition = originalPosition;
                shaking = false;
            }
        }
    }

    public void ShakeThatBooty(ShakeParameters parameters)
    {
        elapsedTime = 0;
        withPerlinNoise = parameters.withPerlinNoise;
        speed = parameters.speed;

        if (!shaking)
        {
            randomStart = Random.Range(-1000.0f, 1000.0f);
            elapsedTime = 0;
            originalPosition = transform.localPosition;
            shaking = true;
            perlinNoiseProgress = 0;
        }
    }

    //For testing purpose
    void ShakeThatBooty()
    {
        elapsedTime = 0;

        if (!shaking)
        {
            randomStart = Random.Range(-1000.0f, 1000.0f);
            elapsedTime = 0;
            originalPosition = transform.localPosition;
            shaking = true;
            perlinNoiseProgress = 0;
        }
    }

    public class ShakeParameters
    {
        public static readonly ShakeParameters Small = new ShakeParameters(0.4f, 0.05f);
        public static readonly ShakeParameters SmallPerlin = new ShakeParameters(0.4f, 0.05f, 25, true);

        //public static readonly ShakeParameters Medium = new ShakeParameters(10, 10, 10);
        //public static readonly ShakeParameters High = new ShakeParameters(10, 10, 10);

        public float duration;
        public float magnitude;
        public float speed;
        public bool withPerlinNoise;

        private ShakeParameters(float duration, float magnitude)
        {
            this.duration = duration;
            this.magnitude = magnitude;
            speed = 0;
            withPerlinNoise = false;
        }

        private ShakeParameters(float duration, float magnitude, float speed, bool withPerlinNoise)
        {
            this.duration = duration;
            this.magnitude = magnitude;
            this.speed = speed;
            this.withPerlinNoise = withPerlinNoise;
        }
    }
}
