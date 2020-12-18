using System;
using System.Collections.Generic;
using OpenTK;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.ColorSpaces;
using OpenTK.Graphics.OpenGL;


namespace Project1
{
    public class Texture
    {
        int Handle;
        public Texture(string path)
        {
            Handle = GL.GenTexture();
            Use();

            Image<Rgba32> image = Image.Load<Rgba32>(path); 
            

            image.Mutate(x => x.Flip(FlipMode.Vertical));

            if (image.TryGetSinglePixelSpan(out var pixelSpan))
            {
                Rgba32[] tempPixels = pixelSpan.ToArray();
                List<byte> pixels = new List<byte>();

                foreach (Rgba32 p in tempPixels)
                {
                    pixels.Add(p.R);
                    pixels.Add(p.G);
                    pixels.Add(p.B);
                    pixels.Add(p.A);
                }
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


                float maxAniso;
                GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
                GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

              

            } 
        }
        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {

            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);

        }

    }
}
