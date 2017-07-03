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
using System.Collections.Generic;

namespace Apollo.Framework.Core
{
    public class SceneGraphRenderSystem : IRenderSystem
    {
        /// <summary>
        /// The game's current <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/>.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get;
            private set;
        }

        /// <summary>
        /// The <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> used for drawing
        /// <see cref="IRenderable"/> objects to the screen.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get;
            private set;
        }

        /// <summary>
        /// Color to clear the screen with before each draw cycle.
        /// </summary>
        public Color ClearColor
        {
            get;
            set;
        }

        /// <summary>
        /// Reference to the scene to render.
        /// </summary>
        public Scene CurrentScene
        {
            get;
            set;
        }

        public SceneGraphRenderSystem(GraphicsDevice graphics)
        {
            GraphicsDevice = graphics;
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            ClearColor = Color.Black;
        }

        /// <summary>
        /// Actions to be performed before the current draw cycle.
        /// </summary>
        public void PreProcess()
        {
            
        }

        /// <summary>
        /// Draws objects to the screen.
        /// </summary>
        public void Render()
        {
            GraphicsDevice.Clear(ClearColor);

            if (CurrentScene == null)
                return;

            // make sure we have a camera
            if (CurrentScene.Camera == null)
                return;

            // Gets the scene graph in list for so we can iterate over it and do the draw calls.
            // This is done with linear list so that the sprite batch can be started and stopped
            // based on the node's blend mode.
            List<IRenderable> renderList = GetRenderList(CurrentScene.Root);

            // begin the sprite batch with deferred rendering, default blend state, using the camera's transform view matrix
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, CurrentScene.Camera.ViewTransform);

            // Draw each node.
            foreach (IRenderable renderable in renderList)
            {
                BlendState currBlendState = GraphicsDevice.BlendState;

                // change the blend state if necessary
                if (renderable.BlendState != currBlendState)
                {
                    SpriteBatch.End();

                    SpriteBatch.Begin(SpriteSortMode.Deferred, renderable.BlendState, null, null, null, null, CurrentScene.Camera.ViewTransform);
                    Render(renderable);
                }
                else
                    Render(renderable);
            }

            SpriteBatch.End();
        }

        /// <summary>
        /// Actions to be performed after the current draw cycle.
        /// </summary>
        public void PostProcess()
        {
            
        }

        private void Render(IRenderable renderable)
        {
            SpriteBatch.Draw(
                renderable.Texture,
                renderable.WorldTransform.Translation,
                renderable.Texture.Bounds,
                renderable.Tint,
                -renderable.WorldTransform.Rotation,
                renderable.Origin,
                renderable.WorldTransform.Scale,
                SpriteEffects.None,
                0
                );
        }

        private List<IRenderable> GetRenderList(INode node)
        {
            List<IRenderable> nodeList = new List<IRenderable>();

            foreach (INode child in node.Children)
                nodeList.AddRange(GetRenderList(child));

            if (node is IRenderable)
                nodeList.Add((IRenderable)node);

            return nodeList;
        }
    }
}
