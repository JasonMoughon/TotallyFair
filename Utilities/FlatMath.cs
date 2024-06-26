﻿using System;
using Microsoft.Xna.Framework;


namespace TotallyFair.Utilities
{
    public static class FlatMath
    {
        public static float Clamp(float value, float min, float max)
        {
            if (min == max)
            {
                return min;
            }

            if (min > max)
            {
                throw new ArgumentOutOfRangeException("min is greater than the max.");
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (min == max)
            {
                return min;
            }

            if (min > max)
            {
                throw new ArgumentOutOfRangeException("min is greater than the max.");
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }


        public static float Length(Vector2 v)
        {
            return MathF.Sqrt(v.X * v.X + v.Y * v.Y);
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2 Normalize(Vector2 v)
        {
            float len = FlatMath.Length(v);
            return new Vector2(v.X / len, v.Y / len);
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            // a · b = ax * bx + ay * by
            return a.X * b.X + a.Y * b.Y;
        }

        public static float Cross(Vector2 a, Vector2 b)
        {
            // cz = ax * by − ay * bx
            return a.X * b.Y - a.Y * b.X;
        }

        public static double GetAngle(Vector2 a, Vector2 b)
        {
            return Math.Atan2(b.Y - a.Y, b.X - a.X);
        }
    }
}
