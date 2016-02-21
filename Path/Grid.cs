using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid
{
    public Node[,] grid;
    //public Vector2 worldBottomLeft;

    public Grid(Tiles[,] map, Vector2 worldBottomLeft)
    {
        grid = new Node[map.GetLength(0), map.GetLength(1)];
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Vector2 worldPoint = worldBottomLeft + new Vector2(x + 0.5f, y + 0.5f);
                grid[x, y] = new Node(map[x, y] != Tiles.Wall, worldPoint, x, y);
            }
        }
    }

    /// <summary>
    /// Retourne un node de la grille pour une position absolue donnée.
    /// </summary>
    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        //float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        //float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        //percentX = Mathf.Clamp01(percentX);
        //percentY = Mathf.Clamp01(percentY);

        //int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        //int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        //return grid[x, y];


        //TODO WTF ?
        return grid[((int)worldPosition.x), ((int)worldPosition.y)];
    }

    public List<Node> Get4Neighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int gridSizeX = grid.GetLength(0);
        int gridSizeY = grid.GetLength(1);

        //bas
        int checkX = node.gridX + 0;
        int checkY = node.gridY + -1;
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(grid[checkX, checkY]);
        }

        //haut
        checkX = node.gridX + 0;
        checkY = node.gridY + 1;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(grid[checkX, checkY]);
        }

        //droite
        checkX = node.gridX + 1;
        checkY = node.gridY + 0;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(grid[checkX, checkY]);
        }
        //gauche
        checkX = node.gridX + -1;
        checkY = node.gridY + 0;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(grid[checkX, checkY]);
        }

        return neighbours;
    }

    public List<Node> Get8Neighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int gridSizeX = grid.GetLength(0);
        int gridSizeY = grid.GetLength(1);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

}
