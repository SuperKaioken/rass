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
    //enum eType { AXE, LANCE, SHIP, SPIKEY, SLIME, BALL, THING, PULSE, BLUEEYE, GOLDEYE };
    public class EnemyObject
    {
        public Vector2 velocity;
        public Vector2 position;
        public int explosionPos = 0;
        public Texture2D[] sprite;
        public int spritePosition;
        public Rectangle destRect;
        public bool alive;
        public bool dying = false;
        public Rectangle rect;
        public int numEnemies;
        public Random random;
        public int timePassed = 0;
        public SoundEffect sound;        

        public EnemyObject(Texture2D[] loadedTexture, Vector2 startPosition, Vector2 startVelocity)
        {
            spritePosition = 0;
            position = startPosition;
            sprite = loadedTexture;
            velocity = startVelocity;
            alive = true;
            rect = new Rectangle((int)position.X, (int)position.Y, sprite[spritePosition].Width, sprite[spritePosition].Height);
            numEnemies = OptionsMenuScreen.getEnemies();
            random = new Random();

            
            //velocity.X = 100;
            //if (numEnemies == 0)
            //    velocity.X = 1;
            //else if (numEnemies == 1)
            //    velocity.X = 2;
            //else if (numEnemies == 2)
            //    velocity.X = 3; 

        }
        
        public void Update()
        {
            checkCollision();
            //if it's the axe enemy, do this (need some way to check what enemy it is)
            axeMovement();
            timePassed++;
            if (timePassed > 25)
            {
                spritePosition++;
                if (spritePosition > sprite.Length - 1)
                {
                    spritePosition = 0;
                    Random rand = new Random();
                    if(rand.Next(10) == 5 && sound != null)
                        sound.Play();
                }
                timePassed = 0;
            }

            if (!this.dying)
            {
                if (position.X < 0)
                    velocity.X = velocity.X * -1.0f;
                else if (position.X > GameStateManagement.GameplayScreen.viewportRect.Width - sprite[spritePosition].Width)
                    velocity.X = velocity.X * -1.0f;
            }
        }

        public void checkCollision()
        {
            this.rect = new Rectangle((int)this.position.X, (int)this.position.Y, this.rect.Width, this.rect.Height);
            foreach (BallObject ball in GameStateManagement.GameplayScreen.dudeBalls)
            {
                if (ball.alive && !this.dying)
                {
                    ball.rect = new Rectangle((int)ball.position.X, (int)ball.position.Y, ball.rect.Width, ball.rect.Height);
                    if (ball.rect.Intersects(this.rect))
                    {
                        this.alive = false;
                        ball.alive = false;
                        this.dying = true;                                    
                        GameplayScreen.killsNeeded--;
                        EnemyGenerator.playExplosion();
                    }
                }
                else if (this.dying)
                {
                    explode();
                    return;
                }
            }

            foreach (SuperBallObject sball in GameStateManagement.GameplayScreen.SuperdudeBalls)
            {
                if (sball.alive && !this.dying)
                {
                    sball.rect = new Rectangle((int)sball.position.X, (int)sball.position.Y, sball.rect.Width, sball.rect.Height);
                    if (sball.rect.Intersects(this.rect))
                    {
                        this.alive = false;
                        this.dying = true;
                        GameplayScreen.killsNeeded--;
                        EnemyGenerator.playExplosion();
                    }
                }
                else if (this.dying)
                {
                    explode();
                    return;
                }
            }

            if (this.rect.Intersects(GameplayScreen.dude.destRect) && !this.dying)
            {                   
                GameplayScreen.dude.health -= 5;
                GameplayScreen.killsNeeded--;
                EnemyGenerator.playExplosion();
                this.dying = true;
                this.alive = false;
            }                
            else if (this.dying)
            {
                explode();
                return;
            }            
        }

        public void explode()
        {
            if (explosionPos < 15)
            {
                explosionPos++;
            }
            else
            {
                this.dying = false;
                explosionPos = 0;
            }
        }

        public void axeMovement()
        {
            position.X += velocity.X;
            position.Y += (float)random.NextDouble() * 2.0f;
            position.Y -= (float)random.NextDouble() * 2.0f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (this.alive)
            {
                if (velocity.X < 0)
                    spriteBatch.Draw(sprite[spritePosition], position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0);
                else
                    spriteBatch.Draw(sprite[spritePosition], position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            }
            else if(this.dying)
            {
                spriteBatch.Draw(sprite[explosionPos], position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            }
        }
    }
}
