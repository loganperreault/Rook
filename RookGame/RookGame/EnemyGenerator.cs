using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace GameOfPawns
{
    public class EnemyGenerator : Object
    {

        protected int maxEnemies;
        protected int maxEnemiesTotal;

        List<Enemy> Enemies;
        List<Timer> Timers;
        List<float> PositionMargins;

        public EnemyGenerator(int x, int y) : base(x, y)
        {
            Enemies = new List<Enemy>();
            Timers = new List<Timer>();
            PositionMargins = new List<float>();
            maxEnemies = 1;
            maxEnemiesTotal = 1000;
        }

        public void setMaxEnemies(int maxEnemies)
        {
            this.maxEnemies = maxEnemies;
        }

        public void setMaxEnemiesTotal(int maxEnemiesTotal)
        {
            this.maxEnemiesTotal = maxEnemiesTotal;
        }

        public override void Initialize()
        {
            base.Initialize();
            
        }

        /// <summary>
        /// Add a type of Enemy to the pool of generated enemies.
        /// </summary>
        /// <param name="enemy">The enemy object to add to the pool.</param>
        /// <param name="time">The time between enemy generations.</param>
        public void addEnemy(Enemy enemy, float time)
        {
            addEnemy(enemy, time, 0.0f, 0.0f);
        }

        /// <summary>
        /// Add a type of Enemy to the pool of generated enemies.
        /// </summary>
        /// <param name="enemy">The enemy object to add to the pool.</param>
        /// <param name="time">The time between enemy generations.</param>
        /// <param name="timeMargin">The margin of randomness between generation times.</param>
        /// <param name="positionMargin">The margin of the position at which each enemy is generated.</param>
        public void addEnemy(Enemy enemy, float time, float timeMargin, float positionMargin)
        {
            Enemies.Add(enemy);
            Timer timer = new Timer(time, timeMargin);
            timer.Go();
            Timers.Add(timer);
            GameOfPawns.echo(Timers.Count.ToString());
            PositionMargins.Add(positionMargin);
        }

        public override void Update(GameTime gameTime)
        {
            // update timers and add enemies as needed
            if (objects.Count < maxEnemies)
            {
                for (int i = 0; i < Timers.Count; i++)
                {
                    Timers[i].Update(gameTime);
                }
                addEnemies();
            }
            // update existing enemies on map
            base.Update(gameTime);
        }

        private void addEnemies()
        {
            bool added = false;
            for (int i = 0; i < Timers.Count; i++)
            {
                if (Timers[i].Check())
                {
                    // TODO: call function that grabs position offset. Also save original position so it doesn't keep changing out of margin range.
                    Enemy enemy = (Enemy)Enemies[i].Clone();
                    addObject(enemy);
                    enemy.LoadContent();
                    added = true;
                }
            }

            if (added)
            {
                for (int i = 0; i < Timers.Count; i++)
                {
                    Timers[i].Restart();
                }
            }
        }

    }
}
