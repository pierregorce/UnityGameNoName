using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : PhysicalEntities
{
    [Header("Common")]
    public GameObject bolt;
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
            Instantiate(bloodParticle, transform.position, Quaternion.identity);
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
                Instantiate(speedParticle, transform.position, Quaternion.identity);
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
        //Fire Input
        if (Input.GetMouseButtonDown(0))
        {
            //Shoot
            Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 start = transform.position;
            Vector2 direction = target - start;
            direction.Normalize();
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            //Send Projectile
            GameObject projectile = Instantiate(bolt, start, rotation) as GameObject;
            projectile.GetComponent<Rigidbody2D>().AddForce(direction * 1000);
            projectile.GetComponent<Projectile>().sender = gameObject;

            //Camera Shake
            Camera.main.GetComponent<CameraShake>().ShakeThatBooty(CameraShake.ShakeParameters.PerlinLevel1);

            //Bump back
            ApplyForce(-direction * 1.2f);
        }


        if (Input.GetMouseButtonDown(1))
        {
            //Shoot spell 2 : nova
            int j = Random.Range(10, 30);
            for (int i = 0; i < 360; i += j)
            {
                Quaternion rotation = Quaternion.AngleAxis(i, Vector3.forward);
                Vector2 direction = rotation * Vector3.right;
                GameObject projectile = Instantiate(bolt, transform.position, rotation) as GameObject;
                projectile.GetComponent<Rigidbody2D>().AddForce(direction * 1200);
                projectile.GetComponent<Projectile>().sender = gameObject;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            //Spell 3 : diversion (like wow hunt)
            GetComponent<PhysicalEntities>().ApplyForce(currentDirection * 20);
            //add une trainée au sol
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            //Spell 4 : repousse (like diablo wizard)
            //=> select all mortality and add small damage
            //=> select all movable and add force ump back
        }



        //Move Input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

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

}
