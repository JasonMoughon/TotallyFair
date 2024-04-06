using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;

namespace TotallyFair.Graphics
{
    public class GameSprite2D
    {
        public Dictionary<AnimationState, Animation2D> AnimationLibrary = new Dictionary<AnimationState, Animation2D>();
        public AnimationState CurrentState { get; private set; }
        public Size SpriteSize;
        public Vector2 Position;
        public float Rotation;
        public bool Visible;

        public GameSprite2D(Vector2 position, AnimationState state, Texture2D[] textures, float deltaTime, bool continuous)
        {
            Position = position;
            CurrentState = state;
            AddAnimation(state, textures, deltaTime, continuous);
        }

        public void AddAnimation(AnimationState state, Texture2D[] textures, float deltaTime, bool continuous)
        {
            SpriteSize = new Size(textures[0].Width, textures[0].Height);
            AnimationLibrary.Add(state, new Animation2D());
            AnimationLibrary[state].Continuous = continuous;
            AnimationLibrary[state].AddAnimation(textures, deltaTime);
        }

        public void RemoveAnimation(AnimationState state)
        {  
            AnimationLibrary.Remove(state);
        }

        public void ChangeAnimationState(AnimationState state)
        {
            if (AnimationLibrary.ContainsKey(state))
            {
                AnimationLibrary[CurrentState].Stop();
                CurrentState = state;
            }
        }

        public void Start()
        {
            AnimationLibrary[CurrentState].Start();
        }

        public void Play(SpriteBatch batch)
        {
            if (AnimationLibrary.ContainsKey(CurrentState))
            {
                if (!AnimationLibrary[CurrentState].Running) Start();
                Update(batch);
            }
        }

        public void Update(SpriteBatch batch)
        {
            if (!IsRunning()) Visible = false;
            Texture2D CurrentTexture = AnimationLibrary[CurrentState].GetCurrentTexture();
            batch.Draw(CurrentTexture, Position, null, Microsoft.Xna.Framework.Color.White, Rotation, new Vector2(CurrentTexture.Width/2, CurrentTexture.Height/2), 1, SpriteEffects.None, 1f);
        }

        private bool IsRunning()
        {
            if (!AnimationLibrary.ContainsKey(CurrentState)) return false;
            return AnimationLibrary[CurrentState].Running;
        }
    }
}
