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
            Vector2 velocity = new Vector2();
            float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
            float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);
            transform.position = new Vector3(posX, posY, transform.position.z);
        }
    }
}
