using UnityEngine;
using System.Collections;

public class Mortality : MonoBehaviour {

    public int initialHealth = 100;
    public int health { get; private set; }
    public bool dead { get; private set; }

    // events
    public delegate void OnDeathEvent();
    public event OnDeathEvent OnDeath = delegate { };
    public delegate void OnReviveEvent();
    public event OnReviveEvent OnRevive = delegate { };
    public delegate void OnHealthDownEvent(int value);
    public event OnHealthDownEvent OnHealthDown = delegate { };

    public GameObject deadPlaceholder;


    void Start()
    {
        // init
        Revive();
    }

    public void Revive()
    {
        health = initialHealth;
        dead = (health <= 0);
        OnRevive();
    }

    public void DecrementHealth(int amount)
    {
        health -= amount;
        OnHealthDown(amount);

        if (health <= 0 && !dead)
        {
            dead = true;

            if (deadPlaceholder != null)
            {
                int[] rotationAngles = { 0, 90, 180, 270 };

                Vector2 anchor = GameManager.instance.GetCurrentGrid().WorldPointFromNode(transform.position);

                GameObject placeholderClone = Instantiate(deadPlaceholder, anchor, Quaternion.identity) as GameObject;

                placeholderClone.transform.Rotate(new Vector3(0, 0, rotationAngles[Random.Range(0, rotationAngles.Length)]));
                float s = Random.Range(0.8f, 1.2f);
                placeholderClone.transform.localScale = new Vector3(placeholderClone.transform.localScale.x * s, placeholderClone.transform.localScale.y * s, placeholderClone.transform.localScale.z);
                Destroy(placeholderClone, 20);
            }
            OnDeath();

        }
    }
}
