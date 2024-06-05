using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TotallyFair.Graphics;

namespace TotallyFair.GameComponents
{
    public class Environmental : Collidable
    {
        public Environmental(Vector2 position, string name, bool isCPU, bool isStatic, bool isRectangular, AnimationState state, Texture2D[] textures, float deltaTime, bool continuous)
        {
            Position = position;
            Velocity = new();
            Restitution = 0.5f;
            Mass = 0.25f;
            Force = new();
            CollisionBox = new(Position, isRectangular, textures[0].Width);
            Sprite = new GameSprite2D(state, textures, deltaTime, continuous);
            Name = name;
            IsStatic = isStatic;
            IsCPU = isCPU;
        }

        public override void Update(float deltaTime)
        {
            //Cannot do anything if no textures
            if (Sprite.AnimationLibrary.Count == 0) return;
            //Start animation if not running
            if (!Sprite.AnimationLibrary[Sprite.CurrentState].Running) Sprite.Start();
        }
    }
}
