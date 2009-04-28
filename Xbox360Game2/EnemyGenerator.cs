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
    public enum level { one = 50, two = 75, three = 100, four = 150 };    
    
    public class EnemyGenerator
    {
        List<EnemyObject> enemies;
        public static Texture2D[] axeSprite, ballSprite, lanceSprite, thingSprite, spikeySprite, shipSprite, pulseSprite, slimeSprite, blueeyeSprite, goldeyeSprite;
        public static Texture2D[] wolfSprite, rabbitSprite, boarSprite;
        public static Texture2D[] explosionSprite;
        Random random;
        int maxEnemiesOnScreen;
        int killsNeeded;
        ContentManager content = GameplayScreen.Content;        

        public EnemyGenerator()
        {
            random = new Random();
            enemies = new List<EnemyObject>();
            maxEnemiesOnScreen = 10;
            killsNeeded = (int)level.one;
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

            //make the wolf enemy
            wolfSprite = new Texture2D[3];
            for (int i = 0; i < 3; i++)
            {
                wolfSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\wolf" + i.ToString());
            }

            //make the rabbit enemy
            rabbitSprite = new Texture2D[6];
            for (int i = 0; i < 6; i++)
            {
                rabbitSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\rabbit" + i.ToString());
            }

            //make the boar enemy
            boarSprite = new Texture2D[2];
            for (int i = 0; i < 2; i++)
            {
                boarSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\boar" + i.ToString());
            }

            //make the spikey enemy
            spikeySprite = new Texture2D[1];
            spikeySprite[0] = Content.Load<Texture2D>("Sprites\\Enemies\\spikey");

            //make the ship enemy
            shipSprite = new Texture2D[1];
            shipSprite[0] = Content.Load<Texture2D>("Sprites\\Enemies\\ship");

            //make the explosions
            explosionSprite = new Texture2D[16];
            for (int i = 0; i < 16; i++)
            {
                explosionSprite[i] = Content.Load<Texture2D>("Sprites\\Enemies\\explosion" + i.ToString());
            }
        }

        public void Update()
        {
            foreach (EnemyObject enemy in enemies)
                enemy.Update();

            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].alive && !enemies[i].dying)
                    enemies.RemoveAt(i);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (EnemyObject enemy in enemies)
            {
                if (enemy.dying)
                {
                    enemy.sprite = new Texture2D[explosionSprite.Length];
                    explosionSprite.CopyTo(enemy.sprite, 0);
                }

                enemy.Draw(gameTime, spriteBatch);
            }
        }

        public void MakeEnemy(level level)
        {
            Random rand = new Random();
            Random rand2 = new Random();
            int side = 0;
            int velocity = 5;
            if (rand2.Next(2) == 0)
            {
                side = 0;
                velocity = 5;
            }
            else
            {
                side = GameStateManagement.GameplayScreen.viewportRect.Width;
                velocity = -5;
            }

            if (enemies.Count() < (int)maxEnemiesOnScreen && killsNeeded > 0)
            {
                switch (level)
                {
                    case level.one: 
                        switch (rand.Next(4))
                        {
                            case 0:
                                enemies.Add(new EnemyObject(axeSprite, new Vector2(side, (GameStateManagement.GameplayScreen.viewportRect.Height - 200) * MathHelper.Clamp((float)random.NextDouble(), 0, GameStateManagement.GameplayScreen.viewportRect.Height - 150)), new Vector2(3, 0)));
                                if(side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                enemies[enemies.Count() - 1].sound = content.Load<SoundEffect>("Sounds\\Enemies\\whoosh2");
                                break;
                            case 1:
                                enemies.Add(new EnemyObject(ballSprite, new Vector2(side, (GameStateManagement.GameplayScreen.viewportRect.Height - 200) * MathHelper.Clamp((float)random.NextDouble(), 0, GameStateManagement.GameplayScreen.viewportRect.Height - 150)), new Vector2(4, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                enemies[enemies.Count() - 1].sound = content.Load<SoundEffect>("Sounds\\Enemies\\R2D2");
                                break;
                            case 2:
                                enemies.Add(new EnemyObject(lanceSprite, new Vector2(side, (GameStateManagement.GameplayScreen.viewportRect.Height - 200) * MathHelper.Clamp((float)random.NextDouble(), 0, GameStateManagement.GameplayScreen.viewportRect.Height - 150)), new Vector2(5, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                enemies[enemies.Count() - 1].sound = content.Load<SoundEffect>("Sounds\\Enemies\\sfxlightsaber");
                                break;
                            case 3:
                                enemies.Add(new EnemyObject(boarSprite, new Vector2(side, GameStateManagement.GameplayScreen.viewportRect.Height - 150), new Vector2(5, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                //enemies[enemies.Count() - 1].sound = content.Load<SoundEffect>("Sounds\\Enemies\\bubbles");
                                break;
                        }
                        break;
                    case level.two:
                        switch (rand.Next(5))
                        {
                            case 0:
                                enemies.Add(new EnemyObject(blueeyeSprite, new Vector2(side, (GameStateManagement.GameplayScreen.viewportRect.Height - 200) * MathHelper.Clamp((float)random.NextDouble(), 0, GameStateManagement.GameplayScreen.viewportRect.Height - 150)), new Vector2(3, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                enemies[enemies.Count() - 1].sound = null;
                                break;
                            case 1:
                                enemies.Add(new EnemyObject(slimeSprite, new Vector2(side, GameStateManagement.GameplayScreen.viewportRect.Height - 150), new Vector2(2, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                enemies[enemies.Count() - 1].sound = content.Load<SoundEffect>("Sounds\\Enemies\\bubbles");
                                break;
                            case 2:
                                enemies.Add(new EnemyObject(goldeyeSprite, new Vector2(side, (GameStateManagement.GameplayScreen.viewportRect.Height - 200) * MathHelper.Clamp((float)random.NextDouble(), 0, GameStateManagement.GameplayScreen.viewportRect.Height - 150)), new Vector2(3, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                enemies[enemies.Count() - 1].sound = null;
                                break;
                            case 3:
                                enemies.Add(new EnemyObject(spikeySprite, new Vector2(side, (GameStateManagement.GameplayScreen.viewportRect.Height - 200) * MathHelper.Clamp((float)random.NextDouble(), 0, GameStateManagement.GameplayScreen.viewportRect.Height - 150)), new Vector2(4, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                enemies[enemies.Count() - 1].sound = null;
                                break;
                            case 4:
                                enemies.Add(new EnemyObject(wolfSprite, new Vector2(side, GameStateManagement.GameplayScreen.viewportRect.Height - 150), new Vector2(7, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                //enemies[enemies.Count() - 1].sound = content.Load<SoundEffect>("Sounds\\Enemies\\bubbles");
                                break;
                        }
                        break;
                    case level.three:
                        switch (rand.Next(4))
                        {
                            case 0:
                                enemies.Add(new EnemyObject(pulseSprite, new Vector2(side, (GameStateManagement.GameplayScreen.viewportRect.Height - 200) * MathHelper.Clamp((float)random.NextDouble(), 0, GameStateManagement.GameplayScreen.viewportRect.Height - 150)), new Vector2(7, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                enemies[enemies.Count() - 1].sound = content.Load<SoundEffect>("Sounds\\Enemies\\ESPARK1");
                                break;
                            case 1:
                                enemies.Add(new EnemyObject(shipSprite, new Vector2(side, GameStateManagement.GameplayScreen.viewportRect.Height - 150), new Vector2(5, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                enemies[enemies.Count() - 1].sound = null;
                                break;
                            case 2:
                                enemies.Add(new EnemyObject(thingSprite, new Vector2(side, (GameStateManagement.GameplayScreen.viewportRect.Height - 200) * MathHelper.Clamp((float)random.NextDouble(), 0, GameStateManagement.GameplayScreen.viewportRect.Height - 150)), new Vector2(3, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                enemies[enemies.Count() - 1].sound = content.Load<SoundEffect>("Sounds\\Enemies\\alarm4");
                                break;
                            case 3:
                                enemies.Add(new EnemyObject(rabbitSprite, new Vector2(side, GameStateManagement.GameplayScreen.viewportRect.Height - 150), new Vector2(0.5f, 0)));
                                if (side != 0)
                                    enemies[enemies.Count() - 1].position.X -= enemies[enemies.Count() - 1].sprite[enemies[enemies.Count() - 1].spritePosition].Width;
                                //enemies[enemies.Count() - 1].sound = content.Load<SoundEffect>("Sounds\\Enemies\\bubbles");
                                break;
                        }
                        break;

                }
            }
        }

        public static void playExplosion()
        {
            SoundEffect explosion = GameplayScreen.Content.Load<SoundEffect>("Sounds\\explosion-02");
            explosion.Play();
        }

        public void setKillsNeeded(int kills)
        {            
            killsNeeded = kills;
        }
    }
}