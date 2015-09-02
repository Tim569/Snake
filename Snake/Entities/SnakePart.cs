using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace Snake.Entities
{
    public class SnakePart : Entity
    {
        private Direction _facedDirection = Direction.North;
        public Direction FacedDirection
        {
            get { return _facedDirection; }
            set
            {
                FacedDirectionOld = _facedDirection;
                _facedDirection = value;
            }
        }
        public Direction FacedDirectionOld { get;  set; }
        public bool IsCornering
        {
            get { return FacedDirection != FacedDirectionOld; }
        }


        public SnakePart(Vector2 pos, int width, int height)
            : base(pos, width, height)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            var flipSide = SpriteEffects.None;
            float rotation = 0f;
            var origin = Vector2.Zero;

            switch (FacedDirection)
            {
                case Direction.North:
                    if (IsCornering && FacedDirectionOld == Direction.East)
                        flipSide = SpriteEffects.FlipHorizontally;
                    break;
                case Direction.South:
                    if (IsCornering && FacedDirectionOld == Direction.East)
                    {
                        rotation = MathHelper.PiOver2;
                        origin = origin = new Vector2(0, Texture.Width);
                    }
                    flipSide = SpriteEffects.FlipVertically;
                    break;
                case Direction.East:
                    if (IsCornering && FacedDirectionOld == Direction.South)
                        flipSide = SpriteEffects.FlipHorizontally;
                    rotation = MathHelper.PiOver2;
                    origin = new Vector2(0, Texture.Width);
                    break;
                case Direction.West:
                    if (IsCornering && FacedDirectionOld == Direction.North)
                        flipSide = SpriteEffects.FlipHorizontally;
                    rotation = -MathHelper.PiOver2;
                    origin = new Vector2(Texture.Width, 0);
                    break;
            }

            spriteBatch.Draw(Texture, null, Bounds, null, origin, rotation, null, Color.White, flipSide);
        }
    }
}
