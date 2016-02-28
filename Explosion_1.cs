using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosion_1 : MonoBehaviour
{

    private List<string> triggers = new List<string>() {
        "Type1", "Type2", "Type3"
    };

    void Start()
    {
        Animator animator = GetComponent<Animator>();

        // Select a explosion animation
        animator.SetTrigger(triggers[Random.Range(0, triggers.Count)]);
 
        // Speed it
        animator.speed = Random.Range(1.15f, 1.35f);

        // Scale it
        float s = Random.Range(0.3f, 0.6f);
        transform.localScale = new Vector3(s, s);

        // Color it
        if (Random.Range(0, 100) > 66)
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }

        //Destroy it
        Destroy(gameObject, 3f);

    }
}
