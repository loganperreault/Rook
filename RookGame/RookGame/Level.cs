using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace GameOfPawns
{
    class Level
    {

        public Room room;
        protected ContentManager content;
        protected GraphicsDevice gd;
        protected Player player;

        public void Initialize(ContentManager content, GraphicsDevice gd, Player player)
        {
            this.content = content;
            this.gd = gd;
            this.player = player;
            room = new Room();
            room.Initialize(content, gd, player);
        }

        public virtual void LoadContent()
        {
            room.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            room.Scroll();
            room.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            room.Draw(spriteBatch);
        }

    }
}
