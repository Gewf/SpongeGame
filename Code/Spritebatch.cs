using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace OpenTKPlatformerExample
{
	class Spritebatch
	{
		public static void DrawSprite(Texture2D texture, RectangleF rectangle)
		{
			DrawSprite(texture, new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.Width / texture.Width, rectangle.Height / texture.Height), Color.White, Vector2.Zero);
		}
		public static void DrawSprite(Texture2D texture, RectangleF rectangle, Color color, RectangleF? sourceRec = null)
		{
			DrawSprite(texture, new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.Width / texture.Width, rectangle.Height / texture.Height), color, Vector2.Zero, sourceRec);
		}
		public static void DrawSprite(Texture2D texture, Vector2 position)
		{
			DrawSprite(texture, position, Vector2.One, Color.White, Vector2.Zero);
		}
		public static void DrawSprite(Texture2D texture, Vector2 position, Vector2 scale)
		{
			DrawSprite(texture, position, scale, Color.White, Vector2.Zero);
		}
		public static void DrawSprite(Texture2D texture, Vector2 position, Vector2 scale, Color color)
		{
			DrawSprite(texture, position, scale, color, Vector2.Zero);
		}
		public static void DrawSprite(Texture2D texture, Vector2 position, Vector2 scale, Color color, Vector2 origin, RectangleF? sourceRec = null)
		{
			Vector2[] verts = new Vector2[4] 
			{
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1),
			};

			GL.BindTexture(TextureTarget.Texture2D, texture.ID);

			GL.Begin(PrimitiveType.Quads);
			for (int i = 0; i < 4; i++)
			{
				GL.Color4(color);
				if (sourceRec == null)
				{
					GL.TexCoord2(verts[i].X, verts[i].Y);
				}
				else
				{
					GL.TexCoord2(
						(sourceRec.Value.X + verts[i].X * sourceRec.Value.Width) / (float)texture.Width,
						(sourceRec.Value.Y + verts[i].Y * sourceRec.Value.Height) / (float)texture.Height);
				}

				verts[i].X *= texture.Width;
				verts[i].Y *= texture.Height;
				verts[i] -= origin;
				verts[i] *= scale;
				verts[i] += position;

				GL.Vertex2(verts[i]);
			}
			GL.End();
		}

		public static void Begin(GameWindow window)
		{
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(-window.ClientSize.Width / 2f, window.ClientSize.Width / 2f, window.ClientSize.Height / 2f, -window.ClientSize.Height / 2f, 0, 1);
		}

        internal static void DrawSprite(object spriteClimb, RectangleF rec)
        {
            throw new NotImplementedException();
        }
    }
}
