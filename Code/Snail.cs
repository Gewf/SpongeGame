using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKPlatformerExample
{
    class Snail
    {
        public bool shell;
        public Vector2 velocity;
        public Vector2 position;
        public static Texture2D[] walkAnimation = new Texture2D[2];
        public static Texture2D shells;
        private int cooldowntime;
        public int hash;
        int index = 0;
        private Vector2 gravity;
        private Vector2 size;
        private Texture2D sprite;
        private bool isdead;
        private int deathtime;

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
        public Snail(Vector2 startPos, Vector2 gravity)
        {
            this.gravity = gravity;
            hash = Game.rand.Next(int.MinValue, int.MaxValue);
            this.position = startPos;
            this.velocity = Vector2.Zero;
            this.velocity.X = -2;
            this.size = new Vector2(49, 33);
            sprite = walkAnimation[index];

        }
        public static void Load()
        {
            walkAnimation = new Texture2D[2];
            walkAnimation[0] = ContentPipe.LoadTexture("snailWalk1.png");
            walkAnimation[1] = ContentPipe.LoadTexture("snailWalk2.png");
            shells = ContentPipe.LoadTexture("snailShell.png");
        }
        public void Update(ref Level level, Player player)
        {

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
                if (Environment.TickCount > deathtime + 2500)
                {
                    foreach (Enemy e in Game.enemies.ToArray())
                    {
                        if (e.hash == hash)
                            Game.enemies.Remove(e);
                    }
                }
                position.Y += velocity.Y;
                velocity += gravity * 2;

                return;
            }
            if (!shell)
            {
                index += 1;
                if (index == 6)
                {
                    index = 0;
                }
                if (index < 3)
                {
                    sprite = walkAnimation[0];

                }
                else
                {
                    sprite = walkAnimation[1];
                    position.Y += 3;
                }
            }
            if (shell)
            {
                if (velocity.X < -10f)
                    velocity.X = -10f;
                if (velocity.X > 10f)
                    velocity.X = 10f;
            }
            this.velocity += gravity;
            this.position += velocity;
            ResolveCollisions(ref level);
            if (CollisionPlayer(new RectangleF(player.position.X, player.position.Y, 26, 38)))
            {
                if (player.position.Y + 36 <= position.Y && !player.dying)
                {
                    if (!shell)
                    {
                        player.velocity.Y = -7;
                        if (Input.KeyDown(OpenTK.Input.Key.Z) || Input.A)
                        {
                            player.velocity.Y *= 2;
                        }
                        shell = true;
                        sprite = shells;
                        SoundManager.sounds[5].Play();
                        velocity = Vector2.Zero;
                    }
                    else if (cooldowntime + 500 < Environment.TickCount)
                    {
                        SoundManager.sounds[5].Play();
                        cooldowntime = Environment.TickCount;
                        player.velocity.Y = -7;
                        if (Input.KeyDown(OpenTK.Input.Key.Z) || Input.A)
                        {
                            player.velocity.Y *= 2;
                        }
                        if (velocity.X != 0)
                        {
                               
                            velocity.X = 0;

                        }
                        else
                        {
                            if (player.position.X > position.X - size.X / 6)
                            {
                                velocity.X = -25;
                            }
                            else if (player.position.X <= position.X -size.X/6)
                            {
                                velocity.X = 25;
                            }
                          }
                    }

                }
                else
                {
                    if (!shell)
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
                    else if (cooldowntime + 500 <Environment.TickCount)
                    {

                        cooldowntime = Environment.TickCount;
                        if (velocity.X == 0)
                            if (player.position.X > position.X)
                            {
                                velocity.X = -25;
                                SoundManager.sounds[5].Play();
                            }
                            else
                            {
                                velocity.X = 25;
                                SoundManager.sounds[5].Play();
                            }
                        else
                            if (Player.fireflower)
                        {
                            player.isInvincible = true;
                            player.invincibletime = Environment.TickCount;
                            Player.fireflower = false;
                        }
                        else if (!player.isInvincible) 
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
            size = new Vector2(sprite.Width, sprite.Height);
        }
        public void Draw()
        {
            RectangleF rec = DrawRec;
            if (velocity.X > 0)
            {
                rec.X += rec.Width;
                rec.Width = -rec.Width;

            }
            if (isdead)
            {
                rec.Y += rec.Height;
                rec.Height = -rec.Height;
            }


            Spritebatch.DrawSprite(sprite, rec);
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

                    if (level[x, y].IsSolid && this.ColRec.IntersectsWith(blockRec))
                    {
                        if (level[x, y].IsCoin &&shell)
                        {
                            ObjectHandler.AddObject(ObjectHandler.ObjectType.Coin, new Vector2(level[x, y].posX * 48, (level[x, y].posY - 1) * 48));
                            Block b = new Block(BlockType.Block1, level[x, y].posX, level[x, y].posY, 0, 648);
                            level[x, y] = b;
                            SoundManager.sounds[2].Play();
                            View.Coins += 1;
                        }
                        else if (level[x, y].IsFlower && shell)
                        {

                            ObjectHandler.AddObject(ObjectHandler.ObjectType.Fireflower, new Vector2(level[x, y].posX * 48, (level[x, y].posY - 1) * 48));
                            Block b = new Block(BlockType.Block1, level[x, y].posX, level[x, y].posY, 0, 72);
                            level[x, y] = b;

                        }
                        
                        #region Resolve
                        if (this.position.Y > level[x, y].posY * Game.GRIDSIZE)
                            velocity.X *= -1;
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
                        }

                        #endregion
                    }
                    if (this.velocity.Y > 0 && !Input.KeyDown(OpenTK.Input.Key.Down) && level[x, y].IsPlatform && this.ColRec.IntersectsWith(blockRec))
                    {
                        if (this.position.Y - this.velocity.Y + this.size.Y / 2f <= blockRec.Top) //if we were above last frame
                        {
                            this.velocity.Y = 0;
                            this.position.Y = blockRec.Top - this.size.Y / 2f;
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
