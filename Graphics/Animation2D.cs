using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Timers;

namespace TotallyFair.Graphics
{
    public class Animation2D
    {
        
        public Texture2D[] Animation;
        public float DeltaTime;
        private int _Index = 0;
        public Timer AnimationTimer = new Timer();                  //Timer to cycle through queue and present new texture
        public bool Continuous = true;
        public bool Running = false;

        public Animation2D()
        {
            AnimationTimer.Elapsed += new ElapsedEventHandler(TimerTick);
        }

        public void AddAnimation(Texture2D[] newTextures, float newTime)
        {
            Animation = newTextures;
            DeltaTime = newTime;
            if (DeltaTime < 10) DeltaTime = 10;    //Minimum display time for each texture
        }

        public void DeleteAnimations()
        {
            Stop();
            Array.Resize(ref Animation, 0);
        }

        public void Start()
        {
            if (DeltaTime == 0f) return;                     //Cannot start if no times/animations have been added.
            AnimationTimer.Interval = DeltaTime;
            Running = true;
            AnimationTimer.Start();
            _Index = 0;
        }

        private void TimerTick(Object sender, ElapsedEventArgs args)
        {
            if (!Running) return;
            _Index += 1;
            if (_Index >= Animation.Length)
            {
                if (Continuous) _Index = 0;
                else Stop();
            }
        }

        public void Stop()
        {
            Running = false;
            AnimationTimer.Stop();
            _Index = 0;        
        }

        public Texture2D GetCurrentTexture()
        {
            if (Animation.Length == 0) return null;

            //Sanity Check
            if (_Index >= Animation.Length) _Index = Animation.Length - 1;
            if (_Index < 0) _Index = 0;

            return Animation[_Index];
        }
    }
}
