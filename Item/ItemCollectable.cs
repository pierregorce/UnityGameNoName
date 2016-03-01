using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;

//faire un truc à la mortality c'est mieux

public class ItemCollectable : MonoBehaviour
{
    bool active = true;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator DeadEffect()
    {
        float percent = 0;
        float speed = 2f;
        Vector3 originalPosition = transform.position;
        Vector3 attackPosition = transform.position + new Vector3(0, 1, 0);

       // Debug.Log("start : "+ originalPosition);
        
        while (percent <= 1)
        {

            percent += Time.deltaTime * speed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            //Debug.Log("continue : " + transform.position);
            yield return null;
        }

       // Debug.Log("end");
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == TagName.Friendly && active)
        {
            other.GetComponent<PlayerItemController>().LootItem(2);
            StartCoroutine(DeadEffect());
            active = false;
        }

    }

}
