using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace GameOfPawns
{
    public class Person : Object
    {

        // Movement for the player
        protected float scaleSpeed;
        protected float walkSpeed;
        protected float runSpeed;
        protected float moveSpeed;
        protected float dashSpeed;
        protected float dashDistance;

        // TODO: Audio Technica
        protected SoundEffect punchSound;

        // Which frames in animations that damage is delt
        public int punchHitFrame;
        public int kickHitFrame;

        // How much damage is delt
        protected int punchDamage;
        protected int kickDamage;

        // dash stuff
        public int dashStopFrame;

        // global iterators
        protected int gi;
        protected int gj;
        protected int gk;

        public delegate void PerformAction();
        public PerformAction act;

        // possible animations
        public Animation animationStand;
        public Animation animationRun;
        public Animation animationDuck;
        public Animation animationDash;
        public Animation animationPunch;
        public Animation animationKick;
        public Animation animationHit;
        public Animation animationDazed;

        public Timer[] Timers;

        // State of the player
        public bool Active;
        public bool dealDamage;
        public bool visible;

        // Amount of hit points that player has
        public int Health;

        // Whether or not the player is acting
        public bool acting;

        public Person(int x, int y) : base(x, y)
        {
            
        }
        
        public override void Initialize()
        {
            base.Initialize();
            this.content = GameOfPawns.cm;
            this.gd = GameOfPawns.gd;
            Active = true;                      // Set the player to be active
            Health = 100;                       // Set Active flag
            scaleSpeed = 1.0f;                  // Set the player health
            walkSpeed = 2.0f;                   // Set walk speed
            runSpeed = 4.0f;                    // Set run speed
            moveSpeed = runSpeed;               // Set player move speed
            dashSpeed = 16.0f;                  // Set a constant player dash speed
            dashDistance = 4;                   // Set the number of frames the dash will move for
            acting = false;                     // Set player as not acting
            dealDamage = true;
            punchHitFrame = 2;
            kickHitFrame = 2;
            punchDamage = 20;
            kickDamage = 20;
            dashStopFrame = 3;
            solid = true;
            visible = true;
            act = null;
        }

        public override void LoadContent()
        {
            punchSound = content.Load<SoundEffect>("Sound/explosion");
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            UpdateAnimation(gameTime);
        }

        public void UpdateAnimation(GameTime gameTime) {
            checkActivity();
            if (act == null)
                animation = animationStand;
            if (animation != null)
            {
                animation.setPosition(this.Position);
                animation.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
                base.Draw(spriteBatch);
        }

        public void setSpeedWalk()
        {
            moveSpeed = walkSpeed;
        }

        public void setSpeedRun()
        {
            moveSpeed = runSpeed;
        }

        public void setFlip(bool flip)
        {
            if (flip)
                this.setFlip(1);
            else
                this.setFlip(-1);
        }
        
        public bool getFlip()
        {
            if (flip == 1)
                return true;
            else
                return false;
        }

        public virtual void checkActivity() {
            if (act != null)
                act();
            if (animation != null && !animation.Active)
                act = null;
        }

        public virtual void Hit()
        {
            // restart hit animation each time
            if (act != Hit) {
                act = Hit;
                gi = 10;
                acting = true;
                animationHit.reset();
                animation = animationHit;
            } else {
                if (gi > 0)
                {
                    Position.X -= (flip * dashSpeed) * gi / 20;
                    gi--;
                }
                else
                {
                    act = null;
                }
            }
        }

        public void Block()
        {
            act = Block;
            // restart hit animation each time
            if (animation != animationHit) {
                gi = 7;
                acting = true;
                animationHit.reset();
                animation = animationHit;
            } else {
                if (gi > 0)
                {
                    Position.X -= (flip * dashSpeed) * gi / 20;
                    gi--;
                }
                else
                {
                    act = null;
                    acting = false;
                }
            }
        }

        public void Duck() {
            act = Duck;
            animation = animationDuck;
        }

        public virtual void Run() {
            act = Run;
            if (flip == 1) {
                Position.X += scaleSpeed*moveSpeed;
                animation = animationRun;
            } else {
                Position.X -= scaleSpeed*moveSpeed;
                animation = animationRun;
            }
        }

        public void Stand() {
            animation = animationStand;
        }

        public void Dash() {
            int startDelay = 20;
            int stopDelay = 30;
            int dashLength = 8;
            // restart dashing animation each time
            if (act != Dash) {
                act = Dash;
                gi = startDelay;
                gj = stopDelay;
                gk = dashLength;
                acting = true;
                animationDash.reset();
                animation = animationDash;
            } else if (gi > 0) {
                    gi--;
                    animation.setFrame(0);
            } else {
                if (gk > 0)
                {
                    gk--;
                    Position.X += (flip * dashSpeed);
                    if (animation.currentFrame == dashStopFrame)
                        animation.setFrame(dashStopFrame - 1);
                }
                else if (gj > 0)
                {
                    gj--;
                    animation.setFrame(dashStopFrame);
                    Position.X += flip * moveSpeed * gj / ( 30 * 2 );
                } else {
                    acting = false;
                    act = null;
                }
            }
        }

        public void Punch() {
            // if ducking, perform kick instead
            if (animation == animationDuck)
            {
                Kick();
            } else if (act != Punch) {
                act = Punch;
                acting = true;
                animationPunch.reset();
                animation = animationPunch;
            }
        }

        public void Kick() {
            // restart kicking animation each time
            if (act != Kick) {
                act = Kick;
                acting = true;
                animationKick.reset();
                animation = animationKick;
            }
        }

        // this should be a little more robust. perhaps check if new location is a collision
        public void handleCollision(float collision)
        {
            if (animation != animationDash)
            {
                this.Position.X += collision;
            }
        }

        public void handleHit(Person person)
        {
            punchSound.Play();
            if (person.animation == person.animationPunch)
            {
                this.takeDamage(person.punchDamage);
                Hit();
            }
            else if (person.animation == person.animationKick)
            {
                this.takeDamage(person.kickDamage);
                Hit();
            }
        }

        public void handleBlock()
        {
            Block();
        }

        public void takeDamage(int damage)
        {
            // TODO: here we can add factors like shield, special resistance, etc.
            Health -= damage;
            if (Health <= 0)
            {
                Health = 0;
                exists = false;
            }
        }

        // Get the width of the player ship
        new public int Width {
            get { return (animation==null?0:animation.FrameWidth); }
        }

        // Get the height of the player ship
        new public int Height {
            get { return (animation==null?0:animation.FrameHeight); }
        }

    }
}
