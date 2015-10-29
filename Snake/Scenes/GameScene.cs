using System;
using System.Collections.Generic;
using Gease.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Entities;
using Microsoft.Xna.Framework.Content;
using Gease.Serialization;
using System.IO;
using Microsoft.Win32;
using Gease.Input;
using Microsoft.Xna.Framework.Input;

namespace Snake.Scenes
{
    public class GameScene : Scene
    {
        private Entities.Snake _snake;
        private List<Apple> _apples = new List<Apple>();
        private Texture2D _appleTexture;

        private SpriteFont _font;

        private int _score = 0;
        private int _highScore = 0;
        private Color _scoreColor = new Color(Color.Black, 128);

        private bool _gameStarted = false;
        private bool _isGameOver = false;


        public GameScene() : base(nameof(GameScene))
        {
            _snake = new Entities.Snake(_apples);
            _snake.OnDeath += OnSnakeDeath;
            _snake.OnAppleEaten += OnAppleEaten;

            SpawnApple();
        }
        public override void LoadContent()
        {
            _snake.LoadContent(ContentManager);

            _appleTexture = ContentManager.Load<Texture2D>("Apple");
            foreach (var apple in _apples)
                apple.Texture = _appleTexture;

            _font = ContentManager.Load<SpriteFont>("ScoreFont");
            _font.Spacing = 4f;


            // TODO: Replace this code with own class. Maybe new API? #1
            if (!Directory.Exists(Path.Combine(Config.HighScoreFile, "../")))
                Directory.CreateDirectory(Path.Combine(Config.HighScoreFile, "../"));

            if (!File.Exists(Config.HighScoreFile))
                File.Create(Config.HighScoreFile).Close();
            else
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(Config.HighScoreFile)))
                {
                    _highScore = reader.ReadInt32();
                }
            }
            ////////////////////////////////////////////////////////////
        }

        private void OnSnakeDeath(object sender, EventArgs e)
        {
            _isGameOver = true;


            // TODO: Replace this code with own class. Maybe new API? #2
            if (_score >= _highScore)
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(Config.HighScoreFile, FileMode.Create)))
                {
                    writer.Write(_score);
                }
            }
            //////////////////////////////////////////////////////
        }

        private void OnAppleEaten(object sender, EventArgs e)
        {
            _apples.Remove((Apple) sender);
            SpawnApple();

            // No matter how often apples get eaten in one tick, in reality, one tick means one possible eaten apple.
            if (!_onTickScored)
                _score++;
            _onTickScored = true;
        }
        private bool _onTickScored = false;
        
        private void SpawnApple()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            Apple apple = null;
            bool isMisplaced = true;
            while (isMisplaced)
            {
                isMisplaced = false;
                var appleX = random.Next(0, Config.WindowWidth / Config.EntitySize) * Config.EntitySize;
                var appleY = random.Next(0, Config.WindowHeight / Config.EntitySize) * Config.EntitySize;
                apple = new Apple(new Vector2(appleX, appleY), Config.EntitySize, Config.EntitySize);

                foreach (var snakePart in _snake.BodyParts)
                {
                    if (snakePart.Position == apple.Position)
                        isMisplaced = true;
                }
                foreach (var a in _apples)
                {
                    if (a.Position == apple.Position)
                        isMisplaced = true;
                }
            }
            apple.Texture = _appleTexture;

            _apples.Add(apple);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (KeyInput.IsKeyDownAny() && !KeyInput.IsKeyDown(Keys.R) && !KeyInput.IsKeyDown(Keys.Escape))
                _gameStarted = true;

            if (!_gameStarted || _isGameOver)
                return;

            _snake.Update(gameTime);
            _onTickScored = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Begin();

            _snake.Draw(spriteBatch);
            foreach (var apple in _apples)
            {
                apple.Draw(spriteBatch);
            }

            Vector2 scoreSize = _font.MeasureString(_score.ToString());
            spriteBatch.DrawString(_font, _score.ToString(), new Vector2(Config.WindowWidth - scoreSize.X, 0), _scoreColor);

            Vector2 highScoreSize = _font.MeasureString(_score.ToString(_highScore.ToString()));
            float scale = 0.5f;
            spriteBatch.DrawString(_font, _highScore.ToString(), new Vector2(Config.WindowWidth - highScoreSize.X * scale, highScoreSize.Y), _scoreColor, 0f, Vector2.One, scale, SpriteEffects.None, 0f);

            if (_isGameOver)
            {
                string restartText = "Press \"R\" to restart.";
                Vector2 restartSize = _font.MeasureString(restartText);
                spriteBatch.DrawString(_font, restartText, new Vector2((Config.WindowWidth - restartSize.X) / 2, (Config.WindowHeight - restartSize.Y) / 4), Color.Black);
            }
            else if (!_gameStarted)
            {
                string startText = "Press any key.";
                Vector2 startSize = _font.MeasureString(startText);
                spriteBatch.DrawString(_font, startText, new Vector2((Config.WindowWidth - startSize.X) / 2, (Config.WindowHeight - startSize.Y) / 4), Color.Black);
            }
                

            spriteBatch.End();
        }
    }
}