using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour
{
    Color[] iceColor = new Color[5];


    void Start()
    {

        iceColor[0] = new Color(203 / 255f, 223 / 255f, 244 / 255f, 1);
        iceColor[1] = new Color(228 / 255f, 255 / 255f, 255 / 255f, 1);
        iceColor[2] = new Color(254 / 255f, 254 / 255f, 255 / 255f, 1);
        iceColor[3] = new Color(183 / 255f, 206 / 255f, 229 / 255f, 1);
        iceColor[4] = new Color(205 / 255f, 254 / 255f, 254 / 255f, 1);

        GetComponent<SpriteRenderer>().color = iceColor[Random.Range(0, iceColor.Length)];



        float x = Random.Range(-150, 150);
        float y = Random.Range(-50, 50);
        GetComponent<Rigidbody2D>().drag = 2f;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y));
        BounceTimes(bounceNumber);

        Destroy(gameObject, Random.Range(5.5f, 10f));
    }

    public int bounceNumber = 3;
    float startTime = 0;
    float lastYOffset = 0;
    public float bounceVelocity = 7;
    float gravity = -9.81f;
    float yOffset = 0;

    public void BounceTimes(int numberOfBounces)
    {
        // init
        startTime = Time.time;
        lastYOffset = 0.0f;
        bounceNumber = numberOfBounces;
        bounceVelocity *= 0.50f;
    }

    void Update()
    {

        // if done with bounces, stop
        if (bounceNumber <= 0)
        {
            GetComponent<Rigidbody2D>().isKinematic = true; //stop forces
            //GetComponent<SpriteRenderer>().sortingLayerName = "ParticlesOnGround";
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
        //Collision avec les murs et les items destroyables
        ItemDestroyable itemDestroyable = other.GetComponent<ItemDestroyable>();

        if (itemDestroyable!=null)
        {
            GetComponent<Rigidbody2D>().isKinematic = true; //stop forces
           // GetComponent<SpriteRenderer>().sortingLayerName = "ParticlesOnGround";
           // bounceNumber = 0;
            BounceTimes(bounceNumber - 1);
        }

    }


}



