using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator
{

    /// <summary>
    /// Retourne un mesh pour les valeurs 1 d'une map.
    /// </summary>
    public static Mesh GenerateSquaredMesh(int[,] map, int squareSize, int value)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices;
        List<int> triangles;
        List<Vector2> uvs;

        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == value)
                {
                    vertices.Add(new Vector3((x * squareSize), (y * squareSize), 0));

                    float percentX = Mathf.InverseLerp(0, map.GetLength(0), vertices[vertices.Count - 1].x);
                    float percentY = Mathf.InverseLerp(0, map.GetLength(1), vertices[vertices.Count - 1].y);
                    uvs.Add(new Vector2(percentX, percentY));



                    vertices.Add(new Vector3((x * squareSize) + squareSize, (y * squareSize), 0));

                    percentX = Mathf.InverseLerp(0, map.GetLength(0), vertices[vertices.Count - 1].x);
                    percentY = Mathf.InverseLerp(0, map.GetLength(1), vertices[vertices.Count - 1].y);
                    uvs.Add(new Vector2(percentX, percentY));




                    vertices.Add(new Vector3((x * squareSize) + squareSize, (y * squareSize) + squareSize, 0));

                    percentX = Mathf.InverseLerp(0, map.GetLength(0), vertices[vertices.Count - 1].x);
                    percentY = Mathf.InverseLerp(0, map.GetLength(1), vertices[vertices.Count - 1].y);
                    uvs.Add(new Vector2(percentX, percentY));




                    vertices.Add(new Vector3((x * squareSize), (y * squareSize) + squareSize, 0));

                    percentX = Mathf.InverseLerp(0, map.GetLength(0), vertices[vertices.Count - 1].x);
                    percentY = Mathf.InverseLerp(0, map.GetLength(1), vertices[vertices.Count - 1].y);
                    uvs.Add(new Vector2(percentX, percentY));
                }


            }
        }

        int index = 0;

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == value)
                {
                    triangles.Add(index + 2);
                    triangles.Add(index + 1);
                    triangles.Add(index);
                    triangles.Add(index);
                    triangles.Add(index + 3);
                    triangles.Add(index + 2);
                    index += 4;
                }
            }
        }

     
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

}
