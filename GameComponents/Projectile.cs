using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TotallyFair.Graphics;

namespace TotallyFair.GameComponents
{
    public class Projectile : Collidable
    {

        public Projectile()
        {
            Position = new();
            Velocity = new();
            Restitution = 0.5f;
            Mass = 0.25f;
            Force = new();
            CollisionBox = new(Position, false, 0f);
            Name = "";
            IsColliding = false;
            IsStatic = false;
            IsCPU = true;
        }

        public Projectile(Vector2 position, string name, bool isCPU, bool isStatic, bool isRectangular, AnimationState state, Texture2D[] textures, float deltaTime, bool continuous)
        {
            Position = position;
            Velocity = new();
            Restitution = 0.5f;
            Mass = 0.25f;
            Force = new();
            CollisionBox = new(Position, isRectangular, textures[0].Width / 2);
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


            //Update Velocity
            Vector2 acceleration = Force / Mass;
            Velocity += acceleration * deltaTime;

            if (Force.X >= 10f) Force.X += -Force.X / 10f;
            else Force.X = 0f;

            if (Force.Y >= 10f) Force.Y += -Force.Y / 10f;
            else Force.Y = 0f;

            if (Math.Abs(Velocity.X) >= 5f) Velocity.X += -Velocity.X * deltaTime;
            else Velocity.X = 0;
            if (Math.Abs(Velocity.Y) >= 5f) Velocity.Y += -Velocity.Y * deltaTime;
            else Velocity.Y = 0;

        }

        public void Chase(Vector2 Target)
        {
            Vector2 Velocity = new(Target.X - Position.X, Target.Y - Position.Y);
            float c = (float)Math.Sqrt(Math.Pow(Velocity.X, 2) + Math.Pow(Velocity.Y, 2));
            Force = new Vector2((2000 / c) * Velocity.X, (2000 / c) * Velocity.Y);
        }
    }
}
