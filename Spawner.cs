using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;



public struct RandomInt
{
    public int min;
    public int max;

    public RandomInt(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
    public int GetRandomInt()
    {
        //Possibilité d'implementer un seed avec system random
        return Random.Range(min, max);
    }
}


public class Spawner : MonoBehaviour
{
    public GameObject monster1;
    public GameObject monster2;

    public GameObject barel;
    public GameObject brasero;
    public GameObject pipe;
    public GameObject totem;
    public GameObject totemTall;
    public GameObject spike;

    [Header("Items")]
    public GameObject box;
    public GameObject jewels;
    public GameObject healthItem;
    public GameObject speedItem;

    private MapGenerator map;

    //TODO


    // WALL ILLUSTRATOR
    // persistance block detroy
    // sorting layer https://forums.tigsource.com/index.php?topic=50972.0

    // changer ombre monster 1

    //object factory mieux que de link les prefabs partouts...  

    //FAIRE 2 COLLISIONNEUR POUR LE PLAYER : MOUVEMENT & DAMAGE ;)
    //trap with arrow left to right
    //Collision sans rigidbody with raycast ?



    //ui event text TODO EVENT LIKE AMAZING/KILLINGSPREE a la cs !


    //game item flash / jewels
    //acid on ground - faire sur les tiles a coté
    // HUD - manque level sur hud + reprendre proprement le easing
    //hud faire les spells + controls android


    void Start()
    {

        map = GameManager.instance.mapGenerator;

        PlaceAllObjects(
            quantity: new RandomInt(4, 7),
            width: new RandomInt(1, 1),
            height: new RandomInt(1, 1),
            objectToInstanciate: healthItem,
            holder: map.transform,
            type: Tiles.Items,
            marginSize: 0,
            placeByCenter: true
        );

        PlaceAllObjects(
            quantity: new RandomInt(6, 9),
            width: new RandomInt(1, 1),
            height: new RandomInt(1, 1),
            objectToInstanciate: speedItem,
            holder: map.transform,
            type: Tiles.Items,
            marginSize: 0,
            placeByCenter: true
        );

        PlaceAllObjects(
            quantity: new RandomInt(2, 4),
            width: new RandomInt(3, 3),
            height: new RandomInt(4, 4),
            objectToInstanciate: totemTall,
            holder: map.transform,
            type: Tiles.Wall,
            itemSize: new Vector2(1, 2)
        );

        PlaceAllObjects(
            quantity: new RandomInt(10, 15),
            width: new RandomInt(1, 1),
            height: new RandomInt(1, 1),
            objectToInstanciate: jewels,
            holder: map.transform,
            type: Tiles.Items,
            marginSize: 0
        );

        PlaceAllObjects(
            quantity: new RandomInt(3, 10),
            width: new RandomInt(5, 12),
            height: new RandomInt(3, 8),
            objectToInstanciate: box,
            holder: map.transform,
            type: Tiles.Wall,
            fillWith: true
            );

        PlaceAllObjects(
            quantity: new RandomInt(2, 3),
            width: new RandomInt(3, 5),
            height: new RandomInt(4, 6),
            objectToInstanciate: spike,
            holder: map.transform,
            type: Tiles.Wall,
            fillWith: true
            );

        PlaceAllObjects(
            quantity: new RandomInt(3, 10),
            width: new RandomInt(3, 3),
            height: new RandomInt(3, 3),
            objectToInstanciate: barel,
            holder: map.transform,
            type: Tiles.Wall,
            placeByCenter: true
            );

        PlaceAllObjects(
            quantity: new RandomInt(3, 10),
            width: new RandomInt(3, 3),
            height: new RandomInt(3, 3),
            objectToInstanciate: brasero,
            holder: map.transform,
            type: Tiles.Wall
            );

        PlaceAllObjects(
            quantity: new RandomInt(3, 10),
            width: new RandomInt(3, 3),
            height: new RandomInt(3, 3),
            objectToInstanciate: pipe,
            holder: map.transform,
            type: Tiles.Wall
            );

        PlaceAllObjects(
            quantity: new RandomInt(3, 10),
            width: new RandomInt(5, 5),
            height: new RandomInt(3, 3),
            objectToInstanciate: totem,
            holder: map.transform,
            type: Tiles.Wall,
            itemSize: new Vector2(3, 1)
            );

        PlaceAllObjects(
            quantity: new RandomInt(1, 1),
            width: new RandomInt(3, 3),
            height: new RandomInt(3, 3),
            objectToInstanciate: monster1,
            holder: map.transform,
            type: Tiles.Floor
            );

        //PlaceAllObjects(
        //    quantity: new RandomInt(1, 1),
        //    width: new RandomInt(3, 3),
        //    height: new RandomInt(3, 3),
        //    objectToInstanciate: monster2,
        //    holder: map.transform,
        //    type: Tiles.Floor
        //    );



    }

