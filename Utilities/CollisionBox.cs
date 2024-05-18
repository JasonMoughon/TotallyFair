using Microsoft.Xna.Framework;
using System;

namespace TotallyFair.Utilities
{
    //2D Area by which a collidable resides
    public class CollisionBox
    {
        public Vector2 Center;
        public bool IsRectangular;
        public float CollisionRadius;

        public CollisionBox(Vector2 center, bool isRectangular, float collisionRadius)
        {
            Center = center;
            IsRectangular = isRectangular;
            CollisionRadius = collisionRadius;
        }

        public Vector2 GetMidRight()
        {
            if (!IsRectangular) return SetQuadrant(0);
            else return new Vector2(Center.X + CollisionRadius, 0);
        }

        public Vector2 GetTopRight()
        {
            if (!IsRectangular) return SetQuadrant(Math.PI / 4);
            else return new Vector2(Center.X + CollisionRadius, Center.Y - CollisionRadius);
        }

        public Vector2 GetMidTop()
        {
            if (!IsRectangular) return SetQuadrant(Math.PI / 2);
            else return new Vector2(0, Center.Y - CollisionRadius);
        }

        public Vector2 GetTopLeft()
        {
            if (!IsRectangular) return SetQuadrant(Math.PI * 3 / 4);
            else return new Vector2(Center.X - CollisionRadius, Center.Y - CollisionRadius);
        }

        public Vector2 GetMidLeft()
        {
            if (!IsRectangular) return SetQuadrant(Math.PI);
            else return new Vector2(Center.X - CollisionRadius, 0);
        }

        public Vector2 GetBottomLeft()
        {
            if (!IsRectangular) return SetQuadrant(Math.PI * 5 / 4);
            else return new Vector2(Center.X - CollisionRadius, Center.Y + CollisionRadius);
        }

        public Vector2 GetMidBottom()
        {
            if (!IsRectangular) return SetQuadrant(Math.PI * 3 / 2);
            else return new Vector2(0, Center.Y + CollisionRadius);
        }

        public Vector2 GetBottomRight()
        {
            if (!IsRectangular) return SetQuadrant(Math.PI * 7 / 4);
            else return new Vector2(Center.X + CollisionRadius, Center.Y + CollisionRadius);
        }

        private Vector2 SetQuadrant(double radians)
        {
            return new Vector2(Center.X + (float)Math.Cos(radians) * CollisionRadius, Center.Y - (float)Math.Sin(radians) * CollisionRadius);
        }
    }
}
