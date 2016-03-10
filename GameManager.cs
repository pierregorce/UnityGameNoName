using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject player
    {
        get; private set;
    }

    public MapGenerator mapGenerator;
    public UIManager uiManager;
    private Spawner spawner;
    public static GameManager instance { get; private set; }

    private List<Vector2> patternPosition;

    void Awake()
    {
        instance = this;
        player = Instantiate(playerPrefab) as GameObject;
        mapGenerator = GetComponent<MapGenerator>();
        uiManager = GetComponent<UIManager>();
    }
    void Start()
    {
        spawner = GameObject.Find(GameObjectName.Spawner).GetComponent<Spawner>();
        PlacePlayer();
        uiManager.ShowInformationUI("START", "ROOM 1", "Destroy all ennemies. 15 Remaining.");
        uiManager.SetMonsterProgress(10, 10);
        uiManager.SetRoomProgress(9, 1);
    }

    private void PlacePlayer()
    {
        Vector2? patternPosition = mapGenerator.GetCurrentRoom().FindRandomPattern(TilesPattern.floor3);

        if (patternPosition != null)
        {
            player.transform.position = new Vector2(patternPosition.Value.x + 1.5f, patternPosition.Value.y + 1.5f);
            //Camera.main.transform.parent.position = new Vector3(playerClone.transform.position.x, playerClone.transform.position.y, transform.parent.position.z);
        }
        else
        {
            Application.Quit();
        }
    }

    private Tiles[,] GetCurrentMap()
    {
        if (mapGenerator != null)
        {
            return mapGenerator.GetCurrentRoom().GetMap();
        }
        return null;
    }

    public Grid GetCurrentGrid()
    {
        if (mapGenerator != null)
        {
            return new Grid(GetCurrentMap(), mapGenerator.GetCurrentRoom().GetRect().position);
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
