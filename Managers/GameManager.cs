using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TotallyFair.GameComponents;
using TotallyFair.Graphics;

namespace TotallyFair.Managers
{
    /// <summary>
    /// Game Manager Object:
    /// Responsible for managing all Game Object (Players, Consumables, Computers, etc.).
    /// Manages positions of objects, collisions, and bounds of game window.
    /// Keep objects positioned where they are supposed to be.
    /// </summary>
    public class GameManager
    {
        public struct PlayerData
        {
            public Vector2 Position;
            public Size CollisionBox;
            public Player GameObject;
        }

        public struct ConsumableData
        {
            public Vector2 Position;
            public Size CollisionBox;
            public Consumable GameObject;

            public ConsumableData(Texture2D texture, Vector2 position)
            {
                GameObject = new Consumable();
                Position = position;
                CollisionBox = new(texture.Width, texture.Height / 2);
            }
        }


        //Int code to represent Player ID
        public Dictionary<int, PlayerData> Players = new Dictionary<int, PlayerData>();
        public Dictionary<int, ConsumableData> Consumables = new Dictionary<int, ConsumableData>();

        public void AddPlayer(int id, Vector2 position, AnimationState animationState, Texture2D[] textures, float deltaTime, bool continuous)
        {
            PlayerData data = new PlayerData();
            data.GameObject = new Player($"Player{id}", position, animationState, textures, deltaTime, continuous);
            data.Position = position;
            data.CollisionBox = new(textures[0].Width, textures[0].Height / 2);
            if (!Players.ContainsKey(id)) Players.Add(id, data); 
        }

        public void AddConsumable(Texture2D texture, Vector2 position)
        {
            ConsumableData data = new ConsumableData(texture, position);
            Consumables.Add(Consumables.Count, data);
        }

        public void RemovePlayer(int id)
        {
            if (!Players.ContainsKey(id)) return;
            Players.Remove(id);
        }

        public void RemoveConsumable(int id)
        {
            if (!Consumables.ContainsKey(id)) return;
            Consumables.Remove(id);
        }
    }
}
