using Gease.Input;
using Gease.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Snake.Scenes;
using Gease.Extensions;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public class SnakeGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SceneManager _sceneManager;

        private const int _exitTransitionTime = 765;
        private const int _exitIgnoreTime = 255;
        private Texture2D _exitTexture;
        private Color _exitColor;


        public SnakeGame()
        {
            Window.Title = "Snake";
            Window.AllowUserResizing = false;

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Config.WindowWidth;
            _graphics.PreferredBackBufferHeight = Config.WindowHeight;
            _graphics.PreferMultiSampling = true;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            _sceneManager = new SceneManager();
            _sceneManager.AddScene<GameScene>(new ContentManager(Services, Content.RootDirectory));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _sceneManager.ChangeScene(nameof(GameScene), false);
            
            _exitTexture = new Texture2D(_spriteBatch.GraphicsDevice, Config.WindowWidth, Config.WindowHeight).CreateTexture(Color.Black);
        }
        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            _sceneManager.Update(gameTime);
            
            base.Update(gameTime);

            KeyInput.Update(gameTime);
            MouseInput.Update(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            if (KeyInput.IsKeyDownOnce(Keys.R))
            {
                Initialize();
            }

            if (KeyInput.IsKeyDown(KeyAction.Exit) && KeyInput.KeyPressDuration(KeyAction.Exit) >= _exitTransitionTime)
                Exit();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSeaGreen);
            
            _sceneManager.Draw(_spriteBatch);


            _spriteBatch.Begin();

            if (KeyInput.IsKeyDown(KeyAction.Exit))
                _exitColor = new Color(_exitColor, (KeyInput.KeyPressDuration(KeyAction.Exit) - _exitIgnoreTime) / (_exitTransitionTime / _exitIgnoreTime));
            else
                _exitColor = Color.Transparent;
            _spriteBatch.Draw(_exitTexture, Window.ClientBounds, _exitColor);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}