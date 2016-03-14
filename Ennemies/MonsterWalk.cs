using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Utils;

public class MonsterWalk : MonsterEntity
{
    public enum States
    {
        Idle, Seek, Patrol
    }

    public FiniteStateMachine<States> brain;

    private Vector2? patrolTarget;
    private Vector2 patrolStart;
    public int patrolDistance = 5;
    public int patrolAroundDistance = 2;

    private Transform target;
    Vector3 currentWaypointGizmo;
    private Vector2[] path;
    private int nodeIndex;
    private Coroutine followPathCoroutine;
    private Coroutine updatePathCoroutine;

    protected override void Start()
    {
        base.Start();

        brain = new FiniteStateMachine<States>();

        // This calls the Run() function while on run state.
        // I will probably replace it with with a state callback or something similar sometime in the future to avoid calling TryGetValue all the time.
        brain.AddTransition(States.Idle, States.Patrol, StartPatrol);
        brain.AddTransition(States.Seek, States.Patrol, StartPatrol);
        brain.AddTransition(States.Patrol, States.Patrol, DoPatrol);

        brain.AddTransition(States.Idle, States.Seek, StartSeek);
        brain.AddTransition(States.Patrol, States.Seek, StartSeek);
        brain.AddTransition(States.Seek, States.Seek, DoSeek);

        brain.AddTransition(States.Patrol, States.Idle, StartIdle);
        brain.AddTransition(States.Seek, States.Idle, StartIdle);
        brain.AddTransition(States.Idle, States.Idle, null);

        brain.Initialise(States.Idle);
    }

    void StartPatrol()
    {
        if (followPathCoroutine != null)
        {
            StopCoroutine(followPathCoroutine);
        }
        if (updatePathCoroutine != null)
        {
            StopCoroutine(updatePathCoroutine);
        }

        patrolStart = transform.position;
        patrolTarget = GameManager.instance.mapGenerator.GetCurrentRoom().FindNearEmptyPosition(transform.position, patrolAroundDistance);

        if (patrolTarget != null)
        {
            updatePathCoroutine = StartCoroutine(UpdatePath(new PathTarget(patrolTarget.Value)));
        }
    }

    void StartSeek()
    {
        if (followPathCoroutine != null)
        {
            StopCoroutine(followPathCoroutine);
        }
        if (updatePathCoroutine != null)
        {
            StopCoroutine(updatePathCoroutine);
        }
        updatePathCoroutine = StartCoroutine(UpdatePath(new PathTarget(target.transform)));
    }

    void StartIdle()
    {
        if (followPathCoroutine != null)
        {
            StopCoroutine(followPathCoroutine);
        }
        if (updatePathCoroutine != null)
        {
            StopCoroutine(updatePathCoroutine);
        }
    }

    void DoPatrol()
    {
        if (patrolTarget == null)
        {
            StartPatrol();
        }

        Node node = GameManager.instance.GetCurrentGrid().NodeFromWorldPoint(transform.position);
        Node nodeTarget = GameManager.instance.GetCurrentGrid().NodeFromWorldPoint(patrolTarget.Value);

        if (patrolTarget != null && node.Equals(nodeTarget))
        {
            StartPatrol();
        }
    }

    void DoSeek()
    {
        //Debug.Log("Je seek");
        //En faite rien à mettre ici...
    }

    void DoAttack()
    {
        if (Time.time > nextAttackTime)
        {
            nextAttackTime = Time.time + timeBetweenAttacks;
            target.GetComponent<Mortality>().DecrementHealth(baseAttackDamage);

            Vector2 direction = target.position - transform.position; //direction entre this and target
            direction.Normalize();
            target.GetComponent<PhysicalEntities>().ApplyForce(direction * 5);
        }
    }

    void Brain()
    {

        float distance = (target.position - transform.position).magnitude;

        if (distance < 0.75)
        {
            DoAttack();
        }

        if (distance <= seekDistance)
        {
            brain.Advance(States.Seek);

        }
        else if (distance > seekDistance && distance < seekDistance + patrolDistance)
        {
            brain.Advance(States.Patrol);
        }
        else
        {
            brain.Advance(States.Idle);
        }
    }

    protected override void Update()
    {
        base.Update();
        target = GameManager.instance.player.transform;
        Brain();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        //todo placeholder death for permanence
    }

