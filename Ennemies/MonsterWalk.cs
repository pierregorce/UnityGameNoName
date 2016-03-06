﻿using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Utils;

public class MonsterWalk : MonsterEntity
{
    //enum States
    //{
    //    Idle, Chasing, Bumped, Attacking
    //}
    //private States state;


    private float nextNewPatrolTime;
    private float timeBetweenNextNewPatrol = 3.5f;


    private Transform target;

    private Vector2[] path;
    private int nodeIndex;
    private Coroutine coroutine;
    private Tiles[,] map;

    protected override void Start()
    {
        base.Start();
        map = GameManager.instance.GetCurrentMap();
        StartCoroutine(UpdatePath());
    }

    protected override void Update()
    {
        base.Update();
        target = GameManager.instance.player.transform;
        float distance = (target.position - transform.position).magnitude;


        //Attack
        if (Time.time > nextAttackTime)
        {
            if (distance <= 0.75)
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                target.GetComponent<Mortality>().DecrementHealth(baseAttackDamage);

                Vector2 direction = target.position - transform.position; //direction entre this and target
                direction.Normalize();
                target.GetComponent<PhysicalEntities>().ApplyForce(direction * 5);
            }
        }


        //Patrol
        if (distance < seekDistance) ///todo mettre du state c'est plus propre
        {

        }


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
            GameObject hurtedParticle = Instantiate(hurtParticle, transform.position, Quaternion.identity) as GameObject;
            hurtedParticle.transform.parent = GameObject.Find(GameObjectName.GameManager).transform;
            //todo random spawn
        }
    }

    Vector3 currentWaypointGizmo;

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


            if (distance < seekDistance && distance > 0.7f)
            {
                Vector2 direction = currentWaypoint - (Vector2)transform.position; //direction entre this and target
                direction.Normalize();
                moveVelocity = direction * moveSpeed;
                //transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentWaypoint.x, currentWaypoint.y), speed * Time.deltaTime);
            }
            else
            {
                moveVelocity = Vector2.zero;
            }

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

    private IEnumerator UpdatePath()
    {
        while (true)
        {
            if (target != null)
            {
                map = GameObject.Find(GameObjectName.GameManager).GetComponent<GameManager>().GetCurrentMap();
                PathfindingManager.getInstance().RequestPath(transform.position, target.position, new Grid(map, new Vector2(0, 0)), OnPathFound);
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful && newPath.Length > 0 && target != null)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            nodeIndex = 0;
            path = newPath;
            //Debug.Log("new path found, relance de follow path");
            coroutine = StartCoroutine(FollowPath());
        }
        else
        {
            path = null;
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


        }


    }



}
