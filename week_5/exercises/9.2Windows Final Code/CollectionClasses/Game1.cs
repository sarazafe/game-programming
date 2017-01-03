using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CollectionClasses
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WindowWidth = 800;
        const int WindowHeight = 600;

        // drawing support
        Texture2D currentCharacter;
        Rectangle drawRectangle;

        // random character support
        Random rand = new Random();
        List<Texture2D> characters = new List<Texture2D>();

        // click support
        ButtonState previousButtonState = ButtonState.Released;

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

            // load character sprites
            characters.Add(Content.Load<Texture2D>(@"graphics\character0"));
            characters.Add(Content.Load<Texture2D>(@"graphics\character1"));
            characters.Add(Content.Load<Texture2D>(@"graphics\character2"));
            characters.Add(Content.Load<Texture2D>(@"graphics\character3"));

            // start character 0 in center of window
            currentCharacter = characters[0];
            drawRectangle = new Rectangle(WindowWidth / 2 - currentCharacter.Width / 2,
                WindowHeight / 2 - currentCharacter.Height / 2,
                currentCharacter.Width, currentCharacter.Height);
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

            // make character follow mouse
            MouseState mouse = Mouse.GetState();
            drawRectangle.X = mouse.X - drawRectangle.Width / 2;
            drawRectangle.Y = mouse.Y - drawRectangle.Height / 2;

            // clamp character in window
            if (drawRectangle.Left < 0)
            {
                drawRectangle.X = 0;
            }
            if (drawRectangle.Right > WindowWidth)
            {
                drawRectangle.X = WindowWidth - drawRectangle.Width;
            }
            if (drawRectangle.Top < 0)
            {
                drawRectangle.Y = 0;
            }
            if (drawRectangle.Bottom > WindowHeight)
            {
                drawRectangle.Y = WindowHeight - drawRectangle.Height;
            }

            // change character on left mouse click (NOT LEFT MOUSE PRESS!)
            if (mouse.LeftButton == ButtonState.Released &&
                previousButtonState == ButtonState.Pressed)
            {
                // change to random character
                currentCharacter = characters[rand.Next(4)];
                drawRectangle.Width = currentCharacter.Width;
                drawRectangle.Height = currentCharacter.Height;
            }
            previousButtonState = mouse.LeftButton;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw character
            spriteBatch.Begin();
            spriteBatch.Draw(currentCharacter, drawRectangle, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
