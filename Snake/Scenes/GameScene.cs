using System;
using System.Collections.Generic;
using Gease.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Entities;
using Microsoft.Xna.Framework.Content;

namespace Snake.Scenes
{
    public class GameScene : Scene
    {
        private SnakeBody _snake;
        private List<Apple> _apples = new List<Apple>();
        private Texture2D _appleTexture;


        private bool _isGameOver = false;


        public GameScene() : base(nameof(GameScene))
        {
            _snake = new SnakeBody(_apples);
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
        }
        
        private void OnSnakeDeath(object sender, EventArgs e) => _isGameOver = true;

        private void OnAppleEaten(object sender, SnakeBody.AppleEventArgs e)
        {
            _apples.Remove(e.Apple);
            SpawnApple();
        }

        private void SpawnApple()
        {
            var random = new Random();

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

            if (_isGameOver)
                return;

            _snake.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            _snake.Draw(spriteBatch);
            foreach (var apple in _apples)
            {
                apple.Draw(spriteBatch);
            }
        }
    }
}