using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKPlatformerExample
{
    class Ghost
    {
        private static Texture2D chase, stand;
        private Texture2D sprite;
        public Vector2 position;
        public int hash;
        bool l;
        public Vector2 size;
        public static void Load()
        {
            chase = ContentPipe.LoadTexture("ghost.png");
            stand = ContentPipe.LoadTexture("ghost_normal.png");
        }
        public Ghost(Vector2 startPos, Vector2 gravity)
        {

            this.hash = Game.rand.Next(int.MinValue, int.MaxValue);
            this.position = startPos;
            this.size = new Vector2(51, 73);
        }
        private Vector2 Lerp(Vector2 p1, Vector2 p2, float step)
        {
            if (p2.X == 0) return p1;
            if (p2 == null) return p1;
            if (Math.Sqrt(Math.Pow(p2.X - p1.X,2) + Math.Pow(p2.Y - p1.Y,2)) > step)
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
            l = false;
            if (player.position.X > position.X)
            {
                l = true;
                if (player.facingRight)
                {
                    position = Lerp(position, player.position, 4f);
                    sprite = chase;
                }else
                {
                    sprite = stand;
                }
            }
            else if (player.position.X <= position.X)
            {
                if (!player.facingRight)
                {
                    position = Lerp(position, player.position, 4f);
                    sprite = chase;
                }else
                {
                    sprite = stand;
                }
            }


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
        }
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
        public void Draw()
        {
            RectangleF rec = DrawRec;
            if (l)
            {
                rec.X += rec.Width;
                rec.Width = -rec.Width;
            }
            Spritebatch.DrawSprite(sprite, rec,Color.DarkGray);
        }
    }
}
