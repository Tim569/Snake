using Gease.Extensions;
using Gease.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Snake.Entities
{
    public enum Direction { North, East, South, West }

    public class SnakeBody
    {
        // Length of the body, head NOT included.
        private const int _bodyStartLength = 6;

        private List<SnakePart> _bodyParts;
        public List<SnakePart> BodyParts { get { return _bodyParts; } }
        public SnakePart Head
        {
            get { return _bodyParts?[0]; }
        }

        private Texture2D _headTexture;
        private Texture2D _bodyTexture;
        private Texture2D _bodyCornerTexture;

        private readonly int _partSize = Config.EntitySize;
        
        private Direction _currentDirection = Direction.North;
        private Direction _nextDirection = Direction.North;

        private const float _moveInterval = 0.1f;
        private float _timeSinceMove = 0f;

        public bool WallsActive { get; set; } = Config.WallsActive;

        public List<Apple> Apples { get; set; }
        public delegate void OnAppleEatenHandler(object sender, AppleEventArgs e);
        public event OnAppleEatenHandler OnAppleEaten;
        public class AppleEventArgs : EventArgs
        {
            public Apple Apple { get; set; }
            public AppleEventArgs(Apple apple)
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


        public SnakeBody(List<Apple> apples)
        {
            Apples = apples;
            ResetBody();
        }
        public void LoadContent(ContentManager content)
        {
            _headTexture = content.Load<Texture2D>("SnakeHead");
            _bodyTexture = content.Load<Texture2D>("SnakeBody");
            _bodyCornerTexture = content.Load<Texture2D>("SnakeBodyCorner");

            foreach (var part in _bodyParts)
                part.Texture = _bodyTexture;
            Head.Texture = _headTexture;
        }

        public void ResetBody()
        {
            _bodyParts = new List<SnakePart>();

            // TODO: Random body generation.
            // var _random = new Random();

            var headX = _partSize * ((Config.WindowWidth / _partSize) / 2);
            var headY = _partSize * ((Config.WindowHeight / _partSize) / 2);
            var head = new SnakePart(new Vector2(headX, headY), Config.EntitySize, Config.EntitySize);

            _bodyParts.Add(head);
            for (int i = 0; i < _bodyStartLength; i++)
            {
                SpawnPart();
            }

            foreach (var part in _bodyParts)
                part.Texture = _bodyTexture;
            Head.Texture = _headTexture;
        }

        private void SpawnPart()
        {
            var partX = _bodyParts[_bodyParts.Count - 1].Position.X;
            var partY = _bodyParts[_bodyParts.Count - 1].Position.Y;

            switch (_currentDirection)
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

            var part = new SnakePart(new Vector2(partX, partY), _partSize, _partSize);
            part.FacedDirection = _bodyParts[_bodyParts.Count - 1].FacedDirection;
            part.Texture = _bodyTexture;
            _bodyParts.Add(part);
        }


        public void Update(GameTime gameTime)
        {
            if (KeyInput.IsKeyDownOnce(KeyAction.Up) && _currentDirection != Direction.South)
                _nextDirection = Direction.North;
            else if (KeyInput.IsKeyDownOnce(KeyAction.Left) && _currentDirection != Direction.East)
                _nextDirection = Direction.West;
            else if (KeyInput.IsKeyDownOnce(KeyAction.Down) && _currentDirection != Direction.North)
                _nextDirection = Direction.South;
            else if (KeyInput.IsKeyDownOnce(KeyAction.Right) && _currentDirection != Direction.West)
                _nextDirection = Direction.East;

            _timeSinceMove += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeSinceMove >= _moveInterval)
            {
                _currentDirection = _nextDirection;
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
                UpdateFacedDirections();
            }
        }

        private Vector2 GetNextHeadPosition()
        {
            var nextHeadPosition = Vector2.Zero;
            switch (_currentDirection)
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

        private void UpdateFacedDirections()
        {
            Head.FacedDirection = _currentDirection;
            for (int i = _bodyParts.Count - 1; i >= 1; i--)
            {
                _bodyParts[i].FacedDirection = _bodyParts[i - 1].FacedDirection;

                //if (i == _bodyParts.Count - 1)
                //    continue;

                // If previous part had different Directions in the two updates;
                if (_bodyParts[i].FacedDirection != _bodyParts[i].FacedDirectionOld)
                {
                    // Be Corner Texture;
                    _bodyParts[i].Texture = _bodyCornerTexture;
                }
                else
                    _bodyParts[i].Texture = _bodyTexture;
            }
            Head.FacedDirectionOld = _currentDirection;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var part in _bodyParts)
            {
                part.Draw(spriteBatch);
            }
        }
    }
}