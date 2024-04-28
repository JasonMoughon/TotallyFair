using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotallyFair.CONFIG;
using TotallyFair.Graphics;

namespace TotallyFair.GameComponents
{
    public class Player
    {
        private static PLAYER_CONFIG DEFAULT_PLAYER = new PLAYER_CONFIG();

        ///////////////////////////////////////////////////////////////////////
        // Player parameters useful for user and computer players.           //
        ///////////////////////////////////////////////////////////////////////
        public bool GameWon = false; //No Default Config for obvious reasons
        public bool IsCPU = DEFAULT_PLAYER.IS_CPU;
        public bool IsAttacking = false;
        public string Name = DEFAULT_PLAYER.NAME;
        public Card[] Hand = new Card[DEFAULT_PLAYER.DEFAULT_HAND_SIZE];
        public int DefensiveStat = DEFAULT_PLAYER.DEFAULT_DEFENSIVE_STAT;
        public int OffensiveStat = DEFAULT_PLAYER.DEFAULT_OFFENSIVE_STAT;
        public int TotalHealth = DEFAULT_PLAYER.DEFAULT_HEALTH;
        public int HealthCapacity = DEFAULT_PLAYER.DEFAULT_HEALTH;
        public GameSprite2D Sprite;
        public Vector2 Velocity = new Vector2(0, 0);
        public float Mass = 5f;

        ///////////////////////////////////////////////////////////////////////
        // Player parameters useful for computer players.                    //
        ///////////////////////////////////////////////////////////////////////
        public int HandWeight;
        public Dictionary<string, Card[]> OpponentHands;

        public Player(string playerName, Vector2 position, AnimationState state, Texture2D[] textures, float deltaTime, bool continuous)
        {
            Name = playerName;
            Sprite = new GameSprite2D(position, state, textures, deltaTime, continuous);
        }
        public void AddDefensiveStats(int Stat)
        {
            DefensiveStat += Stat;

            //Clamp to allowable range
            if (DefensiveStat < DEFAULT_PLAYER.MIN_DEFENSIVE_STAT) DefensiveStat = DEFAULT_PLAYER.MIN_DEFENSIVE_STAT;
            if (DefensiveStat > DEFAULT_PLAYER.MAX_DEFENSIVE_STAT) DefensiveStat = DEFAULT_PLAYER.MAX_DEFENSIVE_STAT;
        }

        public void AddOffensiveStat(int Stat)
        {
            OffensiveStat += Stat;

            //Clamp to allowable range
            if (OffensiveStat < DEFAULT_PLAYER.MIN_OFFENSIVE_STAT) OffensiveStat = DEFAULT_PLAYER.MIN_OFFENSIVE_STAT;
            if (OffensiveStat > DEFAULT_PLAYER.MAX_OFFENSIVE_STAT) OffensiveStat = DEFAULT_PLAYER.MAX_OFFENSIVE_STAT;
        }

        public void AddHealth(int Stat)
        {
            TotalHealth += Stat;

            //Clamp to allowable range
            if (TotalHealth <= 0) TotalHealth = 0;
            if (TotalHealth >= HealthCapacity) TotalHealth = HealthCapacity;

        }

        public void IncreaseHealthCapacity(int Stat)
        {
            HealthCapacity += Stat;

            //Clamp to allowable range
            if (HealthCapacity <= DEFAULT_PLAYER.MIN_TOTAL_HEALTH) HealthCapacity = DEFAULT_PLAYER.MIN_TOTAL_HEALTH;
            if (HealthCapacity > DEFAULT_PLAYER.MAX_TOTAL_HEALTH) HealthCapacity = DEFAULT_PLAYER.MAX_TOTAL_HEALTH;
        }

        public void TakeDamage(int Opp_OffensiveStat)
        {
            //Defend from Edge Cases
            if (DEFAULT_PLAYER.MAX_OFFENSIVE_STAT == 0) return;
            if (Opp_OffensiveStat > DEFAULT_PLAYER.MAX_OFFENSIVE_STAT) Opp_OffensiveStat = DEFAULT_PLAYER.MAX_OFFENSIVE_STAT;

            Random Rand = new Random();

            double Damage = (Rand.NextDouble() * 0.4 + 0.8) * Opp_OffensiveStat - DefensiveStat;
            if (Damage <= 0) Damage = 1; //Minimum Damage

            TotalHealth -= (int)Math.Ceiling(Damage);
        }

        public void AddCardToHand(Card NewCard, int Index)
        {
            if (Index < 0 || Index >= DEFAULT_PLAYER.DEFAULT_HAND_SIZE || NewCard == null) return;

            Hand[Index] = NewCard;
        }

        public void ClearHand()
        {
            for (int i = 0; i < Hand.Length; i++) Hand[i] = null;
        }

