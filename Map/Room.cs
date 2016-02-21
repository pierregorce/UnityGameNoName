using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ChildRoom
{
    public Room parent;
    public Rect rect;
    public bool placed = false;

    public ChildRoom(Room parent, Rect rect)
    {
        this.parent = parent;
        this.rect = rect;

    }
}

public class Room
{
    public GameObject container;
    public Room parent;
    public List<ChildRoom> childs = new List<ChildRoom>();
    public List<Vector2[]> tunnels = new List<Vector2[]>();

    private int width, height;

    //Coordonnées des sides en relatif
    private Side? startSide;
    public Coord? startPoint { get; private set; } //Get utilisé pour pouvoir faire les liaisons.
    public Side? exitSide { get; private set; } //Il faut pouvoir le get pour créer une room suivante à partir du point de sortie courant.
    public Coord? exitPoint { get; private set; } //Get utilisé pour pouvoir faire les liaisons.

    private int childQuantity = 8;
    private int margin = Random.Range(10, 20);

    //La map de retour
    private Tiles[,] map;

    public Room(Room parent, int width, int height, int childQuantity)
    {
        this.parent = parent;
        this.width = width;
        this.height = height;
        this.childQuantity = childQuantity;

        if (parent == null)
        {
            //Si l'on a pas de parent, il s'agit de la première room => pas de start side.
            startSide = null;
            startPoint = null;
        }
        else
        {
            startSide = GetStartSideFromEndSide((Side)parent.exitSide);
            startPoint = GetRandomCoord((Side)startSide);
        }

        AddChildRooms();
    }

