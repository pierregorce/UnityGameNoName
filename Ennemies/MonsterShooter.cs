using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;

public class MonsterShooter : MonsterEntity
{
    private Transform target;
    public GameObject projectile;

    protected override void Start()
    {
        base.Start();
        target = GameObject.Find(GameObjectName.GameManager).GetComponent<GameManager>().player.transform;
    }

    void Update()
    {
        if (Time.time > nextAttackTime)
        {
            float distance = (target.position - transform.position).magnitude;
            if (distance <= seekDistance)
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                DoAttack();
            }
        }
    }

    void DoAttack()
    {
        if (projectile != null)
        {
            GameObject shootProjectile = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
            shootProjectile.transform.position = transform.GetChild(0).transform.position;
            shootProjectile.transform.parent = transform.GetChild(0).transform;
            shootProjectile.GetComponent<Projectile>().sender = gameObject;

            Vector3 targetPosition = target.position;
            Vector3 startPosition = transform.GetChild(0).transform.position;
            Vector2 direction = targetPosition - startPosition;
            direction.Normalize();

            shootProjectile.GetComponent<Rigidbody2D>().AddForce(direction * 1000);
            shootProjectile.GetComponent<Projectile>().sender = gameObject;
        }
    }

    //protected override void OnDeath()
    //{
    //    //todo placeholder death for permanence
    //}

























    protected override void OnHealthDown(int value)
    {
        if (hurtParticle != null)
        {
            for (int i = 0; i < Random.Range(3, 6); i++)
            {
                GameObject hurtedParticle = Instantiate(hurtParticle, transform.position, Quaternion.identity) as GameObject;
                hurtedParticle.transform.parent = transform;
                //todo random spawn
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (isGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, 0), seekDistance);
        }
    }




}
