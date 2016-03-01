using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;
using System.Linq;

public class Projectile : MonoBehaviour {

    public int damage = 5;
    public GameObject explosion;

    public GameObject sender;

    void Start () {
        Destroy(gameObject, 5f);
    }
	
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        bool destroy = false;

        // Block Mask
        if (TagName.BlockMask.Contains(other.tag))
        {
            destroy = true;
        }



        if (tag.Equals(TagName.FriendlyProjectiles))
        {


        }

        //TODO PAS DE TAG PLAYER MAIS UN TAG FRIENDLY
        if (other.tag.Equals(TagName.Enemy))
        {


        }

        if (tag.Equals(TagName.EnemyProjectiles))
        {

        }


        // Mortality
        Mortality mortality = other.GetComponent<Mortality>();

        if (mortality != null)
        {
            if (other.gameObject != sender)
            {
                destroy = true;
                mortality.DecrementHealth(damage);
                //todo faire reculer le target
            }
        }

        // Destroy
        if (destroy)
        {
            Destroy(gameObject);
            // Explosion
            if (explosion != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 p = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
                    Instantiate(explosion, new Vector2(transform.position.x + p.x, transform.position.y + p.y), Quaternion.identity);
                }
            }
        }
    }

}
