﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;

public class GameManager : MonoBehaviour
{

    public GameObject playerPrefab;
    public LevelData levelData;

    public GameObject player { get; private set; }
    public MapGenerator mapGenerator { get; private set; }
    public UIManager uiManager { get; private set; }
    private Spawner spawner;

    public static GameManager instance { get; private set; }

    private List<Vector2> patternPosition;

    void Awake()
    {
        instance = this;
        player = Instantiate(playerPrefab) as GameObject;
        mapGenerator = GetComponent<MapGenerator>();
        uiManager = GetComponent<UIManager>();
        mapGenerator.Generate(levelData);
    }

    void Start()
    {
        spawner = GameObject.Find(GameObjectName.Spawner).GetComponent<Spawner>();
        PlacePlayer();
        EnterInRoom(mapGenerator.GetCurrentRoom());
    }

    public void EnterInRoom(Room room)
    {
        SpawnRoom(room);
        uiManager.ShowInformationUI("START", "ROOM " + (mapGenerator.GetCurrentRoomIndex() + 1), "Destroy all ennemies. " + spawner.GetEnnemyRemaining() + " Remaining.");
        uiManager.SetMonsterProgress(spawner.GetEnnemyTotal(levelData), spawner.GetEnnemyRemaining());
        uiManager.SetRoomProgress(mapGenerator.GetRoomCount(), (mapGenerator.GetCurrentRoomIndex() + 1));
    }

    void SpawnRoom(Room room)
    {
        if (!room.roomSpawned)
        {
            mapGenerator.GenerateTorch(room, new RandomInt(5, 10));
            spawner.Spawn(levelData, room);
            room.roomSpawned = true;
        }
    }


    void Update()
    {
        Debug.Log("Current Room : " + mapGenerator.GetCurrentRoomIndex());
    }

    public void SpellButtonEvent(string buttonIndex)
    {
        player.GetComponent<PlayerController>().SpellButtonEvent(buttonIndex);
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

    public Grid GetCurrentGrid()
    {
        if (mapGenerator != null)
        {
            return new Grid(mapGenerator.GetCurrentMap(), mapGenerator.GetCurrentRoom().GetRect().position);
        }
        return null;
    }

    public void OnDrawGizmos()
    {
        if (mapGenerator == null)
        {
            return;
        }
        Tiles[,] t = mapGenerator.GetCurrentMap();

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
