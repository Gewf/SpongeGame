using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenTKPlatformerExample
{
	class Program
	{
        static void Main(string[] args)
        {
            SoundManager.LoadSounds();
                Game window = new Game();
                window.Run();
                return;
        }
	}
}
