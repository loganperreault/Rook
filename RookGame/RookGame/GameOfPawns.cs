using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameOfPawns
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameOfPawns : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Player player;
        public static Player player;

        // The game time
        public static GameTime gameTime;

        // Keyboard states used to determine key presses
        public static KeyboardState currentKeyboardState;
        public static KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        public static GamePadState currentGamePadState;
        public static GamePadState previousGamePadState;

        // Current level
        Level level;

        // Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;

        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        // A random number generator
        public static Random random;
        List<Projectile> projectiles;

        // The font used to display UI elements
        public static SpriteFont font;
        List<Message> messages;

        // TESTING
        public static Message testMessage;

        // SETTINGS
        public static int playHeight;        // The height above the bottom of the screen at which all gameplay takes place

        // Keys
        public static Keys exitKey;
        public static Keys punchKey;
        public static Keys grabKey;
        public static Keys duckKey;
        public static Keys dashKey;

        // Keypresses
        public static bool punchPress;
        public static bool grabPress;
        public static bool duckPress;
        public static bool dashPress;
        public static bool exitPress;

        public static Keys key;

        private SpriteBatch targetBatch;
        public static RenderTarget2D target;

        public static ContentManager cm;
        public static GraphicsDevice gd;

        public GameOfPawns()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1068;
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            // content and gd
            cm = Content;
            gd = GraphicsDevice;
            
            // Create the player instance
            player = new Player(GraphicsDevice.Viewport.TitleSafeArea.X + 115, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height - playHeight);

            // Starting level
            level = new Level_1();
            level.Initialize(Content, GraphicsDevice, player);

            // Set Resolution to 534 x 300 scaled
            targetBatch = new SpriteBatch(GraphicsDevice);
            target = new RenderTarget2D(GraphicsDevice, 534, 300);
            GraphicsDevice.SetRenderTarget(target);
            

            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);

            // Initialize our random number generator
            random = new Random();

            // Initialize the projectiles list
            projectiles = new List<Projectile>();

            // Set play heigt
            playHeight = 30;

            // Initialize message to null
            //message = null;
            messages = new List<Message>();
            testMessage = new Message();

            // Keys
            punchKey = Keys.F;
            grabKey = Keys.G;
            duckKey = Keys.Down;
            dashKey = Keys.Space;
            exitKey = Keys.Escape;

            // Keypresses
            punchPress = false;
            grabPress = false;
            duckPress = false;
            dashPress = false;
            exitPress = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the player resources
            player.Initialize();
            player.LoadContent();

            // Load Level
            level.LoadContent();

            // Load the score font
            font = Content.Load<SpriteFont>("gameFont");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Loads a strip image file containing the desired animation.
        /// </summary>
        /// <param name="strip">The file name of the image</param>
        /// <param name="frameCount">The number of total frames</param>
        /// <param name="frametime">The time each frame should be displayed for</param>
        /// <param name="scale">The scale of the animation relative to the original image</param>
        /// <param name="looping">Whether or not the animation should loop</param>
        /// <returns>An Animation object that has been initialized and ready to use</returns>
        public static Animation loadAnimation(ContentManager content, String strip, int frameCount, int frametime, bool looping)
        {
            return loadAnimation(content, strip, frameCount, frametime, looping, Vector2.Zero, Color.White);
        }

        /// <summary>
        /// Loads a strip image file containing the desired animation.
        /// </summary>
        /// <param name="strip">The file name of the image</param>
        /// <param name="position">The position in the image to start reading from (typically zero,zero)</param>
        /// <param name="frameCount">The number of total frames</param>
        /// <param name="frametime">The time each frame should be displayed for</param>
        /// <param name="color">The color to draw with</param>
        /// <param name="scale">The scale of the animation relative to the original image</param>
        /// <param name="looping">Whether or not the animation should loop</param>
        /// <returns>An Animation object that has been initialized and ready to use</returns>
        public static Animation loadAnimation(ContentManager content, String strip, int frameCount, int frametime, bool looping, Vector2 position, Color color)
        {
            Texture2D texture = content.Load<Texture2D>(strip);
            Animation animation = new Animation();
            animation.Initialize(texture, frameCount, frametime, looping, position, color);
            return animation;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime time)
        {

            // Update the game time
            gameTime = time;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Checks for escape, which will possibly pull up a menu later. Right now it simply makes the game non-fullscreen
            if (currentKeyboardState.IsKeyDown(exitKey)) {
                if (!exitPress) {
                    graphics.ToggleFullScreen();
                    exitPress = true;
                }
            } else {
                exitPress = false;
            }

            // Save the previous state of the keyboard and game pad so we can determine single key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Update entire level
            level.Update(gameTime);

            // Update the collision
            UpdateCollision();

            // Update the messages
            foreach (Message message in messages) {
                message.Update(gameTime);
            }
            testMessage.Update(gameTime);

            // print player position to screen
            //print(player.Position.X.ToString(), 0);
            //print("HEALTH: "+player.Health.ToString(), 0);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            // Set GD to render to target (will be drawn to backbuffer at the end)
            GraphicsDevice.SetRenderTarget(target);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            level.Draw(spriteBatch);

            // Draw the message
            foreach (Message message in messages)
            {
                message.Draw(spriteBatch);
            }
            testMessage.Draw(spriteBatch);

            spriteBatch.End();

            // Reset GD to backbuffer and draw target
            GraphicsDevice.SetRenderTarget(null);
            targetBatch.Begin();
            targetBatch.Draw(target, new Rectangle(0, 0, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height), Color.White);
            targetBatch.End();

            base.Draw(gameTime);
        }

        // Update the message objects
        private void UpdateMessages() {
            for (int i = 0; i < messages.Count; i++) {
                messages[i].Update(gameTime);
                if (messages[i].active == false) {
                    messages.RemoveAt(i);
                }
            }
        }

        // Check for collisions
        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect function to 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            //Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);

        }

        // Update the projectiles
        private void UpdateProjectiles()
        {
            // Update the Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();

                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        public void print(String text, float lifetime) {
            Message message = new Message();
            int msgs = 0;
            for (int i = 0; i < messages.Count; i++)
                if (messages[i].active)
                    msgs++;
            message.Initialize(text,font,Color.White, new Vector2(10,10+(msgs*30)));
            if (lifetime>0) message.setTimeLimit(TimeSpan.FromSeconds(lifetime), gameTime);
            messages.Add(message);
        }

        public static void printTest(String text)
        {
            testMessage = new Message();
            testMessage.Initialize(text, font, Color.White, new Vector2(10, 10));
        }

        public static void echo(String text)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

    }
}
