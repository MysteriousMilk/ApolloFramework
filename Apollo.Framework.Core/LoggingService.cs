// ******************************************************************
// Apollo Framework: Game Engine Framework for MonoGame
//
// MIT License
// Copyright(c) 2017 MysteriousMilk
//
// This source code file is subject to the terms and conditions defined 
// in the LICENSE file, distributed as part of this source code package.
// ******************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apollo.Framework.Core
{
    public class LoggingService : ILogger
    {
        private List<TextWriter> _OutputChannels;
        private StringBuilder _Log;
        private IGamePlatform _Platform;

        public LoggingService(IGamePlatform platform, TextWriter writer)
        {
            _OutputChannels = new List<TextWriter>()
            {
                writer
            };

            _Platform = platform;
            _Platform.Game.Exiting += OnPlatformExit;

            _Log = new StringBuilder();
        }

        public LoggingService(IGamePlatform platform, Stream stream)
        {
            _OutputChannels = new List<TextWriter>()
            {
                new StreamWriter(stream, Encoding.UTF8)
            };

            _Platform = platform;
            _Platform.Game.Exiting += OnPlatformExit;

            _Log = new StringBuilder();
        }

        /// <summary>
        /// Adds an output channel to the logger.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to output the log to.</param>
        public void AddOutputChannel(Stream stream)
        {
            AddOutputChannel(new StreamWriter(stream, Encoding.UTF8));
        }

        /// <summary>
        /// Adds an output channel to the logger.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to output the log to.</param>
        public void AddOutputChannel(TextWriter writer)
        {
            _OutputChannels.Add(writer);
            writer.Write(_Log.ToString());
        }

        /// <summary>
        /// Writes a new entry to the log.
        /// </summary>
        /// <param name="type">Specifies the <see cref="LogEntryType"/>.</param>
        /// <param name="message">The message to write.</param>
        public void WriteLine(LogEntryType type, string message)
        {
            StringBuilder lineBuilder = new StringBuilder();
            lineBuilder.Append(string.Format("[{0}] ", type.ToString()));
            lineBuilder.Append(string.Format("[{0}] ", DateTime.Now.ToString()));
            lineBuilder.Append(message);

            // add line to in memory log
            _Log.Append(lineBuilder.ToString());
            _Log.Append(Environment.NewLine);

            // write line to all output channels
            foreach (TextWriter writer in _OutputChannels)
                writer.WriteLine(lineBuilder.ToString());
        }

        private void OnPlatformExit(object sender, EventArgs e)
        {
            foreach (TextWriter writer in _OutputChannels)
            {
                if (writer != null)
                    writer.Dispose();
            }
        }
    }
}
