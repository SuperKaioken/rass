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
    public class EnemyGenerator
    {
        List<EnemyObject> enemies;
        public static Texture2D[] axeSprite, ballSprite, lanceSprite, thingSprite, spikeySprite, shipSprite, pulseSprite, slimeSprite, blueeyeSprite, goldeyeSprite;
        Random random;

        public EnemyGenerator()
        {
            random = new Random();
            enemies = new List<EnemyObject>();
            enemies.Add(new EnemyObject(axeSprite, new Vector2(0, (GameStateManagement.GameplayScreen.viewportRect.Height - 150) * MathHelper.Clamp((float)random.NextDouble(), 0, GameStateManagement.GameplayScreen.viewportRect.Height - 150)), new Vector2(5, 0)));
            enemies.Add(new EnemyObject(ballSprite, new Vector2(100, 100), new Vector2(-10, 0)));
            enemies.Add(new EnemyObject(lanceSprite, new Vector2(300, 200), new Vector2(50, 0)));                
        }

        public static void LoadContent(ContentManager Content)
        {
#region //store the axe enemy
            //store the axe enemy
#endregion
#region axeSprite = new Texture2D[4];
            axeSprite = new Texture2D[4];
#endregion
#region for (int i = 0; i < 4; i++)
            for (int i = 0; i < 4; i++)
#endregion
#region {
            {
#endregion
#region axeSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\axe" + i.ToString());
               axeSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\axe" + i.ToString());
#endregion
#region }
            }
#endregion

            //make the ball enemy
            ballSprite = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                ballSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\ball" + i.ToString());
            }

            //make the blue eye enemy
             blueeyeSprite = new Texture2D[3];
             for (int i = 0; i < 3; i++)
             {
                 blueeyeSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\blueeye" + i.ToString());
             }

            //make the goldeye enemy
             goldeyeSprite = new Texture2D[3];
             for (int i = 0; i < 3; i++)
             {
                 goldeyeSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\goldeye" + i.ToString());
             }

            //make the lance enemy
            lanceSprite = new Texture2D[9];
            for (int i = 0; i < 9; i++)
            {
                lanceSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\lance" + i.ToString());
            }

            //make the pulse enemy
             pulseSprite = new Texture2D[4];
             for (int i = 0; i < 4; i++)
             {
                 pulseSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\pulse" + i.ToString());
             }

            //make the slime enemy
            slimeSprite = new Texture2D[5];
            for (int i = 0; i < 5; i++)
            {
                slimeSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\slime" + i.ToString());
            }

            //make the thing enemy
            thingSprite = new Texture2D[6];
            for (int i = 0; i < 6; i++)
            {
                thingSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\thing" + i.ToString());
            }
        }
        
        public void Update()
        {
            foreach (EnemyObject enemy in enemies)
                enemy.Update();

            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].alive)
                    enemies.RemoveAt(i);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (EnemyObject enemy in enemies)
                enemy.Draw(gameTime, spriteBatch);
        }
    }
}