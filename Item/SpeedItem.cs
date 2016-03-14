using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;

public class SpeedItem : MonoBehaviour
{
    bool active = true;
    public GameObject speedParticle;

    void Start()
    {
        GetComponent<Animator>().speed = Random.Range(1.05f, 1.15f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == TagName.Friendly && active)
        {
            GetComponent<Animator>().SetTrigger("Pickup");

            other.GetComponent<PlayerController>().GainSpeed(0.4f, 3.5f);

            StartCoroutine(DeadEffect());
            active = false;

            for (int i = 0; i < Random.Range(6, 10); i++)
            {
                //Instantiate(particles, transform.position, Quaternion.identity);

                GameObject g = ObjectPool.instance.GetPooledObject(speedParticle);
                g.GetComponent<Particle>().Init();
                g.transform.position = transform.position;
            }

        }

    }

    private IEnumerator DeadEffect()
    {
        float percent = 0;
        float speed = 3f;
        Vector3 originalPosition = transform.position;
        Vector3 attackPosition = transform.position + new Vector3(0, 1, 0);

        while (percent <= 1)
        {
            percent += Time.deltaTime * speed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }

        Destroy(gameObject);
    }


}
