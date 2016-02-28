using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Utils
{
    //Permet d'identifier des groupements d'objets lors des checks globaux (collision par exemple).
    public class TagName
    {
        public const string Floor = "floor";
        public const string Wall = "walls";
        public const string ItemFixed = "itemFixed";
        public const string ItemDestroyable = "itemDestroyable";
        public const string ItemCollectable = "itemCollectable";
        public const string Enemy = "enemy";
        public const string EnemiyProjectiles = "enemyProjectiles";
        public const string Friendly = "friendly";
        public const string FriendlyProjectiles = "friendlyProjectiles";
        public const string Particle = "particle";
        public const string Player = "player";

        //Permet d'identifier les tags sur lequels se produisent des collisions.
        public static string[] BlockMask = { Wall, ItemDestroyable, ItemFixed };
    }

    //Permet d'aller find des gameObject par leur nom. Ne fonctionne pas avec les (clone : instanciate)
    public class GameObjectName
    {
        public const string Ui = "ui";
        public const string Map = "map";
        public const string Spawner = "spawner";
        public const string GameManager = "gameManager";
    }

    public class SortingLayerName
    {
        public const string A_particleOnGround = "ParticlesOnGround";
        public const string B_obstacle = "Obstacle";
        //...

    }

}
