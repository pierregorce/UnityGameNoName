using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Tiles
{
    Floor = 0,
    Wall = 1,
    Special = 2,
    Items = 3,
    UnSet = 4
}

public enum TilesDisposition //Clockwise
{
    Nord, NordEst, Est, SudEst, Sud, SudOuest, Ouest, NordOuest
}

public struct TilesPattern
{
    public static Tiles[,] floor1 = new Tiles[1, 1] { { Tiles.Floor } };
    public static Tiles[,] floor3 = new Tiles[3, 3] { { Tiles.Floor, Tiles.Floor, Tiles.Floor }, { Tiles.Floor, Tiles.Floor, Tiles.Floor }, { Tiles.Floor, Tiles.Floor, Tiles.Floor } };

    public static Tiles[,] wallSud = new Tiles[3, 3] {
            {
                Tiles.Wall,
                Tiles.Floor,
                Tiles.Floor },
            {
                Tiles.Wall,
                Tiles.Floor,
                Tiles.Floor
            },
            {
                Tiles.Wall,
                Tiles.Floor,
                Tiles.Floor
            }
        };

    public static Tiles[,] wallNord = new Tiles[3, 3] {
            {
                Tiles.Floor,
                Tiles.Floor,
                Tiles.Wall },
            {
                Tiles.Floor,
                Tiles.Floor,
                Tiles.Wall
            },
            {
                Tiles.Floor,
                Tiles.Floor,
                Tiles.Wall
            }
        };

    public static Tiles[,] wallOuest = new Tiles[3, 3] {
            {
                Tiles.Wall,
                Tiles.Wall,
                Tiles.Wall },
            {
                Tiles.Floor,
                Tiles.Floor,
                Tiles.Floor
            },
            {
                Tiles.Floor,
                Tiles.Floor,
                Tiles.Floor
            }
        };

    public static Tiles[,] wallEst = new Tiles[3, 3] {
            {
                Tiles.Floor,
                Tiles.Floor,
                Tiles.Floor },
            {
                Tiles.Floor,
                Tiles.Floor,
                Tiles.Floor
            },
            {
                Tiles.Wall,
                Tiles.Wall,
                Tiles.Wall
            }
        };



    public Tiles[,] pattern;

    public TilesPattern(int sizeX, int sizeY, Tiles tileType)
    {
        pattern = new Tiles[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                pattern[i, j] = tileType;
            }
        }
    }

    public TilesPattern(Tiles[,] pattern)
    {
        this.pattern = pattern;
    }

    public TilesPattern(int sizeX, int sizeY, Tiles tileType, int borderSize, Tiles borderType)
    {
        //TODO
        pattern = null;
    }

}

public enum Side
{
    Left, Right, Up, Down
};

public struct Coord
{
    public int x, y;

    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Vector3 ToVect3()
    {
        return new Vector3(x, y, 0);
    }
    public Vector2 ToVect2()
    {
        return new Vector2(x, y);
    }

    public bool IsZero()
    {
        return x == 0 && y == 0;
    }
}

