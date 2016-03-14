using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;
using System.Linq;

public class Projectile : PhysicalEntities
{
    public int bumpForce = 3;
    public int damage = 5;
    public GameObject explosion;
    [HideInInspector]
    public GameObject sender;

    public override void Init()
    {
        base.Init();
        Disable(3);
        //Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool destroy = false;

        // Block Mask
        if (TagName.BlockMask.Contains(other.tag))
        {
            destroy = true;
        }

        // Mortality
        Mortality mortality = other.GetComponent<Mortality>();

        if (mortality != null)
        {

            if (other.gameObject != sender)
            {
                // Si projectile AMI
                if (tag.Equals(TagName.FriendlyProjectiles))
                {
                    PhysicalEntities physics = other.GetComponent<PhysicalEntities>();
                    //Ne recherche que les collisions avec les ennemys
                    if (other.tag.Equals(TagName.Enemy))
                    {
                        destroy = true;
                        mortality.DecrementHealth(damage);
                        if (physics != null && physics.bumpSensible)
                        {
                            Vector2 direction = other.transform.position - transform.position; //direction entre this and target
                            direction.Normalize();
                            physics.ApplyForce(direction * bumpForce);
                        }

                    }
                    if (TagName.BlockMask.Contains(other.tag))
                    {
                        destroy = true;
                        mortality.DecrementHealth(damage);
                    }
                }

                // Si projectile ENEMY
                if (tag.Equals(TagName.EnemyProjectiles))
                {
                    //Ne recherche que les collisions avec les friendly
                    if (other.tag.Equals(TagName.Friendly))
                    {
                        PhysicalEntities physics = other.GetComponent<PhysicalEntities>();
                        destroy = true;
                        mortality.DecrementHealth(damage);
                        if (physics != null && physics.bumpSensible)
                        {
                            Vector2 direction = other.transform.position - transform.position; //direction entre this and target
                            direction.Normalize();
                            physics.ApplyForce(direction * bumpForce);
                        }
                    }
                }
            }
        }

        // Destroy
        if (destroy)
        {
            gameObject.SetActive(false);
            // Explosion
            if (explosion != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 p = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);

                    GameObject g = ObjectPool.instance.GetPooledObject(explosion);
                    g.GetComponent<Explosion>().Init();
                    g.transform.position = new Vector2(transform.position.x + p.x, transform.position.y + p.y);

                    //Instantiate(explosion, new Vector2(transform.position.x + p.x, transform.position.y + p.y), Quaternion.identity);
                }
            }
        }
    }

}
