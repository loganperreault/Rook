using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace GameOfPawns
{
    public class Item
    {

        // Graphics
        protected ContentManager content;
        protected GraphicsDevice gd;

        List<Object> objects;
        String texture_name;
        protected Texture2D texture;
        protected Animation animation;

        public Vector2 Position;

        public int Width, Height;

        public int flip;

        public void Initialize()
        {
            this.content = GameOfPawns.cm;
            this.gd = GameOfPawns.gd;
            if (texture_name == null)
                texture = null;
            else
                texture = content.Load<Texture2D>(texture_name);
            objects = new List<Object>();
            Width = gd.Viewport.Width;
            Height = gd.Viewport.Height;
            flip = 1;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (animation!=null)
            {
                animation.setPosition(new Vector2(0, 0));
                animation.setPosition(Position);
                animation.Update(gameTime);
            }

            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].Equals(GameOfPawns.player))
                    GameOfPawns.player.Update(gameTime);
                else
                    objects[i].Update(gameTime);
            }

        }

        public void LoadContent()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].LoadContent();
            }
        }

    }
}
