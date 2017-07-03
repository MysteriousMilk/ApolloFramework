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
using System.IO;

namespace Apollo.Framework.Core
{
    #region Interfaces
    public interface IRenderable
    {
        /// <summary>
        /// The origin point of the <see cref="IRenderable"/>.
        /// </summary>
        Vector2 Origin { get; set; }

        /// <summary>
        /// The transform matrix of the <see cref="IRenderable"/> in world space.
        /// </summary>
        Matrix2 WorldTransform { get; }

        /// <summary>
        /// The texture to be applied to the <see cref="IRenderable"/>.
        /// </summary>
        Texture2D Texture { get; set; }

        /// <summary>
        /// The source rectangle of the <see cref="IRenderable"/> object used for drawing.
        /// </summary>
        Rectangle SourceRectangle { get; }

        /// <summary>
        /// The tint of the <see cref="IRenderable"/> object.
        /// </summary>
        Color Tint { get; set; }

        /// <summary>
        /// The graphical <see cref="Microsoft.Xna.Framework.Graphics.BlendState"/> to apply to the <see cref="IRenderable"/>.
        /// </summary>
        BlendState BlendState { get; set; }
    }

    public interface INode
    {
        /// <summary>
        /// Globally unique identifier for the <see cref="INode"/>.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// The name delegated to the <see cref="INode"/> by the Apollo Framework.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// User defined tag given to the <see cref="INode"/> by the user.
        /// </summary>
        string Tag { get; set; }

        /// <summary>
        /// The origin point of the <see cref="INode"/>.
        /// </summary>
        Vector2 Origin { get; set; }

        /// <summary>
        /// Position of the <see cref="Node"/> relative to it's parent <see cref="INode"/>.
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Absolute position of the <see cref="Node"/> within the game world.
        /// </summary>
        Vector2 PositionAbs { get; }

        /// <summary>
        /// The scale factor to be applied to the <see cref="INode"/>.
        /// </summary>
        Vector2 Scale { get; set; }

        /// <summary>
        /// The rotation to be applied to the <see cref="INode"/>, specified in degrees.
        /// </summary>
        float Rotation { get; set; }

        /// <summary>
        /// Used for sorting <see cref="INode"/> objects on screen.
        /// </summary>
        /// <remarks>
        /// Items are sorted on screen first by their order in the scene graph and then by ZOrder.
        /// </remarks>
        int ZOrder { get; set; }

        /// <summary>
        /// The transform matrix of the <see cref="INode"/> in world space.
        /// </summary>
        Matrix2 WorldTransform { get; }

        /// <summary>
        /// The parent <see cref="INode"/>.
        /// All nodes have a parent unless it is the root <see cref="INode"/>.
        /// </summary>
        INode Parent { get; set; }

        /// <summary>
        /// Collection of child nodes.
        /// </summary>
        IEnumerable<INode> Children { get; }

        /// <summary>
        /// Collection of custom properties.
        /// </summary>
        PropertyCollection CustomProperties { get; }

        /// <summary>
        /// Adds a new child <see cref="INode"/> to this <see cref="INode"/>, setting
        /// this <see cref="INode"/> as the parent.
        /// </summary>
        /// <param name="child">The <see cref="INode"/> to add.</param>
        void Add(INode child);

        /// <summary>
        /// Gets the location of the node in world coordinates.
        /// </summary>
        /// <returns>The position of the <see cref="INode"/> in world coordinates.</returns>
        Vector2 GetWorldPosition();

        /// <summary>
        /// Removes a child <see cref="INode"/> from the current parent <see cref="INode"/>.
        /// </summary>
        /// <param name="child">The child <see cref="INode"/> to remove.</param>
        void Remove(INode child);

        /// <summary>
        /// Removes all child <see cref="INode"/> instances that match the given <see cref="Predicate{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> condition used to determine which
        /// child <see cref="INode"/> objects to remove.</param>
        void RemoveAll(Predicate<INode> match);

        /// <summary>
        /// Removes all child <see cref="INode"/> objects.
        /// </summary>
        void Clear();

        /// <summary>
        /// This method is called every cycle used to update the state of the <see cref="INode"/>.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Creates a new copy of the <see cref="INode"/> and all of its children.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The cloned <see cref="INode"/> will have a null parent until added as a 
        /// child to another <see cref="INode"/>.
        /// The cloned <see cref="INode"/> will also be assigned a new identifier.
        /// </remarks>
        INode Clone();
    }

    public interface IScene
    {
        /// <summary>
        /// Adds a <see cref="INode"/> to the root of the scene.
        /// </summary>
        /// <param name="node">The node item to add.</param>
        void AddNode(INode node);

        /// <summary>
        /// Ands one <see cref="INode"/> as a child of another <see cref="INode"/>.
        /// </summary>
        /// <param name="node">The <see cref="INode"/> to add.</param>
        /// <param name="parent">The <see cref="INode"/> to attach the given child <see cref="INode"/> to.</param>
        /// <remarks>
        /// If the parent <see cref="INode"/> is already in the scene graph, the <see cref="INode"/> being attached will
        /// have itself and its children added to the lookup tables.
        /// </remarks>
        void AddNode(INode node, INode parent);