    /// <summary>
    /// Verifie que l'emplacement d'une child room est correct. Si correct l'ajoute dans la collection.
    /// </summary>
    /// <param name="testedChildRoom"></param>
    /// <returns></returns>
    private bool PlaceChildsRoom(ChildRoom testedChildRoom)
    {

        //check overlap
        bool overlap = false;

        foreach (var childRoom in childs)
        {
            if (childRoom.rect.Overlaps(testedChildRoom.rect))
            {
                overlap = true;
            }
        }

        if (!overlap)
        {
            childs.Add(testedChildRoom);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Affecte les childs room, leur nombre dépend de la taille de la room.
    /// </summary>
    private void AddChildRooms()
    {
        int padding = Random.Range(3, 5);

        int childMaxWidth = this.width / 3;
        int childMinWidth = this.width / 6;
        int childMaxHeight = this.height / 3;
        int childMinHeight = this.height / 6;

        //add rooms
        int placed = 0;
        int count = 0;

        while (placed < childQuantity)
        {
            int width = Random.Range(childMinWidth, childMaxWidth);
            int height = Random.Range(childMinHeight, childMaxHeight);
            int x = Random.Range(padding, (int)(GetRect().width - width - padding * 2));
            int y = Random.Range(padding, (int)(GetRect().height - height - padding * 2));
            ChildRoom childRoom = new ChildRoom(this, new Rect(x, y, width, height));

            if (PlaceChildsRoom(childRoom))
            {
                placed++;
            }

            // this is for debug stuff - shouldn't ever happen
            count++;
            if (count > 250)
            {
                Debug.LogWarning("Impossible de place toutes les childs room : " + placed + "/" + childQuantity);
                break;
            }
        }
    }

    /// <summary>
    /// Ajoute les tunnels donc la liste.
    /// </summary>
    private void PlaceTunnels()
    {
        Pathfinding p = new Pathfinding(GetSimpleMap(), Vector2.zero);

        List<ChildRoom> closedChildRoomList = new List<ChildRoom>();
        ChildRoom currentChildRoom = GetRandomChildRoom(closedChildRoomList);

        //Start
        if (startPoint != null)
        {
            Vector2[] path = p.RequestPath(((Coord)startPoint).ToVect2(), currentChildRoom.rect.center);
            tunnels.Add(path);
        }

        for (int i = 0; i < childs.Count; i++)
        {
            currentChildRoom = GetRandomChildRoom(closedChildRoomList);

            if (currentChildRoom != null)
            {
                Vector2[] path = p.RequestPath(closedChildRoomList[closedChildRoomList.Count - 2].rect.center, currentChildRoom.rect.center);
                tunnels.Add(path);
            }
        }

        //End
        if (exitPoint != null)
        {
            Vector2[] path = p.RequestPath(closedChildRoomList[closedChildRoomList.Count - 1].rect.center, ((Coord)exitPoint).ToVect2());
            tunnels.Add(path);
        }
    }

    private ChildRoom GetRandomChildRoom(List<ChildRoom> closedList)
    {
        if (closedList.Count == childs.Count)
        {
            return null;
        }

        ChildRoom currentChildRoom = childs[Random.Range(0, childs.Count)];

        while (closedList.Contains(currentChildRoom))
        {
            currentChildRoom = childs[Random.Range(0, childs.Count)];
        }

        closedList.Add(currentChildRoom);
        return currentChildRoom;
    }

    /// <summary>
    /// Renvoie un side aléatoire
    /// </summary>
    private Side GetRandomSide()
    {
        System.Array values = System.Enum.GetValues(typeof(Side));
        return (Side)values.GetValue(Random.Range(0, values.Length));
    }

    /// <summary>
    /// Set ExitSide qui doit être différent de StartSide
    /// </summary>
    public Side SetExitSide()
    {
        Side exitSide = GetRandomSide();
        //Side end doit être différent de la Side start
        if (startSide != exitSide)
        {
            this.exitSide = exitSide;
            exitPoint = GetRandomCoord(exitSide);
            return exitSide;
        }
        else
        {
            return SetExitSide();
        }
    }

    /// <summary>
    /// Retourne le coté de sortie par partir d'un coté d'entrée
    /// </summary>
    private Side GetStartSideFromEndSide(Side exitSide)
    {
        if (exitSide == Side.Up)
        {
            return Side.Down;
        }
        else if (exitSide == Side.Left)
        {
            return Side.Right;
        }
        else if (exitSide == Side.Right)
        {
            return Side.Left;
        }
        else //(endSide == Side.Down)
        {
            return Side.Up;
        }
    }

    /// <summary>
    /// Retourne un coordonnée aléatoire en fonction d'un coté
    /// </summary>
    private Coord GetRandomCoord(Side side)
    {
        Coord coord = new Coord(0, 0);

        if (side == Side.Right)
        {
            coord = new Coord(width - 1, Random.Range(1, height - 1));
        }

        if (side == Side.Left)
        {
            coord = new Coord(0, Random.Range(1, height - 1));
        }

        if (side == Side.Up)
        {
            coord = new Coord(Random.Range(1, width - 1), height - 1);
        }

        if (side == Side.Down)
        {
            coord = new Coord(Random.Range(1, width - 1), 0);
        }

        return coord;
    }

    /// <summary>
    /// Retourne un rectangle en relatif si aucune room parent. Retourne en absolue si l'on a un room parent.
    /// </summary>
    public Rect GetRect()
    {
        //Retourne un rectangle en relatif si aucune room parent.
        //Retourne en absolue si l'on a un room parent => utilisé une fois le monde créé pour du pathfind par exemple.

        int left = 0;
        int bottom = 0;
        if (parent != null)
        {
            Rect parentRect = parent.GetRect();
            Coord parentExitPoint = parent.exitPoint == null ? new Coord(0, 0) : (Coord)parent.exitPoint;
            Coord startPoint = this.startPoint == null ? new Coord(0, 0) : (Coord)this.startPoint;

            if (startSide == Side.Right)
            {
                left = (int)parentRect.x - width - margin;
                bottom = (int)parentRect.y + parentExitPoint.y - startPoint.y;
            }

            if (startSide == Side.Up)
            {
                left = (int)parentRect.x + parentExitPoint.x - startPoint.x;
                bottom = (int)parentRect.y - height - margin;
            }

            if (startSide == Side.Down)
            {
                left = (int)parentRect.x + parentExitPoint.x - startPoint.x;
                bottom = (int)parentRect.y + (int)parentRect.height + margin;
            }

            if (startSide == Side.Left)
            {
                left = (int)parentRect.x + (int)parentRect.width + margin;
                bottom = (int)parentRect.y + parentExitPoint.y - startPoint.y;
            }

            return new Rect(left, bottom, width, height);
        }
        else
        {
            return new Rect(left, bottom, width, height);
        }


    }

    /// <summary>
    /// Retourne la room vide avec ses bordures.
    /// </summary>
    public Tiles[,] GetSimpleMap()
    {
        Tiles[,] map = new Tiles[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = Tiles.Wall;
                }
                else
                {
                    //map[x, y] = Tiles.Floor;
                }

                if (startPoint != null)
                {
                    if (((Coord)startPoint).x == x && ((Coord)startPoint).y == y)
                    {
                        map[x, y] = Tiles.Special;
                    }
                }
                if (exitPoint != null)
                {
                    if (((Coord)exitPoint).x == x && ((Coord)exitPoint).y == y)
                    {
                        map[x, y] = Tiles.Special;
                    }
                }
            }
        }




        return map;
    }

