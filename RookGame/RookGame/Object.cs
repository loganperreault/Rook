using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace GameOfPawns
{
    public class Object
    {

        // Graphics
        protected ContentManager content;
        protected GraphicsDevice gd;

        public List<Object> objects;
        String texture_name;
        public Texture2D texture;
        public Animation animation;
        public bool exists;
        public bool solid;
        protected Object parent;
        protected Object root;
        protected Room room;
        public bool initialized;

        public Vector2 Position;

        public int Width, Height;

        public int flip;

        public virtual void Initialize()
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
            solid = false;
            if (parent == null)
                root = this;
            else
                root = parent.root;

            initialized = true;
        }

        public void setFlip(int flip)
        {
            if (flip == 1 || flip == -1)
            {
                this.flip = flip;
                for (int i = 0; i < objects.Count; i++)
                {
                    objects[i].setFlip(flip);
                }
            }
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)this.Position.X, (int)this.Position.Y, Width, Height);
        }

        public void setRoot(Object root)
        {
            this.root = root;
        }

        public void setRoom(Room room)
        {
            this.room = room;
        }

        public void setParent(Object parent)
        {
            this.parent = parent;
        }

        public void setSolid(bool solid)
        {
            this.solid = solid;
        }

        public virtual void Dispose() { }

        public virtual void LoadContent()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].LoadContent();
            }
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
                if (objects[i].exists)
                {
                    objects[i].Update(gameTime);
                }
                else
                {
                    objects[i].Dispose();
                    objects.RemoveAt(i);
                }
            }
        }

        public Object(float x, float y) {
            setPosition(x, y);
            texture_name = null;
            exists = true;
        }

        public float checkCollision(float request, bool facing)
        {
            float position = request;
            if (this.solid)
            {
                // check collision and return if found
                // position = rectangle collision result
                
                // right
                if (facing)
                {
                    if (request > Position.X)
                    {
                        GameOfPawns.echo("COLLISION RIGHT");
                    }
                }
                else
                {
                    if (request < Position.X + Width)
                    {
                        GameOfPawns.echo("COLLISION LEFT");
                    }
                }
            }
            // check all sub objects
            if (objects!=null)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    position = objects[i].checkCollision(request, facing);
                }
            }
            return position;
        }

        public void addTexture(String imagename)
        {
            texture_name = imagename;
        }

        public void addAnimation(Animation anim)
        {
            animation = anim;
        }

        public Object addObject(float x, float y, String imagename) {
            return addObject(x, y, imagename, 0);
        }

        public Object addObject(float x, float y, String imagename, int transparent) {
            return addObject(x, y, imagename, transparent, 1, 1);
        }

        public Object addObject(float x, float y, String imagename, int repeatX, int repeatY) {
            return addObject(x, y, imagename, 0, repeatX, repeatY);
        }

        public Object addObject(float x, float y, String imagename, int transparent, int repeatX, int repeatY)
        {
            Object obj = new Object(x, y);
            obj.addTexture(imagename);
            addObject(obj);
            if (repeatX > 1)
                addObject(x + obj.texture.Width, y, imagename, repeatX - 1, repeatY);
            if (repeatY > 1)
                addObject(x, y + obj.texture.Height, imagename, repeatX, repeatY - 1);
            return obj;
        }

        public void addObject(Object obj) {
            obj.setParent(this);
            obj.setRoom(room);
            if (!obj.initialized)
                obj.Initialize();
            obj.setPosition(this.Position.X + obj.Position.X, this.Position.Y - obj.Position.Y);
            obj.setFlip(flip);
            objects.Add(obj);
        }

        public void setSize(int width, int height) {
            if (width > 0)
                this.Width = width;
            if (height > 0)
                this.Height = height;
        }

        public void setPosition(float x, float y) {
            if (objects != null)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    objects[i].setPosition(objects[i].Position.X + Position.X + x, objects[i].Position.Y - Position.Y + y);
                }
            }
            Position.X = x;
            Position.Y = y;
        }

        public void scrollHorizontal(float moveX)
        {
            if (moveX == 0)
                return;
            this.Position.X += moveX;
            if (objects != null) {
                for (int i = 0; i < objects.Count; i++) {
                    objects[i].scrollHorizontal(moveX);
                }
            }
        }

        public void scrollVertical(float moveY)
        {
            if (moveY == 0)
                return;
            this.Position.Y += moveY;
            if (objects != null) {
                for (int i = 0; i < objects.Count; i++) {
                    objects[i].scrollVertical(moveY);
                }
            }
        }
        
        public virtual void Draw(SpriteBatch spriteBatch)
        {

            SpriteEffects effect = SpriteEffects.None;
            if (flip == -1)
                effect = SpriteEffects.FlipHorizontally;
            if (texture != null)
                spriteBatch.Draw(texture, new Vector2(this.Position.X, this.Position.Y-(texture.Height/2)), null, Color.White, 0f, new Vector2(50 / 2, 50 / 2), 1f, effect, 0f);

            if (animation != null)
            {
                if (flip == 1)
                    animation.setFlip(false);
                else if (flip == -1)
                    animation.setFlip(true);
                animation.Draw(spriteBatch);
            }

            if (objects != null) {
                for (int i = 0; i < objects.Count; i++) {
                    objects[i].Draw(spriteBatch);
                }
            }

        }

    }
}
