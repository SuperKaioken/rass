#region Using Statements
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace GameStateManagement
{
    public class Boss
    {
        public static Texture2D[] standSheet, walkSheet, attackSheet, hitSheet, deathSheet;
        int currentSprite = 0;
        Texture2D[] currentSheet;
        Rectangle rect;
        Vector2 position;
        bool alive, dying;
        int health;
        public int timePassed = 0;
        Vector2 velocity;
        public int AITimer = 0;
        enum state {STAND, WALK, ATTACK, HIT, DEATH};
        state previousState, currentState, nextState;
        Texture2D healthBar;
        SpriteFont font;
        SpriteFont outlineFont;
        Vector2 previousVelocity;
        public int deathTimer;

        public Boss(ContentManager content)
        {
            LoadContent(content);
            currentSheet = new Texture2D[standSheet.Count()];
            currentSheet = standSheet;
            currentState = state.STAND;
            nextState = state.STAND;
            previousState = state.STAND;
            alive = true;
            position = new Vector2(GameStateManagement.GameplayScreen.viewportRect.Width / 2, GameStateManagement.GameplayScreen.viewportRect.Height - 275);
            velocity = Vector2.Zero;
            previousVelocity = new Vector2(-1, 0);
            health = 200;
        }

        public void LoadContent(ContentManager Content)
        {
            healthBar = Content.Load<Texture2D>("Sprites\\Health Bar\\HealthBar");
            font = Content.Load<SpriteFont>("Fonts\\menufont");
            outlineFont = Content.Load<SpriteFont>("Fonts\\outline");

            standSheet = new Texture2D[2];
            standSheet[0] = Content.Load<Texture2D>("Sprites\\Boss\\bossstand0");
            standSheet[1] = Content.Load<Texture2D>("Sprites\\Boss\\bossstand1");

            walkSheet = new Texture2D[5];
            for (int i = 0; i < 5; i++)
                walkSheet[i] = Content.Load<Texture2D>("Sprites\\Boss\\bosswalk" + i.ToString());

            attackSheet = new Texture2D[6];
            for (int i = 0; i < 6; i++)
                attackSheet[i] = Content.Load<Texture2D>("Sprites\\Boss\\bossattack" + i.ToString());

            hitSheet = new Texture2D[1];
            hitSheet[0] = Content.Load<Texture2D>("Sprites\\Boss\\bosshit");

            deathSheet = new Texture2D[9];
            deathSheet[0] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie0");
            deathSheet[1] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie0");
            deathSheet[2] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie0");
            deathSheet[3] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie1");
            deathSheet[4] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie1");
            deathSheet[5] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie1");
            deathSheet[6] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie2");
            deathSheet[7] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie2");
            deathSheet[8] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie2");

        }

        public void AI()
        {
            AITimer++;
            previousState = currentState;
            currentState = nextState;
            switch (currentState)
            {
                case state.STAND:
                    currentSheet = standSheet;
                    if (previousState != state.STAND)
                        previousVelocity = velocity;
                    velocity.X = 0;
                    if (AITimer == 20)
                    {
                        nextState = state.WALK;
                        AITimer = 0;
                    }
                    break;
                case state.WALK:
                    if (previousState == state.ATTACK)
                        currentSprite = 0;
                    currentSheet = walkSheet;
                    if (GameStateManagement.GameplayScreen.dude.destRect.X < this.position.X + this.currentSheet[currentSprite].Width / 2)
                        velocity.X = -1;
                    else
                        velocity.X = 1;
                    if (Math.Abs(this.position.X + this.currentSheet[currentSprite].Width / 3 - GameStateManagement.GameplayScreen.dude.destRect.X) < 100.0f)
                    {
                        nextState = state.ATTACK;
                    }
                    break;
                case state.ATTACK:
                    currentSheet = attackSheet;
                    if (previousState != state.ATTACK)
                        previousVelocity = velocity;
                    velocity.X = 0;
                    nextState = state.ATTACK;
                    if (Math.Abs(this.position.X + this.currentSheet[currentSprite].Width / 3 - GameStateManagement.GameplayScreen.dude.destRect.X) > 100.0f && currentSprite == 0)
                    {
                        nextState = state.WALK;
                    }
                    break;
                case state.HIT:
                    currentSheet = hitSheet;
                    nextState = previousState;
                    velocity.X = 0;
                    if (health <= 0)
                    {
                        nextState = state.DEATH;
                    }
                    break;
                case state.DEATH:
                    currentSheet = deathSheet;
                    nextState = state.DEATH;
                    if (previousState != state.DEATH)
                        previousVelocity = velocity;
                    velocity.X = 0;
                    break;
                default:
                    break;
            }
            if (previousState != currentState)
            {
                currentSprite = 0;
                AITimer = 0;
            }
        }

        public void Update()
        {
            checkCollision();
            AI();
            position += velocity;
            timePassed++;
            if (!this.dying)
            {
                if (timePassed > 15)
                {
                    currentSprite++;
                    if (currentSprite > currentSheet.Length - 1)
                    {
                        currentSprite = 0;
                    }
                    timePassed = 0;
                }
            }
            else
                die();
        }

        public void checkCollision()
        {
            if (this.alive)
                this.rect = new Rectangle((int)this.position.X + this.currentSheet[currentSprite].Width / 3, (int)this.position.Y + this.currentSheet[currentSprite].Height / 2, 200, 200);
            else
                this.rect = new Rectangle();

            foreach (BallObject ball in GameStateManagement.GameplayScreen.dudeBalls)
            {

                if (ball.alive)
                {
                    ball.rect = new Rectangle((int)ball.position.X, (int)ball.position.Y, ball.rect.Width, ball.rect.Height);
                    if (ball.rect.Intersects(this.rect))
                    {
                        if (health > 0)
                        {
                            this.health -= 2;
                        }
                        else
                        {
                            this.alive = false;
                            this.dying = true;
                            nextState = state.DEATH;
                            currentSprite = 0;
                        }

                        ball.alive = false;
                    }
                    return;
                }                
            }

            foreach (SuperBallObject sball in GameStateManagement.GameplayScreen.SuperdudeBalls)
            {
                if (sball.alive)
                {
                    sball.rect = new Rectangle((int)sball.position.X, (int)sball.position.Y, sball.rect.Width, sball.rect.Height);
                    if (sball.rect.Intersects(this.rect))
                    {
                        if (health > 0)
                        {
                            this.health -= 2;
                            currentState = state.WALK;
                            nextState = state.HIT;
                            if (velocity.X != 0)
                                previousVelocity = velocity;
                            currentSprite = 0;
                        }
                        else
                        {
                            this.alive = false;
                            this.dying = true;
                            nextState = state.DEATH;
                            currentSprite = 0;
                        }
                    }
                    return;
                }
            }

            if (this.rect.Intersects(GameplayScreen.dude.destRect))
            {
                if (this.currentState == state.ATTACK && currentSprite == 4)
                {
                    GameplayScreen.dude.health -= 2;
                }
            }
        }

        public void die()
        {
            if (timePassed > 15)
            {
                currentSprite++;
                if (currentSprite > currentSheet.Length - 1)
                {
                    deathTimer++;
                    currentSprite = currentSheet.Length - 1;
                    if (deathTimer > 50)
                        GameStateManagement.GameplayScreen.killsNeeded = 0;
                }               
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (this.alive || this.dying)
            {
                Vector2 checkVelocity = velocity;
                if (checkVelocity.X == 0)
                    checkVelocity = previousVelocity;

                if (checkVelocity.X <= 0)
                    spriteBatch.Draw(currentSheet[currentSprite], position, Color.White);
                else if (checkVelocity.X > 0)
                    spriteBatch.Draw(currentSheet[currentSprite], new Vector2(position.X-25, position.Y), null, Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0.0f);
            }

            //Draw the negative space for the health bar            
            spriteBatch.Draw(healthBar, new Rectangle(GameStateManagement.GameplayScreen.viewportRect.Width - (healthBar.Width / 2) - 40, 20, (healthBar.Width / 2) + 5, 25), new Rectangle(0, 45, healthBar.Width / 2, 30), Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 0);

            //Draw the current health level based on the current Health
            spriteBatch.Draw(healthBar, new Rectangle(GameStateManagement.GameplayScreen.viewportRect.Width - (healthBar.Width / 2) - 40, 20, (int)((healthBar.Width / 2) * ((double)this.health / 200)), 20), new Rectangle(0, 45, healthBar.Width / 2, 30), Color.Red, 0.0f, Vector2.Zero, SpriteEffects.None, 0);

            //Draw the box around the health bar
            spriteBatch.Draw(healthBar, new Rectangle(GameStateManagement.GameplayScreen.viewportRect.Width - (healthBar.Width / 2) - 40, 20, (healthBar.Width / 2) + 5, 25), new Rectangle(0, 0, (healthBar.Width / 2) + 5, 25), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}