using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace GameOfPawns
{

    public class Room
    {

        protected ContentManager content;
        protected GraphicsDevice gd;
        protected Player player;
        protected int margin;

        protected List<Enemy> enemies;

        public Object area;
        public int Width, Height;

        public void Initialize(ContentManager content, GraphicsDevice gd, Player player)
        {
            this.content = content;
            this.gd = gd;
            this.player = player;
            this.enemies = new List<Enemy>();
            if (this.Width <= 0)
                this.Width = gd.Viewport.Width;
            if (this.Height <= 0)
                this.Height = gd.Viewport.Height;
            area = new Object(0, 0);
            margin = gd.Viewport.Width / 4;
        }

        public virtual void LoadContent() {
            area.setRoom(this);
        }

        public void Scroll()
        {
            int screenWidth = GameOfPawns.target.Width;
            if (player.Position.X > screenWidth - margin)
            {
                float bump = ((screenWidth - margin) - player.Position.X);
                // check right bound
                if (screenWidth - (area.Position.X+bump) > this.Width)
                    bump = screenWidth - area.Position.X - this.Width;
                area.scrollHorizontal(bump);
                player.Position.X = screenWidth - margin;
            } 
            else if (player.Position.X < margin)
            {
                float bump = (margin - player.Position.X);
                // check left bound
                if (area.Position.X + bump > 0)
                    bump = area.Position.X;
                area.scrollHorizontal(bump);
                player.Position.X = margin;
            }
        }

        public void addEnemy(Enemy enemy)
        {
            enemies.Add(enemy);
        }

        public void removeEnemy(Enemy enemy)
        {
            enemies.Remove(enemy);
        }

        public void setSize(int width)
        {
            this.Width = width;
        }

        public void setSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void Update(GameTime gameTime)
        {
            area.Update(gameTime);
            if (enemies.Count == 0)
            {
                GameOfPawns.player.setSpeedWalk();
            }
            else
            {
                GameOfPawns.player.setSpeedRun();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            area.Draw(spriteBatch);
        }

        protected void createArea(int width, int height)
        {
            area = new Object(width, height);
            area.setRoom(this);
            area.Initialize();
        }

        public void EnemyCheckin(Enemy enemy)
        {

        }

    }
}
