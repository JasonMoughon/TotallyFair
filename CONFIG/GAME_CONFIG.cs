using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotallyFair.GameComponents;

namespace TotallyFair.CONFIG
{
    public class GAME_CONFIG
    {
        public int DEFAULT_PLAYER_AMOUNT { get; } = 6;
        public int MAX_PLAYER_AMOUNT { get; } = 6;
        public int MIN_PLAYER_AMOUNT { get; } = 2;

        public Dictionary<int, Vector2[]> STARTING_POSITIONS;
        
        public GAME_CONFIG(Map map)
        {
            STARTING_POSITIONS = new Dictionary<int, Vector2[]>()
        {
            { 2, new Vector2[2]{ new Vector2(map.Bounds.Width / 2, map.Bounds.Height - 50), new Vector2(map.Bounds.Width / 2, 50) } },
            { 3, new Vector2[3]{ new Vector2(map.Bounds.Width / 2, map.Bounds.Height - 50), new Vector2(40, map.Bounds.Height / 3), new Vector2(map.Bounds.Width - 40, map.Bounds.Height / 3) } },
            { 4, new Vector2[4]{ new Vector2(map.Bounds.Width / 2, map.Bounds.Height - 50), new Vector2(40, map.Bounds.Height / 2), new Vector2(map.Bounds.Width / 2, 20), new Vector2(map.Bounds.Width - 40, map.Bounds.Height / 2) } },
            { 5, new Vector2[5]{ new Vector2(map.Bounds.Width / 2, map.Bounds.Height - 50), new Vector2(40, map.Bounds.Height * 3 / 4), new Vector2(40, map.Bounds.Height / 4), new Vector2(map.Bounds.Width - 80, map.Bounds.Height / 4), new Vector2(map.Bounds.Width - 80, map.Bounds.Height * 3 / 4) } },
            { 6, new Vector2[6]{ new Vector2(map.Bounds.Width / 2, map.Bounds.Height - 50), new Vector2(40, map.Bounds.Height * 3 / 4), new Vector2(40, map.Bounds.Height / 4), new Vector2(map.Bounds.Width / 2, 20), new Vector2(map.Bounds.Width - 80, map.Bounds.Height / 4), new Vector2(map.Bounds.Width - 80, map.Bounds.Height * 3 / 4) } },
        };
        }
    }
}
