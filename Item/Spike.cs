using Assets.Scripts.Utils;
using UnityEngine;

public class Spike : MonoBehaviour
{

    private float nextEventTime;
    public float timeBetweenEvent = 1.5f;

    private float nextAttackTime;
    public float timeBetweenAttack = 0.6f;

    public int baseAttackDamage = 15;

    private Animator animator;

    enum State
    {
        OPEN, CLOSE
    }

    private State state = State.CLOSE;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time > nextEventTime)
        {
            if (state == State.CLOSE)
            {
                nextEventTime = Time.time + timeBetweenEvent;
                if (animator != null)
                {
                    GetComponent<Animator>().SetTrigger("Open");
                }
                state = State.OPEN;
            }
        }

        if (Time.time > nextEventTime)
        {
            if (state == State.OPEN)
            {
                nextEventTime = Time.time + timeBetweenEvent;
                if (animator != null)
                {
                    GetComponent<Animator>().SetTrigger("Close");
                }
                state = State.CLOSE;
            }
        }

    }

    void OnTriggerStay2D(Collider2D other)
    {
        Mortality mortality = other.GetComponent<Mortality>();

        if (mortality == null)
        {
            return;
        }

        // Mortality tod check
        if (tag.Equals(TagName.TrapEnemy))
        {
            if (other.tag.Equals(TagName.Friendly))
            {
                if (state == State.OPEN)
                {
                    if (Time.time > nextAttackTime)
                    {
                        mortality.DecrementHealth(baseAttackDamage);
                        nextAttackTime = Time.time + timeBetweenAttack;
                    }
                }
            }
        }

        if (tag.Equals(TagName.TrapNeutral))
        {
            if (state == State.OPEN)
            {
                if (Time.time > nextAttackTime)
                {
                    mortality.DecrementHealth(baseAttackDamage);
                    nextAttackTime = Time.time + timeBetweenAttack;
                }
            }
            mortality.DecrementHealth(baseAttackDamage);
        }

        //// Explosion
        //if (explosion != null)
        //{
        //    for (int i = 0; i < 3; i++)
        //    {
        //        Vector3 p = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
        //        Instantiate(explosion, new Vector2(transform.position.x + p.x, transform.position.y + p.y), Quaternion.identity);
        //    }
        //}

    }




}
