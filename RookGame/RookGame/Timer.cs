using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameOfPawns
{
    public class Timer
    {

        Random random = GameOfPawns.random;
        TimeSpan timer;
        TimeSpan time;
        float length;
        float margin;
        bool on;

        public Timer(float length)
        {
            this.length = length;
            this.margin = 0.0f;
            this.on = false;
        }

        public Timer(float length, float margin)
        {
            this.length = length;
            this.margin = margin;
            this.on = false;
        }

        // stop counting and reset
        public void Stop()
        {
            timer = TimeSpan.Zero;
            on = false;
        }

        // stop counting, but don't reset
        public void Pause()
        {
            on = false;
        }

        public void Restart()
        {
            on = true;
            float generatedTime = (float)(random.NextDouble() * (2 * margin) + (length - margin));
            time = TimeSpan.FromSeconds(generatedTime);
            timer = TimeSpan.Zero;
        }

        public void Go() {
            if (!on)
                Restart();
        }

        public void Update(GameTime gameTime)
        {
            if (on)
                timer += gameTime.ElapsedGameTime;
        }

        public bool Check()
        {
            return (on && timer > time);
        }

    }
}
