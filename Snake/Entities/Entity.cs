using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Gease.Extensions;

namespace Snake.Entities
{
    public class Entity
    {
        public Vector2 Position { get; set; }
        public int Width { get; }
        public int Height { get; }
        public Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); }
        }

        public Texture2D Texture { get; set; }


        public Entity(Vector2 pos, int width, int height)
        {
            Position = pos;
            Width = width;
            Height = height;
        }
        public void LoadContent(ContentManager content)
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null)
                Texture = new Texture2D(spriteBatch.GraphicsDevice, Width, Height).CreateTexture(Color.Red);

            spriteBatch.Draw(Texture, Bounds, Color.White);
        }


    }
}