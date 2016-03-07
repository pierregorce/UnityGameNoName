using UnityEngine;
using System.Collections;
using System;

public class Node 
    : System.IEquatable<Node>
{

    public bool walkable;
    public Vector2 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool _walkable, Vector2 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public override string ToString()
    {
        return "GridX : " + gridX + " / GridY : " + gridY;
    }

    public bool Equals(Node other)
    {
        if (other.gridX == gridX && other.gridY == gridY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
