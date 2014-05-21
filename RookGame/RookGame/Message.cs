using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfPawns
{
    public class Message
    {

        public String text;                     // The text to be displayed on screen
        public Vector2 position;                // The position of the message relative to the top left corner of the screen
        public SpriteFont font;                 // The font to be used for the message
        public Color color;                     // The color to be used for the message
        public bool active;                     // The state of the message
        bool timeCheck;                         // Whether or not the text is displayed for a discrete time
        TimeSpan lifetime;                      // The time the message will be displayed on screen for
        TimeSpan startTime;                     // The start time of the message

        /// <summary>
        /// Creates a new message.
        /// </summary>
        /// <param name="text">The text that should be displayed.</param>
        /// <param name="font">The font that should be used</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The position of the text relative to top left.</param>
        public void Initialize(String text, SpriteFont font, Color color, Vector2 position) {
            this.text = text;
            this.font = font;
            this.color = color;
            this.position = position;

            this.active = true;
            this.timeCheck = false;
            this.lifetime = TimeSpan.FromSeconds(0);
        }

        public void setTimeLimit(TimeSpan lifetime, GameTime gameTime) {
            this.timeCheck = true;
            this.lifetime = lifetime;
            this.startTime = gameTime.TotalGameTime;
        }

        public void Update(GameTime gameTime)
        {
            // Check if the message's lifetime has expired
            if (active==true && timeCheck==true) {
                if ((gameTime.TotalGameTime-startTime) < lifetime) {
                } else {
                    active = false;
                }
            }

            // TODO: maybe a key check?
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (active == true) {
                spriteBatch.DrawString(font, text, position, color);
            }
            if (timeCheck == false)
                active = false;
        }

    }
}
