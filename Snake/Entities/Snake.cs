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

    public class Snake
    {
        private SnakeMovement _movement;

        // Length of the body, head NOT included.
        private const int _bodyStartLength = 3;
        private readonly int _partSize = Config.EntitySize;
        private const int _distanceToWallAtSpawn = 1;

        private List<SnakePart> _bodyParts;
        public List<SnakePart> BodyParts { get { return _bodyParts; } }
        public SnakePart Head
        {
            get { return _bodyParts.Count > 0 ? _bodyParts?[0] : null; }
        }

        public Texture2D HeadTexture { get; set; }
        public Texture2D BodyTexture { get; set; }
        public Texture2D BodyCornerTexture { get; set; }
        
        public List<Apple> Apples { get; set; }
        public delegate void OnAppleEatenHandler(object sender, EventArgs e);
        public event OnAppleEatenHandler OnAppleEaten;

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


        public Snake(List<Apple> apples)
        {
            Apples = apples;
            ResetBody();
        }
        public void LoadContent(ContentManager content)
        {
            HeadTexture = content.Load<Texture2D>("SnakeHead");
            BodyTexture = content.Load<Texture2D>("SnakeBody");
            BodyCornerTexture = content.Load<Texture2D>("SnakeBodyCorner");

            foreach (var part in _bodyParts)
                part.Texture = BodyTexture;
            Head.Texture = HeadTexture;
        }
        public void ResetBody()
        {
            _bodyParts = new List<SnakePart>();
            
            var _random = new Random();
            
            var headX = _partSize * _random.Next(_distanceToWallAtSpawn, (Config.WindowWidth - _partSize) / _partSize - _distanceToWallAtSpawn);
            var headY = _partSize * _random.Next(_distanceToWallAtSpawn, (Config.WindowHeight - _partSize) / _partSize - _distanceToWallAtSpawn);
            var head = new SnakePart(new Vector2(headX, headY), Config.EntitySize, Config.EntitySize);

            var direction = (Direction)_random.Next(Enum.GetNames(typeof(Direction)).Length);
            bool validDirection = false;
            switch (direction)
            {
                case Direction.North:
                    validDirection = headY + _partSize + _bodyStartLength * _partSize <= Config.WindowHeight;
                    break;
                case Direction.East:
                    validDirection = headX - _bodyStartLength * _partSize >= 0;
                    break;
                case Direction.South:
                    validDirection = headY - _bodyStartLength * _partSize >= 0;
                    break;
                case Direction.West:
                    validDirection = headX + _partSize + _bodyStartLength * _partSize <= Config.WindowWidth;
                    break;
            }
            direction = validDirection ? direction : (Direction) (((int) direction + 2) % 4);

            _movement = new SnakeMovement(this, direction);
            

            _bodyParts.Add(head);
            for (int i = 0; i < _bodyStartLength; i++)
            {
                SpawnPart();
            }

            foreach (var part in _bodyParts)
            {
                part.Texture = BodyTexture;
                part.FacedDirection = direction;
            }
            Head.Texture = HeadTexture;
            head.FacedDirection = direction;
        }

        public void SpawnPart()
        {
            var partX = _bodyParts[_bodyParts.Count - 1].Position.X;
            var partY = _bodyParts[_bodyParts.Count - 1].Position.Y;

            switch (_movement.CurrentDirection)
            {
                case Direction.North:
                    partY += _partSize;
                    break;
                case Direction.South:
                    partY -= _partSize;
                    break;
                case Direction.East:
                    partX -= _partSize;
                    break;
                case Direction.West:
                    partX += _partSize;
                    break;
            }

            var part = new SnakePart(new Vector2(partX, partY), _partSize, _partSize);
            part.FacedDirection = _bodyParts[_bodyParts.Count - 1].FacedDirection;
            part.Texture = BodyTexture;
            _bodyParts.Add(part);
        }


        public bool CheckDeath(Vector2 nextPos)
        {
            if (Config.WallsActive &&
                nextPos.X > Config.WindowWidth - _partSize || nextPos.X < 0 ||
                nextPos.Y > Config.WindowHeight - _partSize || nextPos.Y < 0)
            {
                return true;
            }

            foreach (var part in _bodyParts)
            {
                if (nextPos == part.Position)
                    return true;
            }

            return false;
        }

        public void CheckApples(Vector2 nextPos)
        {
            foreach (var apple in Apples)
            {
                if (apple.Position == nextPos)
                {
                    SpawnPart();
                    OnAppleEaten?.Invoke(apple, EventArgs.Empty);
                    // Checks if the apple did not spawn on the next HeadPosition, which isn't updated to the BodyPart List yet and therefore not checked otherwise.
                    while (Apples[Apples.Count - 1].Position == nextPos)
                    {
                        Apples.RemoveAt(Apples.Count - 1);
                        OnAppleEaten?.Invoke(apple, EventArgs.Empty);
                    }
                    break;
                }
            }
        }


        public void Update(GameTime gameTime)
        {
            _movement.Update(gameTime);
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