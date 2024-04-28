using Microsoft.Xna.Framework;

namespace TotallyFair.GameComponents
{
    public class GameObject<T>
    {
        public Vector2 Position;
        public float CollisionRadius;
        public float Width;
        public float Height;
        public bool IsColliding;
        public bool IsRectangular;
        public T Object;
    }
}
