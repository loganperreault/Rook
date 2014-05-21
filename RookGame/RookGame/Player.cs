using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GameOfPawns
{
    public class Player : Person
    {

        public Player(int x, int y) : base(x, y) { }

        bool dashPress;
        bool punchPress;
        bool grabPress;
        bool duckPress;

        public Animation animationHighGrab;
        public Animation animationLowGrab;
        public Animation animationChokeHold;
        public Animation animationHighThrow;
        public Animation animationLowThrow;
        public Animation animationHumanShield;

        public Object objectChokeHold;

        int grabTime;
        public int highGrabHitFrame;
        public int lowGrabHitFrame;

        public Enemy grabbed;

        KeyboardState cks;
        GamePadState cgs;
        Keys key;
        Keys dashKey;
        Keys punchKey;
        Keys grabKey;
        Keys duckKey;

        public override void Initialize()
        {
            base.Initialize();

            flip = 1;
            Health = 1000;
            kickDamage = 35;
            grabTime = 1000;
            highGrabHitFrame = 1;
            lowGrabHitFrame = 1;

            grabbed = null;
            
        }

        private void LoadAnimations(String folder)
        {
            animationStand = GameOfPawns.loadAnimation(content,folder + "Stand", 1, 0, true);
            animationRun = GameOfPawns.loadAnimation(content,folder + "Run", 8, 50, true);
            animationDuck = GameOfPawns.loadAnimation(content,folder + "Duck", 1, 0, true);
            animationDash = GameOfPawns.loadAnimation(content,folder + "Dash", 4, 50, false);
            animationPunch = GameOfPawns.loadAnimation(content,folder + "Punch", 6, 50, false);
            animationKick = GameOfPawns.loadAnimation(content,folder + "Kick", 6, 50, false);
            animationHit = GameOfPawns.loadAnimation(content,folder + "Stand", 1, 0, true);
            animationHighGrab = GameOfPawns.loadAnimation(content,folder + "HighGrab", 4, 50, false);
            animationLowGrab = GameOfPawns.loadAnimation(content,folder + "LowGrab", 2, 50, false);
            //animationChokeHold = GameOfPawns.loadAnimation(content,folder + "ChokeHold", 1, 0, true);
            animationChokeHold = GameOfPawns.loadAnimation(content,folder + "ChokeHold1", 1, 0, true);
            animationHighThrow = GameOfPawns.loadAnimation(content,folder + "HighThrow", 3, 100, false);
            animationLowThrow = GameOfPawns.loadAnimation(content,folder + "LowThrow", 2, 100, false);
            animationHumanShield = GameOfPawns.loadAnimation(content,folder + "HumanShield", 1, 0, true);

            objectChokeHold = new Object(0, 30);
            objectChokeHold.Initialize();
            objectChokeHold.addObject(0, 0, folder + "ChokeHold1");
            objectChokeHold.addObject(0, 0, "Enemy1/grabbed/grabbed");
            objectChokeHold.addObject(0, 0, folder + "ChokeHold2");


            animationHighGrab.setFrameTimes(new int[]{50,grabTime,50,50});
            animationLowGrab.setFrameTimes(new int[]{50,grabTime});
        }

        public override void LoadContent()
        {
            base.LoadContent();
            LoadAnimations("Rook/");
        }

        /// <summary>
        /// Updates the player every frame
        /// </summary>
        /// <param name="gameTime">The time at which the player is being updated.</param>
        public override void Update(GameTime gameTime)
        {

            cks = GameOfPawns.currentKeyboardState;
            cgs = GameOfPawns.currentGamePadState;
            dashPress = GameOfPawns.dashPress;
            punchPress = GameOfPawns.punchPress;
            duckPress = GameOfPawns.duckPress;
            grabPress = GameOfPawns.grabPress;
            key = Keys.None;
            dashKey = GameOfPawns.dashKey;
            punchKey = GameOfPawns.punchKey;
            grabKey = GameOfPawns.grabKey;
            duckKey = GameOfPawns.duckKey;

            // Check Dash
            if (cks.IsKeyDown(dashKey) || cgs.Buttons.X == ButtonState.Pressed) {
                if (act == null && !dashPress) {
                    Dash();
                }
                dashPress = true;
            } else {
                dashPress = false;
            }

            // Check Punch/Kick
            if (cks.IsKeyDown(punchKey) || cgs.Buttons.A == ButtonState.Pressed) {
                if (act == null && !punchPress)
                {
                    Punch();
                }
                else if (act == Duck && !duckPress)
                {
                    Kick();
                }
                punchPress = true;
            } else {
                punchPress = false;
            }

            // Check High/Low Grab
            if (cks.IsKeyDown(grabKey) || cgs.Buttons.B == ButtonState.Pressed) {
                if (act == null && !grabPress)
                {
                    grabPress = true;
                    HighGrab();
                }
                /*
                else if (act == Grab && !grabPress)
                {
                    handleThrow(grabbed);
                }
                */
                //grabPress = true;
            } else {
                grabPress = false;
            }

            if (act == Duck)
            {
                if (!cks.IsKeyDown(duckKey) && cgs.DPad.Down != ButtonState.Pressed)
                    act = null;
            }

            if (act == Run)
            {
                if (cks.IsKeyDown(Keys.Left) || cgs.DPad.Left == ButtonState.Pressed) {
                    setFlip(false);
                } else if (cks.IsKeyDown(Keys.Right) || cgs.DPad.Right == ButtonState.Pressed) {
                    setFlip(true);
                } else {
                    act = null;
                    key = Keys.None;
                    Stand();
                }
            }

            // Not acting...
            if (act == null) {
                if (cks.IsKeyDown(duckKey) || cgs.DPad.Down == ButtonState.Pressed) {
                    key = duckKey;
                    Duck();
                } else {
                    if (cks.IsKeyDown(Keys.Left) || cgs.DPad.Left == ButtonState.Pressed) {
                        setFlip(false);
                        Run();
                    } else if (cks.IsKeyDown(Keys.Right) || cgs.DPad.Right == ButtonState.Pressed) {
                        setFlip(true);
                        if (act == null)
                            Run();
                    } else {
                        act = null;
                        key = Keys.None;
                        Stand();
                    }
                }
            }

            // Pass the game time to the update function
            base.Update(gameTime);

        }

        public void HighGrab() 
        {
            // if ducking, perform low grab instead
            if (animation == animationDuck)
            {
                LowGrab();
            // restart high grab animation each time
            } else if (act != HighGrab) {
                act = HighGrab;
                animationHighGrab.reset();
                animation = animationHighGrab;
            }
        }

        public void LowGrab()
        {
            // restart low grab animation each time
            if (act != LowGrab) {
                act = LowGrab;
                animationLowGrab.reset();
                animation = animationLowGrab;
            }
        }

        public void handleHit(Enemy person)
        {
            base.handleHit(person);
            if (person.animation == person.animationSlidePunch)
            {
                this.takeDamage(person.slidePunchDamage);
                Hit();
            }
            else if (person.animation == person.animationSlideKick)
            {
                this.takeDamage(person.slideKickDamage);
                Hit();
            }
        }


        public void handleGrab(Enemy person)
        {
            // change animation to chokehold
            grabbed = person;
            animation = null;
            this.addObject(objectChokeHold);
            handleGrab();
        }

        public void handleGrab()
        {
            act = handleGrab;
            // wait for initial grab key to be released
            if (!cks.IsKeyDown(grabKey) && cgs.Buttons.B != ButtonState.Pressed)
            {
                grabPress = false;
                Grab();
            }
        }

        public void Grab()
        {
            act = Grab;
            // Check Throw Key
            if (cks.IsKeyDown(grabKey) || cgs.Buttons.B == ButtonState.Pressed)
                handleThrow(grabbed);
        }

        public void handleThrow(Enemy person)
        {
            objects.Remove(objectChokeHold);
            grabbed = person;
            if (grabbed.animation == grabbed.animationSlideKick)
            {
                animationLowThrow.reset();
                animation = animationLowThrow;
                Throw();
            } 
            else
            {
                animationHighThrow.reset();
                animation = animationHighThrow;
                Throw();
            } 
            grabbed = null; // let go
        }

        public void Throw()
        {
            act = Throw;
            if (animation.currentFrame == animation.frameCount)
                grabbed.handleThrown();
        }

    }
}
