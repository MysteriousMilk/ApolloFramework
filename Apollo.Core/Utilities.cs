// ******************************************************************
// Apollo Framework: Game Engine Framework for MonoGame
//
// MIT License
// Copyright(c) 2017 MysteriousMilk
//
// This source code file is subject to the terms and conditions defined 
// in the LICENSE file, distributed as part of this source code package.
// ******************************************************************

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Apollo.Core
{
    public static class Utilities
    {
        /// <summary>
        /// Loads a <see cref="GraphicsResource"/> from a file.
        /// </summary>
        /// <typeparam name="TResource">The type of <see cref="GraphicsResource"/> to load.</typeparam>
        /// <param name="filename">The path to the file in the filesystem.</param>
        /// <param name="device">The current <see cref="GraphicsDevice"/>.</param>
        /// <returns>The loaded <see cref="GraphicsResource"/>.</returns>
        public static TResource FromFile<TResource>(string filename, GraphicsDevice device)
            where TResource : GraphicsResource
        {
            TResource resource = default(TResource);
            Type resType = typeof(TResource);

            using (var stream = TitleContainer.OpenStream(filename))
            {
                if (resType.Equals(typeof(Texture2D)))
                    resource = (LoadTextureStream(stream, device) as TResource);
                else if (resType.Equals(typeof(Effect)))
                    resource = (LoadEffectStream(stream, device) as TResource);
            }

            return resource;
        }

        /// <summary>
        /// Loads an image from a <see cref="Stream"/>.  The image is then converted into a texture 
        /// that can be used within the framework.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> containing the image data.</param>
        /// <param name="device">The current <see cref="GraphicsDevice"/>.</param>
        /// <returns>A <see cref="Texture2D"/> from the image data contained in the <see cref="Stream"/>.</returns>
        public static Texture2D LoadTextureStream(Stream stream, GraphicsDevice device)
        {
            // load the texture from the stream
            Texture2D texture = Texture2D.FromStream(device, stream);

            // premultiply the texture data
            PremultiplyTexture(ref texture);

            return texture;
        }

        /// <summary>
        /// Loads an <see cref="Effect"/> for a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that contains the <see cref="Effect"/> data.</param>
        /// <param name="device">The current <see cref="GraphicsDevice"/>.</param>
        /// <returns></returns>
        public static Effect LoadEffectStream(Stream stream, GraphicsDevice device)
        {
            Effect effect = null;

            using (var reader = new BinaryReader(stream))
            {
                effect = new Effect(device, reader.ReadAllBytes());
            }

            return effect;
        }

        /// <summary>
        /// Reads all bytes from a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The object used to read binary.</param>
        /// <returns>Array of bytes contained within the binary reader stream.</returns>
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;

            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Premultiplies the <see cref="Texture2D"/> on the CPU so that it persists
        /// if the graphics device is lost/reset.
        /// </summary>
        /// <param name="texture">The non-premuliplied <see cref="Texture2D"/>.</param>
        public static void PremultiplyTexture(ref Texture2D texture)
        {
            Color[] buffer = new Color[texture.Width * texture.Height];
            texture.GetData(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Color.FromNonPremultiplied(
                        buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
            }
            texture.SetData(buffer);
        }
    }
}
