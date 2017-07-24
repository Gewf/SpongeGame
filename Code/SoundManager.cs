
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
            sounds.Add(new Sound(Environment.CurrentDirectory+"/Content/jump.wav"));
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
        public Sound(string soundPath)
        {
            sound.MediaEnded += MediaDone;
            path = soundPath;
            this.sound.Open(new Uri(path));
            
        }
        public void Play()
        {
            
            if (Game.slevel.Contains( "story"))
            {
                return;
            }
            if (path.Contains("coin.wav"))
            this.sound.Open(new Uri(path));
            this.sound.Volume = SoundManager.volume;
            this.sound.Play();

        }

        private void MediaDone(object sender, EventArgs e)
        {
            this.sound.Stop();
        }
    }
}
