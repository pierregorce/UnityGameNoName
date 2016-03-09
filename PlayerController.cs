using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : PhysicalEntities
{
    [Header("Common")]
    public GameObject bolt;
    public GameObject bloodParticle;
    private Mortality mortality;

    [HideInInspector]
    public int initialXp = 0;
    [HideInInspector]
    public int currentXp = 0;

    void Start()
    {
        mortality = GetComponent<Mortality>();
        mortality.initialHealth = 150;
        mortality.Revive();
        // GameObject.Find("UI Life Text").GetComponent<Text>().text = "150/150";

        mortality.OnHealthDown += OnLoseLife;

        GameManager.instance.uiManager.SetLife(mortality.health, mortality.health);
        GameManager.instance.uiManager.SetXp(currentXp, currentXp);

    }

    void OnLoseLife(int value)
    {
        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            Instantiate(bloodParticle, transform.position, Quaternion.identity);
        }

        GetComponent<PlayerItemController>().LooseLife(value);
    }

    protected override void Update()
    {
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
