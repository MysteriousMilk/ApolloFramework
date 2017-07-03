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

namespace Apollo.Framework.Core.Nodes
{
    /// <summary>
    /// Represents a <see cref="INode"/> that has a texture/image applied to it.
    /// </summary>
    public class Sprite : Node, IRenderable
    {
        /// <summary>
        /// The texture to be applied to the <see cref="IRenderable"/>.
        /// </summary>
        public Texture2D Texture
        {
            get;
            set;
        }

        /// <summary>
        /// The tint of the <see cref="IRenderable"/> object.
        /// </summary>
        public Color Tint
        {
            get;
            set;
        }

        /// <summary>
        /// The source rectangle of the <see cref="IRenderable"/> object used for drawing.
        /// </summary>
        public Rectangle SourceRectangle
        {
            get
            {
                return Texture != null ? Texture.Bounds : Rectangle.Empty;
            }
        }

        /// <summary>
        /// The graphical <see cref="Microsoft.Xna.Framework.Graphics.BlendState"/> to apply to the node and it's children.
        /// </summary>
        public BlendState BlendState
        {
            get;
            set;
        }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Tint = Color.White;
            BlendState = BlendState.AlphaBlend;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }
    }
}
