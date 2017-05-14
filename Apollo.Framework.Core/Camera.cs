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

namespace Apollo.Framework.Core
{
    /// <summary>
    /// A 2D camera implementation.
    /// </summary>
    /// <remarks>
    /// Based on Khalid Abuhakmeh's camera code from this StackOverflow post:
    /// http://stackoverflow.com/questions/712296/xna-2d-camera-engine-that-follows-sprite
    /// </remarks>
    public class Camera
    {
        private Vector2 _position;
        private Vector2 _origin;

        /// <summary>
        /// The position of the camera in the game/world coordinate system.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }

        /// <summary>
        /// The current graphics <see cref="Viewport"/>.
        /// </summary>
        public Viewport View
        {
            get;
            set;
        }

        /// <summary>
        /// The matrix view tranform of the <see cref="Camera"/>.
        /// </summary>
        public Matrix2 ViewTransform
        {
            get;
            set;
        }

        /// <summary>
        /// The <see cref="Camera"/> rotation/orientation.
        /// </summary>
        public float Rotation
        {
            get;
            set;
        }
        
        /// <summary>
        /// The scale of the <see cref="Camera"/>.
        /// </summary>
        public float Scale
        {
            get;
            set;
        }

        /// <summary>
        /// Center of the screen in screen coordinates (pixels).
        /// </summary>
        public Vector2 ScreenCenter
        {
            get
            {
                return new Vector2(View.Width / 2, View.Height / 2);
            }
        }

        /// <summary>
        /// The boundary <see cref="Rectangle"/> of the <see cref="Camera"/>.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)(Position.X - _origin.X),
                                     (int)(Position.Y - _origin.Y),
                                     View.Width,
                                     View.Height);
            }
        }

        /// <summary>
        /// The <see cref="INode"/> game object that the <see cref="Camera"/> is focused on (snapped to).
        /// </summary>
        public INode Focus
        {
            get;
            set;
        }

        /// <summary>
        /// The movement speed (tween speed) of the <see cref="Camera"/> when it is tracking a 
        /// <see cref="IFocusable"/> object.
        /// </summary>
        public float MoveSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// Default camera constructor.
        /// </summary>
        /// <param name="view">The current graphics <see cref="Viewport"/>.</param>
        public Camera(Viewport view)
        {
            View = view;
            Scale = 1;
            MoveSpeed = 1.25f;
        }

        /// <summary>
        /// Updates the game object.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        public void Update(GameTime gameTime)
        {
            // calculate the view transform
            ViewTransform = Matrix2.CreateTranslation(-Position.X, -Position.Y) *
                            Matrix2.CreateRotationZ(-MathHelper.ToRadians(Rotation)) *
                            Matrix2.CreateScale(Scale, Scale) *
                            Matrix2.CreateTranslation(_origin.X, _origin.Y);

            _origin = ScreenCenter / Scale;

            if (Focus != null)
            {
                // delta time
                var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Move the Camera to the position that it needs to go
                _position.X += (Focus.PositionAbs.X - Position.X) * MoveSpeed * delta;
                _position.Y += (Focus.PositionAbs.Y - Position.Y) * MoveSpeed * delta;
            }
        }

        /// <summary>
        /// Determines whether the target is in view given the specified position.
        /// This can be used to increase performance by not drawing objects
        /// directly in the viewport
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///     <c>true</c> if [is in view] [the specified position]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInView(Vector2 position, Texture2D texture)
        {
            // If the object is not within the horizontal bounds of the screen
            if ((position.X + texture.Width) < (Position.X - _origin.X) || (position.X) > (Position.X + _origin.X))
                return false;

            // If the object is not within the vertical bounds of the screen
            if ((position.Y + texture.Height) < (Position.Y - _origin.Y) || (position.Y) > (Position.Y + _origin.Y))
                return false;

            // In View
            return true;
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, ViewTransform);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix2.Invert(ViewTransform));
        }
    }
}
