#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

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
    enum Dirs { RUNLEFT, RUNRIGHT, UPRIGHT, UPLEFT, UP, RUNSHOOTRIGHT, RUNSHOOTLEFT, STANDRIGHT, STANDLEFT, JUMPUP, JUMPDOWN };   

    #region GameObject
    class GameObject
    {
        public int health;
        public Texture2D sprite;        
        public Dirs dir = Dirs.STANDRIGHT;
        public float timer = 0;
        public float interval = 1000f / 6f;
        public int frameCountRun = 6, frameCountStand = 4, frameCountUpright = 7, frameCountUp = 2, frameCountRunShoot = 4, frameCountJump = 8;
        public int currentFrame = 0;
        public Texture2D [] spriteSheetStand, spriteSheetRun, spriteSheetUpright, spriteSheetUp, spriteSheetRunShoot, spriteSheetJump;
        public int spriteHeight = 75, spriteHeightJump = 55;
        public int spriteWidthRun = 67, spriteWidthStand = 68, spriteWidthUpright = 50, spriteWidthUp = 33, spriteWidthRunShoot = 66;
        public int spriteWidthJump = 52;
        public Rectangle destRect;
        public Vector2 position;
        public Vector2 center;
        public Vector2 width;

        public GameObject(Texture2D loadedTexture)
        {
            position = Vector2.Zero;
            sprite = loadedTexture;
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            width = new Vector2(sprite.Width / 2);
        }
    }
    #endregion

    #region BallObject
    public class BallObject : GameplayScreen
    {
        public Texture2D sprite;
        public Vector2 position;
        public float rotation;
        public Vector2 center;
        public Vector2 velocity;
        public bool alive;
        public Vector2 width;
        public Rectangle rect;

        public BallObject(Texture2D loadedTexture)
        {
            rotation = 0.0f;
            position = Vector2.Zero;
            sprite = loadedTexture;
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            width = new Vector2(sprite.Width / 2);
            velocity = Vector2.Zero;
            alive = false;
            rect = new Rectangle(0, 0, sprite.Width, sprite.Height);
        }
    }
    #endregion    

    #region SuperBallObject
    public class SuperBallObject
    {
        public Texture2D sprite;
        public Vector2 position;
        public float rotation;
        public Vector2 center;
        public Rectangle destRect;
        public int currentFrame = 0;
        public Vector2 velocity;
        public Texture2D[] FireBallFrame;
        public bool alive;
        public Vector2 width;       

        public SuperBallObject(Texture2D loadedTexture)
        {
            rotation = 0.0f;
            position = Vector2.Zero;
            sprite = loadedTexture;
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            width = new Vector2(sprite.Width / 2);
            velocity = Vector2.Zero;
            alive = false;
        }
    }
    #endregion  

    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Globals
        ContentManager Content;
        
        SpriteFont gameFont;
        SpriteFont font;
        SpriteFont outlineFont;

        string egg = "";

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();      

        SpriteBatch spriteBatch;
        Texture2D backgroundTexture;
        Rectangle viewportRect;
        GameObject dude;

        int bulletOptions = OptionsMenuScreen.currentBullets;
        int maxdudeBalls = 30;
        int maxSuperdudeBalls = 3;
        int superCount = 0;
        public BallObject[] dudeBalls;
        public SuperBallObject[] SuperdudeBalls;
        EnemyObject[] enemies;
        Texture2D healthBar;
        GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);
        KeyboardState previousKeyboardState = Keyboard.GetState();
        float spaceTime;

        Song eEgg;
        Song lastLevelIntro;
        Song lastLevelSong;
        SoundEffect gunShot;
        SoundEffect explosion;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            if (bulletOptions == 0)
                maxdudeBalls = 30;
            else if (bulletOptions == 1)
                maxdudeBalls = 15;
            else if (bulletOptions == 2)
                maxdudeBalls = 5;

            //graphics = new GraphicsDeviceManager(ScreenManager);
            //Content.RootDirectory = "Content";
        }
        #endregion

        #region Load/Unload Content
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = Content.Load<SpriteFont>("Fonts\\gamefont");
            font = Content.Load<SpriteFont>("Fonts\\menufont");
            outlineFont = Content.Load<SpriteFont>("Fonts\\outline");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);

            eEgg = Content.Load<Song>("Sounds\\Some Cut");
            lastLevelIntro = Content.Load<Song>("Sounds\\Europe - The Final Countdown");
            lastLevelSong = Content.Load<Song>("Sounds\\through_the");
            gunShot = Content.Load<SoundEffect>("Sounds\\Gun_Silencer");
            explosion = Content.Load<SoundEffect>("Sounds\\explosion-02");

            backgroundTexture = Content.Load<Texture2D>("Backgrounds\\back5");

            dude = new GameObject(Content.Load<Texture2D>("Sprites\\Contra\\Stand\\contra-stand0"));

            dude.spriteSheetStand = new Texture2D [4];
            for (int i = 0; i < 4; i++)
                dude.spriteSheetStand[i] = Content.Load<Texture2D>("Sprites\\Contra\\Stand\\contra-stand" + i.ToString());

            dude.spriteSheetRun = new Texture2D[6];
            for (int i = 0; i < 6; i++)
                dude.spriteSheetRun[i] = Content.Load<Texture2D>("Sprites\\Contra\\Run\\contra-right" + i.ToString());

            dude.spriteSheetRunShoot = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                dude.spriteSheetRunShoot[i] = Content.Load<Texture2D>("Sprites\\Contra\\RunShoot\\contra-run-shoot" + i.ToString());

            dude.spriteSheetUp = new Texture2D[2];
            dude.spriteSheetUp[0] = Content.Load<Texture2D>("Sprites\\Contra\\Up\\contra-up0");
            dude.spriteSheetUp[1] = Content.Load<Texture2D>("Sprites\\Contra\\Up\\contra-up1");

            dude.spriteSheetJump = new Texture2D[8];
            for (int i = 0; i < 8; i++)
                dude.spriteSheetJump[i] = Content.Load<Texture2D>("Sprites\\Contra\\Jump\\contra-jump" + i.ToString());

            dude.spriteSheetUpright = new Texture2D[7];
            for (int i = 0; i < 7; i++)
                dude.spriteSheetUpright[i] = Content.Load<Texture2D>("Sprites\\Contra\\Upright\\contra-upright" + i.ToString());


            dude.destRect = new Rectangle((int)ScreenManager.GraphicsDevice.Viewport.Width / 2, (int)ScreenManager.GraphicsDevice.Viewport.Height - 150, dude.spriteWidthStand, dude.spriteHeight);
            dude.position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height - 80);

            dude.health = 100;
            healthBar = Content.Load<Texture2D>("Sprites\\Health Bar\\HealthBar");

            dudeBalls = new BallObject[maxdudeBalls];
            for (int i = 0; i < maxdudeBalls; i++)
            {
                dudeBalls[i] = new BallObject(Content.Load<Texture2D>("Sprites\\Weapons\\cannonball"));
            }

            SuperdudeBalls = new SuperBallObject[maxSuperdudeBalls];
            for (int i = 0; i < maxSuperdudeBalls; i++)
            {                
                SuperdudeBalls[i] = new SuperBallObject(Content.Load<Texture2D>("Sprites\\Weapons\\FireBallRightF1"));
                SuperdudeBalls[i].FireBallFrame = new Texture2D[2];
                SuperdudeBalls[i].FireBallFrame[0] = Content.Load<Texture2D>("Sprites\\Weapons\\FireBallRightF1");
                SuperdudeBalls[i].FireBallFrame[1] = Content.Load<Texture2D>("Sprites\\Weapons\\FireBallRightF2");
            }  
            
            viewportRect = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            //if (backgroundTexture.ToString() == "back5")
            //{
                //MediaPlayer.Play(lastLevelIntro);
                //while (MediaPlayer.State == MediaState.Playing)
                //{
                //    //Do nothing
                //}
                //MediaPlayer.Stop();
                //Thread.Sleep(13000);
                //MediaPlayer.Play(lastLevelSong);
            //}
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            Content.Unload();
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {            
            if (IsActive)
            {               
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
#if XBOX
                KeyboardState keyboardState = Keyboard.GetState();

                #region No Keys Pressed               
                /*If the sprite is not jumping then use the standing sprite based on which way was facing
                  before no keys were pressed again */
                if ((gamePadState.ThumbSticks.Left.X > -0.2f && gamePadState.ThumbSticks.Left.X < 0.2f) && (gamePadState.ThumbSticks.Left.Y > -0.2f && gamePadState.ThumbSticks.Left.Y < 0.2f))
                {
                    if (dude.dir != Dirs.JUMPUP && dude.dir != Dirs.JUMPDOWN)
                    {
                        if (superCount > 75 && previousGamePadState.Triggers.Left < 0.5f)
                        {
                            superCount = 0;
                            //foreach (SuperBallObject superBall in SuperdudeBalls)
                            //{
                            //    if (superBall.alive)
                            //    {
                            //        if (superBall.currentFrame > superBall.frameCountFB - 1)
                            //            superBall.currentFrame = 0;
                            //        dude.timer = 0.0f;
                            //        superBall.sourceRect = new Rectangle(superBall.spriteWidthSpecial, 0, superBall.spriteWidthSpecial, superBall.spriteHeightSpecial);
                            //    }
                            //}
                            SuperFireDudeBall();
                        }

                        if (dude.dir == Dirs.RUNRIGHT || dude.dir == Dirs.RUNSHOOTRIGHT || dude.dir == Dirs.UPRIGHT || dude.dir == Dirs.UP || dude.dir == Dirs.STANDRIGHT)
                        {
                            dude.currentFrame = 0;
                            dude.dir = Dirs.STANDRIGHT;
                        }
                        else if (dude.dir == Dirs.RUNLEFT || dude.dir == Dirs.RUNSHOOTLEFT || dude.dir == Dirs.UPLEFT)
                        {
                            dude.currentFrame = 0;
                            dude.dir = Dirs.STANDLEFT;
                        }

                        if (dude.dir == Dirs.JUMPDOWN || dude.dir == Dirs.JUMPUP)
                            dude.currentFrame = 0;

                        if (dude.timer > dude.interval)
                        {
                            dude.currentFrame++;
                            if (dude.currentFrame > dude.spriteSheetStand.Length - 1)
                                dude.currentFrame = 0;
                            dude.timer = 0.0f;
                        }

                        dude.sprite = dude.spriteSheetStand[dude.currentFrame];
                    }
                    //Else he was jumping when the keys were stopped being pressed so continue the jumping procedure
                    else
                        Jump();
                }
                //}
                #endregion
                #region One Key Pressed     
                if (dude.dir != Dirs.JUMPDOWN && dude.dir != Dirs.JUMPUP)
                {
                    #region Running/Shooting
                    /*This code checks to see the the sprite was previously shooting. If so then do not run the running
                      without shooting sprite sheet for half a second (Stops the jerkyness while shooting and running */
                    if ((dude.dir == Dirs.RUNSHOOTLEFT || dude.dir == Dirs.RUNSHOOTRIGHT) && spaceTime < 500)
                    {
                        spaceTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (gamePadState.ThumbSticks.Left.X >= 0.25f && (gamePadState.ThumbSticks.Left.Y >= -0.25f && gamePadState.ThumbSticks.Left.Y <= 0.25f))                        
                        {
                            dude.dir = Dirs.RUNSHOOTRIGHT;
                            dude.destRect.X += 4;
                        }
                        else if (gamePadState.ThumbSticks.Left.X <= -0.25f && (gamePadState.ThumbSticks.Left.Y >= -0.25f && gamePadState.ThumbSticks.Left.Y <= 0.25f))
                        {
                            dude.dir = Dirs.RUNSHOOTLEFT;
                            dude.destRect.X -= 4;
                        }

                        if (dude.timer > dude.interval)
                        {
                            dude.currentFrame++;
                            if (dude.currentFrame > dude.spriteSheetRunShoot.Length - 1)
                                dude.currentFrame = 0;
                            dude.timer = 0.0f;
                        }

                        dude.sprite = dude.spriteSheetRunShoot[dude.currentFrame];
                    }
                    #endregion
                    else
                    {
                        spaceTime = 0;
                        if (gamePadState.ThumbSticks.Left.X > -0.25f && gamePadState.ThumbSticks.Left.X < 0.25f)
                        {
                            #region Right Look
                            if (gamePadState.ThumbSticks.Right.X >= 0.25f && (gamePadState.ThumbSticks.Right.Y >= -0.25f && gamePadState.ThumbSticks.Right.Y <= 0.25f))
                            {
                                dude.dir = Dirs.STANDRIGHT;
                                dude.sprite = dude.spriteSheetStand[1];

                                if (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right < 0.5f)
                                    FireDudeBall();
                            }
                            #endregion
                            #region Upright Look
                            else if (gamePadState.ThumbSticks.Right.X >= 0.5f && gamePadState.ThumbSticks.Right.Y >= 0.5f)
                            {
                                dude.dir = Dirs.UPRIGHT;
                                dude.sprite = dude.spriteSheetUpright[0];

                                if (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right < 0.5f)
                                    FireDudeBall();
                            }
                            #endregion
                            #region Up Look
                            else if ((gamePadState.ThumbSticks.Right.X > -0.25f && gamePadState.ThumbSticks.Right.X < 0.25f) && gamePadState.ThumbSticks.Right.Y >= 0.5f)
                            {
                                dude.dir = Dirs.UP;
                                dude.sprite = dude.spriteSheetUp[1];

                                if (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right < 0.5f)
                                    FireDudeBall();
                            }
                            #endregion
                            #region Upleft Look
                            else if (gamePadState.ThumbSticks.Right.X < -0.5f && gamePadState.ThumbSticks.Right.Y > 0.5f)
                            {
                                dude.dir = Dirs.UPLEFT;
                                dude.sprite = dude.spriteSheetUpright[1];

                                if (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right < 0.5f)
                                    FireDudeBall();
                            }
                            #endregion
                            #region Left Look
                            else if (gamePadState.ThumbSticks.Right.X < -0.25f && (gamePadState.ThumbSticks.Right.Y >= -0.25f && gamePadState.ThumbSticks.Right.Y <= 0.25f))
                            {
                                dude.dir = Dirs.STANDLEFT;
                                dude.sprite = dude.spriteSheetStand[1];

                                if (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right < 0.5f)
                                    FireDudeBall();
                            }
                            #endregion
                            #region Right Trigger
                            else if (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right < 0.5f)
                            {
                                FireDudeBall();
                            }
                            #endregion
                            #region Left Trigger
                            else if (gamePadState.Triggers.Left > 0.5f && previousGamePadState.Triggers.Left > 0.5f)
                            {
                                superCount++;
                            }
                            #endregion
                            #region A Press
                            if (gamePadState.Buttons.A == ButtonState.Pressed && previousGamePadState.Buttons.A == ButtonState.Released)
                            {
                                dude.dir = Dirs.JUMPUP;
                                Jump();
                            }
                            #endregion
                        }
                        else
                        {
                            #region Move Right and Shoot
                            if (gamePadState.ThumbSticks.Left.X >= 0.25f &&
                                (gamePadState.ThumbSticks.Left.Y >= -0.25f && gamePadState.ThumbSticks.Left.Y <= 0.25f) &&
                                (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right < 0.5f))
                            {
                                if (dude.dir != Dirs.RUNSHOOTRIGHT)
                                    dude.currentFrame = 0;

                                if (dude.timer > dude.interval)
                                {
                                    dude.currentFrame++;
                                    if (dude.currentFrame > dude.spriteSheetRunShoot.Length - 1)
                                        dude.currentFrame = 0;
                                    dude.timer = 0.0f;
                                }

                                dude.sprite = dude.spriteSheetRunShoot[dude.currentFrame];
                                dude.dir = Dirs.RUNSHOOTRIGHT;
                                dude.destRect.X += 2;

                                FireDudeBall();
                            }
                            #endregion
                            #region Move Right
                            else if (gamePadState.ThumbSticks.Left.X >= 0.25f && (gamePadState.ThumbSticks.Left.Y >= -0.25f && gamePadState.ThumbSticks.Left.Y <= 0.25f))
                            {
                                if (dude.dir != Dirs.RUNRIGHT)
                                    dude.currentFrame = 0;

                                if (dude.timer > dude.interval)
                                {
                                    dude.currentFrame++;
                                    if (dude.currentFrame > dude.spriteSheetRun.Length - 1)
                                        dude.currentFrame = 0;
                                    dude.timer = 0.0f;
                                }

                                dude.sprite = dude.spriteSheetRun[dude.currentFrame];
                                dude.dir = Dirs.RUNRIGHT;
                                dude.destRect.X += 2;                                
                            }
                            #endregion                            
                            #region Move Upright (and shoot)
                            else if (gamePadState.ThumbSticks.Left.X >= 0.5f && gamePadState.ThumbSticks.Left.Y >= 0.5f &&
                                (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right < 0.5f))
                            {
                                if (dude.dir != Dirs.UPRIGHT)
                                    dude.currentFrame = 0;

                                if (dude.timer > dude.interval)
                                {
                                    dude.currentFrame++;
                                    if (dude.currentFrame > dude.spriteSheetUpright.Length - 1)
                                        dude.currentFrame = 0;
                                    dude.timer = 0.0f;
                                }

                                dude.sprite = dude.spriteSheetUpright[dude.currentFrame];
                                dude.dir = Dirs.UPRIGHT;

                                dude.destRect.X += 4;

                                FireDudeBall();
                            }
                            #endregion
                            #region Move Upright
                            else if (gamePadState.ThumbSticks.Left.X >= 0.5f && gamePadState.ThumbSticks.Left.Y >= 0.5f)
                            {
                                if (dude.dir != Dirs.UPRIGHT)
                                    dude.currentFrame = 0;

                                if (dude.timer > dude.interval)
                                {
                                    dude.currentFrame++;
                                    if (dude.currentFrame > dude.spriteSheetUpright.Length - 1)
                                        dude.currentFrame = 0;
                                    dude.timer = 0.0f;
                                }

                                dude.sprite = dude.spriteSheetUpright[dude.currentFrame];
                                dude.dir = Dirs.UPRIGHT;

                                dude.destRect.X += 4;
                            }
                            #endregion                            
                            #region Up (and Shoot)
                            else if (gamePadState.ThumbSticks.Left.Y >= 0.5f && (gamePadState.ThumbSticks.Left.X >= -0.25f && gamePadState.ThumbSticks.Left.X <= 0.25f))
                            {
                                if (dude.dir != Dirs.UP)
                                    dude.currentFrame = 0;

                                dude.dir = Dirs.UP;

                                if (dude.timer > dude.interval)
                                {
                                    dude.currentFrame++;
                                    if (dude.currentFrame > dude.spriteSheetUp.Length - 1)
                                        dude.currentFrame = 0;
                                    dude.timer = 0.0f;
                                }

                                dude.sprite = dude.spriteSheetUp[dude.currentFrame];

                                if (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right <= 0.5f)
                                    FireDudeBall();
                            }
                            #endregion
                            #region Move Upleft (and shoot)
                            else if ((gamePadState.ThumbSticks.Left.X <= -0.5f && gamePadState.ThumbSticks.Left.Y >= 0.5f) &&
                                    (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right <= 0.5f))
                            {
                                if (dude.dir != Dirs.UPLEFT)
                                    dude.currentFrame = 0;

                                if (dude.timer > dude.interval)
                                {
                                    dude.currentFrame++;
                                    if (dude.currentFrame > dude.spriteSheetUpright.Length - 1)
                                        dude.currentFrame = 0;
                                    dude.timer = 0.0f;
                                }

                                dude.sprite = dude.spriteSheetUpright[dude.currentFrame];
                                dude.dir = Dirs.UPLEFT;

                                dude.destRect.X -= 4;

                                FireDudeBall();
                            }
                            #endregion                            
                            #region Move Upleft
                            else if (gamePadState.ThumbSticks.Left.X <= -0.5f && gamePadState.ThumbSticks.Left.Y >= 0.5f)
                            {
                                if (dude.dir != Dirs.UPLEFT)
                                    dude.currentFrame = 0;

                                if (dude.timer > dude.interval)
                                {
                                    dude.currentFrame++;
                                    if (dude.currentFrame > dude.spriteSheetUpright.Length - 1)
                                        dude.currentFrame = 0;
                                    dude.timer = 0.0f;
                                }

                                dude.sprite = dude.spriteSheetUpright[dude.currentFrame];
                                dude.dir = Dirs.UPLEFT;

                                dude.destRect.X -= 4;
                            }
                            #endregion         
                            #region Move Left and Shoot
                            else if ((gamePadState.ThumbSticks.Left.X <= -0.25f &&
                                    (gamePadState.ThumbSticks.Left.Y >= -0.25f && gamePadState.ThumbSticks.Left.Y <= 0.25f)) &&
                                    (gamePadState.Triggers.Right >= 0.5f && previousGamePadState.Triggers.Right < 0.5f))
                            {
                                if (dude.dir != Dirs.RUNSHOOTLEFT)
                                    dude.currentFrame = 0;

                                dude.dir = Dirs.RUNSHOOTLEFT;

                                if (dude.timer > dude.interval)
                                {
                                    dude.currentFrame++;
                                    if (dude.currentFrame > dude.spriteSheetRunShoot.Length - 1)
                                        dude.currentFrame = 0;
                                    dude.timer = 0.0f;
                                }

                                dude.sprite = dude.spriteSheetRunShoot[dude.currentFrame];
                                FireDudeBall();
                            }

                            #endregion    
                            #region Move Left
                            else if (gamePadState.ThumbSticks.Left.X <= -0.25f && (gamePadState.ThumbSticks.Left.Y >= -0.25f && gamePadState.ThumbSticks.Left.Y <= 0.25f))
                            {
                                if (dude.dir != Dirs.RUNLEFT)
                                    dude.currentFrame = 0;

                                if (dude.timer > dude.interval)
                                {
                                    dude.currentFrame++;
                                    if (dude.currentFrame > dude.spriteSheetRun.Length - 1)
                                        dude.currentFrame = 0;
                                    dude.timer = 0.0f;
                                }

                                dude.sprite = dude.spriteSheetRun[dude.currentFrame];
                                dude.dir = Dirs.RUNLEFT;
                                dude.destRect.X -= 2;
                            }
                            #endregion                                                                                                                                        
                            #region A Press
                            if (gamePadState.Buttons.A == ButtonState.Pressed && previousGamePadState.Buttons.A == ButtonState.Released)
                            {
                                dude.dir = Dirs.JUMPUP;
                                Jump();
                            }
                            #endregion
                        }
                    }
                }
                else
                    Jump();
                #endregion

                #region Boundaries
                if (dude.destRect.X > ScreenManager.GraphicsDevice.Viewport.Width - dude.destRect.Width)
                {
                    dude.destRect.X -= 5;
                }
                else if (dude.destRect.X < 5)
                {
                    dude.destRect.X += 5;
                }
                #endregion
#endif

                dude.timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                UpdateDudeBalls();
                UpdateSuperDudeBalls();
                // TODO: Add your update logic here

                previousGamePadState = gamePadState;
#if !XBOX
                previousKeyboardState = keyboardState;
#endif
            }

            //foreach (EnemyObject enemy in enemies)
            //    enemy.Update();

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        #endregion

        #region Bullets
        public void UpdateDudeBalls()
        {
            foreach (BallObject ball in dudeBalls)
            {             
                if (ball.alive)
                {                    
                    ball.position += ball.velocity;
                    if (!viewportRect.Contains(new Point((int)ball.position.X, (int)ball.position.Y)))
                    {
                        ball.alive = false;
                        continue;
                    }
                }
            }
        }

        public void UpdateSuperDudeBalls()
        {
            foreach (SuperBallObject ball in SuperdudeBalls)
            {
                if (ball.alive)
                {
                    if (ball.currentFrame <= 3)
                    {
                        ball.currentFrame++;
                        ball.sprite = ball.FireBallFrame[1];
                    }
                    else if(ball.currentFrame <= 8)
                    {
                        if (ball.currentFrame == 8)
                            ball.currentFrame = 0;
                        else
                            ball.currentFrame++;
                        ball.sprite = ball.FireBallFrame[0];
                    }

                    ball.position += ball.velocity;
                    if (!viewportRect.Contains(new Point((int)ball.position.X, (int)ball.position.Y)))
                    {
                        ball.alive = false;
                        continue;
                    }
                }
            }
        }

        public void FireDudeBall()
        {
            foreach (BallObject ball in dudeBalls)
            {
                if (!ball.alive)
                {
                    gunShot.Play();
                    ball.alive = true;
                    ball.position.Y = dude.position.Y - dude.sprite.Height / 2 - 12;
                    if (dude.dir == Dirs.RUNSHOOTLEFT || dude.dir == Dirs.RUNLEFT || dude.dir == Dirs.STANDLEFT)
                    {
                        ball.position.X = dude.destRect.X + dude.destRect.Width / 2 - 35;
                        ball.velocity.X = -8.0f;
                        ball.velocity.Y = 0.0f;
                    }
                    else if (dude.dir == Dirs.RUNSHOOTRIGHT || dude.dir == Dirs.RUNRIGHT || dude.dir == Dirs.STANDRIGHT)
                    {
                        ball.position.X = dude.destRect.X + dude.destRect.Width / 2 + 15;
                        ball.velocity.X = 8.0f;
                        ball.velocity.Y = 0.0f;
                    }
                    else if (dude.dir == Dirs.UPRIGHT)
                    {
                        ball.position.Y -= 15;
                        ball.position.X = dude.destRect.X + dude.destRect.Width / 2 + 5;
                        ball.velocity.X = 8.0f;
                        ball.velocity.Y = -4.0f;
                    }
                    else if (dude.dir == Dirs.UPLEFT)
                    {
                        ball.position.Y -= 15;
                        ball.position.X = dude.destRect.X + dude.destRect.Width / 2 - 15;
                        ball.velocity.X = -8.0f;
                        ball.velocity.Y = -4.0f;
                    }
                    else if (dude.dir == Dirs.UP)
                    {
                        ball.position.Y -= 15;
                        ball.position.X = dude.destRect.X + dude.destRect.Width / 2 - 12;
                        ball.velocity.X = 0.0f;
                        ball.velocity.Y = -8.0f;
                    }
                    return;
                }
            }
        }

        public void SuperFireDudeBall()
        {
            foreach (SuperBallObject ball in SuperdudeBalls)
            {
                if (!ball.alive)
                {
                    ball.alive = true;
                    ball.position.Y = dude.position.Y - dude.sprite.Height / 2 - 65;
                    if (dude.dir == Dirs.RUNSHOOTLEFT || dude.dir == Dirs.RUNLEFT || dude.dir == Dirs.STANDLEFT)
                    {
                        ball.position.X = dude.destRect.X + dude.destRect.Width / 2 - 150;
                        ball.velocity.X = -8.0f;
                        ball.velocity.Y = 0.0f;
                    }
                    else if (dude.dir == Dirs.RUNSHOOTRIGHT || dude.dir == Dirs.RUNRIGHT || dude.dir == Dirs.STANDRIGHT)
                    {
                        ball.position.X = dude.destRect.X + dude.destRect.Width / 2 + 15;
                        ball.velocity.X = 8.0f;
                        ball.velocity.Y = 0.0f;
                    }
                    else if (dude.dir == Dirs.UPRIGHT)
                    {
                        ball.position.Y -= 15;
                        ball.position.X = dude.destRect.X + dude.destRect.Width / 2 + 5;
                        ball.velocity.X = 8.0f;
                        ball.velocity.Y = -4.0f;
                    }
                    else if (dude.dir == Dirs.UPLEFT)
                    {
                        ball.position.Y -= 15;
                        ball.position.X = dude.destRect.X + dude.destRect.Width / 2 - 15;
                        ball.velocity.X = -8.0f;
                        ball.velocity.Y = -4.0f;
                    }
                    else if (dude.dir == Dirs.UP)
                    {
                        ball.position.Y -= 15;
                        ball.position.X = dude.destRect.X + dude.destRect.Width / 2 - 12;
                        ball.velocity.X = 0.0f;
                        ball.velocity.Y = -8.0f;
                    }
                    return;
                }
            }
        }
        #endregion

        #region Jump
        public void Jump()
        {
            if (dude.timer > dude.interval)
            {
                dude.currentFrame++;
                if (dude.currentFrame > dude.spriteSheetJump.Length - 1)
                    dude.currentFrame = 0;
                dude.timer = 0.0f;
            }

            dude.sprite = dude.spriteSheetJump[dude.currentFrame];


            if (dude.destRect.Y > 100 && dude.dir == Dirs.JUMPUP)
            {
                dude.destRect.Y -= 4;
                dude.dir = Dirs.JUMPUP;
            }
            else
                dude.dir = Dirs.JUMPDOWN;

            if (dude.destRect.Y < 450 && dude.dir == Dirs.JUMPDOWN)
            {
                dude.destRect.Y += 4;
                dude.dir = Dirs.JUMPDOWN;
            }

            if (dude.destRect.Y >= ScreenManager.GraphicsDevice.Viewport.Height - 150)
            {
                dude.destRect.Y = ScreenManager.GraphicsDevice.Viewport.Height - 150;
                dude.dir = Dirs.STANDRIGHT;
            }
        }
        #endregion

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 2;
            }
        }

        #region Draw
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            //spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);

            //spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
            //                       enemyPosition, Color.DarkRed);
            spriteBatch.Draw(backgroundTexture, viewportRect, Color.White);

            if (dude.dir == Dirs.RUNLEFT || dude.dir == Dirs.UPLEFT || dude.dir == Dirs.RUNSHOOTLEFT || dude.dir == Dirs.STANDLEFT)            
                spriteBatch.Draw(dude.sprite, dude.destRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);               
            else if (dude.dir == Dirs.RUNRIGHT || dude.dir == Dirs.UPRIGHT || dude.dir == Dirs.UP || dude.dir == Dirs.RUNSHOOTRIGHT || dude.dir == Dirs.STANDRIGHT)
                spriteBatch.Draw(dude.sprite, dude.destRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            else if (dude.dir == Dirs.JUMPUP || dude.dir == Dirs.JUMPDOWN)
                spriteBatch.Draw(dude.sprite, dude.destRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);

            //spriteBatch.Draw(dude.sprite, dude.destRect, Color.White);

            foreach (BallObject ball in dudeBalls)
            {
                if (ball.alive)
                {
                    spriteBatch.Draw(ball.sprite, ball.position, Color.White);
                }
            }

            foreach (SuperBallObject sball in SuperdudeBalls)
            {
                if (sball.alive)
                {
                    if (dude.dir == Dirs.RUNLEFT || dude.dir == Dirs.UPLEFT || dude.dir == Dirs.RUNSHOOTLEFT || dude.dir == Dirs.STANDLEFT)
                        spriteBatch.Draw(sball.sprite, sball.position, null, Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0.0f);
                    else if (dude.dir == Dirs.RUNRIGHT || dude.dir == Dirs.UPRIGHT || dude.dir == Dirs.UP || dude.dir == Dirs.RUNSHOOTRIGHT || dude.dir == Dirs.STANDRIGHT)
                        spriteBatch.Draw(sball.sprite, sball.position, null, Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);

                    //if (leftSuperFB == true)
                    //{
                    //    spriteBatch.Draw(sball.spriteleft, dude.destRect, sball.sourceRect, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                    //    spriteBatch.Draw(sball.spriteleft, sball.position, Color.White);
                    //}
                    //else
                    //    spriteBatch.Draw(sball.sprite, sball.position, Color.White);
                }
            }

            //Draw the negative space for the health bar            
            spriteBatch.Draw(healthBar, new Rectangle((ScreenManager.Game.Window.ClientBounds.Width / 2) - (healthBar.Width / 2) - 40, 10, (healthBar.Width / 2) + 5, 25), new Rectangle(0, 45, healthBar.Width / 2, 30), Color.Black);

            //Draw the current health level based on the current Health
            spriteBatch.Draw(healthBar, new Rectangle((ScreenManager.Game.Window.ClientBounds.Width / 2) - (healthBar.Width / 2) - 40, 10, (int)((healthBar.Width / 2) * ((double)dude.health / 100)), 20), new Rectangle(0, 45, healthBar.Width / 2, 30), Color.Green);

            //Draw the box around the health bar
            spriteBatch.Draw(healthBar, new Rectangle((ScreenManager.Game.Window.ClientBounds.Width / 2) - (healthBar.Width / 2) - 40, 10, (healthBar.Width / 2) + 5, 25), new Rectangle(0, 0, (healthBar.Width / 2) + 5, 25), Color.White);
            
            spriteBatch.DrawString(outlineFont, "HEALTH", new Vector2(15, 0), Color.Red);
            spriteBatch.DrawString(font, "HEALTH", new Vector2(15, 0), Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }

        #endregion
    }
}
