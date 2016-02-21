using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    private List<Room> rooms = new List<Room>();
    private string mapHolderName = "GeneratedMap";

    [Header("Map Dimension")]
    [Range(30, 150)]
    public int widthMin = 80;
    [Range(30, 150)]
    public int widthMax = 100;
    [Range(30, 150)]
    public int heightMin = 80;
    [Range(30, 150)]
    public int heightMax = 100;
    [Range(0, 10)]
    public int roomQuantity = 4;

    //TODO CHILD ROOM NUMBER a remonter ici
    [Range(4, 12)]
    public int roomChildQuantityMin = 6;
    [Range(4, 12)]
    public int roomChildQuantityMax = 10;


    [Header("Map Texture")]
    //public Texture2D[] wallTexture;
    //public Texture2D[] floorTexture;

    //public Sprite[] tilesWallTexture;
    //public Sprite[] tilesFloorTexture;

    public Sprite[] mapSprites;
    private Dictionary<string, Texture2D> mapTextures;

    void Awake()
    {

        mapTextures = new Dictionary<string, Texture2D>();
        //Toutes les textures se trouveront dans ce tableau avec pour clé le nom du sprite attribué lors de la découpe par unity.

        foreach (var sprite in mapSprites)
        {
            Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            texture.SetPixels(sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height));
            mapTextures.Add(sprite.name, texture);
        }

        GenerateMap();


        
    }


    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    DestroyMap();
        //    GenerateMap();
        //}
    }

    public Room GetCurrentRoom()
    {
        return rooms[0]; //todo prendre en compte position player
    }

    public ChildRoom GetInitialPop()
    {
        return GetCurrentRoom().childs[0];
    }

    private void GenerateMap()
    {
        Transform mapHolder = new GameObject(mapHolderName).transform;
        mapHolder.parent = transform;

        int roomWidth = Random.Range(widthMin, widthMax);
        int roomHeight = Random.Range(heightMin, heightMax);
        int roomChildQuantity = Random.Range(roomChildQuantityMin, roomChildQuantityMax);

        Room currentRoom = new Room(null, roomWidth, roomHeight, roomChildQuantity);
        rooms.Add(currentRoom);

        for (int i = 0; i < roomQuantity; i++)
        {
            roomWidth = Random.Range(widthMin, widthMax);
            roomHeight = Random.Range(heightMin, heightMax);

            bool roomFind = false;
            Room testedRoom;
            int limit = 0;
            while (!roomFind)
            {
                //Cherche un coté de sortie
                currentRoom.SetExitSide();
                //Crée une room suivante
                testedRoom = new Room(currentRoom, roomWidth, roomHeight, roomChildQuantity);
                //Vérifie qu'il n'y a pas de chevauchement
                if (!AreRoomOverlap(testedRoom, rooms))
                {
                    //Sort de la boucle
                    roomFind = true;
                    rooms.Add(testedRoom);
                    currentRoom = testedRoom;
                }
                limit++;
                if (limit >= 100)
                {
                    break;
                }
            }
        }

        GenerateMapMesh();

        for (int i = 0; i < 10; i++)
        {

            //On va plutot mettre les torch ici et les object via le spawner TODO
           // GenerateObject(Random.Range(3, 10), Random.Range(3, 10));


            //TilesPattern patern = new TilesPattern(5, 3, Tiles.Floor);
            //Vector2 paternPosition = GetCurrentRoom().FindRandomPattern(patern.pattern);
            //Vector2 anchor = new Vector3(paternPosition.x + 1, paternPosition.y + 1);
            //GameObject o = Instantiate(boxFixed[0], anchor, Quaternion.identity) as GameObject;
            //o.transform.parent = mapHolder.transform;
            //GetCurrentRoom().AddPatternToMap((int)(paternPosition.x), (int)(paternPosition.y), 5, 3, Tiles.Items);

            //TilesPattern patern2 = new TilesPattern(3, 3, Tiles.Floor);
            //Vector2 paternPosition2 = GetCurrentRoom().FindRandomPattern(patern2.pattern);
            //Vector2 anchor2 = new Vector3(paternPosition2.x + 1, paternPosition2.y + 1);
            //GameObject o2 = Instantiate(barrel[0], anchor2, Quaternion.identity) as GameObject;
            //o2.transform.parent = mapHolder.transform;
            //GetCurrentRoom().AddPatternToMap((int)(paternPosition2.x), (int)(paternPosition2.y), 3, 3, Tiles.Items);
        }



    }

    private void GenerateObject(int width, int heigth)
    {
        TilesPattern patern = new TilesPattern(width, heigth, Tiles.Floor);
        //Vector2 paternPosition = GetCurrentRoom().FindRandomPattern(patern.pattern);

        //todo garder une margin de 1 pour ne pas break le path finding

        for (int i = 1; i < patern.pattern.GetLength(0)-1; i++)
        {
            for (int j = 1; j < patern.pattern.GetLength(1)-1; j++)
            {
                //Vector2 anchor = new Vector3(paternPosition.x + i, paternPosition.y + j);
                GameObject mapHolder = GameObject.Find(mapHolderName);
                //GameObject o = Instantiate(box[0], anchor, Quaternion.identity) as GameObject;
                //o.transform.parent = mapHolder.transform;
              //  GetCurrentRoom().AddPatternToMap((int) paternPosition.x + i, (int)paternPosition.y + j, 1, 1, Tiles.Wall);
            }
        }

        //todo
        //GetCurrentRoom().AddPatternToMap()


    }

    private void GenerateMapMesh()
    {
        foreach (var room in rooms)
        {
            room.container = new GameObject("room-" + rooms.IndexOf(room));

            if (room.parent == null)
            {
                room.container.transform.parent = GameObject.Find(mapHolderName).transform;
            }
            else
            {
                room.container.transform.parent = room.parent.container.transform;
            }


            Tiles[,] map = room.GetMap();
            Rect roomRect = room.GetRect();

            room.container.transform.position = roomRect.position;


            //Add & Texturise Mesh
            //On différencie le contenu en fonction des tilestype pour obtenir des mesh moins gros. (limit 85000 vertices)
            //Wall
            GenerateMesh(room.container, map, roomRect.position, Tiles.Wall);
            AddColliders(room, map);
            GenerateMapConnectivity(room.container, room);

            //Floor
            GenerateMesh(room.container, map, roomRect.position, Tiles.Floor);

            //Add Random Object


        }
    }

    private void GenerateMesh(GameObject container, Tiles[,] map, Vector2 position, Tiles tileType)
    {
        GameObject meshContainer = new GameObject("mesh-tiles-" + tileType.ToString());
        meshContainer.transform.parent = container.transform;
        //Generate Mesh
        MeshFilter meshFilter = meshContainer.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshContainer.AddComponent<MeshRenderer>();

        Mesh mesh = MeshGenerator.GenerateSquaredMesh(MapUtils.ConvertMap(map), 1, (int)tileType);
        meshFilter.mesh = mesh;
        meshFilter.transform.position = position;

        //Texturise mesh
        int size = 55;

        Texture2D texture = new Texture2D(size * map.GetLength(0), size * map.GetLength(1));

        //List<Color[]> tilesTextures = new List<Color[]>();
        //foreach (var tileTexture in textures)
        //{
        //    tilesTextures.Add(tileTexture.GetPixels(0, 0, size, size));
        //}

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == tileType)
                {
                    string textureForTile = MapUtils.GetTileSpriteName(map, x, y);
                    texture.SetPixels(x * size, y * size, size, size, mapTextures[textureForTile].GetPixels());
                    //texture.SetPixels(x * size, y * size, size, size, tilesTextures[Random.Range(0, tilesTextures.Count)]);
                }
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        Material material = new Material(Shader.Find("Sprites/Diffuse"));
        material.color = Color.white;
        material.mainTexture = texture;

        meshRenderer.material = material;
    }

    private void AddColliders(Room room, Tiles[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == Tiles.Wall)
                {
                    if (MapUtils.Get4Neighbours(map, x, y).Where(m => map[m.x, m.y] == Tiles.Wall).Count() != 4)
                    {
                        BoxCollider2D collider = room.container.AddComponent<BoxCollider2D>();
                        collider.size = Vector2.one;
                        collider.offset = new Vector2(x + 0.5f, y + 0.5f);
                    }
                }
            }
        }
    }

    private void GenerateMapConnectivity(GameObject container, Room room)
    {
        if (room.parent != null)
        {
            //Crée une map de dimension spécifique

            Coord parentExitPoint = (Coord)room.parent.exitPoint;
            Coord startPoint = (Coord)room.startPoint;

            //Les coordonnées doivent être récupérées en absolue.
            Vector2 start = new Vector2(parentExitPoint.x + room.parent.GetRect().x, parentExitPoint.y + room.parent.GetRect().y);
            Vector2 exit = new Vector2(startPoint.x + room.GetRect().x, startPoint.y + room.GetRect().y);

            Tiles[,] mapConnection = null;

            if (room.parent.exitSide == Side.Left || room.parent.exitSide == Side.Right)
            {
                mapConnection = new Tiles[(int)Mathf.Abs(exit.x - start.x), (int)Mathf.Abs(exit.y - start.y) + 3];

                MapUtils.SetFull(mapConnection, 0, 0, mapConnection.GetLength(0), mapConnection.GetLength(1), Tiles.Wall);
                MapUtils.SetFull(mapConnection, 0, 1, mapConnection.GetLength(0), 1, Tiles.Floor);
            }

            if (room.parent.exitSide == Side.Up || room.parent.exitSide == Side.Down)
            {
                mapConnection = new Tiles[(int)Mathf.Abs(exit.x - start.x) + 3, (int)Mathf.Abs(exit.y - start.y)];

                MapUtils.SetFull(mapConnection, 0, 0, mapConnection.GetLength(0), mapConnection.GetLength(1), Tiles.Wall);
                MapUtils.SetFull(mapConnection, 1, 0, 1, mapConnection.GetLength(1), Tiles.Floor);
            }



            GameObject roomConnection = new GameObject("roomConnection-" + rooms.IndexOf(room));

            roomConnection.transform.parent = container.transform;

            Vector2 meshPosition = Vector2.zero;

            if (room.parent.exitSide == Side.Left)
            {
                meshPosition = new Vector2(start.x - mapConnection.GetLength(0), start.y - 1);
            }
            if (room.parent.exitSide == Side.Right)
            {
                meshPosition = new Vector2(start.x, start.y - 1);
            }
            if (room.parent.exitSide == Side.Up)
            {
                meshPosition = new Vector2(start.x - 1, start.y);
            }
            if (room.parent.exitSide == Side.Down)
            {
                meshPosition = new Vector2(start.x - 1, start.y - mapConnection.GetLength(1));
            }

            GenerateMesh(roomConnection, mapConnection, meshPosition, Tiles.Wall);
            GenerateMesh(roomConnection, mapConnection, meshPosition, Tiles.Floor);

            // Add colliders

            for (int x = 0; x < mapConnection.GetLength(0); x++)
            {
                for (int y = 0; y < mapConnection.GetLength(1); y++)
                {
                    if (mapConnection[x, y] == Tiles.Wall)
                    {
                        BoxCollider2D collider = roomConnection.AddComponent<BoxCollider2D>();
                        collider.size = Vector2.one;
                        collider.offset = new Vector2(x + 0.5f, y + 0.5f);
                    }
                }
            }

        }
    }

    private void DestroyMap()
    {
        GameObject mapHolder = GameObject.Find(mapHolderName);
        if (mapHolder != null)
        {
            foreach (Transform child in mapHolder.transform)
            {
                DestroyImmediate(child.gameObject);
            }

            DestroyImmediate(mapHolder);
            rooms.Clear();
        }


    }

    private bool AreRoomOverlap(Room r1, List<Room> rooms)
    {
        foreach (var room in rooms)
        {
            if (r1.GetRect().Overlaps(room.GetRect()))
            {
                return true;
            }
        }
        return false;
    }
}



