using Gease.Extensions;
using Gease.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Snake.Entities
{
    public class SnakeBody
    {
        // Length of the body, head NOT included.
        private const int _bodyStartLength = 6;

        private List<Entity> _bodyParts;
        public List<Entity> BodyParts { get { return _bodyParts; } }
        public Entity Head
        {
            get { return _bodyParts?[0]; }
        }

        private readonly int _partSize = Config.EntitySize;

        public enum Direction { North, East, South, West }
        Direction currentDirection = Direction.North;
        Direction nextDirection = Direction.North;

        private const float _moveInterval = 0.1f;
        private float _timeSinceMove = 0f;

        public bool WallsActive { get; set; } = Config.WallsActive;

        public List<Entity> Apples { get; set; }
        public delegate void OnAppleEatenHandler(object sender, AppleEventArgs e);
        public event OnAppleEatenHandler OnAppleEaten;
        public class AppleEventArgs : EventArgs
        {
            public Entity Apple { get; set; }
            public AppleEventArgs(Entity apple)
            {
                Apple = apple;
            }
        }

        private bool _isDead = false;
        public bool IsDead
        {
            get { return _isDead; }
            set
            {
                _isDead = value;
                if (_isDead)
                    OnDeath?.Invoke(this, EventArgs.Empty);
            }
        }
        public delegate void OnDeathEventHandler(object sender, EventArgs e);
        public event OnDeathEventHandler OnDeath;


        public SnakeBody(List<Entity> apples)
        {
            Apples = apples;
            ResetBody();
        }

        public void ResetBody()
        {
            _bodyParts = new List<Entity>();

            // TODO: Random body generation.
            // var _random = new Random();

            var headX = _partSize * ((Config.WindowWidth / _partSize) / 2);
            var headY = _partSize * ((Config.WindowHeight / _partSize) / 2);
            var head = new Entity(new Vector2(headX, headY), Config.EntitySize, Config.EntitySize);

            _bodyParts.Add(head);
            for (int i = 0; i < _bodyStartLength; i++)
            {
                SpawnPart();
            }
        }

        private void SpawnPart()
        {
            var partX = _bodyParts[_bodyParts.Count - 1].Position.X;
            var partY = _bodyParts[_bodyParts.Count - 1].Position.Y;

            switch (currentDirection)
            {
                case Direction.East:
                    partX -= _partSize;
                    break;
                case Direction.West:
                    partX += _partSize;
                    break;
                case Direction.North:
                    partY += _partSize;
                    break;
                case Direction.South:
                    partY -= _partSize;
                    break;
            }

            _bodyParts.Add(new Entity(new Vector2(partX, partY), _partSize, _partSize));
        }


        public void Update(GameTime gameTime)
        {
            if (KeyInput.IsKeyDownOnce(KeyAction.Up) && currentDirection != Direction.South)
                nextDirection = Direction.North;
            else if (KeyInput.IsKeyDownOnce(KeyAction.Left) && currentDirection != Direction.East)
                nextDirection = Direction.West;
            else if (KeyInput.IsKeyDownOnce(KeyAction.Down) && currentDirection != Direction.North)
                nextDirection = Direction.South;
            else if (KeyInput.IsKeyDownOnce(KeyAction.Right) && currentDirection != Direction.West)
                nextDirection = Direction.East;

            _timeSinceMove += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeSinceMove >= _moveInterval)
            {
                currentDirection = nextDirection;
                Move();
                _timeSinceMove -= _moveInterval;
            }
        }

        private void Move()
        {
            Vector2 nextHeadPosition = GetNextHeadPosition();

            CheckDeath(nextHeadPosition);
            CheckApples(nextHeadPosition);

            if (!IsDead)
            {
                for (int i = _bodyParts.Count - 1; i >= 1; i--)
                {
                    _bodyParts[i].Position = _bodyParts[i - 1].Position;
                }

                Head.Position = nextHeadPosition;
            }
        }

        private Vector2 GetNextHeadPosition()
        {
            var nextHeadPosition = Vector2.Zero;
            switch (currentDirection)
            {
                case Direction.East:
                    nextHeadPosition = new Vector2(Head.Position.X + _partSize, Head.Position.Y);
                    if (!WallsActive && nextHeadPosition.X > Config.WindowWidth - _partSize)
                        nextHeadPosition = new Vector2(0, nextHeadPosition.Y);
                    break;
                case Direction.West:
                    nextHeadPosition = new Vector2(Head.Position.X - _partSize, Head.Position.Y);
                    if (!WallsActive && nextHeadPosition.X < 0)
                        nextHeadPosition = new Vector2(_partSize * (Config.WindowWidth / _partSize) - _partSize, nextHeadPosition.Y);
                    break;
                case Direction.North:
                    nextHeadPosition = new Vector2(Head.Position.X, Head.Position.Y - _partSize);
                    if (!WallsActive && nextHeadPosition.Y < 0)
                        nextHeadPosition = new Vector2(nextHeadPosition.X, _partSize * (Config.WindowHeight / _partSize) - _partSize);
                    break;
                case Direction.South:
                    nextHeadPosition = new Vector2(Head.Position.X, Head.Position.Y + _partSize);
                    if (!WallsActive && nextHeadPosition.Y > Config.WindowHeight - _partSize)
                        nextHeadPosition = new Vector2(nextHeadPosition.X, 0);
                    break;
            }

            return nextHeadPosition;
        }

        private void CheckDeath(Vector2 nextPos)
        {
            foreach (var part in _bodyParts)
            {
                if (nextPos == part.Position)
                    IsDead = true;
            }

            if (WallsActive &&
                nextPos.X > Config.WindowWidth - _partSize ||
                nextPos.X < 0 ||
                nextPos.Y < 0 ||
                nextPos.Y > Config.WindowHeight - _partSize)
            {
                IsDead = true;
            }
        }

        private void CheckApples(Vector2 nextPos)
        {
            foreach (var apple in Apples)
            {
                if (apple.Position == nextPos)
                {
                    SpawnPart();
                    OnAppleEaten?.Invoke(this, new AppleEventArgs(apple));
                    break;
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            Head.Texture = new Texture2D(spriteBatch.GraphicsDevice, _partSize, _partSize).CreateTexture(Color.Black);

            foreach (var part in _bodyParts)
            {
                part.Draw(spriteBatch);
            }
        }
    }
}