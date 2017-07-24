using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKPlatformerExample
{
    class EventGhost
    {
        public int hash;
        public Vector2 position;
        public int counter;
        public int xcounter;
        public bool phase1;
        public bool isHurt; //If true, visible and hurts sponge, else "invisible" and doesnt hurt
        public bool isAttack; //Leaves the ghost herd and goes for Sponge
        public static Texture2D chill;
        public static Texture2D savage;
        public Texture2D sprite;
        private Vector2 size;
        private bool l;
        private Vector2 positiongoto;

        public static void Load()
        {
            chill = ContentPipe.LoadTexture("ghost_normal.png");
            savage = ContentPipe.LoadTexture("ghost.png");
        }
        public EventGhost(Vector2 startPos, Vector2 gravity)
        {

            this.hash = Game.rand.Next(int.MinValue, int.MaxValue);
            this.position = startPos;
            this.size = new Vector2(51, 73);
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
            if (!isHurt)
                Spritebatch.DrawSprite(sprite, rec, Color.FromArgb(128,169,169,169));
            else
                Spritebatch.DrawSprite(sprite, rec, Color.DarkGray);

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
            if (isAttack) xcounter++;
            if (isAttack)
                if (phase1)
                {
                    if (Math.Abs(positiongoto.Y -position.Y)< 10 || xcounter > 35)
                    {
                        phase1 = false;
                        positiongoto = new Vector2(Game.rand.Next(-60, 70), -230 + Game.rand.Next(-25, 25)) + player.position;
                        isHurt = false;
                    }
                    else
                    {
                        positiongoto.X = player.position.X;
                    }
                }else
                {
                   
                    if (positiongoto.X == position.X && positiongoto.Y == position.Y)
                    {
                        isAttack = false;
                        Enemy.noGhostattack = true;
                    }
                }
            if (counter == 0 && !isAttack)
            {
                positiongoto = new Vector2(Game.rand.Next(-300, 500),- 230 + Game.rand.Next(-25, 25));
            }
            counter++;
            if (counter > 9)
            {
                counter = 0;
            }
            l = false;
            if (player.position.X > position.X)
            {
                l = true;
            }

            if (!isAttack)
            {
                position = Lerp(position,player.position + positiongoto ,15);
            }else
            {
                if (phase1)
                position = Lerp(position, positiongoto, 8);
                else
                    position = Lerp(position, positiongoto, 13);
            }
            if (CollisionPlayer(new RectangleF(player.position.X, player.position.Y, 24, 38)) && isHurt)
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

                if (isAttack) sprite = savage;
            else sprite = chill;

        }
        public void Attack(Player player)
        {
            xcounter = 0;
            isAttack = true;
            phase1 = true;
            positiongoto = player.position;
            isHurt = true;
        }
    }
}
