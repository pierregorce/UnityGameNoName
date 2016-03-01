using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;

public class CameraManager : MonoBehaviour
{

    private GameObject player;
    private float smoothTimeX = 0.08f;
    private float smoothTimeY = 0.08f;

    void Start()
    {

    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(TagName.Player);
        }
        else
        {
            //On va deplacer la parent holder de la camera.
            //La camera quand à elle est deplacée lors du shake.
            Vector2 velocity = new Vector2();
            float posX = Mathf.SmoothDamp(transform.parent.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
            float posY = Mathf.SmoothDamp(transform.parent.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);
            transform.parent.position = new Vector3(posX, posY, transform.parent.position.z);
        }
        
    }
}
