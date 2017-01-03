using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TeddyMineExplosion;

namespace ProgrammingAssignment5
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // constants
        const int WindowWidth = 800;
        const int WindowHeight = 600;

        const int SpawnIntervalMin = 1000;
        const int SpawnIntervalMax = 3001;

        const float VelocityMin = -0.5f;
        const float VelocityMax = 0.5f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // graphic and drawing info 
        Texture2D mineSprite;
        Texture2D teddySprite;
        Texture2D explosionSprite;

        // list of mines
        List<Mine> mines;

        // list of bears
        List<TeddyBear> teddys;

        // list of explosions
        List<Explosion> explosions;

        // the mouse
        MouseState mouse;

        // just one mine for each click
        bool canAddMine;

        // spawn timer
        int spawnTimer;

        // random
        Random rand;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            IsMouseVisible = true;

            // initialize lists
            mines = new List<Mine>();
            teddys = new List<TeddyBear>();
            explosions = new List<Explosion>();

            // random
            rand = new Random();
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

            // TODO: use this.Content to load your game content here

            // Load mine sprite
            mineSprite = Content.Load<Texture2D>(@"images\mine");

            // Load teddy sprite
            teddySprite = Content.Load<Texture2D>(@"images\teddybear");

            // Load explosion sprite
            explosionSprite = Content.Load<Texture2D>(@"images\explosion");

            // timer for spawing teddys in miliseconds
            spawnTimer = rand.Next(SpawnIntervalMin, SpawnIntervalMax);
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

            // TODO: Add your update logic here

            // if left mouse is pressed, add a mine to the list of mines (just one by click)
            mouse = Mouse.GetState();
            if(canAddMine && mouse.LeftButton == ButtonState.Pressed){
                mines.Add(new Mine(mineSprite, mouse.X, mouse.Y));
                canAddMine = false;
            }
            // when mouse is released reset flag
            if(mouse.LeftButton == ButtonState.Released)
            {
                canAddMine = true;
            }

            // if timer is <= 0, reset timer and add a new teddy to the list
            if (spawnTimer <= 0)
            {
                // create teddy
                float x = (float)(rand.NextDouble() * (VelocityMax - VelocityMin) + VelocityMin);
                float y = (float)(rand.NextDouble() * (VelocityMax - VelocityMin) + VelocityMin); 
                teddys.Add(new TeddyBear(teddySprite, new Vector2(x, y), WindowWidth, WindowHeight));
                
                // initialize timer
                spawnTimer = rand.Next(SpawnIntervalMin, SpawnIntervalMax);
            }else
            {// update timer with gametime
                spawnTimer -= gameTime.ElapsedGameTime.Milliseconds;
            }

            // update teddys and check for collisions
            foreach (TeddyBear teddy in teddys)
            {
                teddy.Update(gameTime);

                foreach (Mine mine in mines)
                {
                    if (teddy.CollisionRectangle.Intersects(mine.CollisionRectangle))
                    {
                        // inactive teddy and mine
                        teddy.Active = false;
                        mine.Active = false;

                        // add new explosion to list of explosions
                        explosions.Add(new Explosion(explosionSprite, mine.CollisionRectangle.Center.X, mine.CollisionRectangle.Center.Y));
                    }
                }
            }

            // update explosions
            foreach(Explosion explosion in explosions)
            {
                explosion.Update(gameTime);
            }

            // remove inactive mines
            for(int i = mines.Count - 1; i >= 0; i--)
            {
                if (!mines[i].Active)
                {
                    mines.Remove(mines[i]);
                }
            }

            // remove inactive teddys
            for (int i = teddys.Count - 1; i >= 0; i--)
            {
                if (!teddys[i].Active)
                {
                    teddys.Remove(teddys[i]);
                }
            }

            // reemove inactive explosions
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if (!explosions[i].Playing)
                {
                    explosions.Remove(explosions[i]);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);

            spriteBatch.Begin();

            // TODO: Add your drawing code here

            // draw mines in list
            foreach (Mine mine in mines)
            {
                mine.Draw(spriteBatch);
            }

            // draw teddys
            foreach(TeddyBear teddy in teddys)
            {
                teddy.Draw(spriteBatch);
            }

            // draw explosions
            foreach(Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
