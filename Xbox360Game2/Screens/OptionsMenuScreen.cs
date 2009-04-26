#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry bulletMenuEntry;
        MenuEntry difficultyMenuEntry;   

        static string[] bullets = { "Fast", "Medium", "Slow" };
        public static int currentBullets = 0;

        static string[] enemies = { "Slow", "Medium", "Fast" };
        public static int currentDifficulty = 0;
		public static int numEnemies = 0; 

        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            bulletMenuEntry = new MenuEntry(string.Empty);
            difficultyMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry backMenuEntry = new MenuEntry("Back");

            // Hook up menu event handlers.
            bulletMenuEntry.Selected += BulletMenuEntrySelected;
            difficultyMenuEntry.Selected += DifficultyMenuEntrySelected;
            backMenuEntry.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(bulletMenuEntry);
            MenuEntries.Add(difficultyMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }

        public static int getBullets()
        {
            return currentBullets;
        }

		public static int getEnemies()
		{
			return numEnemies;
		}

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            bulletMenuEntry.Text = "Bullet Speed: " + bullets[currentBullets];
            difficultyMenuEntry.Text = "Enemy Count: " + enemies[currentDifficulty];
        }
        #endregion

        #region Handle Input
        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void BulletMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentBullets++;

            if (currentBullets > 2)
                currentBullets = 0;

            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void DifficultyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentDifficulty++;
            
            if(currentDifficulty > 2)
                currentDifficulty = 0;

            SetMenuEntryText();
        }

        #endregion
    }
}
