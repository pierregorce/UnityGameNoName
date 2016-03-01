using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PathfindingManager :MonoBehaviour
{

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    bool isProcessingPath;

    public static PathfindingManager pathFindingManager;

    void Awake()
    {
        pathFindingManager = this;
    }


    private PathfindingManager()
    {
        isProcessingPath = false;
    }

    public static PathfindingManager getInstance()
    {
        return pathFindingManager;
    }

    public struct PathRequest
    {
        public Vector2 pathStart;
        public Vector2 pathEnd;
        public Action<Vector2[], bool> callback;
        public Grid grid;

        public PathRequest(Vector2 _start, Vector2 _end,  Grid _grid, Action<Vector2[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
            grid = _grid;
        }

    }

    /// <summary>
    /// Request path sans surcharge CPU ( 8 direction )
    /// </summary>
    public void RequestPath(Vector2 pathStart, Vector2 pathEnd, Grid grid, Action<Vector2[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, grid, callback);
        pathRequestQueue.Enqueue(newRequest);
        TryProcessNext();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos, Grid grid)
    {
        StartCoroutine(FindPath(startPos, targetPos, grid));
    }

    private IEnumerator FindPath(Vector2 startPos, Vector2 targetPos, Grid grid)
    {

        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0) //  stopper la boucle si la liste ouverte est vide
            {

                // a. Récupération du node avec le plus petit F contenu dans la liste ouverte. On le nommera CURRENT.
                Node currentNode = openSet[0];
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                // b. Basculer CURRENT dans la liste fermée.
                closedSet.Add(currentNode);

                //  stopper la boucle si on ajoute le noeud d'arrivée à la liste fermée
                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }


                //  récupération des voisins de CURRENT
                // Pour chacun des nodes adjacents à CURRENT appliquer la méthode suivante:
                foreach (Node neighbour in grid.Get8Neighbours(currentNode))
                {
                    //Si le node est un obstacle ou est dans la liste fermée ignorez-le et passer à l'analyse d'un autre node.
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }


                    int newMovementCostToNeighbour = currentNode.gCost + Pathfinding.GetDistance(currentNode, neighbour);

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        /* on calcule le nouveau g */
                        neighbour.gCost = newMovementCostToNeighbour;
                        /*on calcule le nouveau h */
                        neighbour.hCost = Pathfinding.GetDistance(neighbour, targetNode);

                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }
        else
        {
            Debug.Log("erreur les tiles a de depart fin ne sont pas walkable / debut : " + startNode.walkable + "fin : " + targetNode.walkable);
        }

        yield return null; //???


        // on est sorti de la liste, on a deux solutions, soit la liste ouverte est vide dans ces cas là il 
        // n'y a pas de solutions et on retoure directement finalPath;

        if (pathSuccess)
        {
            // Soit on maintenant on construit le chemin à rebours;
            waypoints = Pathfinding.RetracePath(startNode, targetNode);
        }

        //Debug.Log("Simple Path Finish Status : " + pathSuccess.ToString() + " - Lenght : " + waypoints.Length);

        FinishedProcessingPath(waypoints, pathSuccess);

    }

    public void FinishedProcessingPath(Vector2[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.grid);
        }
    }

    


}

