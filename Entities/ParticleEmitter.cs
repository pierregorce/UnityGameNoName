using UnityEngine;
using System.Collections;


//[RequireComponent(typeof(Rigidbody))]
public class ParticleEmitter : MonoBehaviour
{

    public GameObject particle;

    private float nextEmissionTime;
    public float timeBetweenEmissionMin = 0.3f;
    public float timeBetweenEmissionMax = 0.6f;

    [Range(-2f, 2f)]
    public float xPositionMin = 0, xPositionMax = 0;
    [Range(-2f, 2f)]
    public float yPositionMin = 0, yPositionMax = 0;

    void Start()
    {
    }

    void Update()
    {
        if (Time.time > nextEmissionTime)
        {
            float timeBeetweenEmission = Random.Range(timeBetweenEmissionMin, timeBetweenEmissionMax);
            nextEmissionTime = Time.time + timeBeetweenEmission;
            float xPosition = Random.Range(xPositionMin, xPositionMax);
            float yPosition = Random.Range(yPositionMin, yPositionMax);
            Instantiate(particle, new Vector2(transform.position.x + xPosition, transform.position.y + yPosition), Quaternion.identity);
        }
    }
}
