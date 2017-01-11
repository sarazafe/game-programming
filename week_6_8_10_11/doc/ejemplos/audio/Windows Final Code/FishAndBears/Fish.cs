using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FishAndBears
{
    /// <summary>
    /// A fish
    /// </summary>
    class Fish
    {
        #region Fields

        // window dimensions
        int windowWidth;
        int windowHeight;

        // graphic and drawing info
        Texture2D sprite;
        const int ImagesPerRow = 2;
        int frameWidth;
        Rectangle drawRectangle = new Rectangle();
        Rectangle sourceRectangle;

        // fish movement
        const int FishMoveAmount = 5;

        // the side the front of the fish is on
        Side front;

        // whether or not the fish is active
        bool active = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a fish with the given characteristics
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        /// <param name="spriteName">the name of the sprite for the fish</param>
        /// <param name="x">the x location of the center of the fish</param>
        /// <param name="y">the y location of the center of the fish</param>
        /// <param name="windowWidth">the window width</param>
        /// <param name="windowHeight">the window height</param>
        public Fish(ContentManager contentManager, string spriteName,
            int x, int y, int windowWidth, int windowHeight)
        {
            // set window dimensions
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            // set temporary draw rectangle location and load content
            drawRectangle.X = x;
            drawRectangle.Y = y;
            LoadContent(contentManager, spriteName);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the collision rectangle for the fish
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
        }

        /// <summary>
        /// Gets the x coordinate for the front of the fish
        /// </summary>
        public int Front
        {
            get
            {
                if (front == Side.Left)
                {
                    return drawRectangle.X;
                }
                else
                {
                    return drawRectangle.X + drawRectangle.Width;
                }
            }
        }

        /// <summary>
        /// Gets and sets whether or not the fish is active
        /// </summary>
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        #endregion

        #region Private properties

        /// <summary>
        /// Gets and sets the x location of the center of the fish
        /// </summary>
        private int X
        {
            get { return drawRectangle.X + drawRectangle.Width / 2; }
            set
            {
                drawRectangle.X = value - drawRectangle.Width / 2;

                // clamp to keep in range
                if (drawRectangle.Left < 0)
                {
                    drawRectangle.X = 0;
                }
                else if (drawRectangle.Right > windowWidth)
                {
                    drawRectangle.X = windowWidth - drawRectangle.Width;
                }
            }
        }

        /// <summary>
        /// Gets and sets the y location of the center of the fish
        /// </summary>
        private int Y
        {
            get { return drawRectangle.Y + drawRectangle.Height / 2; }
            set
            {
                drawRectangle.Y = value - drawRectangle.Height / 2;

                // clamp to keep in range
                if (drawRectangle.Top < 0)
                {
                    drawRectangle.Y = 0;
                }
                else if (drawRectangle.Bottom > windowHeight)
                {
                    drawRectangle.Y = windowHeight - drawRectangle.Height;
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the fish's location, stopping at edges if necessary
        /// </summary>
        /// <param name="keyboard">current keyboard state</param>
        public void Update(KeyboardState keyboard)
        {
            // move the fish based on the keyboard state
            if (keyboard.IsKeyDown(Keys.Right))
            {
                X += FishMoveAmount;

                // set source rectangle for right image
                sourceRectangle.X = 0;
                front = Side.Right;
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                X -= FishMoveAmount;

                // set source rectangle for left image
                sourceRectangle.X = frameWidth;
                front = Side.Left;
            }
            if (keyboard.IsKeyDown(Keys.Up))
            {
                Y -= FishMoveAmount;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                Y += FishMoveAmount;
            }
        }

        /// <summary>
        /// Draws the fish
        /// </summary>
        /// <param name="spriteBatch">the sprite batch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                spriteBatch.Draw(sprite, drawRectangle, sourceRectangle, Color.White);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the fish
        /// </summary>
        /// <param name="contentManager">the content manager to use</param>
        /// <param name="spriteName">the name of the sprite for the fish</param>
        private void LoadContent(ContentManager contentManager, string spriteName)
        {
            // load content, calculate image width, and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            frameWidth = sprite.Width / ImagesPerRow;
            drawRectangle.Width = frameWidth;
            drawRectangle.Height = sprite.Height;

            // center the fish (draw rectangle currently has upper left corner where center should be)
            X = drawRectangle.X;
            Y = drawRectangle.Y;

            // set initial source rectangle
            sourceRectangle = new Rectangle(0, 0, frameWidth, sprite.Height);
        }

        #endregion
    }
}
