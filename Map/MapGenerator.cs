using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;

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

    public GameObject torch;

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
                    Debug.LogWarning("Impossible de trouver un coté de sortie");
                    break;
                }
            }
        }

        GenerateMapMesh();
        GenerateTorch(new RandomInt(10, 20));

    }

    private void GenerateTorch(RandomInt randomQuantityBySide)
    {
        GameObject mapHolder = GameObject.Find(mapHolderName);
        List<Vector2> patternPositionsSud = new List<Vector2>();
        List<Vector2> patternPositionsNord = new List<Vector2>();
        List<Vector2> patternPositionsOuest = new List<Vector2>();
        List<Vector2> patternPositionsEst = new List<Vector2>();

        //Recherches des patterns
        int quantity = randomQuantityBySide.GetRandomInt();
        for (int i = 0; i < quantity; i++)
        {
            Vector2? patternPositionSud = GetCurrentRoom().FindRandomPattern(TilesPattern.wallSud);
            if (patternPositionSud != null) patternPositionsSud.Add(patternPositionSud.Value);
        }

        quantity = randomQuantityBySide.GetRandomInt();
        for (int i = 0; i < quantity; i++)
        {
            Vector2? patternPositionNord = GetCurrentRoom().FindRandomPattern(TilesPattern.wallNord);
            if (patternPositionNord != null) patternPositionsNord.Add(patternPositionNord.Value);
        }

        quantity = randomQuantityBySide.GetRandomInt();
        for (int i = 0; i < quantity; i++)
        {
            Vector2? patternPositionOuest = GetCurrentRoom().FindRandomPattern(TilesPattern.wallOuest);
            if (patternPositionOuest != null) patternPositionsOuest.Add(patternPositionOuest.Value);
        }

        quantity = randomQuantityBySide.GetRandomInt();
        for (int i = 0; i < quantity; i++)
        {
            Vector2? patternPositionEst = GetCurrentRoom().FindRandomPattern(TilesPattern.wallEst);
            if (patternPositionEst != null) patternPositionsEst.Add(patternPositionEst.Value);
        }

        //Suppression des doublons
        patternPositionsSud = patternPositionsSud.Distinct().ToList();
        patternPositionsNord = patternPositionsNord.Distinct().ToList();
        patternPositionsOuest = patternPositionsOuest.Distinct().ToList();
        patternPositionsEst = patternPositionsEst.Distinct().ToList();

        //Instanciation
        foreach (var pattern in patternPositionsSud)
        {
            GameObject o = Instantiate(torch, pattern + new Vector2(1.5f, 0.5f), Quaternion.identity) as GameObject;
        }
        foreach (var pattern in patternPositionsNord)
        {
            GameObject o = Instantiate(torch, pattern + new Vector2(1.5f, 2.5f), Quaternion.identity) as GameObject;
        }
        foreach (var pattern in patternPositionsOuest)
        {
            GameObject o = Instantiate(torch, pattern + new Vector2(0.5f, 1.5f), Quaternion.identity) as GameObject;
        }
        foreach (var pattern in patternPositionsEst)
        {
            GameObject o = Instantiate(torch, pattern + new Vector2(2.5f, 1.5f), Quaternion.identity) as GameObject;
        }
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
            GameObject meshGameObject = GenerateMesh(room.container, map, roomRect.position, Tiles.Wall, TagName.Wall);
            AddColliders(meshGameObject, map);
            GenerateMapConnectivity(room.container, room);

            //Floor
            GenerateMesh(room.container, map, roomRect.position, Tiles.Floor, TagName.Floor);

            //Add Random Object


        }
    }

    private GameObject GenerateMesh(GameObject container, Tiles[,] map, Vector2 position, Tiles tileType, string tag)
    {
        GameObject meshContainer = new GameObject("mesh-tiles-" + tileType.ToString());
        meshContainer.tag = tag;
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
        return meshContainer;
    }

    private void AddColliders(GameObject holder, Tiles[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == Tiles.Wall)
                {
                    if (MapUtils.Get4Neighbours(map, x, y).Where(m => map[m.x, m.y] == Tiles.Wall).Count() != 4)
                    {
                        Debug.Log("add");
                        BoxCollider2D collider = holder.AddComponent<BoxCollider2D>();
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

            GenerateMesh(roomConnection, mapConnection, meshPosition, Tiles.Wall, TagName.Wall);
            GenerateMesh(roomConnection, mapConnection, meshPosition, Tiles.Floor, TagName.Floor);

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



