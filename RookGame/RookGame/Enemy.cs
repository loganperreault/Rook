using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace GameOfPawns
{
    public class Enemy : Person, ICloneable
    {

        Timer punchTimer;
        Timer kickTimer;

        Timer downTimer;

        Timer slidePunchTimer;
        Timer slideKickTimer;
        float slideMinDistance;
        float slideMaxDistance;

        Timer runTimer;
        float runMinDistance;
        float runDistance;

        public float distanceFromPlayer;
        public float collision;

        private float collisionDistance;
        private float attackDistance;

        protected Animation[] punchAnimations;
        protected Animation[] kickAnimations;

        public Animation animationSlidePunch;
        public Animation animationSlideKick;
        public Animation animationGrabbed;
        public Animation animationThrown;
        public Animation animationGettingUp;
        public Animation animationDown;

        public int slidePunchDamage;
        public int slideKickDamage;

        protected float thrownSpeed;

        Random random;

        protected Player player;

        public Enemy(int x, int y) : base(x, y) { }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override void Initialize()
        {
            base.Initialize();

            punchDamage = 20;
            kickDamage = 25;

            flip = -1;
            moveSpeed = 3.0f;

            collisionDistance = 30.0f;
            attackDistance = 50.0f;
            //slideMinDistance = 75.0f;
            //slideMaxDistance = 150.0f;
            slideMinDistance = 50.0f;
            slideMaxDistance = 200.0f;
            slidePunchDamage = 50;
            slideKickDamage = 50;
            runMinDistance = 35.0f;
            dashStopFrame = 1;
            thrownSpeed = 6.0f;

            player = GameOfPawns.player;

            Timers = new Timer[] {
                punchTimer = new Timer(1.5f, 1.0f),
                kickTimer = new Timer(1.5f, 1.0f),
                slidePunchTimer = new Timer(0.5f, 1.0f),
                slideKickTimer = new Timer(100.5f, 1.0f),
                runTimer = new Timer(1.0f, 0.5f),
                downTimer = new Timer(3.0f, 1.0f)
            };
            punchHitFrame = 4;
            kickHitFrame = 4;
            
        }

        protected Animation pickAnimation(Animation[] list) {
            return list[random.Next(list.Length)];
        }

        protected void loadAnimations(String folder)
        {

            random = new Random();

            int[] attackSequence = new int[]{100,125,150,200,75,50};
            int i;

            // load punches
            String[] punchTextures = Directory.GetFiles(content.RootDirectory + "/" + folder + "punch/");
            punchAnimations = new Animation[punchTextures.Length];            
            i = 0;
            foreach (String file in punchTextures)
            {
                punchAnimations[i] = GameOfPawns.loadAnimation(content,folder+"punch/"+Path.GetFileNameWithoutExtension(file), 6, -1, false);
                punchAnimations[i].setFrameTimes(attackSequence);
                i++;
            }

            // load kicks
            i = 0;
            String[] kickTextures = Directory.GetFiles(content.RootDirectory + "/" + folder + "kick/");
            kickAnimations = new Animation[kickTextures.Length];            
            i = 0;
            foreach (String file in kickTextures)
            {
                kickAnimations[i] = GameOfPawns.loadAnimation(content,folder+"kick/"+Path.GetFileNameWithoutExtension(file), 6, -1, false);
                kickAnimations[i].setFrameTimes(attackSequence);
                i++;
            }

            animationStand = GameOfPawns.loadAnimation(content, folder + "block/block", 1, 0, true);
            animationRun = GameOfPawns.loadAnimation(content,folder+"run/walk", 3, 50, true);
            animationHit = GameOfPawns.loadAnimation(content,folder+"block/block", 1, 0, true);
            animationPunch = pickAnimation(punchAnimations);
            animationKick = pickAnimation(kickAnimations);
            animationSlidePunch = GameOfPawns.loadAnimation(content, folder + "slide_punch/slide_punch", 2, 50, false);
            animationSlideKick = GameOfPawns.loadAnimation(content, folder + "slide_kick/slide_kick", 2, 50, false);
            animationGrabbed = GameOfPawns.loadAnimation(content, folder + "grabbed/grabbed", 1, 0, true);
            animationThrown = GameOfPawns.loadAnimation(content, folder + "thrown", 4, 200, false);
            animationDown = GameOfPawns.loadAnimation(content, folder + "down", 1, 0, true);
            animationGettingUp = GameOfPawns.loadAnimation(content, folder + "getting_up", 2, 100, false);
            animationDazed = GameOfPawns.loadAnimation(content, folder + "dazed", 2, 200, true);
            
            animationKick.setFrameTimes(attackSequence);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            room.addEnemy(this);
            loadAnimations("enemy1/");
        }

        public override void Dispose()
        {
            room.removeEnemy(this);
        }

        public override void Update(GameTime gameTime)
        {

            CalculateDistanceFromPlayer();
            CalculateCollision();
            CalculateDamageTaken();
            CalculateDamageToPlayer();

            //GameOfPawns.printTest("ROOK: " + GameOfPawns.player.Health.ToString() + "            " + distanceFromPlayer + "          ENEMY: " + this.Health.ToString());
            //GameOfPawns.printTest(this.Position.ToString());

            if (act == null)
            {

                GameOfPawns.printTest("ROOK: " + GameOfPawns.player.Health.ToString() + "                      ENEMY: " + this.Health.ToString());

                // make enemy always face the player
                if (distanceFromPlayer > 0)
                    setFlip(false);
                else
                    setFlip(true);

                // attack timer
                if (Math.Abs(distanceFromPlayer) < attackDistance)
                {
                    punchTimer.Go();
                    kickTimer.Go();
                }
                else
                {
                    punchTimer.Stop();
                    kickTimer.Stop();
                }

                // slide attack timer
                if (Math.Abs(distanceFromPlayer) > slideMinDistance && Math.Abs(distanceFromPlayer) < slideMaxDistance)
                {
                    slidePunchTimer.Go();
                    slideKickTimer.Go();
                }
                else if (act == null)
                {
                    slidePunchTimer.Stop();
                    slideKickTimer.Stop();
                }

                // run check
                if (Math.Abs(distanceFromPlayer) > runMinDistance)
                    runTimer.Go();
                else
                    runTimer.Stop();

            }

            // check timers
            if (act == null)
            {
                if (!TimerChecks(gameTime))
                {
                    Stand();
                    for (int i = 0; i < Timers.Length; i++)
                    {
                        Timers[i].Update(gameTime);
                        if (animation != animationStand)
                            Timers[i].Restart();
                    }
                }
            }
            else if (act == Down)
            {
                downTimer.Update(gameTime);
            }

            UpdateAnimation(gameTime);
            
        }

        protected bool TimerChecks(GameTime gameTime)
        {
            if (punchTimer.Check())
            {
                animationPunch = pickAnimation(punchAnimations);
                Punch();
                punchTimer.Restart();
            } 
            else if (kickTimer.Check())
            {
                animationKick = pickAnimation(kickAnimations);
                Kick();
                kickTimer.Restart();
            }
            else if (slidePunchTimer.Check())
            {
                animationDash = animationSlidePunch;
                Dash();
            }
            else if (slideKickTimer.Check())
            {
                animationDash = animationSlidePunch;
                Dash();
            }
            else if (runTimer.Check())
            {
                Run();
            }
            else 
            {
                act = null;
                return false;
            }
            return true;
        }

        public override void Run()
        {
            if (act != Run)
            {
                if (Math.Abs(distanceFromPlayer) < 2 * runMinDistance)
                {
                    runDistance = Math.Abs(distanceFromPlayer);
                }
                else
                {
                    float quarter = (Math.Abs(distanceFromPlayer) / 4);
                    runDistance = (float)(random.NextDouble() * (2 * quarter) + (quarter));
                }
            }
            if (runDistance < 0)
            {
                runTimer.Restart();
                act = null;
            }
            else
            {
                runDistance -= moveSpeed;
                runTimer.Go();
                base.Run();
            }
        }

        public override void Hit()
        {
            base.Hit();
            // if done with hit 
            if (act == null && checkDazed())
            {
                act = Dazed;
            }
        }

        public void Dazed()
        {
            animation = animationDazed;
            if (player.animation == player.animationHighGrab &&
                player.animation.currentFrame == player.highGrabHitFrame)
            {
                player.handleGrab(this);
                handleGrab();
            }
        }

        public bool checkDazed()
        {
            // TODO: statistical based on health remaining
            return true;
        }

        private void restartTimers()
        {
            for (int i = 0; i < Timers.Length; i++)
            {
                Timers[i].Restart();
            }
        }

        private void handleInstantShot()
        {
            if (animation == animationPunch)
            {
                animation.setFrame(punchHitFrame);
            }
            else if (animation == animationKick)
            {
                animation.setFrame(kickHitFrame);
            }
        }

        private void CalculateDamageTaken()
        {
            Player p = GameOfPawns.player;
            if (p.act != null && Math.Abs(distanceFromPlayer) <= attackDistance)
            {
                if ((p.getFlip() && distanceFromPlayer > 0) || !p.getFlip() && distanceFromPlayer < 0)
                {
                    if (p.animation == p.animationPunch && p.animation.currentFrame == p.punchHitFrame && p.dealDamage)
                    {
                        if (this.animation == this.animationPunch)
                        {
                            if (this.animation.currentFrame < this.punchHitFrame)
                            {
                                this.handleHit(p);
                                p.dealDamage = false;
                            }
                            else
                            {
                                this.handleBlock();
                            }
                        }
                        else if (this.animation == this.animationKick)
                        {
                            this.handleInstantShot();
                        }
                        else
                        {
                            this.handleBlock();
                        }
                    }
                    else if (p.animation == p.animationKick && p.animation.currentFrame == p.kickHitFrame && p.dealDamage)
                    {
                        if (this.animation == this.animationKick)
                        {
                            if (this.animation.currentFrame < this.kickHitFrame)
                            {
                                this.handleHit(p);
                                p.dealDamage = false;
                            }
                            else
                            {
                                this.handleBlock();
                            }
                        }
                        else if (this.animation == this.animationPunch)
                        {
                            this.handleInstantShot();
                        }
                        else
                        {
                            this.handleBlock();
                        }
                    }
                }
            }
            else
            {
                p.dealDamage = true;
            }
        }

        private void CalculateDamageToPlayer()
        {
            if (act != null && Math.Abs(distanceFromPlayer) <= attackDistance)
            {
                if ((getFlip() && distanceFromPlayer < 0) || !getFlip() && distanceFromPlayer > 0)
                {
                    if (animation == animationPunch && animation.currentFrame == punchHitFrame && dealDamage)
                    {
                        GameOfPawns.player.handleHit(this);
                        dealDamage = false;
                    }
                    else if (animation == animationKick && animation.currentFrame == kickHitFrame && dealDamage)
                    {
                        GameOfPawns.player.handleHit(this);
                        dealDamage = false;
                    }
                    else if (animation == animationSlidePunch && dealDamage)
                    {
                        if (player.animation == player.animationHighGrab &&
                            player.animation.currentFrame == player.highGrabHitFrame)
                        {
                            // player succeeds
                            player.handleThrow(this);
                            handleThrown();
                        }
                        else
                        {
                            // player fails
                            GameOfPawns.player.handleHit(this);
                            dealDamage = false;
                        }
                    }
                    else if (animation == animationSlideKick && dealDamage)
                    {
                        if (player.animation == player.animationLowGrab &&
                            player.animation.currentFrame == player.lowGrabHitFrame)
                        {
                            player.handleThrow(this);
                            handleGrab();
                        }
                        else
                        {
                            GameOfPawns.player.handleHit(this);
                            dealDamage = false;
                        }
                    }
                }
            }
            else if (act == null)
            {
                dealDamage = true;
            }
        }

        public void handleThrown()
        {
            Thrown();
        }

        public void Thrown()
        {
            visible = true;

            int stopDelay = 25;
            int throwLength = 40;
            // restart thrown animation each time
            if (act != Thrown) {
                act = Thrown;
                gj = stopDelay;
                gk = throwLength;
                //animationThrown.reset();
                animation = animationThrown;
                animation.reset();
            }
            else
            {
                if (gk > 0)
                {
                    // show all three frames equally by checking what phase of the throw we are in
                    gk--;
                    Position.X -= (flip * thrownSpeed);
                    int resetframe = animation.frameCount - (((throwLength - (throwLength - gk)) / 3) + 1);
                    if (gk < throwLength / 2)
                        resetframe--;
                    if (animation.currentFrame == animation.frameCount)
                        animation.setFrame(resetframe);
                }
                else if (gj > 0)
                {
                    gj--;
                    animation.setFrame(animation.frameCount - 1);
                    Position.X -= flip * moveSpeed * gj / (30 * 2);
                }
                else
                {
                    downTimer.Restart();
                    Down();
                }
            }
        }

        private void Down()
        {
            act = Down;
            animation = animationDown;
            if (downTimer.Check())
            {
                downTimer.Stop();
                animationGettingUp.reset();
                GetUp();
            }
        }

        private void GetUp()
        {
            act = GetUp;
            animation = animationGettingUp;
        }

        private void CalculateDistanceFromPlayer()
        {
            // this may need to check layers later
            distanceFromPlayer = this.Position.X - GameOfPawns.player.Position.X;
        }

        private void CalculateCollision()
        {
            collision = 0;
            if (animation != animationSlidePunch && animation != animationSlideKick)
            {
                float diff;
                if (distanceFromPlayer >= 0)
                {
                    diff = collisionDistance - distanceFromPlayer;
                    if (diff > 0)
                        collision = -1 * diff;
                }
                else
                {
                    diff = collisionDistance + distanceFromPlayer;
                    if (diff > 0)
                        collision = diff;
                }
            }
            
            // tell the player about the collision
            if (collision != 0)
            {
                GameOfPawns.player.handleCollision(collision);
            }
        }

        protected void handleGrab()
        {
            Grabbed();
        }

        public void Grabbed()
        {
            act = Grabbed;
            visible = false;
            animation = animationGrabbed;
        }

    }
}
