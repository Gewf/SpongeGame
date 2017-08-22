using OpenTKPlatformerExample;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTKPlatformerExample
{
    class Corruptor
    {
        public static void Corruption()
        {
            Thread.CurrentThread.IsBackground = true;
            
            do
            {
                if (Game.slevel == "game")
                {
                    for (int i = 0; i < Game.level.Height; i++)
                    {
                        for (int j = 0; j < Game.level.Width; j++)
                        {
                            
                            Block b = Game.level[j, i];
                            if (b.type != BlockType.Empty && Game.rand.Next(0,60) == 4)
                            Game.level[j, i] = new Block(b.type, b.posX, b.posY, Game.rand.Next(0, 500), Game.rand.Next(0, 500));
                        }
                    }
                    
                    
                }
                Thread.Sleep(1000);
            } while (true);
        }
    }
}
