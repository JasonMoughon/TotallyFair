using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Players:0-99
    /// Consumables:100-999
    /// Projectiles: 1000-9999
    /// Environmentals: >=10000
    /// </summary>

    public class SceneManager
    {
        public Map SceneMap;

        public Dictionary<int, Player> Players = new();
        public Dictionary<int, Consumable> Consumables = new();
        public Dictionary<int, Projectile> Projectiles = new();
        public Dictionary<int, Environmental> Environmentals = new(); //Static Environmental Objects
        private Quad _collisionManager;
        private Camera _camera;
        private GAME_CONFIG _settings;

        public SceneManager(Texture2D texture, Viewport viewport)
        {
            _camera = new(viewport);
            SceneMap = new(texture);
            _settings = new(SceneMap);
            _collisionManager = new(new Vector2(0, 0), new Vector2(texture.Width, texture.Height));
        }
        public void AddPlayer(int id, Vector2 position, AnimationState animationState, Texture2D[] textures, float deltaTime, bool continuous)
        {
            //Key already exists
            if (Players.ContainsKey(id)) return;

            Player p = new($"Player{id}", position, false, false, animationState, textures, deltaTime, continuous);
            Players.Add(id, p);
            InsertIntoQuad(id, p);
        }

        public void RemovePlayer(int id, Vector2 position)
        {
            if (!Players.ContainsKey(id)) return;

            Players.Remove(id);
            _collisionManager.Delete(id, position);
        }

        public void AddProjectile(int id, Vector2 position, AnimationState animationState, Texture2D[] textures, float deltaTime, bool continuous)
        {
            //Key already exists
            if (Projectiles.ContainsKey(id)) return;

            Projectile p = new(position, $"Projectile{id}", true, false, false, animationState, textures, deltaTime, continuous);
            Projectiles.Add(id, p);
            InsertIntoQuad(id, p);
        }

        public void RemoveProjectile(int id, Vector2 position)
        {
            if (!Projectiles.ContainsKey(id)) return;

            DeleteFromQuad(id, Projectiles[id]);
            Projectiles.Remove(id);
        }

        public void AddConsumable(int id, Vector2 position, AnimationState animationState, Texture2D[] textures, float deltaTime, bool continuous)
        {
            //Key already exists
            if (!Consumables.ContainsKey(id)) return;

            Consumable c = new(position, $"Projectile{id}", true, false, false, animationState, textures, deltaTime, continuous);
            Consumables.Add(id, c);
            InsertIntoQuad(id, c);
        }

        public void RemoveConsumable(int id)
        {
            if (!Consumables.ContainsKey(id)) return;

            DeleteFromQuad(id, Consumables[id]);
            Consumables.Remove(id);
        }

        public void AddEnvironment(int id, Vector2 position, AnimationState animationState, Texture2D[] textures, float deltaTime, bool continuous)
        {
            //Key already exists
            if (Environmentals.ContainsKey(id)) return;

            Environmental c = new(position, $"Environment{id}", true, true, true, animationState, textures, deltaTime, continuous);
            Environmentals.Add(id, c);
            InsertIntoQuad(id, c);
        }

        public void RemoveEnvironment(int id)
        {
            if (!Environmentals.ContainsKey(id)) return;

            DeleteFromQuad(id, Environmentals[id]);
            Environmentals.Remove(id);
        }

        public void Update(Viewport viewport)
        {
            _camera.UpdateCamera(viewport);
            if (Players.Count > 0)
            {
                foreach (KeyValuePair<int, Player> c in Players)
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
                        UpdatePosition<Player>(c.Key, _settings.TIME_CONSTANT, collidables);
                        Players[c.Key].Update(_settings.TIME_CONSTANT);

                        //Recenter CollisionBox on position
                        c.Value.CollisionBox.Center = Players[c.Key].Position;

                        //Finally, insert all points of collision box
                        //Insert Center First
                        InsertIntoQuad(c.Key, c.Value);
                    }
                }
            }

            if (Consumables.Count > 0)
            {
                foreach (KeyValuePair<int, Consumable> c in Consumables)
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
                        UpdatePosition<Consumable>(c.Key, _settings.TIME_CONSTANT, collidables);
                        Consumables[c.Key].Update(_settings.TIME_CONSTANT);

                        //Recenter CollisionBox on position
                        c.Value.CollisionBox.Center = Consumables[c.Key].Position;

                        //Finally, insert all points of collision box
                        //Insert Center First
                        InsertIntoQuad(c.Key, c.Value);
                    }
                }
            }

            if (Projectiles.Count > 0)
            {
                foreach (KeyValuePair<int, Projectile> c in Projectiles)
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
                        UpdatePosition<Projectile>(c.Key, _settings.TIME_CONSTANT, collidables);
                        Projectiles[c.Key].Update(_settings.TIME_CONSTANT);

                        //Recenter CollisionBox on position
                        c.Value.CollisionBox.Center = Projectiles[c.Key].Position;

                        //Finally, insert all points of collision box
                        //Insert Center First
                        InsertIntoQuad(c.Key, c.Value);
                    }
                }
            }

            if (Environmentals.Count > 0)
            {
                foreach (KeyValuePair<int, Environmental> c in Environmentals)
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
                        UpdatePosition<Environmental>(c.Key, _settings.TIME_CONSTANT, collidables);
                        Environmentals[c.Key].Update(_settings.TIME_CONSTANT);

                        //Recenter CollisionBox on position
                        c.Value.CollisionBox.Center = Environmentals[c.Key].Position;

                        //Finally, insert all points of collision box
                        //Insert Center First
                        InsertIntoQuad(c.Key, c.Value);
                    }
                }
            }

            //Clean up Quadtree
            _collisionManager.CleanUp();
            _camera.MoveCamera(Players[0].Position);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            //Draw all Players
            SceneMap.Draw(spriteBatch, graphics);

            foreach (KeyValuePair<int, Player> c in Players)
            {
                //spriteBatch.DrawString(GameFont, $"{Players[i].Hand[0].FaceValue}", Vector_PlayerHand[i], Color.White);
                //spriteBatch.DrawString(GameFont, $"{Players[i].Hand[1].FaceValue}", new Vector2(Vector_PlayerHand[i].X, Vector_PlayerHand[i].Y + 25), Color.White);
                c.Value.Sprite.Play(spriteBatch, c.Value.Position);
            }
        }

        private void InsertIntoQuad(int id, Collidable c)
        {
            _collisionManager.Insert(id, c.CollisionBox.Center);
            _collisionManager.Insert(id, c.CollisionBox.GetTopRight());
            _collisionManager.Insert(id, c.CollisionBox.GetTopLeft());
            _collisionManager.Insert(id, c.CollisionBox.GetBottomRight());
            _collisionManager.Insert(id, c.CollisionBox.GetBottomLeft());
        }

        private void DeleteFromQuad(int id, Collidable c)
        {
            _collisionManager.Delete(id, c.CollisionBox.Center);
            _collisionManager.Delete(id, c.CollisionBox.GetTopRight());
            _collisionManager.Delete(id, c.CollisionBox.GetTopLeft());
            _collisionManager.Delete(id, c.CollisionBox.GetBottomLeft());
            _collisionManager.Delete(id, c.CollisionBox.GetBottomRight());
        }

        private void ClampPositionInBounds<T>(int key, float deltaTime)
        {
            Vector2 newPosition = new();
            //Clamp Players to map bounds
            if (typeof(T) == typeof(Player))
            {
                //Protect against null position
                if (!Players.ContainsKey(key)) return;

                (newPosition.X, newPosition.Y) = (Players[key].Position.X, Players[key].Position.Y);

                //Calculate new position
                newPosition.X += Players[key].Velocity.X * (float)deltaTime;
                newPosition.Y += Players[key].Velocity.Y * (float)deltaTime;

                //Clamp to bounds of window
                if (newPosition.X + Players[key].CollisionBox.CollisionRadius > SceneMap.Bounds.Width ||
                    newPosition.X - Players[key].CollisionBox.CollisionRadius < 0 ||
                    newPosition.Y + Players[key].CollisionBox.CollisionRadius > SceneMap.Bounds.Height ||
                    newPosition.Y - Players[key].CollisionBox.CollisionRadius < 0)
                    return;

                Players[key].Position = newPosition;
            }

            //Clamp Consumables to map bounds
            if (typeof(T).Equals(typeof(Consumable)))
            {
                //Protect against null position
                if (!Consumables.ContainsKey(key)) return;

                (newPosition.X, newPosition.Y) = (Consumables[key].Position.X, Consumables[key].Position.Y);

                //Calculate new position
                newPosition.X += Consumables[key].Velocity.X * (float)deltaTime;
                newPosition.Y += Consumables[key].Velocity.Y * (float)deltaTime;

                //Clamp to bounds of window
                if (newPosition.X + Consumables[key].CollisionBox.CollisionRadius > SceneMap.Bounds.Width ||
                    newPosition.X - Consumables[key].CollisionBox.CollisionRadius < 0 ||
                    newPosition.Y + Consumables[key].CollisionBox.CollisionRadius > SceneMap.Bounds.Height ||
                    newPosition.Y - Consumables[key].CollisionBox.CollisionRadius < 0)
                    return;

                Consumables[key].Position = newPosition;
            }

            //Clamp Projectiles to map bounds
            if (typeof(T).Equals(typeof(Projectile)))
            {
                //Protect against null position
                if (!Projectiles.ContainsKey(key)) return;

                (newPosition.X, newPosition.Y) = (Projectiles[key].Position.X, Projectiles[key].Position.Y);

                //Calculate new position
                newPosition.X += Projectiles[key].Velocity.X * (float)deltaTime;
                newPosition.Y += Projectiles[key].Velocity.Y * (float)deltaTime;

                //Clamp to bounds of window
                if (newPosition.X + Projectiles[key].CollisionBox.CollisionRadius > SceneMap.Bounds.Width ||
                    newPosition.X - Projectiles[key].CollisionBox.CollisionRadius < 0 ||
                    newPosition.Y + Projectiles[key].CollisionBox.CollisionRadius > SceneMap.Bounds.Height ||
                    newPosition.Y - Projectiles[key].CollisionBox.CollisionRadius < 0)
                    return;

                Projectiles[key].Position = newPosition;
            }
        }

        private void UpdatePosition<T>(int key, float deltaTime, Dictionary<int, Vector2> collidables)
        {
            //Check for collisions if gameObjects in area
            if (collidables != null)
            {
                foreach (KeyValuePair<int, Vector2> c in collidables)
                {
                    Vector2 normal;
                    float depth;

                    if (typeof(T) == typeof(Player))
                    {
                        //Ensure object exists
                        if (!Players.ContainsKey(key)) return;

                        if (c.Key >= 0 && c.Key < 100) //Indicates a PLAYER
                        {
                            //Ensure object exists
                            if (!Players.ContainsKey(c.Key)) return;

                            if (c.Key != key && IsColliding(Players[key], Players[c.Key], out normal, out depth))
                            {
                                if (Players[key].IsStatic)
                                {
                                    Players[c.Key].Position += normal * depth;
                                }
                                else if (Players[c.Key].IsStatic)
                                {
                                    Players[key].Position += (-normal * depth);
                                }
                                ResolveCollision(Players[key], Players[c.Key], normal);
                            }
                        }

                        if (c.Key >= 100 && c.Key < 1000)
                        {
                            //Ensure object exists
                            if (!Consumables.ContainsKey(c.Key)) return;

                            if (c.Key != key && IsColliding(Players[key], Consumables[c.Key], out normal, out depth))
                            {
                                if (Players[key].IsStatic)
                                {
                                    Consumables[c.Key].Position += normal * depth;
                                }
                                else if (Consumables[c.Key].IsStatic)
                                {
                                    Players[key].Position += (-normal * depth);
                                }
                                ResolveCollision(Players[key], Consumables[c.Key], normal);
                            }
                        }

                        if (c.Key >= 1000 && c.Key < 10000)
                        {
                            //Ensure object exists
                            if (!Projectiles.ContainsKey(c.Key)) return;

                            if (c.Key != key && IsColliding(Players[key], Projectiles[c.Key], out normal, out depth))
                            {
                                if (Players[key].IsStatic)
                                {
                                    Projectiles[c.Key].Position += normal * depth;
                                }
                                else if (Projectiles[c.Key].IsStatic)
                                {
                                    Players[key].Position += (-normal * depth);
                                }
                                ResolveCollision(Players[key], Projectiles[c.Key], normal);
                            }
                        }

                        if (c.Key >= 10000)
                        {
                            //Ensure object exists
                            if (!Environmentals.ContainsKey(c.Key)) return;

                            if (c.Key != key && IsColliding(Players[key], Projectiles[c.Key], out normal, out depth))
                            {
                                if (Players[key].IsStatic)
                                {
                                    Projectiles[c.Key].Position += normal * depth;
                                }
                                else if (Projectiles[c.Key].IsStatic)
                                {
                                    Players[key].Position += (-normal * depth);
                                }
                                ResolveCollision(Players[key], Projectiles[c.Key], normal);
                            }
                        }
                        //Set new GameObject with new position
                        ClampPositionInBounds<Player>(key, deltaTime);
                    }

                    if (typeof(T) == typeof(Projectile))
                    {
                        if (c.Key >= 0 && c.Key <= 100)
                        {
                            //Ensure object exists
                            if (!Players.ContainsKey(key)) return;

                            if (c.Key != key && IsColliding(Consumables[key], Players[c.Key], out normal, out depth))
                            {
                                if (Consumables[key].IsStatic)
                                {
                                    Players[c.Key].Position += normal * depth;
                                }
                                else if (Players[c.Key].IsStatic)
                                {
                                    Consumables[key].Position += (-normal * depth);
                                }
                                ResolveCollision(Consumables[key], Players[c.Key], normal);
                            }
                        }
                        //Set new GameObject with new position
                        ClampPositionInBounds<Projectile>(key, deltaTime);
                    }
                }
            }
        }

        private bool IsColliding(Collidable bodyA, Collidable bodyB, out Vector2 normal, out float depth)
        {
            normal.X = (float)Math.Cos(FlatMath.GetAngle(bodyA.Position, bodyB.Position));
            normal.Y = (float)Math.Sin(FlatMath.GetAngle(bodyA.Position, bodyB.Position));
            if (!bodyA.CollisionBox.IsRectangular && !bodyB.CollisionBox.IsRectangular)
            {
                depth = (float)(Math.Sqrt(Math.Pow(bodyB.Position.X - bodyA.Position.X, 2) + Math.Pow(bodyB.Position.Y - bodyA.Position.Y, 2)) - bodyA.CollisionBox.CollisionRadius + bodyB.CollisionBox.CollisionRadius);
                return (Math.Sqrt(Math.Pow(bodyB.Position.X - bodyA.Position.X, 2) + Math.Pow(bodyB.Position.Y - bodyA.Position.Y, 2)) < bodyA.CollisionBox.CollisionRadius + bodyB.CollisionBox.CollisionRadius);
            }
            else
            {
                //Treat both objects like rectangles
                depth = Math.Abs(bodyB.Position.X - bodyA.Position.X) - (bodyA.CollisionBox.CollisionRadius + bodyB.CollisionBox.CollisionRadius);
                if (Math.Abs(bodyA.Position.Y - bodyB.Position.Y) - (bodyA.CollisionBox.CollisionRadius + bodyB.CollisionBox.CollisionRadius) < depth) depth = Math.Abs(bodyA.Position.Y - bodyB.Position.Y) - (bodyA.CollisionBox.CollisionRadius + bodyB.CollisionBox.CollisionRadius);
                return (bodyA.CollisionBox.CollisionRadius + bodyB.CollisionBox.CollisionRadius >= Math.Abs(bodyB.Position.X - bodyA.Position.X) && bodyA.CollisionBox.CollisionRadius + bodyB.CollisionBox.CollisionRadius >= Math.Abs(bodyA.Position.Y - bodyB.Position.Y));
            }
        }

        private void ResolveCollision(Collidable bodyA, Collidable bodyB, Vector2 normal)
        {
            Vector2 relativeVelocity = bodyB.Velocity - bodyA.Velocity;

            if (FlatMath.Dot(relativeVelocity, normal) > 0f)
            {
                return;
            }

            float e = MathF.Min(bodyA.Restitution, bodyB.Restitution);

            float j = -(1f + e) * FlatMath.Dot(relativeVelocity, normal);
            j /= (1 / bodyA.Mass) + (1 / bodyB.Mass);

            Vector2 impulse = j * normal;

            bodyA.Velocity -= impulse * (1 / bodyA.Mass);
            bodyB.Velocity += impulse * (1 / bodyB.Mass);
        }
    }
}
