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
    class Boss
    {
        Texture2D[] standSheet, walkSheet, attackSheet, hitSheet, deathSheet;
        int currentSprite = 0;
        Texture2D[] currentSheet;
        Rectangle rect;
        Vector2 position;
        bool alive;
        int health;
        public int timePassed = 0;
        Vector2 velocity;
        public int AITimer = 0;
        enum state {STAND, WALK, ATTACK, HIT, DEATH};
        state previousState, currentState, nextState;

        public Boss()
        {
            currentSheet = standSheet;
            currentState = state.STAND;
            currentState = state.STAND;
            previousState = state.STAND;
            alive = true;
            position = new Vector2(GameStateManagement.GameplayScreen.viewportRect.Width + currentSheet[currentSprite].Width, GameStateManagement.GameplayScreen.viewportRect.Height - 150.0f);
            velocity = Vector2.Zero;
            health = 10;
        }

        public static void LoadContent(ContentManager Content)
        {

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
                    velocity.X = -50.0f;
                    if (this.position.X - GameStateManagement.GameplayScreen.dude.position.X < 20.0f)
                    {
                        nextState = state.ATTACK;
                    }
                    break;
                case state.ATTACK:
                    currentSheet = attackSheet;
                    velocity.X = 0;
                    break;
                case state.HIT:
                    currentSheet = hitSheet;
                    nextState = previousState;
                    if (health = 0)
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
            position += velocity;
            timePassed++;
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (this.alive)
            {
                spriteBatch.Draw(currentSheet[currentSprite], position, Color.White);
            }
        }
    }
}