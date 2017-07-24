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
            Console.WriteLine("1.Play");
            Console.WriteLine("2.Options");
            Console.Write("Your choice:");
            int j = 0;
            try
            {
                j = int.Parse(Console.ReadLine());
            }
            catch { Console.WriteLine("Bad input. Write either 1 or 2"); Main(new string[] { }); }
            if (j == 1)
            {
                Game window = new Game();
                window.Run();
                return;
            }else if (j == 2)
            {
                new Settings().ShowDialog();
                Game window = new Game();
                window.Run();
                return;
            }
            else
            {
                Console.WriteLine("Bad input. Write either 1 or 2"); Main(new string[] { });
            }
          
        }
	}
}
