using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Timers;

namespace TotallyFair.Graphics
{
    public class Animation2D
    {
        public Queue<Texture2D> Animation = new Queue<Texture2D>(); //Textures to make up animation
        private Queue<Texture2D> AnimationBuffer = new Queue<Texture2D>();
        public Queue<double> DeltaTimes = new Queue<double>();      //Milliseconds to display current texture
        private Queue<double> DeltaTimesBuffer = new Queue<double>();
        public Timer AnimationTimer = new Timer();                  //Timer to cycle through queue and present new texture
        public bool Continuous = true;
        public bool Running = false;

        public void AddTexture(Texture2D NewTexture, double DeltaTime)
        {
            if (DeltaTime < 100) DeltaTime = 100;                   //Minimum display time for each animation
            Animation.Enqueue(NewTexture);
            DeltaTimes.Enqueue(DeltaTime);
        }

        public void DeleteAnimations()
        {
            Stop();
            Animation = new Queue<Texture2D>();
            DeltaTimes = new Queue<double>();
        }

        public void Start()
        {
            if (DeltaTimes.Count == 0) return;                      //Cannot start if no times/animations have been added.
            Running = true;
            //Set Buffers to save initial animation/time queue
            AnimationBuffer = Animation;
            DeltaTimesBuffer = DeltaTimes;
            AnimationTimer.Interval = DeltaTimes.Peek();
            AnimationTimer.Elapsed += new ElapsedEventHandler(TimerTick);
            AnimationTimer.Enabled = true;
            AnimationTimer.Start();
        }

        private void TimerTick(object Sender, ElapsedEventArgs Args)
        {
            //Cycle through Texture & DeltaTime Queues
            Cycle();
            //Set Interval to new DeltaTime
            if (DeltaTimes.Count > 0) AnimationTimer.Interval = DeltaTimes.Peek();
            else Stop(); //Stop if queue is empty
        }

        private void Cycle()
        {
            if (Continuous)
            {
                Animation.Enqueue(Animation.Peek());
                DeltaTimes.Enqueue(DeltaTimes.Peek());
            }
            Animation.Dequeue();
            DeltaTimes.Dequeue();
        }

        public void Stop()
        {
            AnimationTimer.Enabled = false;
            Running = false;
            Animation = AnimationBuffer;
            DeltaTimes = DeltaTimesBuffer;
        }
    }
}
