using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKPlatformerExample
{
    class GrassBlock
    {
        public static Texture2D[] walkAnimation = new Texture2D[2];
        private Vector2 gravity;
        public Vector2 position;
        private Vector2 velocity;
        private Vector2 size;
        int index = 0;
        public bool jump;
        int deathtime = 0;
        private Texture2D sprite;
        public static Texture2D dead;
        public bool grounded, isdead;
        public int hash;

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
        public GrassBlock(Vector2 startPos, Vector2 gravity)
        {
            this.gravity = gravity;
            hash = Game.rand.Next(int.MinValue, int.MaxValue);
            position = startPos;
            velocity = Vector2.Zero;
            size = new Vector2(50, 48);
        }
        public static void Load()
        {
            walkAnimation = new Texture2D[2];
            walkAnimation[0] = ContentPipe.LoadTexture("grassBlock.png");
            walkAnimation[1] = ContentPipe.LoadTexture("grassBlock_jump.png");
            dead = ContentPipe.LoadTexture("grassBlock_dead.png");
        }
        public void Draw()
        {
            RectangleF rec = DrawRec;
            
            if (jump)
                sprite = walkAnimation[1];
            else
                sprite = walkAnimation[0];
            if (isdead)
            {
                rec.Y += rec.Height;
                rec.Height = -rec.Height;
                sprite = dead;
            }
            Spritebatch.DrawSprite(sprite, rec);
        }
        public void Update(ref Level level, Player player)
        {
            if (!jump)
            {
                index++;
                if (index > 20)
                {
                    index = 0;
                    jump = true;
                    if (player.position.X > position.X - size.X / 6)
                    {
                        velocity = new Vector2(5, -15);
                    }else
                    {
                        velocity = new Vector2(-5, -15);
                    }
                }
            }else
            {
                if (velocity.Y >= 0)
                    jump = false;
                else
                if (player.position.X > position.X - size.X / 6)
                {
                    velocity.X = 5;
                }
                else
                {
                    velocity.X = -5;
                }

            }
            if (position.Y > 850)
            {
                foreach (Enemy e in Game.enemies.ToArray())
                {
                    if (e.hash == hash)
                    {
                        Game.enemies.Remove(e);
                        return;
                    }
                }
            }
            if (isdead)
            {
                if (Environment.TickCount > deathtime + 1500)
                {
                    foreach (Enemy e in Game.enemies.ToArray())
                    {
                        if (e.hash == hash)
                            Game.enemies.Remove(e);
                    }
                }

                    position.Y += velocity.Y;
                    velocity += gravity * 3;
                
                return;
            }



            if (grounded)
            {
                if (velocity.X < -10f)
                    velocity.X = -10f;
                if (velocity.X > 10f)
                    velocity.X = 10f;
            }
            else
            {
                if (velocity.X < -7f)
                    velocity.X = -7f;
                if (velocity.X > 7f)
                    velocity.X = 7f;
            }

            this.velocity += gravity * 2;
            this.position += velocity;
            ResolveCollisions(ref level);
            if (CollisionPlayer(new RectangleF(player.position.X, player.position.Y, 20, 38)))
            {
                if (player.position.Y + 35 <= position.Y && !player.dying)
                {
                    player.velocity.Y = -7;
                    if (Input.KeyDown(OpenTK.Input.Key.Z) || Input.A)
                    {
                        player.velocity.Y *= 2;
                    }
                    sprite = dead;
                    SoundManager.sounds[5].Play();
                    velocity = Vector2.Zero;
                    deathtime = Environment.TickCount;
                    isdead = true;
                }
                else
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
                            velocity = new Vector2(velocity.X, -10);
                        }
        }
        private void ResolveCollisions(ref Level level)
        {
            int minX = (int)Math.Floor((this.position.X - this.size.X / 2f) / Game.GRIDSIZE);
            int minY = (int)Math.Floor((this.position.Y - this.size.Y / 2f) / Game.GRIDSIZE);
            int maxX = (int)Math.Ceiling((this.position.X + this.size.X / 2f) / Game.GRIDSIZE);
            int maxY = (int)Math.Ceiling((this.position.Y + this.size.Y / 2f) / Game.GRIDSIZE);



            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    RectangleF blockRec = new RectangleF(x * Game.GRIDSIZE, y * Game.GRIDSIZE, Game.GRIDSIZE, Game.GRIDSIZE);

                    if ((level[x, y].IsSolid || level[x,y].IsPlatform)&& this.ColRec.IntersectsWith(blockRec) )
                    {
                        if (y * Game.GRIDSIZE > position.Y && !jump)
                        {
                            
                            velocity.X = 0;
                        }
                        #region Resolve
                        if (y * Game.GRIDSIZE <= position.Y )
                            if (x * Game.GRIDSIZE > position.X)
                            {
                                velocity.X = -3;
                            }
                            else
                            {
                                velocity.X = 3;
                            }
                        float[] depths = new float[4]
                        {
                            blockRec.Right - ColRec.Left, //PosX
							blockRec.Bottom - ColRec.Top, //PosY
							ColRec.Right - blockRec.Left, //NegX
							ColRec.Bottom - blockRec.Top  //NegY
						};

                        if (level[x + 1, y].IsSolid)
                            depths[0] = -1;
                        if (level[x, y + 1].IsSolid || level[x, y].IsPlatform)
                            depths[1] = -1;
                        if (level[x - 1, y].IsSolid)
                            depths[2] = -1;
                        if (level[x, y - 1].IsSolid)
                            depths[3] = -1;

                        float min = float.MaxValue;
                        Vector2 minDirection = Vector2.Zero;
                        for (int i = 0; i < 4; i++)
                        {
                            if (depths[i] >= 0 && depths[i] < min)
                            {
                                min = depths[i];
                                switch (i)
                                {
                                    case 0:
                                        minDirection = new Vector2(1, 0);
                                        break;
                                    case 1:
                                        minDirection = new Vector2(0, 1);
                                        break;
                                    case 2:
                                        minDirection = new Vector2(-1, 0);
                                        break;
                                    default:
                                        minDirection = new Vector2(0, -1);
                                        break;
                                }
                            }
                        }

                        if (minDirection == Vector2.Zero)
                        {
                            //Console.WriteLine("This shouldn't really happen...");
                        }
                        else
                        {
                            this.position += minDirection * min;
                            if (this.velocity.X * minDirection.X < 0)
                                this.velocity.X = 0;
                            if (this.velocity.Y * minDirection.Y < 0)
                                this.velocity.Y = 0;

                            if (minDirection == new Vector2(0, -1))
                                grounded = true;
                        }

                        #endregion
                    }
                    if (this.velocity.Y > 0 && !Input.KeyDown(OpenTK.Input.Key.Down) && level[x, y].IsPlatform && this.ColRec.IntersectsWith(blockRec))
                    {
                        if (this.position.Y - this.velocity.Y + this.size.Y / 2f <= blockRec.Top) //if we were above last frame
                        {
                            this.velocity.Y = 0;
                            this.position.Y = blockRec.Top - this.size.Y / 2f;
                            grounded = true;
                        }
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
    }
}