public class MapUtils
{
    /// <summary>
    /// Set une map à une valeur mais pas les bord
    /// </summary>
    public static void Set(Tiles[,] map, int x, int y, int width, int height, Tiles type)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                if (i < 1 || j < 1 || i >= map.GetLength(0) - 1 | j >= map.GetLength(1) - 1)
                {
                    //Out
                }
                else
                {
                    map[i, j] = type;

                }
            }
        }
        //return map;
    }

    /// <summary>
    /// Set les bord en plus
    /// </summary>
    public static void SetFull(Tiles[,] map, int x, int y, int width, int height, Tiles type)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                if (i < 0 || j < 0 || i >= map.GetLength(0) | j >= map.GetLength(1))
                {
                    //Out
                }
                else
                {
                    map[i, j] = type;

                }
            }
        }
        //return map;
    }

    public static List<Coord> Get4Neighbours(Tiles[,] grid, int gridX, int gridY)
    {
        List<Coord> neighbours = new List<Coord>();

        int gridSizeX = grid.GetLength(0);
        int gridSizeY = grid.GetLength(1);

        //bas
        int checkX = gridX + 0;
        int checkY = gridY + -1;
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(new Coord(checkX, checkY));
        }

        //haut
        checkX = gridX + 0;
        checkY = gridY + 1;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(new Coord(checkX, checkY));
        }

        //droite
        checkX = gridX + 1;
        checkY = gridY + 0;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(new Coord(checkX, checkY));
        }
        //gauche
        checkX = gridX + -1;
        checkY = gridY + 0;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
            neighbours.Add(new Coord(checkX, checkY));
        }

        return neighbours;

    }

    public static int[,] ConvertMap(Tiles[,] map)
    {
        int[,] convertedMap = new int[map.GetLength(0), map.GetLength(1)];

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                convertedMap[i, j] = (int)map[i, j];
            }
        }
        return convertedMap;
    }

    public static string GetTileSpriteName(Tiles[,] map, int x, int y)
    {
        Tiles current = map[x, y];
        if (current == Tiles.Wall)
        {
            //----------- ANGLES (8)

            if (CheckTilesNeighbours(map, x, y, nord: null, nordEst: null, est: Tiles.Wall, sudEst: Tiles.Floor, sud: Tiles.Wall, sudOuest: null, ouest: null, nordOuest: null)) return "tiles_1";

            if (CheckTilesNeighbours(map, x, y, nord: null, nordEst: null, est: null, sudEst: null, sud: Tiles.Wall, sudOuest: Tiles.Floor, ouest: Tiles.Wall, nordOuest: null)) return "tiles_3";



            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Wall, nordEst: null, est: Tiles.Floor, sudEst: Tiles.Floor, sud: Tiles.Floor, sudOuest: null, ouest: Tiles.Wall, nordOuest: null)) return "tiles_7";

            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Wall, nordEst: null, est: Tiles.Wall, sudEst: null, sud: Tiles.Floor, sudOuest: Tiles.Floor, ouest: Tiles.Floor, nordOuest: null)) return "tiles_9";




            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Wall, nordEst: Tiles.Floor, est: Tiles.Wall, sudEst: null, sud: null, sudOuest: null, ouest: null, nordOuest: null)) return "tiles_18";

            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Floor, nordEst: Tiles.Floor, est: Tiles.Floor, sudEst: null, sud: Tiles.Wall, sudOuest: null, ouest: Tiles.Wall, nordOuest: null)) return "tiles_19";



            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Floor, nordEst: null, est: Tiles.Wall, sudEst: null, sud: Tiles.Wall, sudOuest: null, ouest: Tiles.Floor, nordOuest: Tiles.Floor)) return "tiles_21";

            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Wall, nordEst: null, est: null, sudEst: null, sud: null, sudOuest: null, ouest: Tiles.Wall, nordOuest: Tiles.Floor)) return "tiles_23";

            // ---------------- STRAIGHT (4)

            //Mur droit haut
            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Floor, nordEst: null, est: null, sudEst: null, sud: null, sudOuest: null, ouest: null, nordOuest: null)) return "tiles_32";
            //Mur droit gauche
            if (CheckTilesNeighbours(map, x, y, nord: null, nordEst: null, est: Tiles.Floor, sudEst: null, sud: null, sudOuest: null, ouest: null, nordOuest: null)) return "tiles_12";
            //Mur droit droite
            if (CheckTilesNeighbours(map, x, y, nord: null, nordEst: null, est: null, sudEst: null, sud: null, sudOuest: null, ouest: Tiles.Floor, nordOuest: null)) return "tiles_17";
            //Mur droit bas
            if (CheckTilesNeighbours(map, x, y, nord: null, nordEst: null, est: null, sudEst: null, sud: Tiles.Floor, sudOuest: null, ouest: null, nordOuest: null)) return "tiles_2";
            //ALL
            //if (CheckTilesNeighbours(map, x, y, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall)) return "tiles_30";

            if (CheckTilesNeighbours(map, x, y, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall)) return "tiles_30";

            //Si non trouvé placeholder
            return "tiles_0";
        }

        if (current == Tiles.Floor)
        {
            Dictionary<string, int> floors = new Dictionary<string, int>();
            floors.Add("tiles_37", 20);
            floors.Add("tiles_38", 10);
            floors.Add("tiles_43", 10);
            floors.Add("tiles_44", 10);
            floors.Add("tiles_49", 25);
            floors.Add("tiles_55", 10);
            floors.Add("tiles_61", 70);

            int percentTotal = floors.Values.Sum(m => m);
            int percentRoll = Random.Range(0, percentTotal);

            int percentCurrent = 0;
            string floorbase = "tiles_0";

            foreach (var item in floors)
            {
                percentCurrent += item.Value;
                if (percentCurrent >= percentRoll)
                {
                    floorbase = item.Key;
                    break;
                }
            }

            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Floor, nordEst: null, est: null, sudEst: null, sud: null, sudOuest: null, ouest: Tiles.Wall, nordOuest: Tiles.Floor)) return "tiles_20";

            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Wall, nordEst: null, est: null, sudEst: null, sud: null, sudOuest: null, ouest: Tiles.Wall, nordOuest: Tiles.Wall)) return "tiles_8";

            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Wall, nordEst: null, est: null, sudEst: null, sud: null, sudOuest: null, ouest: Tiles.Floor, nordOuest: Tiles.Floor)) return "tiles_15";

            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Floor, nordEst: null, est: null, sudEst: null, sud: null, sudOuest: null, ouest: Tiles.Floor, nordOuest: Tiles.Wall)) return "tiles_14";

            if (CheckTilesNeighbours(map, x, y, nord: Tiles.Wall, nordEst: null, est: null, sudEst: null, sud: null, sudOuest: null, ouest: null, nordOuest: null)) return "tiles_13";

            if (CheckTilesNeighbours(map, x, y, nord: null, nordEst: null, est: null, sudEst: null, sud: null, sudOuest: null, ouest: Tiles.Wall, nordOuest: null)) return "tiles_26";


            //Floor avec que des wall autour = INTERDIT
            if (CheckTilesNeighbours(map, x, y, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall, Tiles.Wall)) return "tiles_40";

            return floorbase;
        }

        //Si tiletype non géré : placeholder
        return "tiles_0";
    }

    private static bool CheckTilesNeighbours(Tiles[,] map, int x, int y, Tiles? nord, Tiles? nordEst, Tiles? est, Tiles? sudEst, Tiles? sud, Tiles? sudOuest, Tiles? ouest, Tiles? nordOuest)
    {
        int gridSizeX = map.GetLength(0);
        int gridSizeY = map.GetLength(1);

        //Clockwise

        //Nord
        int checkX = x + 0;
        int checkY = y + 1;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && nord != null)
        {
            if (nord != map[checkX, checkY]) return false;
        }

        //NordEst
        checkX = x + 1;
        checkY = y + 1;
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && nordEst != null)
        {
            if (nordEst != map[checkX, checkY]) return false;
        }
        //Est
        checkX = x + 1;
        checkY = y + 0;
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && est != null)
        {
            if (est != map[checkX, checkY]) return false;
        }
        //SudEst
        checkX = x + 1;
        checkY = y - 1;
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && sudEst != null)
        {
            if (sudEst != map[checkX, checkY]) return false;
        }
        //Sud
        checkX = x + 0;
        checkY = y - 1;
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && sud != null)
        {
            if (sud != map[checkX, checkY]) return false;
        }
        //SudOuest
        checkX = x - 1;
        checkY = y - 1;
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && sudOuest != null)
        {
            if (sudOuest != map[checkX, checkY]) return false;
        }
        //Ouest
        checkX = x - 1;
        checkY = y + 0;
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && ouest != null)
        {
            if (ouest != map[checkX, checkY]) return false;
        }
        //NordOuest
        checkX = x - 1;
        checkY = y + 1;
        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && nordOuest != null)
        {
            if (nordOuest != map[checkX, checkY]) return false;
        }

        //Sinon
        return true;
    }
}