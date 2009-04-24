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
    public class EnemyObject : GameplayScreen
    {
        public Vector2 velocity;
        public Vector2 position;
        public Texture2D[] sprite;
        public int spritePosition;
        public Rectangle destRect;
        public bool alive;
        public Rectangle rect;
        Rectangle viewportRect;
        public int numEnemies;
        public Random random;
        public int timePassed = 0; 

        public EnemyObject(Texture2D[] loadedTexture)
        {
            spritePosition = 0;
            position = new Vector2(300, 100);
            sprite = loadedTexture;
            velocity = Vector2.Zero;
            alive = false;
            //rect = new Rectangle((int)ScreenManager.GraphicsDevice.Viewport.Width / 2, (int)ScreenManager.GraphicsDevice.Viewport.Height - 150, sprite[spritePosition].Width, sprite[spritePosition].Height);
            //viewportRect = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            numEnemies = OptionsMenuScreen.getEnemies();
            random = new Random();
            

            //velocity.X = 100;
            if (numEnemies == 0)
                velocity.X = 1;
            else if (numEnemies == 1)
                velocity.X = 2;
            else if (numEnemies == 2)
                velocity.X = 3; 

        }

        public void Update()
        {
            if (alive)
            {
                position += velocity;
            }
            checkCollision();
            timePassed++;
            if (timePassed > 25)
            {
                spritePosition++;
                if (spritePosition > sprite.Length - 1)
                {
                    spritePosition = 0;
                }
                timePassed = 0; 
            }
            //if it's the axe enemy, do this (need some way to check what enemy it is)
            this.setAxeMovement();
        }

        public void checkCollision()
        {
           /* foreach (BallObject ball in dudeBalls)
            {
                if (ball.alive)
                {
                    if (ball.rect.Intersects(this.rect))
                    {
                        this.alive = false;
                        ball.alive = false;
                    }
                }
            }*/
        }

        public void setAxeMovement()
        {
            position.X += velocity.X;
            position.Y += (float)random.NextDouble() * 2.0f;
            position.Y -= (float)random.NextDouble() * 2.0f;
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
                spriteBatch.Draw(sprite[spritePosition], position, Color.White);
        }
    }
}
