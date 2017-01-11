using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FishAndBears
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WindowWidth = 584;
        const int WindowHeight = 438;

        // menu objects
        Menu mainMenu;

        // game objects
        Fish fish;
        List<TeddyBear> bears = new List<TeddyBear>();

        // field to keep track of game state
        static GameState state;

        // background music
        SoundEffectInstance backgroundMusic;

        // collision sound effects
        SoundEffect destroy;
        SoundEffect[] bounce = new SoundEffect[2];
        Random rand = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution and make mouse visible
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load and start playing background music
            SoundEffect backgroundMusicEffect = Content.Load<SoundEffect>(@"audio\backgroundMusic");
            backgroundMusic = backgroundMusicEffect.CreateInstance();
            backgroundMusic.IsLooped = true;
            backgroundMusic.Play();

            // load other sound effects
            destroy = Content.Load<SoundEffect>(@"audio\zombie");
            bounce[0] = Content.Load<SoundEffect>(@"audio\confetti");
            bounce[1] = Content.Load<SoundEffect>(@"audio\pirate");

            // initialize menu objects
            mainMenu = new Menu(Content, WindowWidth, WindowHeight);

            // initialize fish
            fish = new Fish(Content, @"graphics\fish", WindowWidth / 6, WindowWidth / 6,
                WindowWidth, WindowHeight);

            // initialize bear list
            string baseSpriteName = @"graphics\teddybear";
            Random rand = new Random();
            for (int x = 150; x <= 600; x += 150)
            {
                for (int y = 150; y <= 450; y += 150)
                {
                    bears.Add(new TeddyBear(Content, baseSpriteName + rand.Next(3),
                        x, y, WindowWidth, WindowHeight));
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // process input based on game state
            if (state == GameState.MainMenu)
            {
                // update main menu
                mainMenu.Update(Mouse.GetState());
            }
            else if (state == GameState.Play)
            {
                // update fish
                fish.Update(Keyboard.GetState());

                // update teddy bears
                foreach (TeddyBear bear in bears)
                {
                    bear.Update();
                }

                // check collisions between fish and teddy bears
                CheckAndResolveFishBearCollisions();

                // check and resolve collisions between teddy bears
                CheckAndResolveBearCollisions();

                // check for all teddy bears dead
                if (bears.Count == 0)
                {
                    ChangeState(GameState.Quit);
                }
            }
            else
            {
                Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw based on game state
            spriteBatch.Begin();
            if (state == GameState.MainMenu)
            {
                // draw the main menu
                mainMenu.Draw(spriteBatch);
            }
            else if (state == GameState.Play)
            {
                // draw the game objects
                fish.Draw(spriteBatch);
                foreach (TeddyBear bear in bears)
                {
                    bear.Draw(spriteBatch);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Changes the state of the game
        /// </summary>
        /// <param name="newState">the new game state</param>
        public static void ChangeState(GameState newState)
        {
            state = newState;
        }

        /// <summary>
        /// Checks and resolves collisions between teddy bears in the game
        /// </summary>
        private void CheckAndResolveBearCollisions()
        {
            for (int i = 0; i < bears.Count; i++)
            {
                for (int j = i + 1; j < bears.Count; j++)
                {
                    if (bears[i].Active && bears[j].Active &&
                        bears[i].CollisionRectangle.Intersects(
                            bears[j].CollisionRectangle))
                    {
                        bears[i].Bounce();
                        bears[j].Bounce();
                    }
                }
            }
        }

        /// <summary>
        /// Checks and resolves collisions between the fish and the teddy bears in the game
        /// </summary>
        private void CheckAndResolveFishBearCollisions()
        {
            // check collisions between fish and teddy bears
            for (int i = bears.Count - 1; i >= 0; i--)
            {
                if (fish.Active && bears[i].Active &&
                    fish.CollisionRectangle.Intersects(bears[i].CollisionRectangle))
                {
                    // check where the collision occurred
                    Rectangle overlap = Rectangle.Intersect(fish.CollisionRectangle,
                        bears[i].CollisionRectangle);
                    if (overlap.Left <= fish.Front && overlap.Right >= fish.Front)
                    {
                        // fish ate the bear
                        bears.RemoveAt(i);

                        // play eating sound
                        destroy.Play();
                    }
                    else
                    {
                        // non-head collision
                        bears[i].Bounce();

                        // play bouncing sound
                        bounce[rand.Next(2)].Play();
                    }
                }
            }
        }

        /// <summary>
        /// Removes the dead teddy bears from the list
        /// </summary>
        private void RemoveDeadTeddyBears()
        {
            for (int i = bears.Count - 1; i >= 0; i--)
            {
                if (!bears[i].Active)
                {
                    bears.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Checks if all the bears are dead
        /// </summary>
        private void CheckBearsAllDead()
        {
            bool allBearsDead = true;
            foreach (TeddyBear bear in bears)
            {
                if (bear.Active)
                {
                    allBearsDead = false;
                    break;
                }
            }
            if (allBearsDead)
            {
                ChangeState(GameState.Quit);
            }
        }
    }
}
