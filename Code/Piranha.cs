using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKPlatformerExample
{
    class Piranha
    {
        public static Texture2D p_up, p_down,p_dead;
        public Texture2D sprite;
        public int hash;
        private bool goingUp;
        public Vector2 position;
        private Vector2 size;
        private Vector2 positionfrom;
        private Vector2 positiongoto;
        private bool isdead;
        private int deathtime;
        private int deathadder;
        public bool CollisionPlayer(RectangleF player)
        {
            if (ColRec.IntersectsWith(player))
            {
                return true;
            }
            return false;
        }
        public RectangleF ColRec
        {
            get
            {
                return new RectangleF(position.X - size.X / 2f, position.Y - size.Y / 2f, size.X - 2, size.Y);
            }
        }
        public RectangleF DrawRec
        {
            get
            {
                return new RectangleF(ColRec.X, ColRec.Y, ColRec.Width, ColRec.Height);
            }
        }
        public static void Load()
        {
            p_down = ContentPipe.LoadTexture("piranha_down.png");
            p_up = ContentPipe.LoadTexture("piranha.png");
            p_dead = ContentPipe.LoadTexture("piranha_dead.png");
        }
        public Piranha(Vector2 startPos, Vector2 gravity)
        {
            deathadder = -13;
            this.position = startPos;
            goingUp = true;
            positiongoto = new Vector2(position.X, position.Y - 300); //Sets the max point of the fish, during its jump
            positionfrom = position;
            this.hash = Game.rand.Next(int.MinValue, int.MaxValue);
            
            this.size = new Vector2(51, 73);
        }
        private Vector2 Lerp(Vector2 p1, Vector2 p2, float step)
        {
            if (p2.X == 0) return p1;
            if (p2 == null) return p1;
            if (Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2)) > step)
            {
                double theta = Math.Atan(Math.Abs(p2.Y - p1.Y) / Math.Abs(p2.X - p1.X));
                double deltax = Math.Cos(theta) * step;
                double deltay = Math.Sin(theta) * step;
                if (p1.X > p2.X)
                {
                    deltax *= -1;
                }
                if (p1.Y > p2.Y)
                {
                    deltay *= -1;
                }
                Vector2 ret = new Vector2((float)(p1.X + deltax), (float)(p1.Y + deltay));
                return ret;
            }
            else
            {

                return p2;

            }
        }
        public void Update(ref Level level, Player player)
        {
            if (isdead)
            {
                if (Environment.TickCount > deathtime + 2500)
                {
                    foreach (Enemy e in Game.enemies.ToArray())
                    {
                        if (e.hash == hash)
                            Game.enemies.Remove(e);
                    }
                }
                position.Y += deathadder;
                deathadder++;
                return;
            }
            if (goingUp)
                position = Lerp(position, positiongoto, 15);
            else
                position = Lerp(position, positionfrom, 15);
            if (position.Y == positiongoto.Y)
                goingUp = false;
            if (position.Y == positionfrom.Y)
                goingUp = true;
            
            if (CollisionPlayer(new RectangleF(player.position.X, player.position.Y, 24, 38)))
            {
                if (!player.isInvincible)
                {
                    if (Player.fireflower)
                    {
                        player.isInvincible = true;
                        player.invincibletime = Environment.TickCount;
                        Player.fireflower = false;
                    }
                    else
                    {
                        player.Death();
                    }
                }
            }
            if (ObjectHandler.fireballsonscreen != 0)
                foreach (GameObject o in ObjectHandler.objects.ToArray())
                    if (o.type == ObjectHandler.ObjectType.Fireball)
                        if (Math.Sqrt(Math.Pow(position.X - o.position.X, 2) + Math.Pow(position.Y - o.position.Y, 2)) < 24)
                        {
                            ObjectHandler.objects.Remove(o);
                            ObjectHandler.fireballsonscreen -= 1;
                            isdead = true;
                            //Burn sound
                            deathtime = Environment.TickCount;
                            
                        }

        }
        public void Draw()
        {
            RectangleF rec = DrawRec;
            if (goingUp)
                sprite = p_up;
            else
                sprite = p_down;
            if (isdead)
                sprite = p_dead;
            Spritebatch.DrawSprite(sprite, rec, Color.White);

        }
    }
}
