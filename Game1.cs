﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using TotallyFair.CONFIG;
using TotallyFair.GameComponents;
using TotallyFair.Graphics;
using TotallyFair.Managers;
using TotallyFair.Utilities;

namespace TotallyFair
{
    /// <summary>
    /// Naming Convention:
    /// Public Variables = PascalCase "VariableName"
    /// Private Variables = Leading Underscore "_variableName"
    /// Function Scoped Variables = camelCase "variableName"
    /// Function/Method = PascalCase "FunctionName"
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        /**************************************/
        /* Initialize ALL TEXTURES AND LABELS */
        /**************************************/
        private SpriteFont GameFont; 

        /*******************************/
        /* Initialize ALL GAME OBJECTS */
        /*******************************/
        private CardDeck Deck = new CardDeck();
        private Card[] Dealer = new Card[5];
        private Vector2 Vector_Dealer = new Vector2(0, 0);
        private static Player[] Players = new Player[6];
        private Vector2[] Vector_PlayerHand = new Vector2[6];
        private SceneManager _sceneManager;
        private Map _map;
        private Texture2D[] _bullets = new Texture2D[1]; 

        /************/
        /* SETTINGS */
        /************/
        private GAME_CONFIG _settings;
        private bool _paused;

        /*****************/
        /* MISCELLANEOUS */
        /*****************/
        private static bool DoneInitializing = false;
        private static InputManager InputListener = new InputManager(); //TODO: Update size as needed
        //private Timer OneMinuteInterval = new Timer(new TimerCallback(TimerTick), new TimerState(), 1000, 60000);
        

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
            _graphics.ApplyChanges();

            _map = new(Content.Load<Texture2D>("Map"));
            GraphicsDevice.Viewport = new(0,0, _map.Bounds.Width, _map.Bounds.Height);
            _sceneManager = new(Content.Load<Texture2D>("Map"), GraphicsDevice.Viewport);

            _settings = new GAME_CONFIG(_sceneManager.SceneMap);

            //Initialize Vector/Color Array Items
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
            InputListener.AddKeyPressHandler(false, GameAction_PauseGame, Keys.P);
            InputListener.AddMouseHandler(false, GameAction_Attack, InputManager.MouseClickState.LEFT);

            base.Initialize();

            DoneInitializing = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _bullets[0] = Content.Load<Texture2D>("Circle");

            // TODO: use this.Content to load your game content here
            Texture2D[] PlayerRunningTextures = new Texture2D[6];
            Texture2D[] PlayerRunningLeftTextures = new Texture2D[6];
            float PlayerRunningTime = 50f;

            Texture2D[] PlayerIdleTextures = new Texture2D[1];
            float PlayerIdleTime = 50f;
            
            PlayerIdleTextures[0] = Content.Load<Texture2D>("Bonk_Idle");

            PlayerRunningTextures[0] = Content.Load<Texture2D>("Bonk-Run1");
            PlayerRunningTextures[1] = Content.Load<Texture2D>("Bonk-Run2");
            PlayerRunningTextures[2] = Content.Load<Texture2D>("Bonk-Run3");
            PlayerRunningTextures[3] = Content.Load<Texture2D>("Bonk-Run4");
            PlayerRunningTextures[4] = Content.Load<Texture2D>("Bonk-Run5");
            PlayerRunningTextures[5] = Content.Load<Texture2D>("Bonk-Run6");
            PlayerRunningLeftTextures[0] = Content.Load<Texture2D>("Bonk-RunLeft1");
            PlayerRunningLeftTextures[1] = Content.Load<Texture2D>("Bonk-RunLeft2");
            PlayerRunningLeftTextures[2] = Content.Load<Texture2D>("Bonk-RunLeft3");
            PlayerRunningLeftTextures[3] = Content.Load<Texture2D>("Bonk-RunLeft4");
            PlayerRunningLeftTextures[4] = Content.Load<Texture2D>("Bonk-RunLeft5");
            PlayerRunningLeftTextures[5] = Content.Load<Texture2D>("Bonk-RunLeft6");

            //Initialize Players
            for (int i = 0; i < 6; i++)
            {
                _sceneManager.AddPlayer(i, _settings.STARTING_POSITIONS[6][i], AnimationState.IDLE, PlayerIdleTextures, PlayerIdleTime, true);
                _sceneManager.Players[i].Sprite.AddAnimation(AnimationState.RUNNINGLEFT, PlayerRunningLeftTextures, PlayerRunningTime, true);
                _sceneManager.Players[i].Sprite.AddAnimation(AnimationState.RUNNINGRIGHT, PlayerRunningTextures, PlayerRunningTime, true);

                //By Default, first player is human
                if (i == 0) _sceneManager.Players[i].IsCPU = false;
            }

            GameFont = Content.Load<SpriteFont>("Arial");


            //Deal Cards (Players must be initialized first)
            DealCards();
        }

        protected override void Update(GameTime gameTime)
        {
            //Listen for known events
            InputListener.Update();

            if (!_paused) _sceneManager.Update(GraphicsDevice.Viewport);

            Window.ClientSizeChanged += OnResize;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!DoneInitializing) return;
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            if (!_paused)
            {
                _sceneManager.Draw(_spriteBatch, _graphics);
                _spriteBatch.DrawString(GameFont, $"{_sceneManager.Players[0].Force}", _sceneManager.Players[0].Position, Color.Black);
            }
            else _spriteBatch.DrawString(GameFont, "PAUSED", new(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), Color.White);
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void OnResize(Object sender, EventArgs e)
        {
            //RepositionGameSprites();
        }

        private void DealCards()
        {
            //for (int i = 0; i < 2; i++) foreach (KeyValuePair<int, Player> p in _sceneManager.Collidables) p.Value.AddCardToHand(Deck.DealCard(), i);
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
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            _graphics.ApplyChanges();
        }

        private void GameAction_MoveRight()
        {
            _sceneManager.Players[0].Force.X += 2000f;
        }

        private void GameAction_MoveLeft()
        {
            _sceneManager.Players[0].Force.X -= 2000f;
        }

        private void GameAction_MoveUp()
        {
            _sceneManager.Players[0].Force.Y -= 2000f;
        }

        private void GameAction_MoveDown()
        {
            _sceneManager.Players[0].Force.Y += 2000f;
        }

        private void GameAction_Attack()
        {
            if (_sceneManager.Projectiles.ContainsKey(10000)) _sceneManager.Projectiles.Remove(10000);
            Vector2 mousePosition = new(Mouse.GetState().X, Mouse.GetState().Y);
            double angle = FlatMath.GetAngle(_sceneManager.Players[0].Position, mousePosition);
            _sceneManager.AddProjectile(10000, _sceneManager.Players[0].Position, AnimationState.IDLE, _bullets, 500f, false);
            _sceneManager.Projectiles[10000].Force.X += (float)Math.Cos(angle) * 2500f;
            _sceneManager.Projectiles[10000].Force.Y += (float)Math.Sin(angle) * 2500f;
        }

        private void GameAction_PauseGame()
        {
            _paused = !_paused;
        }
    }
}
