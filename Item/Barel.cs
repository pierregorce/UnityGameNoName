using UnityEngine;

public class Barel : MonoBehaviour {

    private Mortality mortality;
    public GameObject explosion;
    public GameObject smokeParticle;
    public GameObject barelAcid;
    
    // Use this for initialization
    void Start () {
        mortality = GetComponent<Mortality>();
        mortality.OnDeath += OnDeath;
    }

    private void OnDeath()
    {
        Camera.main.GetComponent<CameraShake>().ShakeThatBooty(CameraShake.ShakeParameters.PerlinLevel3);

        for (int i = 0; i < 20; i++)
        {
            Vector3 p = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Instantiate(explosion, new Vector2(transform.position.x + p.x, transform.position.y + p.y), Quaternion.identity);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector3 p = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Instantiate(smokeParticle, new Vector2(transform.position.x + p.x, transform.position.y + p.y), Quaternion.identity);
        }

        Instantiate(barelAcid, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        //todo add on side tiles



    }




}
