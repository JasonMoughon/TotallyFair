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
        public Dictionary<int, GameObject<Player>> Players = new Dictionary<int, GameObject<Player>>();
        public Dictionary<int, GameObject<Consumable>> Consumables = new Dictionary<int, GameObject<Consumable>>();
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
            GameObject<Player> data = new();
            data.Object = new($"Player{id}", position, animationState, textures, deltaTime, continuous);
            data.Position = position;
            data.CollisionRadius = textures[0].Width / 2;
            if (!Players.ContainsKey(id)) Players.Add(id, data);
            _collisionManager.Insert(id, position);
        }

        public void AddConsumable(int id, Texture2D texture, Vector2 position)
        {
            GameObject<Consumable> data = new();
            data.Object = new();
            data.Position = position;
            data.CollisionRadius = texture.Width / 2;
            Consumables.Add(id, data);
            _collisionManager.Insert(id, position);
        }

        public void RemovePlayer(int id, Vector2 position)
        {
            if (!Players.ContainsKey(id)) return;
            Players.Remove(id);
            _collisionManager.Delete(id, position);
        }

        public void RemoveConsumable(int id)
        {
            if (!Consumables.ContainsKey(id)) return;
            Consumables.Remove(id);
        }

        public void Update()
        {
            if (Players.Count == 0) return;
            foreach (KeyValuePair<int,GameObject<Player>> player in Players)
            {
                Dictionary<int, Vector2> collidables = _collisionManager.Search(player.Key, player.Value.Position);
                if (collidables != null)
                {
                    foreach (KeyValuePair<int, Vector2> collidable in collidables)
                    {
                        //Key <= 10 indicates Player Object
                        if (collidable.Key != player.Key && collidable.Key <= 10)
                        {
                            //Debug.WriteLine($"Checking for collision between {collidable.Key}&{player.Key}");
                            Players[collidable.Key].IsColliding = IsColliding(Players[collidable.Key], player.Value);
                            Players[player.Key].IsColliding = Players[collidable.Key].IsColliding;
                            if (Players[player.Key].IsColliding)
                            {
                                Players[player.Key].Object.Velocity += Players[collidable.Key].Object.Velocity;
                            }
                        }
                    }
                }

                //Only update position if player was found and deleted from quad
                if (_collisionManager.Delete(player.Key, player.Value.Object.Sprite.Position))
                {
                    UpdatePosition(player.Key, _settings.TIME_CONSTANT);
                    Players[player.Key].Object.Update();
                    _collisionManager.Insert(player.Key, player.Value.Object.Sprite.Position);
                }
            }
            //TODO: Consumable updates
            foreach (var consumable in Consumables)
            {
                consumable.Value.Object.Update();
            }

            //Clean up Quadtree
            _collisionManager.CleanUp();
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            //Draw all Players
            SceneMap.Draw(spriteBatch, graphics);

            foreach (KeyValuePair<int, GameObject<Player>> player in Players)
            {
                //spriteBatch.DrawString(GameFont, $"{Players[i].Hand[0].FaceValue}", Vector_PlayerHand[i], Color.White);
                //spriteBatch.DrawString(GameFont, $"{Players[i].Hand[1].FaceValue}", new Vector2(Vector_PlayerHand[i].X, Vector_PlayerHand[i].Y + 25), Color.White);
                player.Value.Object.Sprite.Play(spriteBatch);
            }
        }

        private void UpdatePosition(int key, float deltaTime)
        {
            if (key <= 10)
            {
                GameObject<Player> P = Players[key];
                if (Players[key].Object.IsCPU) Players[key].Object.Chase(Players[0].Object.Sprite.Position, _settings.TIME_CONSTANT);
                Players[key].Object.Sprite.Position.X += Players[key].Object.Velocity.X * (float)deltaTime;
                Players[key].Object.Sprite.Position.Y += Players[key].Object.Velocity.Y * (float)deltaTime;

                //Clamp to bounds of window
                if (Players[key].Object.Sprite.Position.X + Players[key].CollisionRadius > SceneMap.Bounds.Width) Players[key].Object.Sprite.Position.X = SceneMap.Bounds.Width - Players[key].CollisionRadius;
                if (Players[key].Object.Sprite.Position.X - Players[key].CollisionRadius < 0) Players[key].Object.Sprite.Position.X = Players[key].Position.X = Players[key].CollisionRadius;
                if (Players[key].Object.Sprite.Position.Y + Players[key].CollisionRadius > SceneMap.Bounds.Height) Players[key].Object.Sprite.Position.Y = SceneMap.Bounds.Height - Players[key].CollisionRadius;
                if (Players[key].Object.Sprite.Position.Y - Players[key].CollisionRadius < 0) Players[key].Object.Sprite.Position.Y = Players[key].CollisionRadius;
            }
        }

        private bool IsColliding(GameObject<Player> gameObject1, GameObject<Player> gameObject2)
        {
            if (!gameObject1.IsRectangular && !gameObject2.IsRectangular)
                return (Math.Sqrt(Math.Pow(gameObject2.Object.Sprite.Position.X - gameObject1.Object.Sprite.Position.X, 2) + Math.Pow(gameObject2.Object.Sprite.Position.Y - gameObject1.Object.Sprite.Position.Y, 2)) <= gameObject1.CollisionRadius + gameObject2.CollisionRadius);
            else
            {
                //Treat both objects like rectangles
                double gameObject1RectangleRadius = Math.Abs(gameObject1.CollisionRadius / Math.Cos(GetAngle(gameObject1.Object.Sprite.Position, gameObject2.Object.Sprite.Position)));
                double gameObject2RectangleRadius = Math.Abs(gameObject2.CollisionRadius / Math.Cos(GetAngle(gameObject1.Object.Sprite.Position, gameObject2.Object.Sprite.Position)));
                return (Math.Sqrt(Math.Pow(gameObject2.Object.Sprite.Position.X - gameObject1.Object.Sprite.Position.X, 2) + Math.Pow(gameObject2.Object.Sprite.Position.Y - gameObject1.Object.Sprite.Position.Y, 2)) <= gameObject1RectangleRadius + gameObject2RectangleRadius);
            }
        }

        private double GetAngle(Vector2 a, Vector2 b)
        {
            return Math.Atan2(b.Y - a.Y, b.X - a.X);
        }
    }
}
