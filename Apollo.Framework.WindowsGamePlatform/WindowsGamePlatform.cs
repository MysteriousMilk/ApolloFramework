// ******************************************************************
// Apollo Framework: Game Engine Framework for MonoGame
//
// MIT License
// Copyright(c) 2017 MysteriousMilk
//
// This source code file is subject to the terms and conditions defined 
// in the LICENSE file, distributed as part of this source code package.
// ******************************************************************

using Apollo.Framework.Core;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Apollo.Framework.Platforms
{
    public class WindowsGamePlatform : IGamePlatform
    {
        public bool IsDebug
        {
            get
            {
                return CheckForDebug();
            }
        }

        public string SettingsDirectory
        {
            get;
            set;
        }

        public string LogDirectory
        {
            get;
            set;
        }

        public string SettingsFileName
        {
            get;
            set;
        }

        public Game Game
        {
            get;
            private set;
        }

        public WindowsGamePlatform(Game game)
        {
            Game = game;
            SettingsFileName = "settings.json";
            SettingsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings").ToString();
            LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs").ToString();

            if (!Directory.Exists(SettingsDirectory))
                Directory.CreateDirectory(SettingsDirectory);

            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
        }

        public WindowsGamePlatform(Game game, string settingsDir, string logDir)
        {
            Game = game;
            SettingsFileName = "settings.json";
            SettingsDirectory = settingsDir;
            LogDirectory = logDir;

            if (!Directory.Exists(SettingsDirectory))
                Directory.CreateDirectory(SettingsDirectory);

            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
        }

        /// <summary>
        /// Opens a file <see cref="Stream"/>. for the given file path.
        /// </summary>
        /// <param name="path">The path to the file to open the <see cref="Stream"/>.</param>
        /// <returns>A <see cref="Stream"/> to read the file contents from.</returns>
        /// <remarks>
        /// This method will just return a stream.  The stream will still need to be closed/disposed
        /// when it is done being used.
        /// </remarks>
        public Stream OpenReadStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Creates a file <see cref="Stream"/>. for the given file path.
        /// </summary>
        /// <param name="path">The path to the file to open/create the <see cref="Stream"/>.</param>
        /// <returns>A <see cref="Stream"/> to write the data contents to.</returns>
        /// <remarks>
        /// This method will just return a stream.  The stream will still need to be closed/disposed
        /// when it is done being used.
        /// </remarks>
        public Stream OpenWriteStream(string path)
        {
            return new FileStream(path, FileMode.Create, FileAccess.Write);
        }

        /// <summary>
        /// Writes all the given text to a file.
        /// </summary>
        /// <param name="directory">Path of the directory to store the file in.</param>
        /// <param name="filename">Name of the file to write.</param>
        /// <param name="text">Text to write to the file.</param>
        public void WriteToFile(string directory, string filename, string text)
        {
            File.WriteAllText(Path.Combine(directory, filename), text);
        }

        /// <summary>
        /// Checks to see if a file exists on the file system for the
        /// <see cref="IGamePlatform"/>.
        /// </summary>
        /// <param name="filename">The full path of the file.</param>
        /// <returns>True if the file exists and False if it does not.</returns>
        public bool DoesFileExist(string filename)
        {
            return File.Exists(filename);
        }

        private bool CheckForDebug()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(DebuggableAttribute), true);
            if (attributes == null || attributes.Length == 0)
                return true;

            var d = (DebuggableAttribute)attributes[0];
            if (d.IsJITTrackingEnabled) return true;
            return false;
        }
    }
}
