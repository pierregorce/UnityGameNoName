using UnityEngine;
using System.Collections;

public class MonsterEntity : MonoBehaviour
{

    [Header("Attack")]
    protected float nextAttackTime;
    public float timeBetweenAttacks = 1;
    public int baseAttackDamage  = 5;

    [Header("Movement")]
    public float speed = 2;
    public int seekDistance = 5;

    [Header("Helper")]
    public bool isGizmos = true;

    [Header("Hurt")]
    public GameObject hurtParticle;

    protected Mortality mortality;
    protected OrientableEntity orientableEntity;

    protected virtual void Start()
    {
        mortality = GetComponent<Mortality>();
        orientableEntity = GetComponent<OrientableEntity>();
        mortality.Revive();
        mortality.OnDeath += OnDeath;
        mortality.OnHealthDown += OnHealthDown;
    }

    protected virtual void OnDeath()
    {
        // perform your specific logic here when this game object "dies"
        mortality.OnDeath -= OnDeath;
        mortality.OnHealthDown -= OnHealthDown;
        Destroy(gameObject);
    }

    protected virtual void OnHealthDown(int value)
    {
            
    }

    protected virtual void OnEnable()
    {
        // register this class' OnDeath() function
    }

    protected virtual void OnDisable()
    {
        // deregister this class' OnDeath() function

    }
}
