using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TotallyFair.CONFIG;
using TotallyFair.GameComponents;
using TotallyFair.Graphics;
using TotallyFair.Utilities;

namespace TotallyFair.Managers
{
    /// <summary>
    /// Scene Manager Object:
    /// Responsible for managing all GameObjects (Players, Consumables, Computers, etc.).
    /// Manages positions of objects, collisions, and bounds of game window.
    /// Keep objects positioned where they are supposed to be.
    /// 
    /// Integer ID Rules:
    /// Players:0-10
    /// Consumables:100-1000
    /// Projectiles: >1000
    /// </summary>
    public class SceneManager
    {
        public Map SceneMap;
        public Dictionary<int, Collidable> Collidables = new Dictionary<int, Collidable>();
        //public Dictionary<int, GameObject<Player>> Players = new Dictionary<int, GameObject<Player>>();
        //public Dictionary<int, GameObject<Consumable>> Consumables = new Dictionary<int, GameObject<Consumable>>();
        private Quad _collisionManager;
        private GAME_CONFIG _settings;

        public SceneManager(Texture2D texture)
        {
            SceneMap = new Map(texture);
            _settings = new(SceneMap);
            _collisionManager = new Quad(new Vector2(0,0), new Vector2(texture.Width, texture.Height));
        }
        public void AddPlayer(int id, Vector2 position, AnimationState animationState, Texture2D[] textures, float deltaTime, bool continuous)
        {
            Player p = new($"Player{id}", position, false, false, animationState, textures, deltaTime, continuous);
            //GameObject<Player> data = new();
            if (!Collidables.ContainsKey(id)) Collidables.Add(id, p);
            _collisionManager.Insert(id, position);
        }

        /*public void AddConsumable(int id, Vector2 position, AnimationState animationState, Texture2D[] textures, float deltaTime, bool continuous)
        {
            Collidable c = new(position, $"Player{id}", false, false, textures[0].Width / 3, animationState, textures, deltaTime, continuous);
            //GameObject<Consumable> data = new();
            data.CollisionBox = new(position, false, texture.Width / 3);
            Consumables.Add(id, data);
            _collisionManager.Insert(id, position);
        }*/

        public void RemovePlayer(int id, Vector2 position)
        {
            if (!Collidables.ContainsKey(id)) return;
            Collidables.Remove(id);
            _collisionManager.Delete(id, position);
        }

        /*public void RemoveConsumable(int id)
        {
            if (!Consumables.ContainsKey(id)) return;
            Consumables.Remove(id);
        }*/

        public void Update()
        {
            if (Collidables.Count == 0) return;
            foreach (KeyValuePair<int,Collidable> c in Collidables)
            {
                Dictionary<int, Vector2> collidables = _collisionManager.Search(c.Key, c.Value.Position);

                //Only update position if player center was found and deleted from quad
                if (_collisionManager.Delete(c.Key, c.Value.CollisionBox.Center))
                {
                    //Try delete all points of collision box
                    _collisionManager.Delete(c.Key, c.Value.CollisionBox.GetTopRight());
                    _collisionManager.Delete(c.Key, c.Value.CollisionBox.GetTopLeft());
                    _collisionManager.Delete(c.Key, c.Value.CollisionBox.GetBottomLeft());
                    _collisionManager.Delete(c.Key, c.Value.CollisionBox.GetBottomRight());

                    //Update Position
                    UpdatePosition(c.Key, _settings.TIME_CONSTANT, collidables);
                    Collidables[c.Key].Update();

                    //Recenter CollisionBox on position
                    c.Value.CollisionBox.Center = Collidables[c.Key].Position;

                    //Finally, insert all points of collision box
                    //Insert Center First
                    _collisionManager.Insert(c.Key, c.Value.CollisionBox.Center);
                    _collisionManager.Insert(c.Key, c.Value.CollisionBox.GetTopRight());
                    _collisionManager.Insert(c.Key, c.Value.CollisionBox.GetTopLeft());
                    _collisionManager.Insert(c.Key, c.Value.CollisionBox.GetBottomLeft());
                    _collisionManager.Insert(c.Key, c.Value.CollisionBox.GetBottomRight());
                }
            }
            //Clean up Quadtree
            _collisionManager.CleanUp();
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            //Draw all Players
            SceneMap.Draw(spriteBatch, graphics);

            foreach (KeyValuePair<int, Collidable> c in Collidables)
            {
                //spriteBatch.DrawString(GameFont, $"{Players[i].Hand[0].FaceValue}", Vector_PlayerHand[i], Color.White);
                //spriteBatch.DrawString(GameFont, $"{Players[i].Hand[1].FaceValue}", new Vector2(Vector_PlayerHand[i].X, Vector_PlayerHand[i].Y + 25), Color.White);
                c.Value.Sprite.Play(spriteBatch, c.Value.Position);
            }
        }

