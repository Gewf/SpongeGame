using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKPlatformerExample
{
    class DialogBox
    {
        public static int meter = 0;
        public static string message = "";
        public static Texture2D charset;
        public static Color cd = Color.White;
        public static void Load()
        {
            charset = ContentPipe.LoadTexture("charset.png");
        }
        public static void DrawChar(char c, int pos, float x, float y)
        {

            int charid = Convert.ToInt32(c);
            int xx = (charid % 16) * 16 + 1;
            int yx = ((int)Math.Floor((double)(charid / 16))) * 16 + 1;
            RectangleF sourceRec = new RectangleF(xx, yx, 15, 15);
            Spritebatch.DrawSprite(charset, new RectangleF(x + pos, y, 32, 32), cd, sourceRec);

        }
        public static void DrawString(string s, float x, float y)
        {
            int j = 0;
            for (int i = 0; i < s.Length; i++)
            {
                int line = (int)Math.Floor((double)i / 18);
                j += 32;
                if (i % 18 == 0)
                {
                    j = 0;
                }
                DrawChar(s[i], j, x, y + line * 32);
            }
        }
        public static bool UpdateString()
        {
            if (message != "")
            {
                
                if (meter == message.Length)
                {
                    return true;
                }else
                {
                    meter++;
                }
            }
            return false;
        }
        public static void DrawString(float x, float y)
        {
            int j = 0;
            for (int i = 0; i < meter; i++)
            {
                int xline = (int)Math.Floor((double)i / 18);
                j += 32;
                if (i % 18 == 0)
                {
                    j = 0;
                }
                DrawChar(message[i], j, x, y + xline * 32);
            }
        }
        public static void LoadString(string s)
        {
            meter = 0;
            message = s;
        }
    }
}
