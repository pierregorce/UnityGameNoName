using UnityEngine;

public class Barel : MonoBehaviour {

    private Mortality mortality;
    public GameObject explosion;


    // Use this for initialization
    void Start () {
        mortality = GetComponent<Mortality>();
        mortality.OnDeath += OnDeath;
    }

    private void OnDeath()
    {
        Camera.main.GetComponent<CameraShake>().ShakeThatBooty(CameraShake.ShakeParameters.MediumPerlin);
        for (int i = 0; i < 8; i++)
        {
            Vector3 p = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
            Instantiate(explosion, new Vector2(transform.position.x + p.x, transform.position.y + p.y), Quaternion.identity);
        }
    }




}
