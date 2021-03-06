﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.Entities
{
    public class Apple : Entity
    {
        public Apple(Vector2 pos, int width, int height)
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

            spriteBatch.Draw(Texture, Bounds, Color.White);
        }
    }
}
