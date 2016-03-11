using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;
using System.Linq;

public class Particle : MonoBehaviour
{
    public Sprite[] textures;

    [Header("Physics things")]
    [Range(2, 30)]
    public float bounceVelocity = 7;
    public float linearDrag = 2;
    public bool isBlockMaskSensible = true;

    [Header("Bouncing")]
    [Range(1, 5)]
    public int reboundMin = 1;
    [Range(1, 5)]
    public int reboundMax = 4;
    public bool isBouncing = true;

    [Header("Force pushed")]
    [Range(-500, -1)]
    public int xForceMin = -150;
    [Range(1, 500)]
    public int xForceMax = 150;
    [Range(-200, 100)]
    public int yForceMin = -50;
    [Range(1, 200)]
    public int yForceMax = 50;
    [Header("Torque pushed")]
    public bool isTorque = true;
    [Range(30, 500)]
    public int torqueMin = 100;
    [Range(30, 500)]
    public int torqueMax = 200;

    [Header("Lifetime")]
    [Range(0, 30)]
    public float lifeTimeMin = 5.5f;
    [Range(0, 30)]
    public float lifeTimeMax = 10f;

    [Header("Scale")]
    public float scaleMin = 0.95f;
    public float scaleMax = 1.05f;


    [Header("Color")]
    public float alphaMin = 1f;
    public float alphaMax = 1f;

    void Start()
    {
        float x = Random.Range(xForceMin, xForceMax);
        float y = Random.Range(yForceMin, yForceMax);

        float s = Random.Range(scaleMin, scaleMax);
        transform.localScale = new Vector3(s * transform.localScale.x, s * transform.localScale.y);

        GetComponent<Rigidbody2D>().drag = linearDrag;
        if (isTorque)
        {
            GetComponent<Rigidbody2D>().AddTorque(Random.Range(torqueMin, torqueMax));
        }
        GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y));
        bounceNumber = Random.Range(reboundMin, reboundMax);
        BounceTimes(bounceNumber);
        GetComponent<SpriteRenderer>().sprite = textures[Random.Range(0, textures.Length)];

        float a = Random.Range(alphaMin, alphaMax);
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = a;
        GetComponent<SpriteRenderer>().color = color;

        Destroy(gameObject, Random.Range(lifeTimeMin, lifeTimeMax));
    }

    //-------------------------- Bounce Effect --------------------

    private int bounceNumber;
    private float startTime = 0;
    private float lastYOffset = 0;
    private float gravity = -9.81f;
    private float yOffset = 0;

    public void BounceTimes(int numberOfBounces)
    {
        startTime = Time.time;
        lastYOffset = 0.0f;
        bounceNumber = numberOfBounces;
        bounceVelocity *= 0.50f;
    }

    void Update()
    {
        if (!isBouncing)
        {
            return;
        }
        // if done with bounces, stop
        if (bounceNumber <= 0)
        {
            GetComponent<Rigidbody2D>().isKinematic = true; //stop forces
            GetComponent<SpriteRenderer>().sortingLayerName = SortingLayerName.A_particleOnGround;
            return;
        }

        // otherwise, calculate current yoffset
        if (bounceNumber > 0)
        {
            // apply classic parabolic formula: h(t) = vt + (gt^2 / 2)
            float time = (Time.time - startTime);
            yOffset = (bounceVelocity * time) + ((gravity * time * time) / 2);

            // add to the current position, but subtract last y offset (additive behavior)
            // since this could be moving in the y-axis from other forces as well
            transform.position = new Vector3
            (
                transform.position.x,
                transform.position.y + yOffset - lastYOffset,
                transform.position.z
            );
            lastYOffset = yOffset;

            // if hitting the "floor", bounce again
            if (yOffset <= 0f)
            {
                BounceTimes(bounceNumber - 1);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        bool stopped = false;

        if (isBlockMaskSensible && TagName.BlockMask.Contains(other.tag))
        {
            stopped = true;
        }

        if (stopped)
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            //stop forces
            // GetComponent<SpriteRenderer>().sortingLayerName = "ParticlesOnGround";
            // bounceNumber = 0;
            BounceTimes(bounceNumber - 1);
        }

        //ItemDestroyable itemDestroyable = other.GetComponent<ItemDestroyable>();
    }


}



