using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace OpenTKPlatformerExample
{
	class ContentPipe
	{
		public static Texture2D LoadTexture(string filePath)
		{
			if (!File.Exists("Content/" + filePath))
			{
				throw new FileNotFoundException("We could not open the file at 'Content/" + filePath + "'");
			}

			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);
			Bitmap bmp = new Bitmap("Content/" + filePath);

			//Console.WriteLine("Bitmap loaded.\nW:{0}\nH:{1}", bmp.Width, bmp.Height);

			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

			bmp.UnlockBits(bmpData);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);

			return new Texture2D(id, bmp.Width, bmp.Height);

		}
	}
}
