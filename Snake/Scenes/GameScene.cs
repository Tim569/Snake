using System;
using System.Collections.Generic;
using Gease.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Entities;

namespace Snake.Scenes
{
    public class GameScene : Scene
    {
        private SnakeBody _snake;
        private List<Entity> _apples = new List<Entity>();

        private bool _isGameOver = false;


        public GameScene() : base(nameof(GameScene))
        {
            _snake = new SnakeBody(_apples);
            _snake.OnDeath += OnSnakeDeath;
            _snake.OnAppleEaten += OnAppleEaten;

            SpawnApple();
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

            Entity apple = null;
            bool isMisplaced = true;
            while (isMisplaced)
            {
                isMisplaced = false;
                var appleX = random.Next(0, Config.WindowWidth / Config.EntitySize) * Config.EntitySize;
                var appleY = random.Next(0, Config.WindowHeight / Config.EntitySize) * Config.EntitySize;
                apple = new Entity(new Vector2(appleX, appleY), Config.EntitySize, Config.EntitySize);

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