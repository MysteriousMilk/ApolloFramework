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
using Newtonsoft.Json;
using System;
using System.IO;

namespace Apollo.Framework.Core
{
    public sealed class SubSystem : IGameComponent, IDrawable, IUpdateable
    {
        #region Singleton
        private static readonly SubSystem _instance = new SubSystem();
        
        public static SubSystem Instance
        {
            get
            {
                if (!IsRegistered)
                    throw new Exception(typeof(SubSystem).Name + " must be initialized before use.");
                return _instance;
            }
        }
        #endregion

        #region Fields
        private GraphicsDeviceManager _Graphics;
        private bool _enabled = true;
        private bool _visible = true;
        private bool _initialized = false;
        private int _drawOrder;
        private int _updateOrder;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Returns true when the engine is initialized.
        /// </summary>
        public static bool IsRegistered
        {
            get
            {
                return _instance.Platform != null;
            }
        }

        /// <summary>
        /// The <see cref="Game"/>'s current <see cref="GraphicsDevice"/>.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return Platform.Game != null ? Platform.Game.GraphicsDevice : null;
            }
        }

        /// <summary>
        /// A reference to the main <see cref="Microsoft.Xna.Framework.Game"/> class.
        /// </summary>
        public Game Game
        {
            get
            {
                return Platform.Game;
            }
        }

        /// <summary>
        /// A reference to the current game platform.
        /// </summary>
        public IGamePlatform Platform
        {
            get;
            private set;
        }

        /// <summary>
        /// Provides a mechanism to locate services throughout the system.
        /// </summary>
        public IServiceLocator Services
        {
            get;
            private set;
        }

        public GameSettings Settings
        {
            get;
            private set;
        }

        #region IUpdateable
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region IDrawable
        public int DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                if (_drawOrder != value)
                {
                    _drawOrder = value;
                    DrawOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    VisibleChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        #endregion
        #endregion

        public SubSystem()
        {
            // create the service locator
            Services = new ServiceLocator();
        }

        public static void RegisterGame(IGamePlatform platform)
        {
            if (IsRegistered)
                return;

            _instance.Platform = platform;
            _instance.Enabled = true;
            _instance.UpdateOrder = 1;
            _instance.Game.Components.Add(_instance);

            // create logger
            string logFile = Path.Combine(_instance.Platform.LogDirectory, "CurrentLog.txt");
            _instance.Services.Register<ILogger>(new LoggingService(_instance.Platform, _instance.Platform.OpenWriteStream(logFile)));

            _instance.InitializeSubSystem();
        }

        public void Initialize()
        {

        }

        internal void InitializeSubSystem()
        {
            if (_initialized)
                return;

            if (!IsRegistered)
                throw new InvalidOperationException("Game must be registered with the " + GetType().Name + " before initialization.");

            ILogger logger = null;
            if (Services.HasInstance<ILogger>())
                logger = Services.GetInstance<ILogger>();

            string settingsPath = Path.Combine(new string[]
            {
                Platform.SettingsDirectory,
                Platform.SettingsFileName
            });

            if (Platform.DoesFileExist(settingsPath))
            {
                string settingsFileContents = string.Empty;

                using (StreamReader reader = new StreamReader(Platform.OpenReadStream(settingsPath)))
                {
                    settingsFileContents = reader.ReadToEnd();
                }

                Settings = JsonConvert.DeserializeObject<GameSettings>(settingsFileContents);
            }
            else
            {
                Settings = GameSettings.CreateDefaultSettings();
                Platform.WriteToFile(Platform.SettingsDirectory, Platform.SettingsFileName, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }

            if (logger != null)
                logger.WriteLine(LogEntryType.Info, "Settings Loaded.");

            _Graphics = Game.Services.GetService<IGraphicsDeviceManager>() as GraphicsDeviceManager;
            if (_Graphics == null)
                _Graphics = new GraphicsDeviceManager(Game);

            _Graphics.PreferredBackBufferWidth = Convert.ToInt32(Settings.GetValue("ResolutionX"));
            _Graphics.PreferredBackBufferHeight = Convert.ToInt32(Settings.GetValue("ResolutionY"));
            _Graphics.IsFullScreen = Convert.ToBoolean(Settings.GetValue("IsFullScreen"));
            _Graphics.ApplyChanges();

            if (logger != null)
                logger.WriteLine(LogEntryType.Info, "Settings Applied.");

            if (logger != null)
                logger.WriteLine(LogEntryType.Info, GetType().Name + " initialized.");

            _initialized = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!_initialized)
            {
                if (Services.HasInstance<ILogger>())
                    Services.GetInstance<ILogger>().WriteLine(LogEntryType.Warning, string.Format("Update call skipped. {0} has not yet been initialized.", GetType().Name));
                return;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (!_initialized)
            {
                if (Services.HasInstance<ILogger>())
                    Services.GetInstance<ILogger>().WriteLine(LogEntryType.Warning, string.Format("Draw call skipped. {0} has not yet been initialized.", GetType().Name));
                return;
            }

            IRenderSystem renderer = Services.GetInstance<IRenderSystem>();

            if (renderer == null)
            {
                if (Services.HasInstance<ILogger>())
                    Services.GetInstance<ILogger>().WriteLine(LogEntryType.Warning, "No rendering service has been registered.");
            }

            renderer.PreProcess();
            renderer.Render();
            renderer.PostProcess();
        }
    }
}
