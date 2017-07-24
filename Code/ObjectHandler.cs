using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKPlatformerExample
{
    class ObjectHandler
    {
        public static List<Sprite> definedSprites = new List<Sprite>();
        public static int fireballsonscreen;
        public static List<GameObject> objects = new List<GameObject>();
        public static void Load()
        {
            definedSprites.Add(new Sprite(ContentPipe.LoadTexture("coinGold.png")));
            definedSprites.Add(new Sprite(ContentPipe.LoadTexture("fireball.png")));
            definedSprites.Add(new Sprite(ContentPipe.LoadTexture("fireflower.png")));
            definedSprites.Add(new Sprite(ContentPipe.LoadTexture("coinGold.png"))); //Load it twice to avoid adding more code - Lazyness at its finest! :)
            definedSprites.Add(new Sprite(ContentPipe.LoadTexture("door.png")));
        }
        public static void AddObject(ObjectType type, Vector2 position,bool left = false)
        {
            if (type == ObjectType.Fireball)
            {
                if (fireballsonscreen >= 2)
                {
                    return;
                }else
                {
                    fireballsonscreen++;
                    SoundManager.sounds[1].Play();
                }
            }
            GameObject o = new GameObject(definedSprites.ElementAt((int)type).texture, position, type,left);
            objects.Add(o);
        }
        public enum ObjectType
        {
            Coin = 0,
            Fireball = 1,
            Fireflower = 2,
            StandingCoin = 3,
            Door = 4
        }
    }
}
