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
        public Vector2 Force;
        public CollisionBox CollisionBox;
        public GameSprite2D Sprite;
        public float Restitution;
        public float Mass;
        public string Name;
        public bool IsColliding;
        public bool IsStatic;
        public bool IsCPU;

        public Collidable()
        {
            Position = new();
            Velocity = new();
            Restitution = 0.5f;
            Mass = 5f;
            Force = new();
            CollisionBox = new(Position, false, 0f);
            Name = "";
            IsColliding = false;
            IsStatic = false;
            IsCPU = true;
        }

        public Collidable(Vector2 position, string playerName, bool isCPU, bool isStatic, bool isRectangular, AnimationState state, Texture2D[] textures, float deltaTime, bool continuous)
        {
            Position = position;
            Velocity = new();
            Restitution = 0.5f;
            Mass = 5f;
            Force = new();
            CollisionBox = new(Position, isRectangular, textures[0].Width / 2);
            Sprite = new GameSprite2D(state, textures, deltaTime, continuous);
            Name = playerName;
            IsStatic = isStatic;
            IsCPU = isCPU;
        }

        public abstract void Update(float deltaTime);
    }
}
