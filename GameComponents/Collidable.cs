using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotallyFair.Graphics;
using TotallyFair.Utilities;

namespace TotallyFair.GameComponents
{
    public abstract class Collidable
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public CollisionBox CollisionBox;
        public GameSprite2D Sprite;
        public string Name;
        public bool IsColliding;
        public bool IsStatic;
        public bool IsCPU;

        public Collidable()
        {
            Position = new();
            Velocity = new();
            CollisionBox = new(Position, false, 0f);
            Name = "";
            IsColliding = false;
            IsStatic = false;
            IsCPU = true;
        }

        public Collidable(Vector2 position, string playerName, bool isCPU, bool isStatic, bool isRectangular, float collisionRadius, AnimationState state, Texture2D[] textures, float deltaTime, bool continuous)
        {
            Position = position;
            Velocity = new();
            CollisionBox = new(Position, isRectangular, collisionRadius);
            Sprite = new GameSprite2D(state, textures, deltaTime, continuous);
            Name = playerName;
            IsStatic = isStatic;
            IsCPU = isCPU;
        }

        public abstract void Update();
        public abstract void Chase(Vector2 position, float deltaTime);
        public abstract void UpdateVelocity(Vector2 position, float deltaTime);
    }
}
