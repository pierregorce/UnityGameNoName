using UnityEngine;
using System.Collections;

public class PhysicalEntities : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float drag = 20f;
    public bool logMovement = false;
    public Vector2 currentDirection { get; private set; }
    protected Vector2 moveVelocity = Vector2.zero;
    protected Vector2 additionnalVelocity = Vector2.zero;
    public bool bumpSensible = true;

    public void ApplyForce(Vector2 force)
    {
        //La force est appliquée de manière cumulative = effet réel.
        additionnalVelocity = new Vector2(additionnalVelocity.x + force.x, additionnalVelocity.y + force.y);
    }

    public void BreakForce()
    {
        additionnalVelocity = Vector2.zero;
    }

    protected virtual void Update()
    {
        //Arrêt : la vélocity est récalculé à chaque instant. On veux donc repartir de 0 à chaque fois.
        moveVelocity = Vector2.zero;
    }

    protected virtual void LateUpdate()
    {
        //drag the additional velocity over time
        additionnalVelocity = additionnalVelocity * (1 - Time.fixedDeltaTime * drag);
        if (logMovement)
        {
            Debug.Log("Velocity " + moveVelocity + " - Additionnal : " + additionnalVelocity);
        }
        GetComponent<Rigidbody2D>().velocity = new Vector2(additionnalVelocity.x + moveVelocity.x, additionnalVelocity.y + moveVelocity.y);

        currentDirection = new Vector2(transform.position.x + moveVelocity.x, transform.position.y + moveVelocity.y) - (Vector2)transform.position;
        currentDirection.Normalize();
    }

}
