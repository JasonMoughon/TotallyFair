using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotallyFair.CONFIG
{
    internal class PLAYER_CONFIG
    {
        public bool IS_CPU { get; } = true;

        public string NAME { get; } = "COMPUTER";

        public int DEFAULT_HAND_SIZE { get; } = 2;

        public int DEFAULT_DEFENSIVE_STAT { get; } = 5;
        public int MAX_DEFENSIVE_STAT { get; } = 10;
        public int MIN_DEFENSIVE_STAT { get; } = 0;

        public int DEFAULT_OFFENSIVE_STAT { get; } = 5;
        public int MAX_OFFENSIVE_STAT { get; } = 10;
        public int MIN_OFFENSIVE_STAT { get; } = 0;

        public int DEFAULT_HEALTH { get; } = 100;
        public int MIN_TOTAL_HEALTH { get; } = 20;
        public int MAX_TOTAL_HEALTH { get; } = 10000;

        public double DEFAULT_FORGET_CHANCE { get; } = 0.5;
        public double DEFAULT_FULLSCALE_FORGET_CHANCE { get; } = 1.0;

        public int MAX_VELOCITY { get; } = 25; //Pixels per Second
    }
}
