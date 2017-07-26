
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

namespace OpenTKPlatformerExample
{
    class SoundManager
    {
        public static List<Sound> sounds = new List<Sound>();
        public static float volume = -1;

        public static void LoadSounds()
        {
            //Index 0 = Jump
            //Index 1 = HitBlock
            //Index 2 = Coin
            //Index 3 = Powerup
            //Index 4 = PowerupRise
            //Index 5 = EnemySquash
            //Index 6 = Song for menu
            sounds.Add(new Sound(Environment.CurrentDirectory + "/Content/jump.wav"));
            sounds.Add(new Sound(Environment.CurrentDirectory + "/Content/hitblock.wav"));
            sounds.Add(new Sound(Environment.CurrentDirectory + "/Content/coin.wav"));
            sounds.Add(new Sound(Environment.CurrentDirectory + "/Content/powerup.wav"));
            sounds.Add(new Sound(Environment.CurrentDirectory + "/Content/rise.wav"));
            sounds.Add(new Sound(Environment.CurrentDirectory + "/Content/squash.wav"));

        }
    }
    class Sound
    {
        private MediaPlayer sound = new MediaPlayer();
        string path;
        bool loop = false;
        public Sound(string soundPath, bool loop = false)
        {
            sound.MediaEnded += MediaDone;
            path = soundPath;
            this.loop = loop;
        }
        public void Play()
        {
          
            this.sound.Open(new Uri(path));
            this.sound.Volume = SoundManager.volume;
            if (loop)
            {
                this.sound.Volume = 1f;
               
            }
            this.sound.Play();

        }
        public void Abort()
        {
            this.sound.Stop();
        }
        private void MediaDone(object sender, EventArgs e)
        {
            
            this.sound.Stop();
            if (loop)
                this.sound.Play();


        }
    }
}
