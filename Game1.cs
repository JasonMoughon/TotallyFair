using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net.Mime;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using TotallyFair.CONFIG;
using TotallyFair.GameComponents;
using TotallyFair.Graphics;
using TotallyFair.Managers;

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
        private Map _map;
        private SpriteFont GameFont; 

        /*******************************/
        /* Initialize ALL GAME OBJECTS */
        /*******************************/
        private CardDeck Deck = new CardDeck();
        private Card[] Dealer = new Card[5];
        private Vector2 Vector_Dealer = new Vector2(0, 0);
        private static Player[] Players = new Player[6];
        private Vector2[] Vector_PlayerHand = new Vector2[6];

        /************/
        /* SETTINGS */
        /************/
        private GAME_CONFIG _settings;

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
            _map = new Map(Content.Load<Texture2D>("Map"));
            _settings = new GAME_CONFIG(_map);

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
            InputListener.AddMouseHandler(false, GameAction_Attack, InputManager.MouseClickState.LEFT);

            base.Initialize();

            DoneInitializing = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

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
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i] = new Player($"Player{i+1}", _settings.STARTING_POSITIONS[6][i], AnimationState.IDLE, PlayerIdleTextures, PlayerIdleTime, true);
                Players[i].Sprite.AddAnimation(AnimationState.RUNNINGLEFT, PlayerRunningLeftTextures, PlayerRunningTime, true);
                Players[i].Sprite.AddAnimation(AnimationState.RUNNINGRIGHT, PlayerRunningTextures, PlayerRunningTime, true);
                if (i == 0) Players[i].IsCPU = false;
            }

            GameFont = Content.Load<SpriteFont>("Arial");


            //Deal Cards (Players must be initialized first)
            DealCards();
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
            _map.Draw(_spriteBatch, _graphics);

            for (int i = 0; i < Players.Length; i++)
            {
                _spriteBatch.DrawString(GameFont, $"{Players[i].Hand[0].FaceValue}", Vector_PlayerHand[i], Color.White);
                _spriteBatch.DrawString(GameFont, $"{Players[i].Hand[1].FaceValue}", new Vector2(Vector_PlayerHand[i].X, Vector_PlayerHand[i].Y + 25), Color.White);
                Players[i].Sprite.Play(_spriteBatch);
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
                if (P.IsCPU) P.Chase(Players[0].Sprite.Position);
                P.Sprite.Position.X += P.Velocity.X * DeltaTime;
                P.Sprite.Position.Y += P.Velocity.Y * DeltaTime;

                //Clamp to bounds of window
                if (P.Sprite.Position.X > _graphics.PreferredBackBufferWidth) P.Sprite.Position.X = _graphics.PreferredBackBufferWidth;
                if (P.Sprite.Position.X - P.Sprite.AnimationLibrary[P.Sprite.CurrentState].GetCurrentTexture().Width < 0) P.Sprite.Position.X = P.Sprite.AnimationLibrary[P.Sprite.CurrentState].GetCurrentTexture().Width;
                if (P.Sprite.Position.Y > _graphics.PreferredBackBufferHeight) P.Sprite.Position.Y = _graphics.PreferredBackBufferHeight;
                if (P.Sprite.Position.Y - P.Sprite.AnimationLibrary[P.Sprite.CurrentState].GetCurrentTexture().Height < 0) P.Sprite.Position.Y = P.Sprite.AnimationLibrary[P.Sprite.CurrentState].GetCurrentTexture().Height;

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
            //Players[0].StrikeBox.Rotation = (float)Math.Atan(Mouse.GetState().X - (Players[0].Sprite.Position.X + Players[0].Sprite.AnimationLibrary[Players[0].Sprite.CurrentState].Animation.Peek().Width / 2) / (Mouse.GetState().Y - (Players[0].Sprite.Position.Y - Players[0].Sprite.AnimationLibrary[Players[0].Sprite.CurrentState].Animation.Peek().Height / 2)));;
            //Players[0].StrikeBox.Visible = true;      
        }
    }
}
