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

namespace Apollo.Core.Nodes
{
    public class Sprite : Node
    {
        public Texture2D Texture
        {
            get;
            set;
        }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,
                WorldTransform.Translation,
                Texture.Bounds,
                Color.White,
                WorldTransform.Rotation,
                Origin,
                WorldTransform.Scale,
                SpriteEffects.None,
                0
                );

            base.Draw(spriteBatch);
        }
    }
}