        private void CalculateHandWeight(Card[] MyHand)
        {
            HandWeight = 0;
            int[] SortedHand = new int[2];
            int HandDifference;
            int Bonus = 0;

            if (MyHand == null) return;

            SortedHand = SortIncreasing(MyHand[0].FaceValue.GetHashCode(), MyHand[1].FaceValue.GetHashCode());
            HandDifference = SortIncreasing(Math.Abs(SortedHand[1] - SortedHand[0]), Math.Abs(SortedHand[0] + 13 - SortedHand[1]))[0];

            if (HandDifference == 0) Bonus += 10; //Same FaceValue
            if (HandDifference <= 3) Bonus += 5; //Potential for Straight
            if (MyHand[0].Suit.Equals(MyHand[1].Suit)) Bonus += 10; //Same Suit

            HandWeight = MyHand[0].FaceValue.GetHashCode() + MyHand[1].FaceValue.GetHashCode() + Bonus - HandDifference;
        }

        private int[] SortIncreasing(int Int1, int Int2)
        {
            int[] ResultArr = new int[2];
            if (Int1 <= Int2)
            {
                ResultArr[0] = Int1;
                ResultArr[1] = Int2;
            }
            else
            {
                ResultArr[0] = Int2;
                ResultArr[1] = Int1;
            }
            return ResultArr;
        }

        public void InitializeOpponents(int TotalOpponents)
        {
            for (int i = 0; i < TotalOpponents; i++) OpponentHands.Add($"Opponent{i}", new Card[TotalOpponents]);
        }

        public void ForgetOpponentHand()
        {
            if (OpponentHands == null) return;
            foreach (KeyValuePair<string, Card[]> OpponentHand in OpponentHands)
            {
                for (int i = 0; i < OpponentHand.Value.Length; i++) if (ChanceEvent(0.5, 1.0)) OpponentHand.Value[i] = null;
            }
        }

        public bool ChanceEvent(double Threshold, double Fullscale)
        {
            Random RandomChance = new Random();
            return RandomChance.NextDouble() * Fullscale > Threshold;
        }

        public void AddExternalVelocity(Vector2 velocity, float mass, float deltaTime)
        {
            Velocity = (Mass * Velocity / (Mass + mass)) * deltaTime;
        }

        public void UpdateVelocity(Vector2 velocity, float deltaTime)
        {
            Velocity += velocity;
            //Scrub X Velocity
            if (Math.Abs(Velocity.X) > DEFAULT_PLAYER.MAX_VELOCITY && Velocity.X < 0) Velocity.X = -DEFAULT_PLAYER.MAX_VELOCITY;
            if (Math.Abs(Velocity.X) > DEFAULT_PLAYER.MAX_VELOCITY && Velocity.X > 0) Velocity.X = DEFAULT_PLAYER.MAX_VELOCITY;
            //Scrub Y Velocity
            if (Math.Abs(Velocity.Y) > DEFAULT_PLAYER.MAX_VELOCITY && Velocity.Y < 0) Velocity.Y = -DEFAULT_PLAYER.MAX_VELOCITY;
            if (Math.Abs(Velocity.Y) > DEFAULT_PLAYER.MAX_VELOCITY && Velocity.Y > 0) Velocity.Y = DEFAULT_PLAYER.MAX_VELOCITY;
        }

        public void Chase(Vector2 Target, float deltaTime)
        {
            Vector2 Velocity = new( Target.X - Sprite.Position.X , Target.Y - Sprite.Position.Y );
            float c = (float)Math.Sqrt(Math.Pow(Velocity.X, 2) + Math.Pow(Velocity.Y, 2));
            UpdateVelocity(new Vector2((500 / c) * Velocity.X, (500 / c) * Velocity.Y), deltaTime);
        }

        public void Update()
        {
            //Update Velocity
            Vector2 UpdateVelocity = new Vector2(0,0);
            if (Math.Abs(Velocity.X) >= 10) UpdateVelocity.X = -Velocity.X * (float)0.15;
            else UpdateVelocity.X = -Velocity.X;
            if (Math.Abs(Velocity.Y) >= 10) UpdateVelocity.Y = -Velocity.Y * (float)0.15;
            else UpdateVelocity.Y = -Velocity.Y;
            this.UpdateVelocity(UpdateVelocity, (float)0.001);

            //Update AnimationState
            switch (Sprite.CurrentState)
            {
                case AnimationState.IDLE:
                    if (Velocity.X < 0) Sprite.ChangeAnimationState(AnimationState.RUNNINGLEFT);
                    if (Velocity.X > 0) Sprite.ChangeAnimationState(AnimationState.RUNNINGRIGHT);
                    if (Velocity.X == 0 && Velocity.Y < 0) Sprite.ChangeAnimationState(AnimationState.RUNNINGLEFT);
                    if (Velocity.X == 0 && Velocity.Y > 0) Sprite.ChangeAnimationState(AnimationState.RUNNINGRIGHT);
                    break;
                case AnimationState.RUNNINGLEFT:
                    if (Velocity.X == 0 && Velocity.Y == 0) Sprite.ChangeAnimationState(AnimationState.IDLE);
                    if (Velocity.X > 0) Sprite.ChangeAnimationState(AnimationState.RUNNINGRIGHT);
                    break;
                case AnimationState.RUNNINGRIGHT:
                    if (Velocity.X == 0 && Velocity.Y == 0) Sprite.ChangeAnimationState(AnimationState.IDLE);
                    if (Velocity.X < 0) Sprite.ChangeAnimationState(AnimationState.RUNNINGLEFT);
                    break;

            }
        }
    }
}
