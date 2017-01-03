﻿using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaCards;

namespace ProgrammingAssignment6
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

        // max valid blockjuck score for a hand
        const int MaxHandValue = 21;

        // dealer max value
        const int MaxDealerValue = 16;

        // deck and hands
        Deck deck;
        List<Card> dealerHand = new List<Card>();
        List<Card> playerHand = new List<Card>();

        // hand placement
        const int TopCardOffset = 100;
        const int HorizontalCardOffset = 150;
        const int VerticalCardSpacing = 125;

        // messages
        SpriteFont messageFont;
        const string ScoreMessagePrefix = "Score: ";
        Message playerScoreMessage;
        Message dealerScoreMessage;
        Message winnerMessage;
		List<Message> messages = new List<Message>();

        // message placement
        const int ScoreMessageTopOffset = 25;
        const int HorizontalMessageOffset = HorizontalCardOffset;
        Vector2 winnerMessageLocation = new Vector2(WindowWidth / 2,
            WindowHeight / 2);

        // menu buttons
        Texture2D quitButtonSprite;
        List<MenuButton> menuButtons = new List<MenuButton>();

        // menu button placement
        const int TopMenuButtonOffset = TopCardOffset;
        const int QuitMenuButtonOffset = WindowHeight - TopCardOffset;
        const int HorizontalMenuButtonOffset = WindowWidth / 2;
        const int VeryicalMenuButtonSpacing = 125;

        // use to detect hand over when player and dealer didn't hit
        bool playerHit = false;
        bool dealerHit = false;

        // game state tracking
        static GameState currentState = GameState.WaitingForPlayer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution and show mouse
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

            // create and shuffle deck
            deck = new Deck(Content, 0, 0);
            deck.Shuffle();

            // first player card
            AddCartToHand(playerHand, HorizontalCardOffset, true);

            // first dealer card
            AddCartToHand(dealerHand, WindowWidth - HorizontalCardOffset, false);

            // second player card
            AddCartToHand(playerHand, HorizontalCardOffset, true);

            // second dealer card
            AddCartToHand(dealerHand, WindowWidth - HorizontalCardOffset, true);

            // load sprite font, create message for player score and add to list
            messageFont = Content.Load<SpriteFont>(@"fonts\Arial24");
            playerScoreMessage = new Message(ScoreMessagePrefix + GetBlockjuckScore(playerHand).ToString(),
                messageFont,
                new Vector2(HorizontalMessageOffset, ScoreMessageTopOffset));
            messages.Add(playerScoreMessage);

            // load quit button sprite for later use
			quitButtonSprite = Content.Load<Texture2D>(@"graphics\quitbutton");

            // create hit button and add to list
            MenuButton hitButton = new MenuButton(Content.Load<Texture2D>(@"graphics\hitbutton"), new Vector2(HorizontalMenuButtonOffset, TopMenuButtonOffset), GameState.PlayerHitting);
            menuButtons.Add(hitButton);

            // create stand button and add to list
            MenuButton standButton = new MenuButton(Content.Load<Texture2D>(@"graphics\standbutton"), new Vector2(HorizontalMenuButtonOffset, TopMenuButtonOffset + VeryicalMenuButtonSpacing), GameState.WaitingForDealer);
            menuButtons.Add(standButton);

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

            // update menu buttons as appropriate
            MouseState mouse = Mouse.GetState();
            if (currentState.Equals(GameState.WaitingForPlayer) || currentState.Equals(GameState.DisplayingHandResults))
            {
                foreach (MenuButton menu in menuButtons)
                {
                    menu.Update(mouse);
                }
            }


            // game state-specific processing
            switch (currentState)
            {
                case GameState.PlayerHitting:
                    AddCartToHand(playerHand, HorizontalCardOffset, true);
                    playerScoreMessage.Text = ScoreMessagePrefix + GetBlockjuckScore(playerHand).ToString();
                    ChangeState(GameState.WaitingForDealer);
                    playerHit = true;
                    break;
                case GameState.WaitingForDealer:
                    if (GetBlockjuckScore(dealerHand) <= MaxDealerValue)
                    {
                        ChangeState(GameState.DealerHitting);
                    }else
                    {
                        ChangeState(GameState.CheckingHandOver);
                    }
                    break;
                case GameState.DealerHitting:
                    AddCartToHand(dealerHand, WindowWidth - HorizontalCardOffset, true);
                    dealerHit = true;
                    ChangeState(GameState.CheckingHandOver);
                    break;
                case GameState.CheckingHandOver:
                    int playerScore = GetBlockjuckScore(playerHand);
                    int dealerScore = GetBlockjuckScore(dealerHand);
                    if((playerScore > MaxHandValue || dealerScore > MaxHandValue))
                    {
                        string winnerMessageStr;
                        if (dealerScore == playerScore)
                        {
                            winnerMessageStr = "Tie!";
                        }
                        else if (dealerScore > MaxHandValue)
                        {
                            winnerMessageStr = "Player won!";
                        }else
                        {
                            winnerMessageStr = "Dealer won!";
                        }
                        manageCheckingHandOver(winnerMessageStr);
                    }
                    else if(!playerHit && !dealerHit)
                    {
                        string winnerMessageStr;
                        if (dealerScore == playerScore)
                        {
                            winnerMessageStr = "Tie!";
                        }
                        else if (playerScore < dealerScore)
                        {
                            winnerMessageStr = "Dealer won!";
                        }
                        else
                        {
                            winnerMessageStr = "Player won!";
                        }
                        manageCheckingHandOver(winnerMessageStr);
                    }
                    else
                    {
                        playerHit = false;
                        dealerHit = false;
                        ChangeState(GameState.WaitingForPlayer);
                    }
                    break;
                case GameState.Exiting:
                    Exit();
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Goldenrod);
						
            spriteBatch.Begin();

            // draw hands
            foreach(Card card in playerHand)
            {
                card.Draw(spriteBatch);
            }

            foreach (Card card in dealerHand)
            {
                card.Draw(spriteBatch);
            }

            // draw messages
            foreach(Message message in messages)
            {
                message.Draw(spriteBatch);
            }

            // draw menu buttons
            foreach(MenuButton button in menuButtons)
            {
                button.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Calculates the Blockjuck score for the given hand
        /// </summary>
        /// <param name="hand">the hand</param>
        /// <returns>the Blockjuck score for the hand</returns>
        private int GetBlockjuckScore(List<Card> hand)
        {
            // add up score excluding Aces
            int numAces = 0;
            int score = 0;
            foreach (Card card in hand)
            {
                if (card.Rank != Rank.Ace)
                {
                    score += GetBlockjuckCardValue(card);
                }
                else
                {
                    numAces++;
                }
            }

            // if more than one ace, only one should ever be counted as 11
            if (numAces > 1)
            {
                // make all but the first ace count as 1
                score += numAces - 1;
                numAces = 1;
            }

            // if there's an Ace, score it the best way possible
            if (numAces > 0)
            {
                if (score + 11 <= MaxHandValue)
                {
                    // counting Ace as 11 doesn't bust
                    score += 11;
                }
                else
                {
                    // count Ace as 1
                    score++;
                }
            }

            return score;
        }

        /// <summary>
        /// Gets the Blockjuck value for the given card
        /// </summary>
        /// <param name="card">the card</param>
        /// <returns>the Blockjuck value for the card</returns>
        private int GetBlockjuckCardValue(Card card)
        {
            switch (card.Rank)
            {
                case Rank.Ace:
                    return 11;
                case Rank.King:
                case Rank.Queen:
                case Rank.Jack:
                case Rank.Ten:
                    return 10;
                case Rank.Nine:
                    return 9;
                case Rank.Eight:
                    return 8;
                case Rank.Seven:
                    return 7;
                case Rank.Six:
                    return 6;
                case Rank.Five:
                    return 5;
                case Rank.Four:
                    return 4;
                case Rank.Three:
                    return 3;
                case Rank.Two:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// It adds a new card for a hand
        /// </summary>
        /// <param name="hands">The list of cards</param>
        /// <param name="x">The x position</param>
        /// <param name="flipOver">Flag to flip over or not</param>
        protected void AddCartToHand(List<Card> hands, int x, bool flipOver)
        {
            Card card = deck.TakeTopCard();
            card.X = x;
            card.Y = (hands.Count * VerticalCardSpacing) + TopCardOffset;
            if (flipOver)
            {
                card.FlipOver();
            }
            hands.Add(card);
        }

        /// <summary>
        /// It manages the checking hand over state
        /// </summary>
        /// <param name="winnerMessageStr">The winner message</param>
        protected void manageCheckingHandOver(string winnerMessageStr)
        {
            // messages
            winnerMessage = new Message(winnerMessageStr, messageFont, winnerMessageLocation);
            messages.Add(winnerMessage);
            dealerScoreMessage = new Message(ScoreMessagePrefix + GetBlockjuckScore(dealerHand).ToString(), messageFont, new Vector2(WindowWidth - HorizontalMessageOffset, ScoreMessageTopOffset));
            messages.Add(dealerScoreMessage);

            // show dealer card
            dealerHand[0].FlipOver();

            // remove buttons
            menuButtons.Clear();

            // add quit button
            MenuButton quitButton = new MenuButton(Content.Load<Texture2D>(@"graphics\quitbutton"), new Vector2(HorizontalMenuButtonOffset, QuitMenuButtonOffset), GameState.Exiting);
            menuButtons.Add(quitButton);

            ChangeState(GameState.DisplayingHandResults);
        }

        /// <summary>
        /// Changes the state of the game
        /// </summary>
        /// <param name="newState">the new game state</param>
        public static void ChangeState(GameState newState)
        {
            currentState = newState;
        }
    }
}
