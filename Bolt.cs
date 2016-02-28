using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;
using System.Linq;

public class Bolt : MonoBehaviour
{
    public int damage = 5;

    public int recoilStrenght = 30;
    public int recoilSpeed = 20;

    public GameObject explosion;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool destroy = false;

        if (TagName.BlockMask.Contains(other.tag))
        {
            destroy = true;
        }

        Mortality mortality = other.GetComponent<Mortality>();

        if (mortality != null)
        {
            if (other.gameObject != gameObject && other.tag != TagName.Player)
            {
                destroy = true;
                mortality.DecrementHealth(damage);
                //todo faire reculer le target
            }
        }


        //WTF ET LES PARTICLES
        //EN FAITE IL FAUT UN MOYEN DE DISTINGUE LES WALL et destroy object edit eu si CAR ON A DES COLDER QUE AU BON ENDROID
        //Collision avec un objet non mortality
        //Debug.Log(other.name);


        //or definir un tag !
        //attention fonctionne pas avec les wall car c'est un mesh
        //CHECHER BY LAYER ORDER
        // other.GetComponent<SpriteRenderer>().sortingLayerName = "qzd"

        if (TagName.BlockMask.Contains(other.gameObject.tag))
        {
            Destroy(gameObject);
        }


        if (destroy)
        {
            Destroy(gameObject);

            for (int i = 0; i < 3; i++)
            {
                Vector3 p = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
                Instantiate(explosion, new Vector2(transform.position.x + p.x, transform.position.y + p.y), Quaternion.identity);
            }
        }


    }
}
