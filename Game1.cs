using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ID_efender_OOP
{
    enum GameState
    {
        StartMenu,
        Controls,
        Instructions,
        Playing,
        Paused,
        GameOver,
        HighScore,
        Reset
    }

    public class Game1 : Game
    {
        private GameState currentState = GameState.StartMenu;

        private KeyboardState currKey, oldKey;

        private Rectangle screenbounds, playerbounds;


        private Background Mountain, Sky;

        private StaticGraphic Hud;
        private static SpriteFont HudFont;
        private static SpriteFont Menu;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D HudBar, pauseOverlay;

        private List<Abducters> Abducts;
        private Turret LeftTurret, RightTurret;

        private Player Ply1;
        private List<Civs> People;


        private float CountDownTimer;
        private int CountDown = 5;
        private int ResetCountDown = 5;
        private int MaxEnemies = 60;

        private int numberDestroyed = 0;

        private int StartmenuSelect = 0;


        public static Random RNG = new Random();

#if DEBUG
        public static SpriteFont debug;
        public static Texture2D pixel;
#endif

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.IsFullScreen = true;



            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {


            Abducts = new List<Abducters>();
            People = new List<Civs>();

            playerbounds = GraphicsDevice.Viewport.Bounds;
            screenbounds = GraphicsDevice.Viewport.Bounds;
            HudBar = Content.Load<Texture2D>("HUD_Bar");
            pauseOverlay = Content.Load<Texture2D>("Pixel");
            playerbounds.Y += HudBar.Height;


            base.Initialize();
        }


        protected override void LoadContent()
        {

#if DEBUG
            debug = Content.Load<SpriteFont>("debug");
            pixel = Content.Load<Texture2D>("pixel");
#endif
            HudFont = Content.Load<SpriteFont>("IngameFont");
            Menu = Content.Load<SpriteFont>("Menu");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            Ply1 = new Player(Content.Load<Texture2D>("Player"), 0, graphics.PreferredBackBufferHeight / 2, 1.5f, Content.Load<SoundEffect>("Sound/Blaster"),
                                Content.Load<Texture2D>("Bull"));

            Hud = new StaticGraphic(HudBar, 600, 0);

            Sky = new Background(Content.Load<Texture2D>("Sky"), 0, 0, 0.5f);
            Mountain = new Background(Content.Load<Texture2D>("Mountain"), 0, 0, 3f);

            for (int i = 0; i < RNG.Next(10, MaxEnemies / 2); i++)
            {
                Abducts.Add(new Abducters(Content.Load<Texture2D>("Enemy/Abducter1"), Content.Load<Texture2D>("Spritesheet/Explosion"), Content.Load<SoundEffect>("Sound/Explosion_Sound"), RNG.Next((int)(graphics.PreferredBackBufferWidth * -1.5f), (int)(graphics.PreferredBackBufferWidth * 1.5f)), RNG.Next(-graphics.PreferredBackBufferHeight, -100), 21, RNG.Next(1, 5)));
            }

            LeftTurret = new Turret(Content.Load<Texture2D>("Turret/Turret" + RNG.Next(0, 2)), Content.Load<Texture2D>("Bull"), Content.Load<SoundEffect>("Sound/Blaster"), screenbounds.X - screenbounds.Width / 2, 400, 3);

            RightTurret = new Turret(Content.Load<Texture2D>("Turret/Turret" + RNG.Next(0, 2)), Content.Load<Texture2D>("Bull"), Content.Load<SoundEffect>("Sound/Blaster"), screenbounds.Width, screenbounds.Height / 2, 3);

            for (int i = 0; i < 30; i++)
            {
                People.Add(new Civs(Content.Load<Texture2D>("Civs/Civ" + RNG.Next(1, 3)), RNG.Next((int)(graphics.PreferredBackBufferWidth * -1.5f), (int)(graphics.PreferredBackBufferWidth * 1.5f)), graphics.PreferredBackBufferHeight - 50, 3, RNG.Next(1, 4)));
            }

        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            currKey = Keyboard.GetState();
            if (currKey.IsKeyDown(Keys.F5))
            {
                Exit();
            }
            switch (currentState)
            {
                #region StartMenu
                case GameState.StartMenu:

                    //moves the selection up
                    if (currKey.IsKeyDown(Keys.Up) && oldKey.IsKeyUp(Keys.Up))
                    {
                        StartmenuSelect--;
                    }
                    //moves the selection down
                    if (currKey.IsKeyDown(Keys.Down) && oldKey.IsKeyUp(Keys.Down))
                    {
                        StartmenuSelect++;
                    }
                    //loops it to the bottom
                    if (StartmenuSelect < 0)
                    {
                        StartmenuSelect = 3;
                    }
                    // loops it to the top
                    if (StartmenuSelect > 3)
                    {
                        StartmenuSelect = 0;
                    }

                    // this controls what one you have hit enter on

                    //this is for play
                    if (currKey.IsKeyDown(Keys.Enter) && oldKey.IsKeyUp(Keys.Enter) && StartmenuSelect == 0)
                    {
                        currentState = GameState.Playing;
                    }
                    //this is for Controls
                    if (currKey.IsKeyDown(Keys.Enter) && oldKey.IsKeyUp(Keys.Enter) && StartmenuSelect == 1)
                    {
                        currentState = GameState.Controls;
                    }
                    //this is for instructions
                    if (currKey.IsKeyDown(Keys.Enter) && oldKey.IsKeyUp(Keys.Enter) && StartmenuSelect == 2)
                    {
                        currentState = GameState.Instructions;
                    }
                    //this is to make it exit
                    if (currKey.IsKeyDown(Keys.Enter) && oldKey.IsKeyUp(Keys.Enter) && StartmenuSelect == 3)
                    {
                        Exit();
                    }

                    break;
                #endregion
                #region Controls
                case GameState.Controls:



                    break;
                #endregion
                #region Instructions
                case GameState.Instructions:

                    break;
                #endregion
                #region Playing
                case GameState.Playing:
                    if (currKey.IsKeyDown(Keys.Escape) && oldKey.IsKeyUp(Keys.Escape))
                    {
                        //Pauses the game
                        currentState = GameState.Paused;
                    }

                    foreach (Abducters abducter in Abducts)
                    {
                        abducter.update(gameTime, screenbounds, Ply1.bullets);
                    }


                    Ply1.Update(currKey, oldKey, gameTime, screenbounds, Abducts);

                    #region enemies update/removal and score adder
                    for (int i = 0; i < Abducts.Count; i++)
                    {
                        if (Abducts[i].POSITION.X > Ply1.POSITION.X + 2000)
                        {
                            Abducts[i] = new Abducters(Content.Load<Texture2D>("Enemy/Abducter1"), Content.Load<Texture2D>("Spritesheet/Explosion"), Content.Load<SoundEffect>("Sound/Explosion_Sound"), (int)Ply1.POSITION.X - 1000, (int)Abducts[i].POSITION.Y, 21, RNG.Next(1, 4));
                            break;
                        }
                        if (Abducts[i].POSITION.X < Ply1.POSITION.X - 2000)
                        {
                            Abducts[i] = new Abducters(Content.Load<Texture2D>("Enemy/Abducter1"), Content.Load<Texture2D>("Spritesheet/Explosion"), Content.Load<SoundEffect>("Sound/Explosion_Sound"), (int)Ply1.POSITION.X + 1000, (int)Abducts[i].POSITION.Y, 21, RNG.Next(1, 4));
                            break;
                        }
                        if (Abducts[i].VisibleItIs == false)
                        {
                            Abducts.RemoveAt(i);
                            numberDestroyed++;
                            break;
                        }
                    }
                    #endregion

                    #region Add enemies
                    if (Abducts.Count < MaxEnemies)
                    {
                        CountDownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        CountDown += (int)CountDownTimer;
                    }
                    if (CountDownTimer <= -1f)
                    {
                        CountDownTimer = 0f;
                    }
                    if (CountDown <= 0)
                    {
                        Abducts.Add(new Abducters(Content.Load<Texture2D>("Enemy/Abducter1"), Content.Load<Texture2D>("Spritesheet/Explosion"), Content.Load<SoundEffect>("Sound/Explosion_Sound"), RNG.Next((int)Ply1.POSITION.X - 1000, (int)Ply1.POSITION.X + 1000), RNG.Next(-1080, -100), 21, RNG.Next(1, 5)));
                        CountDown = ResetCountDown;
                    }


                    #endregion

                    #region turret Update
                    LeftTurret.update(Ply1, gameTime, screenbounds);
                    RightTurret.update(Ply1, gameTime, screenbounds);
                    #endregion

                    #region enviroment
                    Sky.update(currKey, gameTime, oldKey, screenbounds);
                    Mountain.update(currKey, gameTime, oldKey, screenbounds);
                    #endregion

                    #region people Update

                    foreach (Civs civ in People)
                    {
                        civ.update(Abducts);
                    }

                    #endregion

                    //this makes the screenbounds stay on the screen when the player moves location
                    screenbounds.X = (int)Ply1.POSITION.X - screenbounds.Width / 2;

                    break;
                #endregion
                #region Paused
                case GameState.Paused:
                    //resumes the game
                    if (currKey.IsKeyDown(Keys.Escape) && oldKey.IsKeyUp(Keys.Escape))
                    {
                        currentState = GameState.Playing;
                    }
                    break;
                #endregion
                #region GameOver
                case GameState.GameOver:

                    break;
                #endregion
                #region HighScore
                case GameState.HighScore:

                    break;
                #endregion
                #region Reset
                case GameState.Reset:
                    /*
                     * 
                     * This is to reset the game back to its base settings after the player has been defeated
                     * 
                     */

                    Abducts.Clear();
                    People.Clear();

                    numberDestroyed = 0;
                    MaxEnemies = 60;
                    CountDown = 5;

                    Sky = new Background(Content.Load<Texture2D>("Sky"), 0, 0, 0.5f);
                    Mountain = new Background(Content.Load<Texture2D>("Mountain"), 0, 0, 3f);

                    for (int i = 0; i < RNG.Next(10, MaxEnemies / 2); i++)
                    {
                        Abducts.Add(new Abducters(Content.Load<Texture2D>("Enemy/Abducter1"), Content.Load<Texture2D>("Spritesheet/Explosion"), Content.Load<SoundEffect>("Sound/Explosion_Sound"), RNG.Next((int)(graphics.PreferredBackBufferWidth * -0.5f), (int)(graphics.PreferredBackBufferWidth * 1.5f)), RNG.Next(-graphics.PreferredBackBufferHeight, -100), 21, RNG.Next(1, 5)));
                    }

                    for (int i = 0; i < 30; i++)
                    {
                        People.Add(new Civs(Content.Load<Texture2D>("Civs/Civ1"), RNG.Next((int)(graphics.PreferredBackBufferWidth * -1.5f), (int)(graphics.PreferredBackBufferWidth * 1.5f)), graphics.PreferredBackBufferHeight - 50, 3, RNG.Next(1, 4)));
                    }

                    LeftTurret = new Turret(Content.Load<Texture2D>("Turret/Turret" + RNG.Next(0, 2)), Content.Load<Texture2D>("Bull"), Content.Load<SoundEffect>("Sound/Blaster"), screenbounds.X - screenbounds.Width / 2, 400, 3);

                    RightTurret = new Turret(Content.Load<Texture2D>("Turret/Turret" + RNG.Next(0, 2)), Content.Load<Texture2D>("Bull"), Content.Load<SoundEffect>("Sound/Blaster"), screenbounds.Width, screenbounds.Height / 2, 3);

                    Ply1 = new Player(Content.Load<Texture2D>("Player"), 0, graphics.PreferredBackBufferHeight / 2, 1.5f, Content.Load<SoundEffect>("Sound/Blaster"),
                    Content.Load<Texture2D>("Bull"));

                    currentState = GameState.StartMenu;

                    break;
                    #endregion
            }


            // this adjusts the screenbounds


            base.Update(gameTime);
            oldKey = currKey;
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            #region loop background spritebatch

            /*
             * 
             * this is for creating the looping background effect by only using one
             * 
             * it does this by switching the samplerstate to have a linearwrap
             * 
             * then when the source rectangle moves it loops the image back on itself
             * 
             */

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, null);

            Sky.Draw(spriteBatch, gameTime);
            Mountain.Draw(spriteBatch, gameTime);



            spriteBatch.End();

            #endregion


            #region Camera movement

            /*
             * This is for drawing anything in the game when it's actually playing
             * 
             * The reason for this is so that the camera is locked onto the player and they are the only one that's moving on the X axis
             *
             * This is where the player,enemies,background and civilians will be drawn.
             * 
             */

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(new Vector3(Ply1.getPos().X, Ply1.getPos().Y, 0)));

            //the gamestate for drawing
            switch (currentState)
            {
                case GameState.Playing:

                    playingDraw(spriteBatch, gameTime);

                    break;
                case GameState.Paused:

                    playingDraw(spriteBatch, gameTime);

                    break;
            }




#if DEBUG

            //spriteBatch.Draw(pixel, screenbounds, Color.White * 0.5f);

#endif


            spriteBatch.End();







            #endregion


            #region Static draw fields

            /*
             * this is for drawing objects that will be set on the screen
             * 
             * this can vary from menus like the pause screen and the start screen
             * to any of the hud elements
             * 
             */
            spriteBatch.Begin();

            switch (currentState)
            {
                #region StartMenu
                case GameState.StartMenu:

                    spriteBatch.Draw(pauseOverlay, GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

                    //displays the arrow keys
                    if (currKey.IsKeyDown(Keys.Up))
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>("Keys/Arrow"), new Rectangle(graphics.PreferredBackBufferWidth - 300, graphics.PreferredBackBufferHeight - 100, 50, 50), null,
                            Color.Orange, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
                    }
                    if (currKey.IsKeyUp(Keys.Up))
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>("Keys/Arrow"), new Rectangle(graphics.PreferredBackBufferWidth - 300, graphics.PreferredBackBufferHeight - 100, 50, 50), null,
                            Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
                    }
                    if (currKey.IsKeyDown(Keys.Down))
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>("Keys/Arrow"), new Rectangle(graphics.PreferredBackBufferWidth - 240, graphics.PreferredBackBufferHeight - 100, 50, 50), null,
                            Color.Orange);
                    }
                    if (currKey.IsKeyUp(Keys.Down))
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>("Keys/Arrow"), new Rectangle(graphics.PreferredBackBufferWidth - 240, graphics.PreferredBackBufferHeight - 100, 50, 50), null,
                            Color.White);

                    }
                    spriteBatch.Draw(Content.Load<Texture2D>("Keys/Enter"), new Rectangle(graphics.PreferredBackBufferWidth - 190, graphics.PreferredBackBufferHeight - 100, 100, 100), null,
                        Color.White);


                    switch (StartmenuSelect) //this is for drawing the one that the player has selcted
                    {
                        case 0:

                            spriteBatch.DrawString(Menu, "Play", new Vector2(60, graphics.PreferredBackBufferHeight / 2), Color.Orange);
                            spriteBatch.DrawString(Menu, "Controls", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 34), Color.White);
                            spriteBatch.DrawString(Menu, "Instructions", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 66), Color.White);
                            spriteBatch.DrawString(Menu, "Exit", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 100), Color.White);

                            break;
                        case 1:

                            spriteBatch.DrawString(Menu, "Play", new Vector2(60, graphics.PreferredBackBufferHeight / 2), Color.White);
                            spriteBatch.DrawString(Menu, "Controls", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 34), Color.Orange);
                            spriteBatch.DrawString(Menu, "Instructions", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 66), Color.White);
                            spriteBatch.DrawString(Menu, "Exit", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 100), Color.White);

                            break;
                        case 2:

                            spriteBatch.DrawString(Menu, "Play", new Vector2(60, graphics.PreferredBackBufferHeight / 2), Color.White);
                            spriteBatch.DrawString(Menu, "Controls", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 34), Color.White);
                            spriteBatch.DrawString(Menu, "Instructions", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 66), Color.Orange);
                            spriteBatch.DrawString(Menu, "Exit", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 100), Color.White);

                            break;
                        case 3:

                            spriteBatch.DrawString(Menu, "Play", new Vector2(60, graphics.PreferredBackBufferHeight / 2), Color.White);
                            spriteBatch.DrawString(Menu, "Controls", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 34), Color.White);
                            spriteBatch.DrawString(Menu, "Instructions", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 66), Color.White);
                            spriteBatch.DrawString(Menu, "Exit", new Vector2(60, graphics.PreferredBackBufferHeight / 2 + 100), Color.Orange);

                            break;
                    }

                    break;
                #endregion

                #region Controls
                case GameState.Controls:

                    spriteBatch.Draw(pauseOverlay, GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

                    //this is to show what key is getting pressed
                    if (currKey.IsKeyDown(Keys.W))
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>("Keys/W"), new Rectangle(graphics.PreferredBackBufferWidth/4 - 100 , graphics.PreferredBackBufferHeight/2 - 100, 100, 100), null,
                                            Color.Orange);
                    }
                    if (currKey.IsKeyUp(Keys.W))
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>("Keys/W"), new Rectangle(graphics.PreferredBackBufferWidth / 4 - 100, graphics.PreferredBackBufferHeight / 2 - 100, 100, 100), null,
                                            Color.White);
                    }

                    break;
                #endregion
                case GameState.Instructions:

                    spriteBatch.Draw(pauseOverlay, GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

                    break;
                case GameState.Playing:
                    Hud.Draw(spriteBatch, gameTime);


                    spriteBatch.DrawString(HudFont, "Destroyed\n" + numberDestroyed, Vector2.One, Color.Black);

                    break;
                case GameState.Paused:
                    Hud.Draw(spriteBatch, gameTime);


                    spriteBatch.DrawString(HudFont, "Destroyed\n" + numberDestroyed, Vector2.One, Color.Black);

                    spriteBatch.Draw(pauseOverlay, GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
                    spriteBatch.DrawString(HudFont, "Paused\n\nPress Esc to Resume", new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.White);
                    spriteBatch.DrawString(HudFont, "Exit", new Vector2(101, graphics.PreferredBackBufferHeight - 30), Color.White);
                    break;
                case GameState.GameOver:
                    spriteBatch.Draw(pauseOverlay, GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);


                    break;
                case GameState.HighScore:

                    spriteBatch.Draw(pauseOverlay, GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

                    break;

            }





#if DEBUG
            spriteBatch.DrawString(debug, graphics.PreferredBackBufferWidth + "X" + graphics.PreferredBackBufferHeight + "\nfps: " + (int)(1 / gameTime.ElapsedGameTime.TotalSeconds) + "ish" + "\nNum of Enemies" + Abducts.Count, new Vector2(0, screenbounds.Y), Color.White);

#endif

            spriteBatch.End();
            #endregion 

            base.Draw(gameTime);
        }
        void playingDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            foreach (Abducters enemy in Abducts)
            {
                enemy.Draw(spriteBatch, gameTime);

            }
            foreach (Civs civ in People)
            {
                civ.Draw(spriteBatch, gameTime);
            }

            RightTurret.Draw(spriteBatch, gameTime, Ply1);
            LeftTurret.Draw(spriteBatch, gameTime, Ply1);

            Ply1.Draw(spriteBatch, gameTime);
        }
    }
}
