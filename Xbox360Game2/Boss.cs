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

            deathSheet = new Texture2D[3];
            deathSheet[0] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie0");
            deathSheet[1] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie1");
            deathSheet[2] = Content.Load<Texture2D>("Sprites\\Boss\\bossdie2");

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
                    velocity.X = 0;
                    if (AITimer == 20)
                    {
                        nextState = state.WALK;
                        AITimer = 0;
                    }
                    break;
                case state.WALK:
                    currentSheet = walkSheet;
                    if (GameStateManagement.GameplayScreen.dude.destRect.X < this.position.X + this.currentSheet[currentSprite].Width / 2)
                        velocity.X = -1;
                    else
                        velocity.X = 1;
                    if (Math.Abs(this.position.X + this.currentSheet[currentSprite].Width / 3 - GameStateManagement.GameplayScreen.dude.destRect.X) < 150.0f)
                    {
                        nextState = state.ATTACK;
                    }
                    break;
                case state.ATTACK:
                    currentSheet = attackSheet;
                    if (Math.Abs(this.position.X + this.currentSheet[currentSprite].Width / 3 - GameStateManagement.GameplayScreen.dude.destRect.X) > 150.0f)
                    {
                        nextState = state.WALK;
                    }
                    else
                        nextState = state.ATTACK;
                    velocity.X = 0;
                    break;
                case state.HIT:
                    currentSheet = hitSheet;
                    nextState = previousState;
                    if (health == 0)
                    {
                        nextState = state.DEATH;
                    }
                    break;
                case state.DEATH:
                    currentSheet = deathSheet;
                    velocity.X = 0;
                    nextState = state.DEATH;
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
            AI();
            checkCollision();
            position += velocity;
            timePassed++;
            if (!this.dying)
            {
                if (timePassed > 25)
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
                this.rect = new Rectangle((int)this.position.X + this.currentSheet[currentSprite].Width / 3, (int)this.position.Y + this.currentSheet[currentSprite].Height / 2, 419, 300);
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
                            this.health -= 2;
                        else
                        {
                            this.alive = false;
                            this.dying = true;
                            currentSheet = deathSheet;
                            currentState = state.DEATH;
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
                            this.health -= 2;
                        else
                        {
                            this.alive = false;
                            this.dying = true;
                            currentSheet = deathSheet;
                            currentState = state.DEATH;
                        }
                    }
                    return;
                }
            }

            if (this.rect.Intersects(GameplayScreen.dude.destRect))
            {
                GameplayScreen.dude.health -= 5;
                if (health > 0)
                    this.health -= 2;
                else
                {
                    this.alive = false;
                    this.dying = true;
                    currentSheet = deathSheet;
                    currentState = state.DEATH;
                }
            }
        }

        public void die()
        {
            if (timePassed > 25)
            {
                currentSprite++;
                if (currentSprite > currentSheet.Length - 1)
                {
                    this.dying = false;
                }               
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (this.alive || this.dying)
            {
                if (velocity.X <= 0)
                    spriteBatch.Draw(currentSheet[currentSprite], position, Color.White);
                else if (velocity.X > 0)
                    spriteBatch.Draw(currentSheet[currentSprite], position, null, Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0.0f);
            }

            //Draw the negative space for the health bar            
            spriteBatch.Draw(healthBar, new Rectangle((GameStateManagement.GameplayScreen.viewportRect.Width / 2) - (healthBar.Width / 2) + 300, 10, (healthBar.Width / 2) + 5, 25), new Rectangle(0, 45, healthBar.Width / 2, 30), Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 0);

            //Draw the current health level based on the current Health
            spriteBatch.Draw(healthBar, new Rectangle((GameStateManagement.GameplayScreen.viewportRect.Width / 2) - (healthBar.Width / 2) + 300, 10, (int)((healthBar.Width / 2) * ((double)this.health / 200)), 20), new Rectangle(0, 45, healthBar.Width / 2, 30), Color.Red, 0.0f, Vector2.Zero, SpriteEffects.None, 0);

            //Draw the box around the health bar
            spriteBatch.Draw(healthBar, new Rectangle((GameStateManagement.GameplayScreen.viewportRect.Width / 2) - (healthBar.Width / 2) + 300, 10, (healthBar.Width / 2) + 5, 25), new Rectangle(0, 0, (healthBar.Width / 2) + 5, 25), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.DrawString(outlineFont, "BOSS HEALTH", new Vector2(15, GameStateManagement.GameplayScreen.viewportRect.Width), Color.Red);
            spriteBatch.DrawString(font, "BOSS HEALTH", new Vector2(15, GameStateManagement.GameplayScreen.viewportRect.Width), Color.White);
        }
    }
}