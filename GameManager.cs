using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    private MapGenerator mapGenerator;
    private Spawner spawner;

    private int size = 6;

    private List<Vector2> patternPosition;

    void Start()
    {
        mapGenerator = GameObject.Find(GameObjectName.Map).GetComponent<MapGenerator>();
        spawner = GameObject.Find(GameObjectName.Spawner).GetComponent<Spawner>();
        PlacePlayer();
    }

    private void PlacePlayer()
    {
        Vector2? patternPosition = mapGenerator.GetCurrentRoom().FindRandomPattern(TilesPattern.floor3);

        if (patternPosition != null)
        {
            Instantiate(player, new Vector2(patternPosition.Value.x + 1.5f, patternPosition.Value.y + 1.5f), Quaternion.identity);
        }
        else
        {
            Application.Quit();
        }
    }

    public Tiles[,] GetCurrentMap()
    {
        if (mapGenerator!=null)
        {
            return mapGenerator.GetCurrentRoom().GetMap();
        }
        return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
           // patternPosition = map.GetCurrentRoom().FindRandomPattern(new TilesPattern(size, size, Tiles.Floor).pattern);
        }
    }

    public void OnDrawGizmos()
    {
        Tiles[,] t = GetCurrentMap();

        t = null;

        if (t == null) return;

        for (int i = 0; i < t.GetLength(0); i++)
        {
            for (int y = 0; y < t.GetLength(1); y++)
            {
                if (t[i, y] == Tiles.Floor)
                {
                    Gizmos.color = new Color(0, 1, 0, 0.3f);
                    Gizmos.DrawCube(new Vector3(i + 0.5f, y + 0.5f, 0), new Vector3(1, 1, 0));
                }
                if (t[i, y] == Tiles.Wall)
                {
                    Gizmos.color = new Color(0, 0, 1, 0.3f);
                    Gizmos.DrawCube(new Vector3(i + 0.5f, y + 0.5f, 0), new Vector3(1, 1, 0));
                }
                if (t[i, y] == Tiles.Items)
                {
                    Gizmos.color = new Color(1, 0, 0, 0.3f);
                    Gizmos.DrawCube(new Vector3(i + 0.5f, y + 0.5f, 0), new Vector3(1, 1, 0));
                }
            }
        }


        //TilesPattern patern = new TilesPattern(size, size, Tiles.Floor);

        ////patternPosition = map.GetCurrentRoom().FindRandomPattern(patern.pattern);

        //foreach (var item in patternPosition)
        //{
        //    //Gizmos.color = new Color(0, 1, 0, 0.7f);
        //    //Gizmos.DrawCube(new Vector3(item.x + 0.5f, item.y + 0.5f, 0), new Vector3(1, 1, 0));
        //    //Gizmos.color = new Color(1, 0, 1, 0.3f);
        //    //Gizmos.DrawCube(new Vector3(item.x + size / 2, item.y + size / 2, 0), new Vector3(size, size, 0));
        //}


        //// Gizmos.DrawCube(patternPosition, new Vector3(3f, 3f, 3f));

        ////if (path != null)
        ////{
        ////    for (int i = nodeIndex; i < path.Length; i++)
        ////    {
        ////        Gizmos.color = Color.black;
        ////        Gizmos.DrawCube(path[i], new Vector3(0.5f, 0.5f, 0.5f));

        ////        if (i == nodeIndex)
        ////        {
        ////            //Gizmos.DrawLine(transform.position, path[i]);
        ////        }
        ////        else
        ////        {
        ////            Gizmos.DrawLine(path[i - 1], path[i]);
        ////        }
        ////    }
        ////}





    }



}
