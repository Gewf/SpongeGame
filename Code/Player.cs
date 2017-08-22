using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;
using OpenTKPlatformerExample;
using System.Threading;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace OpenTKPlatformerExample
{
    class Player
    {


        public Vector2 position;
        public Vector2 velocity;
        int counter = 0;
        int xcounter = 0;
        public bool isInvincible;
        public int invincibletime;
        public bool dying;
        public Texture2D sprite, spriteJump, spriteFall, firespriteJump, firespriteFall, firespriteShoot, dead;
        private Texture2D[] walkAnimation;
        private Texture2D[] firewalkAnimation;
        private Vector2 size;
        private Vector2 gravity;
        private bool onLadder, climbing;
        public bool grounded, facingRight;
        public static bool fireflower;
        int index = 0;
        private Texture2D spriteClimb,firespriteClimb;

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

        public Player(Vector2 startPos, Vector2 gravity)
        {
            facingRight = true;
            //fireflower = false;
            isInvincible = true;
            this.gravity = gravity;
            this.position = startPos;
            this.velocity = Vector2.Zero;
            this.size = new Vector2(38, 64);
            walkAnimation = new Texture2D[4];
            walkAnimation[0] = ContentPipe.LoadTexture("sponge.png");
            walkAnimation[1] = ContentPipe.LoadTexture("sponge1.png");
            walkAnimation[2] = ContentPipe.LoadTexture("sponge2.png");
            walkAnimation[3] = ContentPipe.LoadTexture("sponge3.png");
            firewalkAnimation = new Texture2D[4];
            firewalkAnimation[0] = ContentPipe.LoadTexture("firesponge.png");
            firewalkAnimation[1] = ContentPipe.LoadTexture("firesponge1.png");
            firewalkAnimation[2] = ContentPipe.LoadTexture("firesponge2.png");
            firewalkAnimation[3] = ContentPipe.LoadTexture("firesponge3.png");
            this.spriteJump = ContentPipe.LoadTexture("spongeJump.png");
            this.spriteFall = ContentPipe.LoadTexture("spongeFall.png");
            this.firespriteJump = ContentPipe.LoadTexture("firespongeJump.png");
            this.firespriteFall = ContentPipe.LoadTexture("firespongeFall.png");
            this.firespriteShoot = ContentPipe.LoadTexture("firespongeshoot.png");
            this.dead = ContentPipe.LoadTexture("spongeDead.png");
            this.spriteClimb = ContentPipe.LoadTexture("spongeClimb.png");
            this.firespriteClimb = ContentPipe.LoadTexture("firespongeClimb.png");
            sprite = walkAnimation[index];
        }

        public void Update(ref Level level)
        {
            if (Game.Level == 6)
            {
                return;
            }

            if (ObjectHandler.fireballsonscreen < 0)
            {
                ObjectHandler.fireballsonscreen = 0;
            }
            if (position.Y > 655 + 240) //View centery + half of room = below level
            {
                if (!dying)
                {
                    Death();
                    return;
                }
                else
                {
                    //Reached outside of room while dead = reset position
                    dying = false;
                    isInvincible = true;
                    facingRight = true;
                    invincibletime = Environment.TickCount;
                    position =
    new Vector2(level.playerStartPos.X + 0.5f, level.playerStartPos.Y + 0.5f) * 48; //48 is gridsize
                    velocity = Vector2.Zero;
                    
                    if (View.Lives == 0)
                    {
                        Game.intro = new Message[]
                        {
                    new Message("Sponge? Where are you?!",Color.DarkGreen),
                    new Message("You died          Press Z to restart",Color.Gray),
                        };
                        Game.introind = 0;
                        View.Lives = 3;
                        View.Coins = 0;
                        Game.slevel = "deathstory";
                        DialogBox.cd = Game.intro[0].color;
                        DialogBox.LoadString(Game.intro[0].msg);
                        Game.Reset();
                    }else
                    {
                        Game.Reload();
                    }
                }
            }
            if (!dying)
            {

                counter++;
                if (counter == 2)
                {
                    counter = 0;
                }
                if (Input.KeyDown(OpenTK.Input.Key.Up) || Input.KeyDown(OpenTK.Input.Key.Down) || Input.up || Input.down)
                    xcounter++;
                if (xcounter == 12)
                {
                    xcounter = 0;
                }
                HandleInput();
                if (invincibletime + 1500 < Environment.TickCount)
                {
                    isInvincible = false;
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
                    if (velocity.X < -8f)
                        velocity.X = -8f;
                    if (velocity.X > 8f)
                        velocity.X = 8f;
                }
                ResolveCollisions(ref level);
                foreach (Enemy ex in Game.enemies.ToArray())
                {
                        if (Math.Abs(ex.position.X - position.X) < 640)
                        {
                        if (ex.enemyType == 2)
                        {
                            if (Math.Abs(ex.position.X - position.X) < 340)
                                ex.update = true;
                        }
                        else
                        {
                            ex.update = true;
                        }
                        
                        }
                        
                    
                    ex.Update(ref level, this);
                }
            }
            if (!climbing)
                this.velocity += gravity;
            this.position += velocity;


        }
        public void Death()
        {
            View.Lives -= 1;
            Enemy.noGhostattack = true;
            dying = true;
            fireflower = false;
            climbing = false;
            velocity.Y = -17;
            velocity.X = 0;
            sprite = dead;
           
        }
        private void HandleInput()
        {

            if (!onLadder)
                climbing = false;
            else if (Input.KeyDown(OpenTK.Input.Key.Up) || Input.up)
                climbing = true;

            if (grounded)
            {
                this.velocity.X *= 0.9f;
            }
            else if (climbing)
            {
                this.velocity.X *= 0.5f;
                this.velocity.Y = 0f;
                if (Input.KeyDown(OpenTK.Input.Key.Up) || Input.up)
                {
                    this.velocity.Y -= 5f;
                }
                if (Input.KeyDown(OpenTK.Input.Key.Down) || Input.down)
                {
                    this.velocity.Y += 5f;
                }
            }

            if (grounded && !Input.KeyDown(OpenTK.Input.Key.Up) && !Input.up)
            {
                climbing = false;
            }
            if (Input.KeyDown(OpenTK.Input.Key.Right) || Input.right)
            {
                int run = 1;
                if (Input.KeyDown(OpenTK.Input.Key.X) || Input.B)
                {
                    run = 2;
                }
                if (climbing)
                {
                    this.velocity.X = 2f;
                }
                else if (grounded)
                {
                    this.velocity.X += 0.5f * run;
                }
                else
                {
                    this.velocity.X += 0.4f * run;
                }
                index += 1;
                if (index == 4)
                {
                    index = 0;
                }
                facingRight = true;
                if (!fireflower)
                    sprite = walkAnimation[index];
                else
                    sprite = firewalkAnimation[index];
            }
            else
            if (Input.KeyDown(OpenTK.Input.Key.Left) || Input.left)
            {
                int run = 1;
                if (Input.KeyDown(OpenTK.Input.Key.X) || Input.B)
                {
                    run = 2;
                }
                if (climbing)
                {
                    this.velocity.X = -2f;
                }
                else if (grounded)
                {
                    this.velocity.X -= 0.5f * run;
                }
                else
                {
                    this.velocity.X -= 0.4f * run;
                }
                index += 1;
                if (index == 4)
                {
                    index = 0;
                }
                facingRight = false;
                if (!fireflower)
                    sprite = walkAnimation[index];
                else
                    sprite = firewalkAnimation[index];
            }
            else
            {
                if (!fireflower)
                    sprite = walkAnimation[0];
                else
                    sprite = firewalkAnimation[0];
            }

            if ((Input.KeyDown(OpenTK.Input.Key.Z) || Input.A) && (grounded || climbing))
            {
                SoundManager.sounds[0].Play();
                this.velocity.Y = -16;
                this.climbing = false;
            }
            if ((Input.KeyPress(OpenTK.Input.Key.X) || (Input.B && !Input.lastB)) && fireflower)
            {
                sprite = firespriteShoot;
                if (facingRight)
                    ObjectHandler.AddObject(ObjectHandler.ObjectType.Fireball, new Vector2(position.X + size.X / 2, position.Y + 5));
                else
                    ObjectHandler.AddObject(ObjectHandler.ObjectType.Fireball, new Vector2(position.X, position.Y + 5), true);
            }
        }
        private void ResolveCollisions(ref Level level)
        {
            int minX = (int)Math.Floor((this.position.X - this.size.X / 2f) / Game.GRIDSIZE);
            int minY = (int)Math.Floor((this.position.Y - this.size.Y / 2f) / Game.GRIDSIZE);
            int maxX = (int)Math.Ceiling((this.position.X + this.size.X / 2f) / Game.GRIDSIZE);
            int maxY = (int)Math.Ceiling((this.position.Y + this.size.Y / 2f) / Game.GRIDSIZE);

            this.grounded = false;
            this.onLadder = false;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    RectangleF blockRec = new RectangleF(x * Game.GRIDSIZE, y * Game.GRIDSIZE, Game.GRIDSIZE, Game.GRIDSIZE);

                    if (level[x, y].IsSolid && this.ColRec.IntersectsWith(blockRec))
                    {


                        if ((level[x, y].posY + 1) * 48 < this.position.Y && !grounded && this.position.X >= 48 * level[x, y].posX && this.position.X < 48 * (level[x, y].posX + 1))
                        {
                            if (level[x, y].IsCoin)
                            {
                                ObjectHandler.AddObject(ObjectHandler.ObjectType.Coin, new Vector2(level[x, y].posX * 48, (level[x, y].posY - 1) * 48));
                                Block b = new Block(BlockType.Block1, level[x, y].posX, level[x, y].posY, 0, 648);
                                level[x, y] = b;
                                SoundManager.sounds[2].Play();
                                View.Coins += 1;
                            }
                            else if (level[x, y].IsFlower)
                            {

                                ObjectHandler.AddObject(ObjectHandler.ObjectType.Fireflower, new Vector2(level[x, y].posX * 48, (level[x, y].posY - 1) * 48));
                                Block b = new Block(BlockType.Block1, level[x, y].posX, level[x, y].posY, 0, 72);
                                level[x, y] = b;

                            }
                            else if (level[x, y].tileX == 0)
                            {
                                SoundManager.sounds[1].Play();
                            }
                        }

                        #region Resolve
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
                    if (this.velocity.Y > 0 && (!Input.KeyDown(OpenTK.Input.Key.Down) && !Input.down) && level[x, y].IsPlatform && this.ColRec.IntersectsWith(blockRec))
                    {
                        if (this.position.Y - this.velocity.Y + this.size.Y / 2f <= blockRec.Top) //if we were above last frame
                        {
                            this.velocity.Y = 0;
                            this.position.Y = blockRec.Top - this.size.Y / 2f;
                            grounded = true;
                        }
                    }
                    if (level[x, y].Isladder && this.ColRec.IntersectsWith(blockRec))
                    {
                        this.onLadder = true;
                    }
                }
            }

        }

        public void Draw(Color color)
        {
            if (Game.Level == 6)
            {
                return;
            }
            RectangleF rec = DrawRec;
            if (dying)
            {
                sprite = dead;
            }
            if (!facingRight)
            {
                rec.X += rec.Width;
                rec.Width = -rec.Width;

            }
            if (isInvincible && counter % 2 == 0)
            {
                return;
            }
            if (climbing)
            {
                if (xcounter<6)
                {
                    rec.X += rec.Width;
                    rec.Width = -rec.Width;
                }
                if (fireflower)
                {

                    Spritebatch.DrawSprite(firespriteClimb, rec, color);
                }
                else
                {
                    Spritebatch.DrawSprite(spriteClimb, rec, color);
                }
            }
            else if (!grounded && velocity.Y <= 0 && !dying)
            {
                if (!fireflower)
                    Spritebatch.DrawSprite(spriteJump, rec, color);
                else
                    Spritebatch.DrawSprite(firespriteJump,rec, color);

            }
            else if (!grounded && velocity.Y > 0 && !dying)
            {
                if (!fireflower)
                    Spritebatch.DrawSprite(spriteFall, rec, color);
                else
                    Spritebatch.DrawSprite(firespriteFall, rec, color);
            }
            else
            {
                Spritebatch.DrawSprite(sprite, rec, color);
            }

            //Spritebatch.DrawSprite(Game.dot, this.ColRec, Color.Red);
        }
    }
}