    protected override void OnHealthDown(int value)
    {
        base.OnHealthDown(value);

        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            //GameObject hurtedParticle = Instantiate(hurtParticle, transform.position, Quaternion.identity) as GameObject;
            //hurtedParticle.transform.parent = GameObject.Find(GameObjectName.GameManager).transform;
            //todo random spawn

            GameObject g = ObjectPool.instance.GetPooledObject(hurtParticle);
            g.GetComponent<Particle>().Init();
            g.transform.position = transform.position;

        }
    }

    private IEnumerator FollowPath()
    {
        //BLOCAGE LORSQUE LE PATH CHANGE EN COURS DE TRAJET
        //une fois ça reglé, regler la position de pop du player
        //regler les tir ensuite ? !
        //Debug.Log("init follow path");
        Vector2 currentWaypoint = path[0];
        currentWaypointGizmo = path[0];
        //transform.position = currentWaypoint;

        while (true)
        {
            if (target == null || path == null)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                //Debug.Log("Break");
                yield break;
            }

            if ((Vector2)transform.position == currentWaypoint)
            {
                //Debug.Log("index increment");
                nodeIndex++;

                if (nodeIndex >= path.Length)
                {
                    Debug.Log("reached end path");
                    nodeIndex = 0;
                    path = null;
                    yield break;
                }

                currentWaypoint = path[nodeIndex];
                currentWaypointGizmo = path[nodeIndex];
            }

            //For non physic object 
            //Pas obligatoire d'utiliser un rigidbody car le path esquive deja les murs // ERREUR SI POUR POUVOIR RECULER il faut modifier la velocity

            //Debug.Log("index : " + nodeIndex + " , destination : " + currentWaypoint + " " + currentWaypoint.GetHashCode());
            //Debug.Log(Vector2.Distance(target.position, transform.position));








            float distance = Vector2.Distance(target.position, transform.position);



            //if (distance < seekDistance && distance > 0.7f)
            //{
            Vector2 direction = currentWaypoint - (Vector2)transform.position; //direction entre this and target
            direction.Normalize();
            moveVelocity = direction * moveSpeed;
            //transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentWaypoint.x, currentWaypoint.y), speed * Time.deltaTime);
            //}




            //Pas de modificatin si move = 0
            float moveHorizontal = target.position.x - currentWaypoint.x;
            if (orientableEntity != null)
            {

                if (moveHorizontal > 0)
                {
                    orientableEntity.CurrentFacingOrientation = OrientableEntity.FacingOrientation.FACING_RIGHT;
                }

                if (moveHorizontal < 0)
                {
                    orientableEntity.CurrentFacingOrientation = OrientableEntity.FacingOrientation.FACING_LEFT;
                }
            }

            //transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

            //On le déplace comme un objet physique pour avoir les collisions entre ennemies ?
            //Vector2 direction = currentWaypoint - transform.position;
            //GetComponent<Rigidbody2D>().velocity = direction.normalized * 4; //no time for velocity car elle est ajouté apres dans le moteur physique
            yield return null;


            //soit grille trop petite soit trop grosse ?

        }
    }

    private IEnumerator UpdatePath(PathTarget pathTarget)
    {
        while (true)
        {
            if (pathTarget != null)
            {
                PathfindingManager.getInstance().RequestPath(transform.position, pathTarget.GetTarget(), GameManager.instance.GetCurrentGrid(), OnPathFound);
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful && newPath.Length > 0 && target != null)
        {
            if (followPathCoroutine != null)
            {
                StopCoroutine(followPathCoroutine);
            }

            nodeIndex = 0;
            path = newPath;
            //Debug.Log("Bam path found for state : " + brain.GetState());
            followPathCoroutine = StartCoroutine(FollowPath());
        }
        else
        {
            path = null;
        }
    }

    class PathTarget
    {
        Transform mobileTarget = null;
        Vector2? fixedTarget = null;

        public PathTarget(Vector2 fixedTarget)
        {
            this.fixedTarget = fixedTarget;
        }
        public PathTarget(Transform mobileTarget)
        {
            this.mobileTarget = mobileTarget;
        }
        public Vector2 GetTarget()
        {
            if (mobileTarget == null)
            {
                return fixedTarget.Value;
            }
            else
            {
                return mobileTarget.position;
            }
        }

    }

    public void OnDrawGizmos()
    {
        if (isGizmos)
        {
            if (path != null)
            {
                for (int i = nodeIndex; i < path.Length; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(path[i], 0.5f * Vector2.one);

                    if (i == nodeIndex)
                    {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(currentWaypointGizmo, 0.5f * Vector2.one);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, 0), seekDistance);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, 0), seekDistance + patrolDistance);

        }


    }
}


