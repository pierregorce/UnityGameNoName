using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;

public class ItemDestroyable : MonoBehaviour
{


    public GameObject particle;

    // Use this for initialization
    void Start()
    {

        GetComponent<Mortality>().OnDeath += OnDeath;


    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDeath()
    {

        //todo faire shine les bloc comme http://www.sombr.com/2014/06/23/give-life-to-explosions-and-debris/

        //todo plutot mettre un placeholder d'une durée 5-6s
        
        //Particle Explosion
        for (int i = 0; i < 25; i++)
        {
            float pX = Random.Range(-0.2f, 0.2f);
            float pY = Random.Range(-0.2f, 0.2f);
            
            GameObject p = Instantiate(particle, new Vector3(transform.position.x + pX +0.5f, transform.position.y + pY+0.5f, 0), Quaternion.identity) as GameObject;

            float s = Random.Range(0.4f, 0.4f);
            p.transform.localScale = new Vector3(s, s);
        }

        //Delocate Map
        GameObject.Find(GameObjectName.Map).GetComponent<MapGenerator>().GetCurrentRoom().AddPatternToMap((int)transform.position.x, (int)transform.position.y, 1, 1, Tiles.Floor);

        //Destory
        Destroy(gameObject);

    }




}