    void Update()
    {

    }

    public void PlaceAllObjects(RandomInt quantity, RandomInt width, RandomInt height, GameObject objectToInstanciate, Transform holder, Tiles type,
        int marginSize = 1,
        bool fillWith = false,
        Vector2? itemSize = null,
        bool placeByCenter = false
        )
    {

        Vector2 itemFinalSize = Vector2.one;
        if (itemSize != null)
        {
            itemFinalSize = itemSize.Value;
        }

        for (int i = 0; i < quantity.GetRandomInt(); i++)
        {
            PlaceObject(width.GetRandomInt(), height.GetRandomInt(), objectToInstanciate, holder, type, itemFinalSize, fillWith, marginSize, placeByCenter);
        }
    }

    private void PlaceObject(int width, int height, GameObject objectToInstanciate, Transform holder, Tiles type, Vector2 itemSize, bool fillWith, int marginSize, bool placeByCenter)
    {
        TilesPattern patern = new TilesPattern(width, height, Tiles.Floor);
        Vector2? paternPosition = map.GetCurrentRoom().FindRandomPattern(patern.pattern);

        if (paternPosition == null)
        {
            Debug.LogWarning("Pattern not found for " + objectToInstanciate.name);
            return;
        }

        //Possibilité de margin de 1 ou pas
        //Possibilité de fillwith ou pas
        //Différente tailles d'item

        //----------------------- Vérifications -----------------------

        if (itemSize.x <= 0 || itemSize.y <= 0)
        {
            //Verification que le itemSize est possible
            Debug.LogError("Erreur itemSize : item size < 1" + " for " + objectToInstanciate.name);
            return;
        }

        if (marginSize != 0)
        {
            if (!fillWith)
            {
                //Verification d'une margin autour du pattern.
                if (itemSize.x + marginSize * 2 != width)
                {
                    Debug.LogError("Erreur MarginSize ; La largeur de l'item est de " + itemSize.x + " alors que la largeur du pattern est de " + width + " for " + objectToInstanciate.name);
                    return;
                }
                if (itemSize.y + marginSize * 2 != height)
                {
                    Debug.LogError("Erreur MarginSize ; La hauteur de l'item est de " + itemSize.y + " alors que la hauteur du pattern est de " + height + " for " + objectToInstanciate.name);
                    return;
                }
            }
            else
            {
                if (itemSize.x + marginSize * 2 > width)
                {
                    Debug.LogError("Erreur MarginSize fillWith ; La largeur de l'item est de " + itemSize.x + " alors que la largeur du pattern est de " + width + " for " + objectToInstanciate.name);
                    return;
                }
                if (itemSize.y + marginSize * 2 > height)
                {
                    Debug.LogError("Erreur MarginSize fillWith ; La hauteur de l'item est de " + itemSize.y + " alors que la hauteur du pattern est de " + height + " for " + objectToInstanciate.name);
                    return;
                }
            }
        }

        //if (fillWith && itemSize == Vector2.one)
        //{
        //    //Verification que le fill width est possible
        //    Debug.LogError("Erreur FillWith : item size = 1" + " for " + objectToInstanciate.name);
        //    return;
        //}

        //----------------------- Instanciation -----------------------

        if (fillWith)
        {
            for (int i = 0 + marginSize; i < width - marginSize; i++)
            {
                for (int j = 0 + marginSize; j < height - marginSize; j++)
                {
                    //Attention on ne prend pas en compte l'item size WTF si il ne fait pas 1...
                    Vector2 anchor = new Vector3(paternPosition.Value.x + i, paternPosition.Value.y + j);
                    if (placeByCenter)
                    {
                        anchor = GameManager.instance.GetCurrentGrid().WorldPointFromNode(anchor);
                    }
                    GameObject o = Instantiate(objectToInstanciate, anchor, Quaternion.identity) as GameObject;
                    o.transform.parent = holder;
                    map.GetCurrentRoom().AddPatternToMap((int)anchor.x, (int)anchor.y, 1, 1, type);
                }
            }
        }

        if (!fillWith)
        {
            //Il n'y a qu'un seul objet à instancier.
            Vector2 anchor = new Vector3(paternPosition.Value.x + marginSize, paternPosition.Value.y + marginSize);
            if (placeByCenter)
            {
                anchor = GameManager.instance.GetCurrentGrid().WorldPointFromNode(anchor);
            }
            GameObject o = Instantiate(objectToInstanciate, anchor, Quaternion.identity) as GameObject;
            o.transform.parent = holder;
            map.GetCurrentRoom().AddPatternToMap((int)anchor.x, (int)anchor.y, (int)itemSize.x, (int)itemSize.y, type);
        }

    }







}


