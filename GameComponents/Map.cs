using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Numerics;

namespace TotallyFair.GameComponents
{
    public class Map
    {
        /// <summary>
        /// Game Map is confined to the bounds of the Display Window.
        /// All of Game Map is shown during runtime.
        /// </summary>
        private Texture2D _texture;
        private Microsoft.Xna.Framework.Vector2 Position = new Microsoft.Xna.Framework.Vector2(0, 0);
        public Size Bounds { get; private set; }

        public Map(Texture2D texture)
        {
            _texture = texture;
            Bounds = new(_texture.Width, _texture.Height);
            Position.X = Bounds.Width / 2; Position.Y = Bounds.Height / 2;
        }

        public void Draw(SpriteBatch batch, GraphicsDeviceManager graphics)
        {
            //Check if screen size changed
            if (Position.X != graphics.PreferredBackBufferWidth / 2) Position.X = graphics.PreferredBackBufferWidth / 2;
            if (Position.Y != graphics.PreferredBackBufferHeight / 2) Position.Y = graphics.PreferredBackBufferHeight / 2;

            //batch.Draw(_texture, Position, new Microsoft.Xna.Framework.Rectangle(), Microsoft.Xna.Framework.Color.White, 0f, new Microsoft.Xna.Framework.Vector2(Bounds.Width/2, Bounds.Height/2), 1, SpriteEffects.None, 1f);
            batch.Draw(_texture, Position, null, Microsoft.Xna.Framework.Color.White, 0, new Microsoft.Xna.Framework.Vector2(_texture.Width/2, _texture.Height/2), 1, SpriteEffects.None, 1f);

        }
    }
}