    /// <summary>
    /// Retourne les tiles de la map.
    /// </summary>
    public Tiles[,] GetMap()
    {
        if (map != null)
        {
            return map;
        }

        //Génère une map avec juste les bordures.
        map = GetSimpleMap();
        //Place tunnel avec la map et juste des bordures.
        PlaceTunnels();
        //Remplit la map avec que des murs
        MapUtils.Set(map, 1, 1, width - 1, height - 1, Tiles.Wall);

        //Places Childs Rooms
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                foreach (var childRoom in childs)
                {
                    if (childRoom.rect.Contains(new Vector2(x, y)))
                    {
                        map[x, y] = Tiles.Floor;
                        childRoom.placed = true;
                    }
                }
            }
        }
        childs.RemoveAll(m => !m.placed);

        //Disform child rooms by adding somes irregularities on sides
        foreach (var room in childs)
        {
            List<Vector2> irregularities = GetSomeIrregularitites(room.rect);
            foreach (var irregularity in irregularities)
            {
                map[(int)(room.rect.x + irregularity.x), (int)(room.rect.y + irregularity.y)] = Tiles.Wall;
            }
        }


        //PLaces Tunnels
        foreach (var tunnel in tunnels)
        {
            int tunnelSize = Random.Range(3, 5);

            foreach (var tunnelCell in tunnel)
            {
                MapUtils.Set(map, (int)(tunnelCell.x - tunnelSize / 2), (int)(tunnelCell.y - tunnelSize / 2), tunnelSize, tunnelSize, Tiles.Floor);
            }

        }

        return map;
    }

    private List<Vector2> GetSomeIrregularitites(Rect rect)
    {
        List<Vector2> irregularities = new List<Vector2>();
        int irregularityQuantityMaxBySide = 0;
        int irregularitiyRatio = 5;

        //Nord
        irregularityQuantityMaxBySide = (int)rect.width / irregularitiyRatio;
        for (int i = 0; i < irregularityQuantityMaxBySide; i++)
        {
            int xPos = Random.Range(0, (int)rect.width);
            int yPos = (int)rect.height - 1;
            irregularities.Add(new Vector2(xPos, yPos));
        }

        //Sud
        irregularityQuantityMaxBySide = (int)rect.width / irregularitiyRatio;
        for (int i = 0; i < irregularityQuantityMaxBySide; i++)
        {
            int xPos = Random.Range(0, (int)rect.width);
            int yPos = 0;
            irregularities.Add(new Vector2(xPos, yPos));
        }
        //Est
        irregularityQuantityMaxBySide = (int)rect.height / irregularitiyRatio;
        for (int i = 0; i < irregularityQuantityMaxBySide; i++)
        {
            int xPos = 0;
            int yPos = Random.Range(0, (int)rect.height);
            irregularities.Add(new Vector2(xPos, yPos));
        }
        //Ouest
        irregularityQuantityMaxBySide = (int)rect.height / irregularitiyRatio;
        for (int i = 0; i < irregularityQuantityMaxBySide; i++)
        {
            int xPos = (int)rect.width - 1; ;
            int yPos = Random.Range(0, (int)rect.height);
            irregularities.Add(new Vector2(xPos, yPos));
        }






        return irregularities;
    }


    /// <summary>
    /// Ajoute des items à la map
    /// </summary>
    public void AddPatternToMap(int x, int y, int width, int height, Tiles tileType)
    {
        MapUtils.SetFull(map, x, y, width, height, tileType);
    }

    public Vector2? FindRandomPattern(Tiles[,] pattern)
    {
        Tiles[,] map = GetMap();
        //Grid grid = new Grid(GetMap(), Vector2.zero);
        List<Vector2> tilesOk = new List<Vector2>();

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                //Check chaque tile de la map

                if ((i + pattern.GetLength(0) < map.GetLength(0)) && (j + pattern.GetLength(1) < map.GetLength(1)))
                {
                    //Si la tile courant + pattern est inclus dans la map.
                    bool tileOK = true;

                    for (int x = 0; x < pattern.GetLength(0); x++)
                    {
                        for (int y = 0; y < pattern.GetLength(1); y++)
                        {
                            if (map[i + x, j + y] != pattern[x, y])
                            {
                                tileOK = false;
                            }
                        }
                    }

                    if (tileOK)
                    {
                        tilesOk.Add(new Vector2(i, j));
                    }

                }
            }
        }
        if (!(tilesOk.Count > 0))
        {
            return null;
        }
        else
        {
            return tilesOk[Random.Range(0, tilesOk.Count)];
        }

    }


}
