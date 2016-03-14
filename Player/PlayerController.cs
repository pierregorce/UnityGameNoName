using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : PhysicalEntities
{
    [Header("Common")]
    public GameObject bolt;
    public GameObject boltNova;
    public GameObject bloodParticle;
    public GameObject speedParticle;
    private Mortality mortality;

    [HideInInspector]
    public int currentXp = 0;
    [HideInInspector]
    public int currentMoney = 0;

    public float baseSpeed = 3.5f;
    public float currentSpeedIncrement = 0;
    public float currentSpeedTimeLimit = 0f;

    private float nextAttackTime;
    public float timeBetweenAttack = 0.3f;

    void Start()
    {
        mortality = GetComponent<Mortality>();
        mortality.initialHealth = 150;
        mortality.Revive();
        // GameObject.Find("UI Life Text").GetComponent<Text>().text = "150/150";

        mortality.OnHealthDown += OnLoseLife;

        GameManager.instance.uiManager.SetLife(mortality.health, mortality.health);
        GameManager.instance.uiManager.SetXp(currentXp, currentXp);
        GameManager.instance.uiManager.SetMoney(currentMoney, currentMoney);
    }

    void OnLoseLife(int value)
    {
        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            GameObject g = ObjectPool.instance.GetPooledObject(bloodParticle);
            g.GetComponent<Particle>().Init();
            g.transform.position = transform.position;
            //Instantiate(bloodParticle, transform.position, Quaternion.identity);
        }

        GetComponent<PlayerStatsController>().LooseLife(value);
    }

    public void GainSpeed(float speedIncrement, float duration)
    {
        currentSpeedIncrement = speedIncrement;
        currentSpeedTimeLimit = Time.time + duration;
        StartCoroutine(SpeedParticule(0.2f));
    }

    IEnumerator SpeedParticule(float seconds)
    {
        while (Time.time <= currentSpeedTimeLimit)
        {
            yield return new WaitForSeconds(seconds);

            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                GameObject g = ObjectPool.instance.GetPooledObject(bloodParticle);
                g.GetComponent<Particle>().Init();
                g.transform.position = transform.position;
                //Instantiate(speedParticle, transform.position, Quaternion.identity);
            }
        }
        yield break;
    }

    protected override void Update()
    {
        if (Time.time >= currentSpeedTimeLimit)
        {
            moveSpeed = baseSpeed;
        }
        else
        {
            moveSpeed = baseSpeed + baseSpeed * currentSpeedIncrement;
        }

        base.Update();
        AttackBrain();

        //Move Input

        //float moveHorizontal = Input.GetAxis("Horizontal");
        float moveHorizontal = CnControls.CnInputManager.GetAxis("Horizontal");
        //float moveVertical = Input.GetAxis("Vertical");
        float moveVertical = CnControls.CnInputManager.GetAxis("Vertical");

        Vector2 moveInput = new Vector2(moveHorizontal, moveVertical);
        Vector2 moveVelocity = moveInput.normalized * moveSpeed;
        this.moveVelocity = moveVelocity;

        //Pas de modificatin si move = 0
        if (moveHorizontal > 0)
        {
            GetComponent<OrientableEntity>().CurrentFacingOrientation = OrientableEntity.FacingOrientation.FACING_RIGHT;
        }
        if (moveHorizontal < 0)
        {
            GetComponent<OrientableEntity>().CurrentFacingOrientation = OrientableEntity.FacingOrientation.FACING_LEFT;
        }
    }

    public void SpellButtonEvent(string buttonIndex)
    {
        Spell spell;
        switch (buttonIndex)
        {
            case "0":
                spell = GameObject.Find("SpellButton0").GetComponent<Spell>();
                if (spell.CanDoSpell())
                {
                    spell.DoSpell();
                    Spell_Bolt();
                }
                break;
            case "1":
                spell = GameObject.Find("SpellButton1").GetComponent<Spell>();
                if (spell.CanDoSpell())
                {
                    spell.DoSpell();
                    Spell_Nova();
                }
                break;
            case "2":
                break;
            default:
                break;
        }
    }

    void AttackBrain()
    {
        //Fire Input
        float baseShootHorizontal = CnControls.CnInputManager.GetAxis("HorizontalShoot");
        float baseShootVertical = CnControls.CnInputManager.GetAxis("VerticalShoot");

        if (baseShootHorizontal != 0f || baseShootVertical != 0f || Input.GetMouseButtonDown(0))
        {
            SpellButtonEvent("0");
        }
        if (baseShootHorizontal != 0f || baseShootVertical != 0f )
        {
            SpellButtonEvent("0");
        }

        if (Input.GetMouseButtonDown(1))
        {
            SpellButtonEvent("1");
        }
    }

    void Spell_Bolt()
    {
        //Fire Input
        float baseShootHorizontal = CnControls.CnInputManager.GetAxis("HorizontalShoot");
        float baseShootVertical = CnControls.CnInputManager.GetAxis("VerticalShoot");
        //Shoot
        Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        Vector2 start = transform.position;
        Vector2 direction = (new Vector2(baseShootHorizontal, baseShootVertical));
        if (Input.GetMouseButtonDown(0))
        {
            direction = target - start;
        }
        //Vector2 direction = target - start;
        direction.Normalize();
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        //Send Projectile
        GameObject projectile = ObjectPool.instance.GetPooledObject(bolt);
        projectile.GetComponent<Projectile>().Init();
        projectile.transform.position = start;
        projectile.transform.rotation = rotation;

        //GameObject projectile = Instantiate(bolt, start, rotation) as GameObject;
        //projectile.GetComponent<Rigidbody2D>().AddForce(direction * 1000);
        projectile.GetComponent<Projectile>().sender = gameObject;
        projectile.GetComponent<PhysicalEntities>().ApplyForce(direction * 100);

        //Camera Shake
        Camera.main.GetComponent<CameraShake>().ShakeThatBooty(CameraShake.ShakeParameters.PerlinLevel1);

        //Bump back
        ApplyForce(-direction * 1.2f);
    }

    void Spell_Nova()
    {
        //Shoot spell 2 : nova
        int j = Random.Range(5, 20);
        for (int i = 0; i < 360; i += j)
        {
            Quaternion rotation = Quaternion.AngleAxis(i, Vector3.forward);
            Vector2 direction = rotation * Vector3.right;
            //GameObject projectile = Instantiate(boltNova, transform.position, rotation) as GameObject;

            GameObject projectile = ObjectPool.instance.GetPooledObject(boltNova);
            projectile.GetComponent<Projectile>().Init();
            projectile.transform.position = transform.position;
            projectile.transform.rotation = rotation;
            //projectile.GetComponent<Rigidbody2D>().AddForce(direction * 1200);
            projectile.GetComponent<PhysicalEntities>().ApplyForce(direction * 100);
            projectile.GetComponent<Projectile>().sender = gameObject;
        }
    }

    void Spell_Diversion()
    {
        //Spell 3 : diversion (like wow hunt)
        GetComponent<PhysicalEntities>().ApplyForce(currentDirection * 20);
        //add une trainée au sol
    }

    void Spell_KnockBack()
    {
        //Spell 4 : repousse (like diablo wizard)
        //=> select all mortality and add small damage
        //=> select all movable and add force ump back
    }

}
