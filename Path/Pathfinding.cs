using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Pathfinding
{

    public Grid grid;

    public Pathfinding(Tiles[,] map, Vector2 worldBottomLeft)
    {
        grid = new Grid(map, worldBottomLeft);
    }

    /// <summary>
    /// Request path instantanément ( 4 directions )
    /// </summary>
    public Vector2[] RequestPath(Vector2 startPos, Vector2 targetPos)
    {
        //Debug.Log("Recherche de " + startPos + "à" + targetPos);
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
                foreach (Node neighbour in grid.Get4Neighbours(currentNode))
                {
                    //Si le node est un obstacle ou est dans la liste fermée ignorez-le et passer à l'analyse d'un autre node.
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }


                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        /* on calcule le nouveau g */
                        neighbour.gCost = newMovementCostToNeighbour;
                        /*on calcule le nouveau h */
                        neighbour.hCost = GetDistance(neighbour, targetNode);

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

        // on est sorti de la liste, on a deux solutions, soit la liste ouverte est vide dans ces cas là il 
        // n'y a pas de solutions et on retoure directement finalPath;

        if (pathSuccess)
        {
            // Soit on maintenant on construit le chemin à rebours;
            waypoints = RetracePath(startNode, targetNode);
        }

        //Debug.Log("Simple Path Finish Status : " + pathSuccess.ToString() + " - Lenght : " + waypoints.Length);
        return waypoints;


    }

    /// <summary>
    /// Request path sans surcharge CPU ( 8 direction )
    /// </summary>

    /// <summary>
    /// Retrace le path de parent en parent
    /// </summary>
    /// <returns></returns>
    public static Vector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        
        Node currentNode = endNode;

        while ((currentNode != startNode))
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        
        Vector2[] waypoints = path.Select(m => m.worldPosition).ToArray();
        Array.Reverse(waypoints);
        return waypoints;

    }

    private static Vector2[] SimplifyPath(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    public static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return (dstX + dstY) * 10;
        //if (dstX > dstY)
        //    return 999 * dstY + 10 * (dstX - dstY);
        //return 999 * dstX + 10 * (dstY - dstX);
    }



}
