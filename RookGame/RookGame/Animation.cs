// Animation.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameOfPawns {
    public class Animation {

        // Declarations
        Texture2D spriteStrip;                              // The image representing the collection of images used for animation
        float scale;                                        // The scale used to display the sprite strip
        int elapsedTime;                                    // The time since we last updated the frame
        int frameTime;                                      // The time we display a frame until the next one
        int[] frameTimes;                                   // The times each individual frame will display for (if inconsistant)
        bool manual;                                        // Whether or not to use manual frame times
        public int frameCount;                                     // The number of frames that the animation contains
        public int currentFrame;                            // The index of the current frame we are displaying
        Color color;                                        // The color of the frame we will be displaying
        Rectangle sourceRect = new Rectangle();             // The area of the image strip we want to display
        Rectangle destinationRect = new Rectangle();        // The area where we want to display the image strip in the game
        public int FrameWidth;                              // Width of a given frame
        public int FrameHeight;                             // Height of a given frame
        public bool Active;                                 // The state of the Animation
        public bool Looping;                                // Determines if the animation will keep playing or deactivate after one run
        private Vector2 Position = new Vector2(0,0);         // Width of a given frame
        public bool flip;                                   // Whether or not the image should be flipped
        private bool frameSet;

        /// <summary>
        /// Creates a new animation of Texture2D objects
        /// </summary>
        /// <param name="texture">The image strip for the animation.</param>
        /// <param name="position">The position to start animating from in the strip.</param>
        /// <param name="frameWidth">The width of a single image frame.</param>
        /// <param name="frameHeight">The height of a single image frame.</param>
        /// <param name="frameCount">The number of frames in the animation.</param>
        /// <param name="frametime">The amount of milliseconds each frame is displayed for.</param>
        /// <param name="color">The color used for the Draw function.</param>
        /// <param name="scale">The scale of the image.</param>
        /// <param name="looping">Whether or not the animation should loop.</param>
        public void Initialize(Texture2D texture, int frameCount, int frametime, bool looping, Vector2 position, Color color) {
            // Keep a local copy of the values passed in
            this.color = color;
            this.FrameWidth = texture.Width/frameCount;
            this.FrameHeight = texture.Height;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.frameSet = false;
            this.scale = 1f;
            this.manual = false;

            Looping = looping;
            setPosition(position);
            spriteStrip = texture;

            // Set the time to zero
            elapsedTime = 0;
            currentFrame = 0;

            // Set the Animation to active by default and facing right
            Active = true;
            flip = false;
        }

        public void setFrameTimes(int[] times) {
            manual = true;
            frameTimes = new int[frameCount];
            for (int i=0; i<frameCount; i++) {
                frameTimes[i] = times[i];
            }
        }

        public void setScale(float scale)
        {
            this.scale = scale;
        }

        public void setFrame(int frame) {
            currentFrame = frame;
            frameSet = true;
        }

        public void setPosition(Vector2 position)
        {
            this.Position = position;
        }

        public void Update(GameTime gameTime) {
            // Do not update the game if we are not active
            if (Active == false)
                return;

            // If single frame
            if (frameCount <= 1) {
                currentFrame = 0;
            } else {

                // Update the elapsed time
                elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                // the frame was set manually, so don't increment
                if (frameSet) {
                    frameSet = false;
                } else {
                    // If the elapsed time is larger than the frame time
                    // we need to switch frames
                    //if (manual) { GameOfPawns.echo(":: " + elapsedTime + " > " + frameTimes[currentFrame]); }
                    if ((manual && elapsedTime > frameTimes[currentFrame]) || (!manual && elapsedTime > frameTime)) {
                        // Move to the next frame
                        currentFrame++;

                        // If the currentFrame is equal to frameCount reset currentFrame to zero
                        if (currentFrame == frameCount) {
                            currentFrame = 0;
                            // If we are not looping deactivate the animation
                            if (Looping == false)
                                Active = false;
                        }

                        // Reset the elapsed time to zero
                        elapsedTime = 0;
                    }
                }
            }

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
            (int)Position.Y - (int)(FrameHeight * scale),
            (int)(FrameWidth * scale),
            (int)(FrameHeight * scale));
        }

        // Draw the Animation Strip
        public void Draw(SpriteBatch spriteBatch) {
            // Only draw the animation when we are active
            if (Active) {
                //spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
                SpriteEffects flipHorizontal;
                if (flip) {
                    flipHorizontal = SpriteEffects.FlipHorizontally;
                } else {
                    flipHorizontal = SpriteEffects.None;
                }
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color, 0.0f, Vector2.Zero, flipHorizontal, 0.0f);
            }
        }

        public void setFlip(bool flip) {
            this.flip = flip;
        }

        public void reset() {
            Active = true;
            currentFrame = 0;
        }

    }
}
