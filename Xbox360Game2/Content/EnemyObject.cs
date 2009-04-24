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
        public Texture2D sprite;
        public Rectangle destRect;
        public bool alive;
        public Rectangle rect;
        Rectangle viewportRect;

        public EnemyObject(Texture2D loadedTexture)
        {
            position = Vector2.Zero;
            sprite = loadedTexture;
            velocity = Vector2.Zero;
            alive = false;
            rect = new Rectangle((int)ScreenManager.GraphicsDevice.Viewport.Width / 2, (int)ScreenManager.GraphicsDevice.Viewport.Height - 150, sprite.Width, sprite.Height);
            viewportRect = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
        }

        public void Update()
        {
            if (alive)
            {
                position += velocity;
            }
            checkCollision();
        }

        public void checkCollision()
        {
            foreach (BallObject ball in dudeBalls)
            {
                if (ball.alive)
                {
                    if (ball.rect.Intersects(this.rect))
                    {
                        this.alive = false;
                        ball.alive = false;
                    }
                }
            }
        }
    }
}
