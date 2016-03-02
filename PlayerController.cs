using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 20;
    public GameObject bolt;
    private Mortality mortality;

    public GameObject bloodParticle;

    void Start()
    {
        mortality = GetComponent<Mortality>();
        mortality.initialHealth = 150;
        mortality.Revive();
        // GameObject.Find("UI Life Text").GetComponent<Text>().text = "150/150";

        mortality.OnHealthDown += OnLoseLife;
    }

    void OnLoseLife(int value)
    {
        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            Instantiate(bloodParticle, transform.position, Quaternion.identity);
        }

        //Debug.Log("Player touch ma vie est de " + mortality.health + " / " + mortality.initialHealth);
        GetComponent<PlayerItemController>().LooseLife(value);

        // GameObject.Find("UI Life Text").GetComponent<Text>().text = mortality.health + "/" + mortality.initialHealth;

        //GameObject canvasText = Instantiate(canvasFloatingText);

        //float randomX = Random.Range(-0.6f, 0.6f);
        //float randomY = Random.Range(-0.6f, 0.6f);

        //canvasText.transform.position = transform.position + new Vector3(randomX, randomY, 0);
        //canvasText.GetComponent<Animator>().SetTrigger("Item");

        //canvasText.transform.Find("Item").GetComponent<Text>().text = "+" + gold;

        //Destroy(canvasText, 1.5f);
    }

    void Update()
    {

        //Fire Input
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 start = transform.position;

            Vector2 direction = target - start;
            direction.Normalize();
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            GameObject projectile = Instantiate(bolt, start, rotation) as GameObject;
            projectile.GetComponent<Rigidbody2D>().AddForce(direction * 1000);
            projectile.GetComponent<Projectile>().sender = gameObject;

            Camera.main.GetComponent<CameraShake>().ShakeThatBooty(CameraShake.ShakeParameters.PerlinLevel1);

        }



    }

    void FixedUpdate()
    {
        //Move Input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 moveInput = new Vector2(moveHorizontal, moveVertical);
        Vector2 moveVelocity = moveInput.normalized * moveSpeed;

        GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + moveVelocity * Time.fixedDeltaTime);

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
