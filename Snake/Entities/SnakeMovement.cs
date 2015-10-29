using Gease.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Entities
{
    public class SnakeMovement
    {
        private Snake _snake;
        private List<SnakePart> _bodyParts;
        private readonly int _partSize = Config.EntitySize;
        private SnakePart Head
        {
            get { return _bodyParts.Count > 0 ? _bodyParts?[0] : null; }
        }

        public Direction CurrentDirection { get; set; }
        public Direction _nextDirection;

        private const float _moveInterval = 0.1f;
        private float _timeSinceMove = 0f;

        public bool WallsActive { get; set; } = Config.WallsActive;

        public SnakeMovement(Snake snake) : this(snake, Direction.North) { }
        public SnakeMovement(Snake snake, Direction dir)
        {
            _snake = snake;
            _bodyParts = _snake.BodyParts;
            CurrentDirection = dir;
            _nextDirection = dir;
        }

        public void Update(GameTime gameTime)
        {
            HandleInput();

            _timeSinceMove += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeSinceMove >= _moveInterval)
            {
                CurrentDirection = _nextDirection;
                Move();
                _timeSinceMove -= _moveInterval;
            }
        }
        private void HandleInput()
        {
            if (KeyInput.IsKeyDownOnce(KeyAction.Up) && CurrentDirection != Direction.South)
                _nextDirection = Direction.North;
            else if (KeyInput.IsKeyDownOnce(KeyAction.Down) && CurrentDirection != Direction.North)
                _nextDirection = Direction.South;
            else if (KeyInput.IsKeyDownOnce(KeyAction.Left) && CurrentDirection != Direction.East)
                _nextDirection = Direction.West;
            else if (KeyInput.IsKeyDownOnce(KeyAction.Right) && CurrentDirection != Direction.West)
                _nextDirection = Direction.East;
        }

        private void Move()
        {
            Vector2 nextHeadPosition = GetNextHeadPosition();

            _snake.IsDead = _snake.CheckDeath(nextHeadPosition);
            _snake.CheckApples(nextHeadPosition);

            if (!_snake.IsDead)
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
            switch (CurrentDirection)
            {
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
            }

            return nextHeadPosition;
        }

        private void UpdateFacedDirections()
        {
            Head.FacedDirection = CurrentDirection;

            for (int i = _bodyParts.Count - 1; i >= 1; i--)
            {
                _bodyParts[i].FacedDirection = _bodyParts[i - 1].FacedDirection;

                // If previous part had different Directions in the two updates;
                if (_bodyParts[i].FacedDirection != _bodyParts[i].FacedDirectionOld)
                {
                    // Be Corner Texture;
                    _bodyParts[i].Texture = _snake.BodyCornerTexture;
                }
                else
                    _bodyParts[i].Texture = _snake.BodyTexture;
            }
            Head.FacedDirectionOld = CurrentDirection;
        }
    }
}
