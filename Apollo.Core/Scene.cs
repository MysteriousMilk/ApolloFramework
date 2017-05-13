// ******************************************************************
// Apollo Framework: Game Engine Framework for MonoGame
//
// MIT License
// Copyright(c) 2017 MysteriousMilk
//
// This source code file is subject to the terms and conditions defined 
// in the LICENSE file, distributed as part of this source code package.
// ******************************************************************

using Apollo.Core.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Apollo.Core
{
    public class Scene : IScene
    {
        private Dictionary<Guid, INode> _NodeIdLookup;
        private Dictionary<string, INode> _NodeTagLookup;
        private SpriteBatch _spriteBatch;
        private INode _Root;

        /// <summary>
        /// The root of the scene graph.
        /// </summary>
        public INode Root
        {
            get
            {
                return _Root;
            }
        }

        /// <summary>
        /// The <see cref="Scene"/> object's camera.
        /// </summary>
        public Camera Camera
        {
            get;
            set;
        }
        
        /// <summary>
        /// Default constructor.  Intializes the scene to it's defaults.
        /// </summary>
        /// <param name="graphics">The current graphics device.</param>
        public Scene(GraphicsDevice graphics)
        {
            // initialize sprite batch
            _spriteBatch = new SpriteBatch(graphics);

            // instantiate the root of the scene graph
            _Root = new Node();

            // create lookup tables
            _NodeIdLookup = new Dictionary<Guid, INode>();
            _NodeTagLookup = new Dictionary<string, INode>();
        }

        /// <summary>
        /// Adds a <see cref="INode"/> to the root of the scene.
        /// </summary>
        /// <param name="node">The node item to add.</param>
        public void AddNode(INode node)
        {
            if (_NodeIdLookup.Keys.Contains(node.Id))
                throw new ArgumentException("Cannot add the same node more than once!", "node");

            if (_NodeTagLookup.Keys.Contains(node.Tag))
                throw new ArgumentException("Cannot add node with duplicate tag!", "node");

            // add node to the root of the scene graph
            Root.Add(node);

            // add quick lookup entries for the node and it's children
            AddNodeToLookup(node);
        }

        /// <summary>
        /// Ands one <see cref="INode"/> as a child of another <see cref="INode"/>.
        /// </summary>
        /// <param name="node">The <see cref="INode"/> to add.</param>
        /// <param name="parent">The <see cref="INode"/> to attach the given child <see cref="INode"/> to.</param>
        /// <remarks>
        /// If the parent <see cref="INode"/> is already in the scene graph, the <see cref="INode"/> being attached will
        /// have itself and its children added to the lookup tables.
        /// </remarks>
        public void AddNode(INode node, INode parent)
        {
            parent.Add(node);

            if (_NodeIdLookup.ContainsKey(parent.Id))
                AddNodeToLookup(node);
        }

        /// <summary>
        /// Adds a <see cref="INode"/> to the parent <see cref="INode"/> with the given
        /// <see cref="Guid"/>.
        /// </summary>
        /// <param name="node">The <see cref="INode"/> to add.</param>
        /// <param name="parentId">The <see cref="Guid"/> of the parent <see cref="INode"/> 
        /// within the scene graph to add the give <see cref="INode"/> object to.</param>
        public void AddNode(INode node, Guid parentId)
        {
            INode parent = null;

            if (_NodeIdLookup.TryGetValue(parentId, out parent))
                AddNode(node, parent);
            else
                throw new ArgumentException("The given parent could not be found.", "parentId");
        }

        /// <summary>
        /// Adds a <see cref="INode"/> to the parent <see cref="INode"/> with the given tag.
        /// </summary>
        /// <param name="node">The <see cref="INode"/> to add.</param>
        /// <param name="parentId">The tag of the parent <see cref="INode"/> within the scene graph
        /// to add the give <see cref="INode"/> object to.</param>
        public void AddNode(INode node, string parentTag)
        {
            INode parent = null;

            if (string.IsNullOrEmpty(parent.Tag))
                throw new ArgumentException("Invalid parent tag.", "parentTag");

            if (_NodeTagLookup.TryGetValue(parentTag, out parent))
                AddNode(node, parent);
            else
                throw new ArgumentException("The given parent could not be found.", "parentTag");
        }

        /// <summary>
        /// This method is called every cycle used to update the state of the <see cref="Scene"/>.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            // update the scene graph
            Root.Update(gameTime);

            // update the camera
            Camera.Update(gameTime);
        }

        /// <summary>
        /// This method is called every cycle to draw the <see cref="Scene"/> to the screen.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(GameTime gameTime)
        {
            // make sure we have a camera
            if (Camera == null)
                throw new NullReferenceException("Scene camera must be set before calling Draw().");

            // Gets the scene graph in list for so we can iterate over it and do the draw calls.
            // This is done with linear list so that the sprite batch can be started and stopped
            // based on the node's blend mode.
            List<INode> drawList = GetNodeList(Root);

            // begin the sprite batch with deferred rendering, default blend state, using the camera's transform view matrix
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.ViewTransform);

            // Draw each node.
            foreach (INode n in drawList)
            {
                BlendState currBlendState = _spriteBatch.GraphicsDevice.BlendState;

                // change the blend state if necessary
                if (n.BlendState != currBlendState)
                {
                    _spriteBatch.End();

                    _spriteBatch.Begin(SpriteSortMode.Deferred, n.BlendState, null, null, null, null, Camera.ViewTransform);
                    n.Draw(_spriteBatch);
                }
                else
                    n.Draw(_spriteBatch);
            }

            _spriteBatch.End();
        }

        private List<INode> GetNodeList(INode node)
        {
            List<INode> nodeList = new List<INode>();

            foreach (INode child in node.Children)
                nodeList.AddRange(GetNodeList(child));

            nodeList.Add(node);

            return nodeList;
        }

        private void AddNodeToLookup(INode node)
        {
            foreach (INode child in node.Children)
                AddNodeToLookup(child);

            if (_NodeIdLookup.ContainsKey(node.Id))
            {
                Debug.WriteLine("Node exists in scene graph already and will not be added to the lookup table.");
            }
            else
            {
                _NodeIdLookup.Add(node.Id, node);

                if (_NodeTagLookup.ContainsKey(node.Tag))
                    Debug.WriteLine("Node with identical tag already exists in lookup table and will not be added twice.");
                else
                    _NodeTagLookup.Add(node.Tag, node);
            }
        }

        private void RemoveNodeFromLookup(INode node)
        {
            foreach (INode child in node.Children)
                RemoveNodeFromLookup(node);

            _NodeIdLookup.Remove(node.Id);
            _NodeTagLookup.Remove(node.Tag);
        }
    }
}
