using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ReplayViewer
{
    public class Texture2D : IDisposable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private int textureId = 0;
        public Texture2D(Stream stream)
        {
            using (var bitmap = new Bitmap(stream))
            {
                this.Width = bitmap.Width;
                this.Height = bitmap.Height;
                this.textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, this.textureId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);
            }
            stream.Dispose();
        }

        public void Draw(Vector2 pos, Vector2 origin, Color4 color)
        {
            pos -= origin;
            GL.Color4(color);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.BindTexture(TextureTarget.Texture2D, this.textureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(pos.X, pos.Y);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(pos.X + this.Width, pos.Y);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(pos.X + this.Width, pos.Y + this.Height);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(pos.X, pos.Y + this.Height);
            GL.End();
        }
        public void Draw(Vector2 pos, float w, float h, Vector2 origin, Color4 color)
        {
            pos -= origin;
            GL.Color4(color);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.BindTexture(TextureTarget.Texture2D, this.textureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(pos.X, pos.Y);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(pos.X + w, pos.Y);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(pos.X + w, pos.Y + h);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(pos.X, pos.Y + h);
            GL.End();
        }
        public void Draw(Vector2 pos, Vector2 origin, Color4 color, float rotation, float scale)
        {
            pos -= origin;
            GL.Color4(color);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            if (rotation != 0 || scale != 0)
            {
                Vector3 diff = new Vector3(-pos.X - origin.X, -pos.Y - origin.Y, 0.0f);
                GL.Translate(-diff);
                GL.Rotate(MathHelper.RadiansToDegrees(rotation), 0.0f, 0.0f, 1.0f);
                GL.Scale(scale, scale, 1.0f);
                GL.Translate(diff);
            }
            GL.BindTexture(TextureTarget.Texture2D, this.textureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(pos.X, pos.Y);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(pos.X + this.Width, pos.Y);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(pos.X + this.Width, pos.Y + this.Height);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(pos.X, pos.Y + this.Height);
            GL.End();
        }
        public void Draw(Vector2 pos, Vector2 origin, Color4 color, Rectangle source, float rotation, float scale)
        {
            pos -= origin;
            Vector2 texCoordMin = new Vector2(source.X / (float)this.Width, source.Y / (float)this.Height);
            Vector2 texCoordMax = new Vector2((source.X + source.Width) / (float)this.Width, (source.Y + source.Height) / (float)this.Height);
            GL.Color4(color);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            if (rotation != 0 || scale != 0)
            {
                Vector3 diff = new Vector3(-pos.X - origin.X, -pos.Y - origin.Y, 0.0f);
                GL.Translate(-diff);
                GL.Rotate(MathHelper.RadiansToDegrees(rotation), 0.0f, 0.0f, 1.0f);
                GL.Scale(scale, scale, 1.0f);
                GL.Translate(diff);
            }
            GL.BindTexture(TextureTarget.Texture2D, this.textureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(texCoordMin.X, texCoordMin.Y);
            GL.Vertex2(pos.X, pos.Y);
            GL.TexCoord2(texCoordMax.X, texCoordMin.Y);
            GL.Vertex2(pos.X + source.Width, pos.Y);
            GL.TexCoord2(texCoordMax.X, texCoordMax.Y);
            GL.Vertex2(pos.X + source.Width, pos.Y + source.Height);
            GL.TexCoord2(texCoordMin.X, texCoordMax.Y);
            GL.Vertex2(pos.X, pos.Y + source.Height);
            GL.End();
        }
        public void Dispose()
        {
            GL.DeleteTexture(this.textureId);
        }
    }
}
