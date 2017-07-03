using Apollo.Framework.Core;
using Apollo.Framework.Core.Nodes;
using Apollo.Framework.Platforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace Apollo.Examples
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ExampleGame1 : Game
    {
        GraphicsDeviceManager _graphics;
        Scene _scene;
        Camera _camera;
        Sprite _moonSprite;
        Sprite _earthSprite;
        Sprite _sunSprite;

        public ExampleGame1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            SubSystem.RegisterGame(new WindowsGamePlatform(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            SubSystem.Instance.Services.GetInstance<ILogger>().AddOutputChannel(Console.Out);
            SubSystem.Instance.Services.Register<IRenderSystem>(new SceneGraphRenderSystem(GraphicsDevice));

            _camera = new Camera(GraphicsDevice.Viewport);

            _scene = new Scene(GraphicsDevice)
            {
                Camera = _camera
            };

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _moonSprite = new Sprite(Utilities.FromFile<Texture2D>(@"Data\Example1\moon.png", GraphicsDevice))
            {
                Position = new Vector2(150.0f, 0.0f),
                Scale = new Vector2(1.25f),
                Name = "Moon"
            };

            _earthSprite = new Sprite(Utilities.FromFile<Texture2D>(@"Data\Example1\exoplanet.png", GraphicsDevice))
            {
                Position = new Vector2(400.0f, 0.0f),
                Scale = new Vector2(0.5f),
                Name = "Earth"
            };
            _earthSprite.Add(_moonSprite);

            _sunSprite = new Sprite(Utilities.FromFile<Texture2D>(@"Data\Example1\sun.png", GraphicsDevice))
            {
                Name = "Sun",
            };
            _sunSprite.Add(_earthSprite);

            _scene.AddNode(_sunSprite);

            _camera.Position = new Vector2(800, 0);
            _camera.Focus = _sunSprite;
            _camera.MoveSpeed = 0.5f;

            (SubSystem.Instance.Services.GetInstance<IRenderSystem>() as SceneGraphRenderSystem).CurrentScene = _scene;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _sunSprite.Rotation += 30.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_sunSprite.Rotation > 359.0f)
                _sunSprite.Rotation -= 360.0f;

            _earthSprite.Rotation += 60.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_earthSprite.Rotation > 359.0f)
                _earthSprite.Rotation -= 360.0f;

            //_moonSprite.Rotation += 5.0f;
            //if (_moonSprite.Rotation > 359.0f)
            //    _moonSprite.Rotation -= 360.0f;

            _scene.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
