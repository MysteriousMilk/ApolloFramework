using Apollo.Core;
using Apollo.Core.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            _camera = new Camera(GraphicsDevice.Viewport);

            _scene = new Scene(GraphicsDevice);
            _scene.Camera = _camera;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _moonSprite = new Sprite(Utilities.FromFile<Texture2D>(@"Data\Example1\moon.png", GraphicsDevice));
            _moonSprite.Position = new Vector2(100.0f, 0.0f);
            _moonSprite.Scale = new Vector2(1.25f);
            _moonSprite.Name = "Moon";

            _earthSprite = new Sprite(Utilities.FromFile<Texture2D>(@"Data\Example1\exoplanet.png", GraphicsDevice));
            _earthSprite.Position = new Vector2(400.0f, 0.0f);
            _earthSprite.Scale = new Vector2(0.5f);
            _earthSprite.Name = "Earth";
            _earthSprite.Add(_moonSprite);

            _sunSprite = new Sprite(Utilities.FromFile<Texture2D>(@"Data\Example1\sun.png", GraphicsDevice));
            _sunSprite.Name = "Sun";
            _sunSprite.Add(_earthSprite);

            _scene.AddNode(_sunSprite);

            _camera.Position = new Vector2(800, 0);
            _camera.Focus = _sunSprite;
            _camera.MoveSpeed = 0.5f;
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

            _sunSprite.Rotation += 0.1f;
            if (_sunSprite.Rotation > 359.0f)
                _sunSprite.Rotation -= 360.0f;

            _earthSprite.Rotation += 0.5f;
            if (_earthSprite.Rotation > 359.0f)
                _earthSprite.Rotation -= 360.0f;

            _moonSprite.Rotation += 5.0f;
            if (_moonSprite.Rotation > 359.0f)
                _moonSprite.Rotation -= 360.0f;

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

            _scene.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
