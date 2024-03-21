﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using TotallyFair.GameComponents;
using TotallyFair.Graphics;

namespace TotallyFair
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        /**************************************/
        /* Initialize ALL TEXTURES AND LABELS */
        /**************************************/
        private Texture2D HomeLogoSprite;
        private Vector2 Vector_HomeLogoSprite = new Vector2(0, 0);

        private Texture2D ResetButton;
        private Vector2 Vector_ResetButton = new Vector2(0, 0);
        private SpriteFont GameFont; 

        /*******************************/
        /* Initialize ALL GAME OBJECTS */
        /*******************************/
        private CardDeck Deck = new CardDeck();
        private Card[] Dealer = new Card[5];
        private Vector2 Vector_Dealer = new Vector2(0, 0);
        private static Player[] Players = new Player[6];
        private Vector2[] Vector_PlayerHand = new Vector2[6];

        /*****************/
        /* MISCELLANEOUS */
        /*****************/
        private static bool DoneInitializing = false;
        private static Input InputListener = new Input(); //TODO: Update size as needed
        private Timer OneMinuteInterval = new Timer(new TimerCallback(TimerTick), new TimerState(), 1000, 60000);
        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //Initialize Game Window Settings
            Window.AllowUserResizing = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            //Initialize Players
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i] = new Player();
            }
            //Deal Cards
            DealCards();
            //Initialize Vector/Color Array Items
            Players[0].Sprite.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight - 50);
            Players[1].Sprite.Position = new Vector2(40, _graphics.PreferredBackBufferHeight * 3 / 4);
            Players[2].Sprite.Position = new Vector2(40, _graphics.PreferredBackBufferHeight / 4);
            Players[3].Sprite.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, 20);
            Players[4].Sprite.Position = new Vector2(_graphics.PreferredBackBufferWidth - 80, _graphics.PreferredBackBufferHeight / 4);
            Players[5].Sprite.Position = new Vector2(_graphics.PreferredBackBufferWidth - 80, _graphics.PreferredBackBufferHeight * 3 / 4);
            Vector_PlayerHand[0] = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight - 80);
            Vector_PlayerHand[1] = new Vector2(40, _graphics.PreferredBackBufferHeight * 3 / 4);
            Vector_PlayerHand[2] = new Vector2(40, _graphics.PreferredBackBufferHeight / 4);
            Vector_PlayerHand[3]  = new Vector2(_graphics.PreferredBackBufferWidth / 2, 20);
            Vector_PlayerHand[4] = new Vector2(_graphics.PreferredBackBufferWidth - 80, _graphics.PreferredBackBufferHeight / 4);
            Vector_PlayerHand[5] = new Vector2(_graphics.PreferredBackBufferWidth - 80, _graphics.PreferredBackBufferHeight * 3 / 4);

            Vector_Dealer.X = _graphics.PreferredBackBufferWidth / 2;
            Vector_Dealer.Y = _graphics.PreferredBackBufferHeight / 2;

            //Add All Game Actions to OnKeyPress Events
            InputListener.AddKeyPressHandler(false, GameAction_Exit, Keys.Escape);
            InputListener.AddKeyPressHandler(false, GameAction_Reshuffle, Keys.R);
            InputListener.AddKeyPressHandler(false, GameAction_FullScreen, Keys.F);
            InputListener.AddKeyPressHandler(true, GameAction_MoveRight, Keys.D);
            InputListener.AddKeyPressHandler(true, GameAction_MoveLeft, Keys.A);
            InputListener.AddKeyPressHandler(true, GameAction_MoveUp, Keys.W);
            InputListener.AddKeyPressHandler(true, GameAction_MoveDown, Keys.S);
            InputListener.AddMouseHandler(false, GameAction_Attack, Input.MouseClickState.LEFT);

            base.Initialize();

            DoneInitializing = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D[] PlayerRunningTextures = new Texture2D[3];
            double[] PlayerRunningTimes = new double[3];

            Texture2D[] PlayerIdleTextures = new Texture2D[1];
            double[] PlayerIdleTimes = new double[1];

            Texture2D[] PlayerStrikeBoxIdleTextures = new Texture2D[1];
            double[] PlayerStrikeBoxIdleTimes = new double[1];
            
            PlayerIdleTextures[0] = Content.Load<Texture2D>("Button_Red");
            PlayerIdleTimes[0] = 100;

            PlayerRunningTextures[0] = PlayerIdleTextures[0];
            PlayerRunningTextures[1] = Content.Load<Texture2D>("Button_Orange");
            PlayerRunningTextures[2] = Content.Load<Texture2D>("Button_Yellow");
            PlayerRunningTimes[0] = 500;
            PlayerRunningTimes[1] = 500;
            PlayerRunningTimes[2] = 500;

            PlayerStrikeBoxIdleTextures[0] = Content.Load<Texture2D>("StrikeBox");
            PlayerStrikeBoxIdleTimes[0] = 1000;

            Players[0].Sprite.AddAnimation(AnimationState.IDLE, PlayerIdleTextures, PlayerIdleTimes, true);
            Players[0].Sprite.AddAnimation(AnimationState.RUNNINGLEFT, PlayerRunningTextures, PlayerRunningTimes, true);
            Players[0].StrikeBox.AddAnimation(AnimationState.IDLE, PlayerStrikeBoxIdleTextures, PlayerStrikeBoxIdleTimes, false);

            Players[1].Sprite.AddAnimation(AnimationState.IDLE, PlayerIdleTextures, PlayerIdleTimes, true);
            Players[1].Sprite.AddAnimation(AnimationState.RUNNINGLEFT, PlayerRunningTextures, PlayerRunningTimes, true);
            Players[1].StrikeBox.AddAnimation(AnimationState.IDLE, PlayerStrikeBoxIdleTextures, PlayerStrikeBoxIdleTimes, false);
            
            Players[2].Sprite.AddAnimation(AnimationState.IDLE, PlayerIdleTextures, PlayerIdleTimes, true);
            Players[2].Sprite.AddAnimation(AnimationState.RUNNINGLEFT, PlayerRunningTextures, PlayerRunningTimes, true);
            Players[2].StrikeBox.AddAnimation(AnimationState.IDLE, PlayerStrikeBoxIdleTextures, PlayerStrikeBoxIdleTimes, false);
            
            Players[3].Sprite.AddAnimation(AnimationState.IDLE, PlayerIdleTextures, PlayerIdleTimes, true);
            Players[3].Sprite.AddAnimation(AnimationState.RUNNINGLEFT, PlayerRunningTextures, PlayerRunningTimes, true);
            Players[3].StrikeBox.AddAnimation(AnimationState.IDLE, PlayerStrikeBoxIdleTextures, PlayerStrikeBoxIdleTimes, false);
            
            Players[4].Sprite.AddAnimation(AnimationState.IDLE, PlayerIdleTextures, PlayerIdleTimes, true);
            Players[4].Sprite.AddAnimation(AnimationState.RUNNINGLEFT, PlayerRunningTextures, PlayerRunningTimes, true);
            Players[4].StrikeBox.AddAnimation(AnimationState.IDLE, PlayerStrikeBoxIdleTextures, PlayerStrikeBoxIdleTimes, false);
            
            Players[5].Sprite.AddAnimation(AnimationState.IDLE, PlayerIdleTextures, PlayerIdleTimes, true);
            Players[5].Sprite.AddAnimation(AnimationState.RUNNINGLEFT, PlayerRunningTextures, PlayerRunningTimes, true);
            Players[5].StrikeBox.AddAnimation(AnimationState.IDLE, PlayerStrikeBoxIdleTextures, PlayerStrikeBoxIdleTimes, false);
            GameFont = Content.Load<SpriteFont>("Arial");

        }

        protected override void Update(GameTime gameTime)
        {
            //Listen for known events
            InputListener.Update();
            UpdatePositions((float)0.001);

            Window.ClientSizeChanged += OnResize;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!DoneInitializing) return;
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            for (int i = 0; i < Players.Length; i++)
            {
                _spriteBatch.DrawString(GameFont, $"{Players[i].Hand[0].FaceValue}", Vector_PlayerHand[i], Color.White);
                _spriteBatch.DrawString(GameFont, $"{Players[i].Hand[1].FaceValue}", new Vector2(Vector_PlayerHand[i].X, Vector_PlayerHand[i].Y + 25), Color.White);
                Players[i].Sprite.Play(_spriteBatch);
                if (Players[i].StrikeBox.Visible) Players[i].StrikeBox.Play(_spriteBatch);
            }

            //_spriteBatch.Draw(Players[0].Sprite.SpriteTexture, Players[0].Sprite.Position, null, Color.White, Players[0].Sprite.Rotation, new Vector2(Players[0].Sprite.SpriteTexture.Width, Players[0].Sprite.SpriteTexture.Height), 1, SpriteEffects.None, 0);
            //_spriteBatch.Draw(Players[0].StrikeBox.SpriteTexture, Players[0].StrikeBox.Position, null, Color.White, Players[0].StrikeBox.Rotation, new Vector2(Players[0].StrikeBox.SpriteTexture.Width, Players[0].StrikeBox.SpriteTexture.Height), 1, SpriteEffects.None, 0);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private static void TimerTick(object timerState)
        {
            if (Players == null || !DoneInitializing) return;
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: starting a new callback.");
            var state = timerState as TimerState;
            Interlocked.Increment(ref state.Counter);

            for (int i = 0; i < Players.Length; i++) Players[i].ForgetOpponentHand();
        }

        class TimerState
        {
            public int Counter;
        }

        private void OnResize(Object  sender, EventArgs e)
        {
            //RepositionGameSprites();
        }

        private void UpdatePositions(float DeltaTime)
        {
            foreach (Player P in Players)
            {
                P.Sprite.Position.X += P.Velocity.X * DeltaTime;
                P.Sprite.Position.Y += P.Velocity.Y * DeltaTime;

                //Clamp to bounds of window
                if (P.Sprite.Position.X + P.Sprite.AnimationLibrary[P.Sprite.CurrentState].Animation.Peek().Width > _graphics.PreferredBackBufferWidth) P.Sprite.Position.X = _graphics.PreferredBackBufferWidth - P.Sprite.AnimationLibrary[P.Sprite.CurrentState].Animation.Peek().Width;
                if (P.Sprite.Position.X < 0) P.Sprite.Position.X = 0;
                if (P.Sprite.Position.Y + P.Sprite.AnimationLibrary[P.Sprite.CurrentState].Animation.Peek().Height > _graphics.PreferredBackBufferHeight) P.Sprite.Position.Y = _graphics.PreferredBackBufferHeight - P.Sprite.AnimationLibrary[P.Sprite.CurrentState].Animation.Peek().Height;
                if (P.Sprite.Position.Y < 0) P.Sprite.Position.Y = 0;

                P.Update();
            }
        }

        private void DealCards()
        {
            for (int i = 0; i < 2; i++) foreach (Player p in Players) p.AddCardToHand(Deck.DealCard(), i);
        }

        /////////////////////////////////////////
        //     List All Game Actions Below     //
        /////////////////////////////////////////
        private void GameAction_Exit()
        {
                Exit();
        }

        private void GameAction_Reshuffle()
        {
            //Reshuffle deck
            Deck.ClearDeck();
            Deck.GenerateDeck();
            foreach (Player p in Players) p.ClearHand();
            DealCards();
        }

        private void GameAction_FullScreen()
        {
            //Use Switch to break after action is executed
            switch (_graphics.IsFullScreen)
            {
                case false: _graphics.IsFullScreen = true; break;
                case true: _graphics.IsFullScreen = false; break;
            }
            _graphics.ApplyChanges();
        }

        private void GameAction_MoveRight()
        {
            Players[0].UpdateVelocity(new Vector2(500,0));
        }

        private void GameAction_MoveLeft()
        {
            Players[0].UpdateVelocity(new Vector2(-500, 0));
        }

        private void GameAction_MoveUp()
        {
            Players[0].UpdateVelocity(new Vector2(0, -500));
        }

        private void GameAction_MoveDown()
        {
            Players[0].UpdateVelocity(new Vector2(0, 500));
        }

        private void GameAction_Attack()
        {
            Players[0].StrikeBox.Rotation = (float)Math.Atan(Mouse.GetState().X - (Players[0].Sprite.Position.X + Players[0].Sprite.AnimationLibrary[Players[0].Sprite.CurrentState].Animation.Peek().Width / 2) / (Mouse.GetState().Y - (Players[0].Sprite.Position.Y - Players[0].Sprite.AnimationLibrary[Players[0].Sprite.CurrentState].Animation.Peek().Height / 2)));;
            Players[0].StrikeBox.Visible = true;      
        }
    }
}
