using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Input;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace OpenTKPlatformerExample
{
    class Game : GameWindow
    {
        #region declarations
        public static int GRIDSIZE = 48;
        public static int Level = 1;
  
        public static List<Enemy> enemies = new List<Enemy>();
        public static Texture2D dot;
        public static string slevel = "";
        public static int introind = 0;
        public static Random rand = new Random();
        Texture2D tileSet;
        View view;
        public static Level level;
        public static Player player;

        public static Message[] intro = new Message[]
        {
            new Message("Looks like anotherworld is finished.",Color.DarkRed),
            new Message("Are you sure aboutthat brother? Thisone looks kinda   depressing to me..", Color.DarkGreen),
            new Message("All the enemies.. powerups.. coins..everything is sad,even the sky.", Color.DarkGreen),
            new Message("Oh don't worry    about that.",Color.DarkRed),
            new Message("SPONGE!",Color.DarkRed),
            new Message("...",Color.DarkOrange),
            new Message("Make your lazy    ass useful and useyour stupid power to soak the       sadness of the newworld.",Color.DarkRed),
            new Message("B-but it's the 2ndtime this month.  And you took away my antidepressant pills last ti-",Color.DarkOrange),
            new Message("HOW MANY TIMES DO I HAVE TO TELL    THIS TO YOU?!",Color.DarkRed),
            new Message("We are out there  risking our lives fighting a giant  dragon to save theprincess.",Color.DarkRed),
            new Message("You might aswell  do SOMETHING      instead of sittinghere all day      crying.",Color.DarkRed),
            new Message("He is our brother man,don't be so   harsh on him.", Color.DarkGreen),
            new Message("You better not be telling me what   to do,you         fucking adopted   terminal-7 host.",Color.DarkRed),
            new Message("But M-", Color.DarkGreen),
            new Message("SHUT YOUR MOUTH   IDIOT!",Color.DarkRed),
            new Message("Do you want us to get DMCA'ed? You  can  only refer toyour drama queen  brother with his  real name.",Color.DarkRed),
            new Message("O-okay brother.", Color.DarkGreen),
            new Message("Now I have some   stuff to do unlikeyou two lifeless  losers.",Color.DarkRed),
            new Message("Do as I said      Sponge.If you're  lucky, I might    consider giving   you your stupid   tic-tacs back.",Color.DarkRed),
            new Message("Now get to work.",Color.DarkRed),
        };
        #endregion
        public static void Reset()
        {
            ObjectHandler.fireballsonscreen = 0;

            Level = 1;
            enemies.Clear();
            ObjectHandler.objects.Clear();
            level = new Level("Content/Levels/Level1.xml");

            player = new Player(new Vector2(level.playerStartPos.X + 0.5f, level.playerStartPos.Y + 0.5f) * GRIDSIZE, new Vector2(0, 0.8f));
           
        } //Call when lives are lost
        public static void Reload()
        {
            ObjectHandler.fireballsonscreen = 0;
            enemies.Clear();
            ObjectHandler.objects.Clear();
            if (Level == 1)
            {
                level = new Level("Content/Levels/Level1.xml");
            }else if (Level == 2)
            {
                level = new Level("Content/Levels/Desert.xml");
            }else if (Level == 3)
            {
                level = new Level("Content/Levels/GhostHouse.xml");
            }else if (Level == 4)
            {
                level = new Level("Content/Levels/GhostEventRoom.xml");
            }else if (Level == 5)
            {
                Console.WriteLine("Add level 5 to reload()");
            }
            player = new Player(new Vector2(level.playerStartPos.X + 0.5f, level.playerStartPos.Y + 0.5f) * GRIDSIZE, new Vector2(0, 0.8f));
        }
        public Game()
            : base(640, 480, new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8))
        {
            Title = "";
            slevel = "story";
            WindowBorder = WindowBorder.Fixed;
            if (SoundManager.volume == -1)
            SoundManager.volume = 0.1f;
            DialogBox.cd = intro[0].color;
            DialogBox.LoadString(intro[0].msg);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            //Set how the alpha blending function works
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            ObjectHandler.Load();
            
            Input.Initialize(this);
            view = new View(Vector2.Zero, 1f, 0.0);

        } //Window initializer

        //Loads a certain level
        public static void LoadLevel(string path)
        {
            enemies.Clear();
            ObjectHandler.objects.Clear();
            level = new Level("Content/Levels/" + path);
            
            player = new Player(new Vector2(level.playerStartPos.X + 0.5f, level.playerStartPos.Y + 0.5f) * GRIDSIZE, new Vector2(0, 0.8f)); //Player object
        }

      
        //Load - is called once per game execution. Load textures and stuff here.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //Load certain classes
            DialogBox.Load();
            //Load enemies
            Slime.Load();
            Snail.Load();
            GrassBlock.Load();
            Ghost.Load();
            EventGhost.Load();
            Piranha.Load();
            //Load the first level and texture
            tileSet = ContentPipe.LoadTexture("TileSet1.png");
            level = new Level("Content/Levels/Level1.xml");
            player = new Player(new Vector2(level.playerStartPos.X + 0.5f, level.playerStartPos.Y + 0.5f) * GRIDSIZE, new Vector2(0, 0.8f)); //Player object
            view.width = level[0, 0].posX + Width / 2 + 35; 
            #region Create dot texture
            {
                int id = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, id);

                Bitmap bmp = new Bitmap(1, 1);
                bmp.SetPixel(0, 0, Color.White);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    1, 1, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte, bmpData.Scan0);

                dot = new Texture2D(id, 1, 1);

                bmp.UnlockBits(bmpData);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            }
            #endregion
        }
        //Update - triggers before render - do math here
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            Input.EarlyUpdate();
            if (!slevel.Contains("story")) //Levels that contain "story" will not render the level or update anything, just display
                                           //the story
                player.Update(ref level);
            if (slevel.Contains("story"))
            {
                if (DialogBox.UpdateString())
                {
                    if (Input.KeyPress(Key.Z) || (Input.A && !Input.lastA))
                    {
                        if (introind < intro.Length - 1)
                        {
                            introind++;
                            DialogBox.cd = intro[introind].color;
                            DialogBox.LoadString(intro[introind].msg);
                        }
                        else
                        {
                            slevel = "game";
                            player.invincibletime = Environment.TickCount;
                            SoundManager.volume = 0.1f;
                            DialogBox.cd = Color.White;
                            intro = new Message[] { };
                        }
                    }
                }
                else
                {
                    if (Input.KeyPress(Key.Z) || (Input.A && !Input.lastA))
                    {
                        DialogBox.meter = DialogBox.message.Length;
                    }

                }
                if (Input.KeyPress(Key.X) || (Input.B && !Input.lastB))
                {
                    intro = new Message[] { };
                    slevel = "game";
                    player.invincibletime = Environment.TickCount;
                    SoundManager.volume = 0.1f;
                    DialogBox.cd = Color.White;
                }
            }
            Input.Update();
            view.SetPosition(player.position, TweenType.Instant, 15);

            
            view.Update();
            foreach (GameObject o in ObjectHandler.objects.ToArray())
            {
                o.Update(player);
            }
            if (0>ObjectHandler.fireballsonscreen)
            {
                ObjectHandler.fireballsonscreen = 0;
            }else if (ObjectHandler.fireballsonscreen > 2)
            {
                ObjectHandler.fireballsonscreen = 2;
            }
            Input.LateUpdate();
        }
        //Render - draw stuff here
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            List<Enemy> afterdraw = new List<Enemy>(); //Enemies drawn after tiles - above them. (atm all except piranhas)
            Color tilec = Color.LightGray;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (slevel.Contains("story"))
                GL.ClearColor(Color.FromArgb(255, 0, 0, 0));
            else if (Level == 1)
            {
                GL.ClearColor(Color.FromArgb(255, 10, 10, 10));
                tilec = Color.LightGray;
            }else if (Level == 2)
            {
                GL.ClearColor(Color.FromArgb(255, 0, 20, 20));
                tilec = Color.LightGray;
            }
            else if (Level == 3 || Level == 4)
            {
                GL.ClearColor(Color.FromArgb(255, 0, 0, 16));
                tilec = Color.Gray;
            }


            Spritebatch.Begin(this);
            view.ApplyTransforms();
            
            if (!slevel.Contains("story"))
                foreach (GameObject o in ObjectHandler.objects)
                {
                    o.Draw(tilec);
                }
            if (!slevel.Contains("story"))
                foreach (Enemy ex in enemies)
                {
                    if (ex.enemyType == 5)
                        ex.Draw();
                    else
                        afterdraw.Add(ex);
                }
            if (!slevel.Contains("story"))
                for (int x = 0; x < level.Width; x++)
                {
                    for (int y = 0; y < level.Height; y++)
                    {

                        int tileSize = 70;
                        RectangleF sourceRec = new RectangleF(0, 0, 0, 0);

                        sourceRec = new RectangleF(level[x, y].tileX, level[x, y].tileY, tileSize, tileSize);

                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                        Spritebatch.DrawSprite(tileSet, new RectangleF(x * GRIDSIZE, y * GRIDSIZE, GRIDSIZE + 1, GRIDSIZE + 1), tilec, sourceRec);
                    }
                }
            foreach (Enemy ex in afterdraw)
            {
                ex.Draw();
            }
                if (!slevel.Contains("story"))
                player.Draw(tilec);
            if (!slevel.Contains("story"))
                view.DrawHUD();
            if (slevel.Contains("story"))
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                DialogBox.DrawString(view.Position.X - 288, view.Position.Y);

            }
            this.SwapBuffers();
        }
    }

    class Message //Message class for storylines
    {             //Just a touple of string and Color
        public string msg;
        public Color color;
        public Message(string msg, Color color)
        {
            this.msg = msg;
            this.color = color;
        }
    }

}