        private void UpdatePosition(int key, float deltaTime, Dictionary<int, Vector2> collidables)
        {
            Vector2 newPosition = new(Collidables[key].Position.X, Collidables[key].Position.Y);
            if (key <= 10)
            {
                if (Collidables[key].IsCPU) Collidables[key].Chase(Collidables[key-1].Position, _settings.TIME_CONSTANT);
                
                //Calculate new position
                newPosition.X += Collidables[key].Velocity.X * (float)deltaTime;
                newPosition.Y += Collidables[key].Velocity.Y * (float)deltaTime;

                //Clamp to bounds of window
                if (newPosition.X + Collidables[key].CollisionBox.CollisionRadius > SceneMap.Bounds.Width ||
                    newPosition.X - Collidables[key].CollisionBox.CollisionRadius < 0 ||
                    newPosition.Y + Collidables[key].CollisionBox.CollisionRadius > SceneMap.Bounds.Height ||
                    newPosition.Y - Collidables[key].CollisionBox.CollisionRadius < 0)
                        return;

                //Exit condition
                Collidables[key].Position = newPosition;

                //Check for collisions if gameObjects in area
                if (collidables != null)
                {
                    foreach (KeyValuePair<int, Vector2> c in collidables)
                        if (c.Key != key && IsColliding(Collidables[key], Collidables[c.Key]))
                        {
                            return;
                            //Players[key].Object
                            //Players[collidable.Key].Object
                            /*if (c.Value.IsStatic)
                            {
                                bodyB.Move(normal * depth);
                            }
                            else if (bodyB.IsStatic)
                            {
                                bodyA.Move(-normal * depth);
                            }
                            else
                            {
                                bodyA.Move(-normal * depth / 2f);
                                bodyB.Move(normal * depth / 2f);
                            }

                            this.ResolveCollision(bodyA, bodyB, normal, depth);*/
                        }
                }
                //Set new GameObject with new position
                Collidables[key].Position = newPosition;
            }
        }

        private bool IsColliding(Collidable bodyA, Collidable bodyB)
        {
            if (!bodyA.CollisionBox.IsRectangular && !bodyB.CollisionBox.IsRectangular)
                return (Math.Sqrt(Math.Pow(bodyB.Position.X - bodyA.Position.X, 2) + Math.Pow(bodyB.Position.Y - bodyA.Position.Y, 2)) < bodyA.CollisionBox.CollisionRadius + bodyB.CollisionBox.CollisionRadius);
            else
                //Treat both objects like rectangles
                return (bodyA.CollisionBox.CollisionRadius + bodyB.CollisionBox.CollisionRadius >= Math.Abs(bodyB.Position.X - bodyA.Position.X) && bodyA.CollisionBox.CollisionRadius + bodyB.CollisionBox.CollisionRadius >= Math.Abs(bodyA.Position.Y - bodyB.Position.Y));
        }

        private double GetAngle(Vector2 a, Vector2 b)
        {
            return Math.Atan2(b.Y - a.Y, b.X - a.X);
        }
    }
}
