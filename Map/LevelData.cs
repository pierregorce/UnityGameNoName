using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "LevelDesign/Create Level", order = 1)]
public class LevelData : ScriptableObject
{
    public int level;
    public int roomQuantity;
    //Ennemies
    public List<RoomMonster> roomMonsters;

    //Items TODO
    //public int barelByRoom;
    //public int speedByRoom;
    //public int healthByRoom;
}

[Serializable]
public class RoomMonster
{
    public EnnemyType monsterType;
    [Range(1, 100)]
    public int proportion;
}
