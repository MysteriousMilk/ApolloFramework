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
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Nodes
{
    /// <summary>
    /// The base class for all game objects in the Apollo Framework.
    /// </summary>
    public class Node : INode
    {
        private List<INode> _children;
        private int _zorder;

        /// <summary>
        /// Globally unique identifier for the <see cref="Node"/>.
        /// </summary>
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// The name delegated to the <see cref="Node"/> by the Apollo Framework.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// User defined tag given to the <see cref="Node"/> by the user.
        /// </summary>
        public string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// The origin point of the <see cref="Node"/>.
        /// </summary>
        public Vector2 Origin
        {
            get;
            set;
        }

        /// <summary>
        /// Position of the <see cref="Node"/> relative to it's parent <see cref="Node"/>.
        /// </summary>
        public Vector2 Position
        {
            get;
            set;
        }

        /// <summary>
        /// Absolute position of the <see cref="Node"/> within the game world.
        /// </summary>
        public Vector2 PositionAbs
        {
            get
            {
                return WorldTransform.Translation;
            }
        }

        /// <summary>
        /// The scale factor to be applied to the <see cref="Node"/>.
        /// </summary>
        public Vector2 Scale
        {
            get;
            set;
        }

        /// <summary>
        /// The rotation to be applied to the <see cref="Node"/>, specified in degrees.
        /// </summary>
        public float Rotation
        {
            get;
            set;
        }

        /// <summary>
        /// Used for sorting <see cref="Node"/> objects on screen.
        /// </summary>
        /// <remarks>
        /// Items are sorted on screen first by their order in the scene graph and then by ZOrder.
        /// </remarks>
        public int ZOrder
        {
            get { return _zorder; }
            set
            {
                if (value != _zorder)
                {
                    _zorder = value;

                    if (Parent != null && Parent is Node)
                        (Parent as Node).SortByZOrder();
                }
            }
        }

        /// <summary>
        /// The transform matrix of the <see cref="Node"/> in local space.
        /// </summary>
        public Matrix2D LocalTransform
        {
            get;
            internal set;
        }

        /// <summary>
        /// The transform matrix of the <see cref="Node"/> in world space.
        /// </summary>
        public Matrix2D WorldTransform
        {
            get;
            internal set;
        }

        /// <summary>
        /// The graphical <see cref="Microsoft.Xna.Framework.Graphics.BlendState"/> to apply to the node and it's children.
        /// </summary>
        public BlendState BlendState
        {
            get;
            set;
        }

        /// <summary>
        /// The parent <see cref="INode"/>.
        /// All nodes have a parent unless it is the root <see cref="INode"/>.
        /// </summary>
        public INode Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Collection of child nodes.
        /// </summary>
        public IEnumerable<INode> Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Collection of custom properties.
        /// </summary>
        public PropertyCollection CustomProperties
        {
            get;
            internal set;
        }

        public Node()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Tag = "Node_" + Convert.ToBase64String(Id.ToByteArray());
            _children = new List<INode>();
            CustomProperties = new PropertyCollection();

            Position = new Vector2();
            Scale = new Vector2(1.0f, 1.0f);
            Rotation = 0.0f;
            _zorder = 0;

            LocalTransform = Matrix2D.Identity;
            WorldTransform = Matrix2D.Identity;
            BlendState = BlendState.AlphaBlend;

            Parent = null;
        }

        /// <summary>
        /// Adds a new child <see cref="INode"/> to this <see cref="INode"/>, setting
        /// this <see cref="INode"/> as the parent.
        /// </summary>
        /// <param name="child">The <see cref="INode"/> to add.</param>
        public void Add(INode child)
        {
            child.Parent = this;
            _children.Add(child);

            // sort by ZOrder
            SortByZOrder();
        }

        /// <summary>
        /// Gets the location of the node in world coordinates.
        /// </summary>
        /// <returns>The position of the <see cref="Node"/> in world coordinates.</returns>
        public Vector2 GetWorldPosition()
        {
            return WorldTransform.Translation;
        }

        /// <summary>
        /// Removes a child <see cref="INode"/> from the current parent <see cref="INode"/>.
        /// </summary>
        /// <param name="child">The child <see cref="INode"/> to remove.</param>
        public void Remove(INode child)
        {
            if (_children.Remove(child))
                child.Parent = null;
        }

        /// <summary>
        /// Removes all child <see cref="INode"/> instances that match the given <see cref="Predicate{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> condition used to determine which
        /// child <see cref="INode"/> objects to remove.</param>
        public void RemoveAll(Predicate<INode> match)
        {
            foreach (INode child in _children)
            {
                if (match.Invoke(child))
                    Remove(child);
            }
        }

        /// <summary>
        /// Removes all child <see cref="INode"/> objects.
        /// </summary>
        public void Clear()
        {
            foreach (Node child in _children)
            {
                Remove(child);
            }
        }

        /// <summary>
        /// This method is called every cycle used to update the state of the <see cref="Node"/>.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public virtual void Update(GameTime gameTime)
        {
            // compute world transform for this node
            if (Parent != null)
            {
                // compute local transform for this node
                LocalTransform = Matrix2D.CreateTranslation(-Parent.Position) *
                                 Matrix2D.CreateScale(Scale) *
                                 Matrix2D.CreateRotationZ(MathHelper.ToRadians(-Rotation)) *
                                 Matrix2D.CreateTranslation(Parent.Position) *
                                 Matrix2D.CreateTranslation(Position);

                // compute world transform
                WorldTransform = Parent.WorldTransform * LocalTransform;
            }
            else
            {
                // compute local transform
                LocalTransform = Matrix2D.CreateScale(Scale) *
                                 Matrix2D.CreateRotationZ(MathHelper.ToRadians(-Rotation)) *
                                 Matrix2D.CreateTranslation(Position);

                // no parent so WorldTransform and LocalTransform are equivalent.
                WorldTransform = LocalTransform;
            }

            // update children
            foreach (Node child in Children)
                child.Update(gameTime);
        }

        /// <summary>
        /// This method is called every cycle to draw the <see cref="Node"/> to the screen.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> used for drawing.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // draw each child
            foreach (Node child in Children)
                child.Draw(spriteBatch);
        }

        /// <summary>
        /// Creates a new copy of the <see cref="Node"/> and all of its children.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The cloned <see cref="Node"/> will have a null parent until added as a 
        /// child to another <see cref="Node"/>.
        /// The cloned <see cref="Node"/> will also be assigned a new identifier.
        /// </remarks>
        public virtual INode Clone()
        {
            Node clone = MemberwiseClone() as Node;

            // assign new id
            Id = Guid.NewGuid();

            // remove parent link
            clone.Parent = null;

            // clone child nodes
            clone.ClearChildList();
            foreach (INode node in Children)
                clone.Add(node.Clone());

            // clone custom properties
            clone.CustomProperties = new PropertyCollection();
            foreach (var property in CustomProperties)
                clone.CustomProperties.SetProperty(property.Key, property.Value);

            return clone;
        }

        /// <summary>
        /// Clears the child list without removing the parent link
        /// (used for cloning).
        /// </summary>
        internal void ClearChildList()
        {
            _children.Clear();
        }

        /// <summary>
        /// Sorts children by ZOrder.
        /// </summary>
        internal void SortByZOrder()
        {
            List<INode> sortedList = _children.OrderBy(n => n.ZOrder).ToList();
            _children.Clear();
            _children.AddRange(sortedList);
        }
    }
}
