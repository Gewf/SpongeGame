using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.IO;
using System.Drawing;
using System.Xml;

namespace OpenTKPlatformerExample
{
	struct Level
	{
		private Block[,] grid;
		private string filePath;
		public Point playerStartPos;

		public int Width
		{
			get
			{
				return grid.GetLength(0);
			}
		}
		public int Height
		{
			get
			{
				return grid.GetLength(1);
			}
		}
		public string FilePath
		{
			get { return filePath; }
		}

		public Level(int width, int height)
		{
			filePath = "none";
			this.grid = new Block[width, height];
			playerStartPos = new Point(width / 2, height - 2);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
					{
						this.grid[x, y] = new Block(BlockType.Block1, x, y,x,y);
					}
					else
					{
						this.grid[x, y] = new Block(BlockType.Empty, x, y,x,y);
					}
				}
			}
		}
		public Level(string filePath)
		{
			this.filePath = filePath;

			try
			{
				if (Path.GetExtension(filePath).ToLower() == ".xml" || Path.GetExtension(filePath).ToLower() == ".tmx")
				{
					#region Loading XML
					using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					{
						XmlDocument doc = new XmlDocument();
						doc.Load(stream);

						int width = int.Parse(doc.DocumentElement.GetAttribute("width"));
						int height = int.Parse(doc.DocumentElement.GetAttribute("height"));

						this.grid = new Block[width, height];
						this.filePath = filePath;
						this.playerStartPos = new Point(1, 1);

						XmlNode tileLayer = doc.DocumentElement.SelectSingleNode("layer[@name='Tile Layer 1']");
						XmlNode data = tileLayer.SelectSingleNode("data");
						XmlNodeList tiles = data.SelectNodes("tile");

						int x = 0, y = 0;
						for (int i = 0; i < tiles.Count; i++)
						{
							int gid = int.Parse(tiles[i].Attributes["gid"].Value);

                            switch (gid)
							{

                                case 0: //Empty space
                                    grid[x, y] = new Block(BlockType.Empty, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                case 26: //Torch
                                    grid[x, y] = new Block(BlockType.Empty, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                case 44: //WaterBody
                                    grid[x, y] = new Block(BlockType.Empty, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                case 103: //WaterHead
                                    grid[x, y] = new Block(BlockType.Empty, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                case 25: //FireflowerBlock
                                    grid[x, y] = new Block(BlockType.FlowerBlock, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                case 20: //LadderTop
                                    grid[x, y] = new Block(BlockType.Ladder, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                case 32: //LadderMid
                                    grid[x, y] = new Block(BlockType.Ladder, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                case 34: //LadderwithPlatform
                                    grid[x, y] = new Block(BlockType.LadderPlatform, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                case 121: //CoinBlock
                                    grid[x, y] = new Block(BlockType.CoinBlock, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                case 125: //Bridge
                                    grid[x, y] = new Block(BlockType.Platform, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                case 41: //Sign
                                    grid[x, y] = new Block(BlockType.Empty, x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;
                                default: //Everything else not defined here is solid
                                    grid[x, y] = new Block(BlockType.Block1,x, y, (((gid - 1) % 12) * 72), (int)Math.Floor((float)(gid - 1) / 12) * 72);
                                    break;


                            }

                            x++;
							if (x >= width)
							{
								x = 0;
								y++;
							}
						}

						XmlNode objectsLayer = doc.DocumentElement.SelectSingleNode("objectgroup[@name='Objects']");
						XmlNodeList objects = objectsLayer.SelectNodes("object");
						for (int i = 0; i < objects.Count; i++)
						{
							switch (objects[i].Attributes["name"].Value)
							{
								case "playerStartPos":
									int posX = int.Parse(objects[i].Attributes["x"].Value);
									int posY = int.Parse(objects[i].Attributes["y"].Value);
									this.playerStartPos = new Point((int)(posX / 70), (int)(posY / 70));
									break;
                                case "slime":
                                    int xx = int.Parse(objects[i].Attributes["x"].Value);
                                    int yy = int.Parse(objects[i].Attributes["y"].Value);
                                    Game.enemies.Add(new Enemy(new Slime(new Vector2(xx/70 + 0.5f,yy/70 + 0.5f) * 48, new Vector2(0, 0.5f)),0));
                                    break;
                                case "snail":
                                    int xxx = int.Parse(objects[i].Attributes["x"].Value);
                                    int yyy = int.Parse(objects[i].Attributes["y"].Value);
                                    Game.enemies.Add(new Enemy(new Snail(new Vector2(xxx / 70 + 0.5f, yyy / 70 + 0.5f) * 48, new Vector2(0, 0.5f)), 1));
                                    break;
                                case "ghost":
                                    int x2= int.Parse(objects[i].Attributes["x"].Value);
                                    int y2 = int.Parse(objects[i].Attributes["y"].Value);
                                    Game.enemies.Add(new Enemy(new Ghost(new Vector2(x2 / 70 + 0.5f, y2 / 70 + 0.5f) * 48, new Vector2(0, 0.5f)), 3));
                                    break;
                                case "eventghost":
                                    int x3 = int.Parse(objects[i].Attributes["x"].Value);
                                    int y3 = int.Parse(objects[i].Attributes["y"].Value);
                                    Game.enemies.Add(new Enemy(new EventGhost(new Vector2(x3 / 70 + 0.5f, y3 / 70 + 0.5f) * 48, new Vector2(0, 0.5f)), 4));
                                    break;
                                case "coin":
                                    int xxxx = int.Parse(objects[i].Attributes["x"].Value);
                                    int yyyy = int.Parse(objects[i].Attributes["y"].Value);
                                    Vector2 po = new Vector2(xxxx / 70, yyyy / 70 ) * 48;
                                    ObjectHandler.AddObject(ObjectHandler.ObjectType.StandingCoin, po);
                                    break;
                                case "grass":
                                    int xxxxx = int.Parse(objects[i].Attributes["x"].Value);
                                    int yyyyy = int.Parse(objects[i].Attributes["y"].Value);
                                    Game.enemies.Add(new Enemy(new GrassBlock(new Vector2(xxxxx / 70 + 0.5f, yyyyy / 70 + 0.5f) * 48, new Vector2(0, 0.5f)), 2));
                                    break;
                                case "piranha":
                                    int x5 = int.Parse(objects[i].Attributes["x"].Value);
                                    int y5 = int.Parse(objects[i].Attributes["y"].Value);
                                    Game.enemies.Add(new Enemy(new Piranha(new Vector2(x5 / 70 + 0.5f, y5 / 70 + 0.5f) * 48, new Vector2(0, 0.5f)), 5));
                                    break;
                                case "door":
                                    int x1 = int.Parse(objects[i].Attributes["x"].Value);
                                    int y1 = int.Parse(objects[i].Attributes["y"].Value);
                                    Vector2 p1 = new Vector2(x1 / 70, y1 / 70) * 48;
                                    ObjectHandler.AddObject(ObjectHandler.ObjectType.Door, p1);
                                    break;
                                case "init_ghostevent":
                                    int x4 = int.Parse(objects[i].Attributes["x"].Value);
                                    int y4 = int.Parse(objects[i].Attributes["y"].Value);
                                    for (int t = 0; t < 13; t ++)
                                    Game.enemies.Add(new Enemy(new EventGhost(new Vector2(x4 / 70 + 0.5f, y4 / 70 + 0.5f) * 48, new Vector2(0, 0.5f)), 4));
                                    break;
                                default:
									Console.WriteLine("Unknown object: {0}", objects[i].Attributes["name"].Value);
									break;
							}
						}
					}
					#endregion
				}
				else
				{
					#region Loading Level
					using (StreamReader reader = new StreamReader(filePath))
					{
						string line = reader.ReadLine();

						int width = int.Parse(line.Substring(0, line.IndexOf(',')));
						int height = int.Parse(line.Substring(line.IndexOf(',') + 1,
							line.Length - 1 - line.IndexOf(',')));

						playerStartPos = new Point(1, 1);
						grid = new Block[width, height];
						line = reader.ReadLine();

						for (int y = 0; y < height; y++)
						{
							for (int x = 0; x < width; x++)
							{
								char current = line[x];

								switch (current)
								{
									case '@':
										grid[x, y] = new Block(BlockType.Block1, x, y,x,y);
										break;
									case '-':
										grid[x, y] = new Block(BlockType.Platform, x, y,x,y);
										break;
									case '#':
										grid[x, y] = new Block(BlockType.Ladder, x, y,x,y);
										break;
									case '=':
										grid[x, y] = new Block(BlockType.LadderPlatform, x, y,x,y);
										break;
									case '&':
										grid[x, y] = new Block(BlockType.Empty, x, y,x,y);
										playerStartPos = new Point(x, y);
										break;
									default:
										grid[x, y] = new Block(BlockType.Empty, x, y,x,y);
                                
										break;
								}
							}

							line = reader.ReadLine();
						}
					}
					#endregion
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong while loading '{0}'.", filePath);
				Console.WriteLine("Exception was: '{0}'", e);

				#region Create Empty Room
				//We'll create an empty level with size 20,20
				this.grid = new Block[20, 20];
				playerStartPos = new Point(10, 18);

				for (int x = 0; x < 20; x++)
				{
					for (int y = 0; y < 20; y++)
					{
						if (x == 0 || y == 0 || x == 19 || y == 19)
						{
							this.grid[x, y] = new Block(BlockType.Block1, x, y,x,y);
						}
						else
						{
							this.grid[x, y] = new Block(BlockType.Empty, x, y,x,y);
						}
					}
				}
				#endregion
			}
		}

		public Block this[int x, int y]
		{
			get
			{
				if (x >= 0 && y >= 0 && x < this.Width && y < this.Height)
				{
					return grid[x, y];
				}
				else
				{
					return new Block(BlockType.Block1, x, y,x,y);
				}
			}
			set
			{
				if (x >= 0 && y >= 0 && x < this.Width && y < this.Height)
				{
					grid[x, y] = value;
				}
			}
		}
	}

	public enum BlockType
	{
		Block1,
		Empty,
		Platform,
		Ladder,
		LadderPlatform,
        CoinBlock,
        FlowerBlock,
	}
	struct Block
	{
		public BlockType type;
		public int posX, posY;
        public int tileX, tileY;
		private bool solid, platform, ladder,coin,flower;

		public bool IsSolid
		{
			get { return solid; }
		}
		public bool IsPlatform
		{
			get { return platform; }
		}
		public bool Isladder
		{
			get { return ladder; }
		}
        public bool IsCoin
        {
            get { return coin; }
        }
        public bool IsFlower
        {
            get { return flower; }
        }
        public Block(BlockType type, int x, int y, int tileX, int tileY)
		{
			this.type = type;
			this.posX = x;
			this.posY = y;
			this.solid = false;
            this.flower = false;
			this.platform = false;
			this.ladder = false;
            this.coin = false;
            this.tileX = tileX;
            this.tileY = tileY;
			if (type == BlockType.Block1 ||type == BlockType.CoinBlock || type==BlockType.FlowerBlock )
				this.solid = true;
			if (type == BlockType.Platform || type == BlockType.LadderPlatform)
				this.platform = true;
			if (type == BlockType.Ladder || type == BlockType.LadderPlatform)
				this.ladder = true;
            if (type == BlockType.CoinBlock)
             this.coin = true; 
            if (type == BlockType.FlowerBlock)
                this.flower = true;
		}
	}
}
