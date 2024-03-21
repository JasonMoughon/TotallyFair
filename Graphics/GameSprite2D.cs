using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TotallyFair.Graphics
{
    public class GameSprite2D
    {
        public Dictionary<AnimationState, Animation2D> AnimationLibrary = new Dictionary<AnimationState, Animation2D>();
        public AnimationState CurrentState { get; private set; } = AnimationState.IDLE;
        public Vector2 Position = new Vector2();
        public float Rotation = 0;
        public bool Visible = false;

        public void AddAnimation(AnimationState Name, Texture2D[] Textures, double[] DeltaTimes, bool Continuous)
        {
            //Need equivalent number of DeltaTimes as Textures
            if (Textures.Length != DeltaTimes.Length) return;

            AnimationLibrary.Add(Name, new Animation2D());
            AnimationLibrary[Name].Continuous = Continuous;
            for (int i = 0; i < Textures.Length; i++) AnimationLibrary[Name].AddTexture(Textures[i], DeltaTimes[i]);
        }

        public void RemoveAnimation(AnimationState Name)
        {  
            AnimationLibrary.Remove(Name);
        }

        public void ChangeAnimationState(AnimationState State)
        {
            if (AnimationLibrary.ContainsKey(State))
            {
                AnimationLibrary[CurrentState].Stop();
                CurrentState = State;
            }
        }

        public void Start()
        {
            AnimationLibrary[CurrentState].Start();
        }

        public void Play(SpriteBatch Batch)
        {
            if (AnimationLibrary.ContainsKey(CurrentState))
            {
                if (!AnimationLibrary[CurrentState].Running) Start();
                Update(Batch);
            }
        }

        public void Update(SpriteBatch Batch)
        {
            if (!IsRunning()) Visible = false;
            Texture2D CurrentTexture = AnimationLibrary[CurrentState].Animation.Peek();
            Batch.Draw(CurrentTexture, Position, null, Color.White, Rotation, new Vector2(CurrentTexture.Width, CurrentTexture.Height), 1, SpriteEffects.None, 0);
        }

        private bool IsRunning()
        {
            if (!AnimationLibrary.ContainsKey(CurrentState)) return false;
            return AnimationLibrary[CurrentState].Running;
        }
    }
}