        /// <summary>
        /// Adds a <see cref="INode"/> to the parent <see cref="INode"/> with the given
        /// <see cref="Guid"/>.
        /// </summary>
        /// <param name="node">The <see cref="INode"/> to add.</param>
        /// <param name="parentId">The <see cref="Guid"/> of the parent <see cref="INode"/> 
        /// within the scene graph to add the give <see cref="INode"/> object to.</param>
        void AddNode(INode node, Guid parentId);

        /// <summary>
        /// Adds a <see cref="INode"/> to the parent <see cref="INode"/> with the given tag.
        /// </summary>
        /// <param name="node">The <see cref="INode"/> to add.</param>
        /// <param name="parentId">The tag of the parent <see cref="INode"/> within the scene graph
        /// to add the give <see cref="INode"/> object to.</param>
        void AddNode(INode node, string parentTag);

        /// <summary>
        /// This method is called every cycle used to update the state of the <see cref="Scene"/>.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        void Update(GameTime gameTime);
    }

    public interface IGamePlatform
    {
        /// <summary>
        /// Flag to indicate if the game is a debug assembly or not.
        /// </summary>
        bool IsDebug { get; }

        /// <summary>
        /// Path to the settings directory.
        /// </summary>
        string SettingsDirectory { get; set; }

        /// <summary>
        /// Path to the log directory.
        /// </summary>
        string LogDirectory { get; set; }

        /// <summary>
        /// The name of the settings file.
        /// </summary>
        string SettingsFileName { get; set; }

        /// <summary>
        /// This is the main game class for the game.
        /// </summary>
        Game Game { get; }

        /// <summary>
        /// Opens a file <see cref="Stream"/>. for the given file path.
        /// </summary>
        /// <param name="path">The path to the file to open the <see cref="Stream"/>.</param>
        /// <returns>A <see cref="Stream"/> to read the file contents from.</returns>
        /// <remarks>
        /// This method will just return a stream.  The stream will still need to be closed/disposed
        /// when it is done being used.
        /// </remarks>
        Stream OpenReadStream(string path);

        /// <summary>
        /// Creates a file <see cref="Stream"/>. for the given file path.
        /// </summary>
        /// <param name="path">The path to the file to open/create the <see cref="Stream"/>.</param>
        /// <returns>A <see cref="Stream"/> to write the data contents to.</returns>
        /// <remarks>
        /// This method will just return a stream.  The stream will still need to be closed/disposed
        /// when it is done being used.
        /// </remarks>
        Stream OpenWriteStream(string path);

        /// <summary>
        /// Checks to see if a file exists on the file system for the
        /// <see cref="IGamePlatform"/>.
        /// </summary>
        /// <param name="filename">The full path of the file.</param>
        /// <returns>True if the file exists and False if it does not.</returns>
        bool DoesFileExist(string filename);

        /// <summary>
        /// Writes all the given text to a file.
        /// </summary>
        /// <param name="directory">Path of the directory to store the file in.</param>
        /// <param name="filename">Name of the file to write.</param>
        /// <param name="text">Text to write to the file.</param>
        void WriteToFile(string directory, string filename, string text);
    }

    public interface IServiceLocator
    {
        /// <summary>
        /// Registers a new service with the service locator.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <param name="service">The instance of the service.</param>
        void Register<T>(T service);

        /// <summary>
        /// Gets an instances of a service from the service locator.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <returns>The instance of the service.</returns>
        /// <remarks>
        /// If the service is not found in the service locator, null
        /// will be returned.
        /// </remarks>
        T GetInstance<T>();

        /// <summary>
        /// Checks to see if an instance of the specified type exists
        /// in the service locator.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <returns>True if the instance exists and False if it does not.</returns>
        bool HasInstance<T>();
    }

    public interface IRenderSystem
    {
        /// <summary>
        /// The game's current <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/>.
        /// </summary>
        GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// The <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> used for drawing
        /// <see cref="IRenderable"/> objects to the screen.
        /// </summary>
        SpriteBatch SpriteBatch { get; }

        /// <summary>
        /// Color to clear the screen with before each draw cycle.
        /// </summary>
        Color ClearColor { get; set; }

        /// <summary>
        /// Actions to be performed before the current draw cycle.
        /// </summary>
        void PreProcess();

        /// <summary>
        /// Draws objects to the screen.
        /// </summary>
        void Render();

        /// <summary>
        /// Actions to be performed after the current draw cycle.
        /// </summary>
        void PostProcess();
    }

    public interface ILogger
    {
        /// <summary>
        /// Adds an output channel to the logger.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to output the log to.</param>
        void AddOutputChannel(Stream stream);

        /// <summary>
        /// Adds an output channel to the logger.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to output the log to.</param>
        void AddOutputChannel(TextWriter writer);

        /// <summary>
        /// Writes a new entry to the log.
        /// </summary>
        /// <param name="type">Specifies the <see cref="LogEntryType"/>.</param>
        /// <param name="message">The message to write.</param>
        void WriteLine(LogEntryType type, string message);
    }
    #endregion

    #region Enumerations
    public enum LogEntryType
    {
        Info,
        Debug,
        Trace,
        Warning,
        Error
    }
    #endregion
}
