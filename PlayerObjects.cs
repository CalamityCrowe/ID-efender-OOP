using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ID_efender_OOP
{

    enum PlayerStates
    {
        StationaryLeft,
        StationaryRight,
        MoveRight,
        MoveLeft,
        Death,

    }

    class Player : MotionGraphic
    {
        private List<Bullet> Bullets;
        public List<Bullet> bullets
        {
            get
            {
                return Bullets;
            }
        }

        private Texture2D bulletTxr;

        private PlayerStates currentState;
        private SoundEffectInstance BlasterInstance;

        public Player(Texture2D txr, int X, int Y, float S, SoundEffect BlasterNoise, Texture2D BB) : base(txr, X, Y, S)
        {
            currentState = PlayerStates.StationaryRight;

            Bullets = new List<Bullet>();

            bulletTxr = BB;
            Bullets = new List<Bullet>();
            BlasterInstance = BlasterNoise.CreateInstance();
        }
        public void Update(KeyboardState Curr_KB, KeyboardState Old_KB, GameTime gt, Rectangle screen, List<Abducters> Abducts)
        {
            #region Player Movement

            if (Curr_KB.IsKeyDown(Keys.W) && Old_KB.IsKeyDown(Keys.W))
            {
                //up
                if (velocity.Y > -3)
                {
                    velocity.Y -= 1;
                }
                if (position.Y <= 216)
                {
                    velocity.Y = 0;
                }
            }
            if (Curr_KB.IsKeyDown(Keys.S) && Old_KB.IsKeyDown(Keys.S))
            {
                //down
                if (velocity.Y < 3)
                {
                    velocity.Y += 1;
                }
                if (position.Y + rect.Height >= screen.Height - 200)
                {
                    velocity.Y = 0;
                }
            }


            if (Curr_KB.IsKeyDown(Keys.D) && Old_KB.IsKeyDown(Keys.D))
            {
                //right
                currentState = PlayerStates.StationaryRight;
                if (velocity.X < 5)
                {
                    velocity.X += 0.5f;
                }
            }
            if (Curr_KB.IsKeyDown(Keys.A) && Old_KB.IsKeyDown(Keys.A))
            {
                //left
                currentState = PlayerStates.StationaryLeft;
                if (velocity.X > -5)
                {
                    velocity.X -= 0.5f;
                }
            }

            //stops the movement of the player on the Y axis when a key isn't pressed so they don't drift off the screen
            if (position.Y <= 216 && Curr_KB.IsKeyUp(Keys.W))
            {
                if (Curr_KB.IsKeyDown(Keys.S))
                {

                }
                else
                {
                    velocity.Y = 0;
                }
            }
            if (position.Y + rect.Height >= screen.Height - 200 && Curr_KB.IsKeyUp(Keys.S))
            {
                if (Curr_KB.IsKeyDown(Keys.W))
                {
                }
                else
                {
                    velocity.Y = 0;
                }
            }

            position += velocity * speed;

            //gliding effect

            //right gliding
            if (velocity.X < 0 && Curr_KB.IsKeyUp(Keys.D))
            {
                velocity.X += ((float)gt.ElapsedGameTime.TotalSeconds * 2);
                if (velocity.X > -0.15f)
                {
                    velocity.X = 0;
                }
            }
            //left gliding
            if (velocity.X > 0 && Curr_KB.IsKeyUp(Keys.A))
            {
                velocity.X -= ((float)gt.ElapsedGameTime.TotalSeconds * 2);
                if (velocity.X < 0.15f)
                {
                    velocity.X = 0;
                }
            }
            //downwards glide
            if (velocity.Y > 0 && Curr_KB.IsKeyUp(Keys.S))
            {
                velocity.Y -= ((float)gt.ElapsedGameTime.TotalSeconds * 4);

                if (velocity.Y < 0.15f)
                {
                    velocity.Y = 0;
                }
            }
            //upwards glide
            if (velocity.Y < 0 && Curr_KB.IsKeyUp(Keys.W))
            {
                velocity.Y += ((float)gt.ElapsedGameTime.TotalSeconds * 4);

                if (velocity.Y > -0.15f)
                {
                    velocity.Y = 0;
                }
            }
            #endregion



            #region Shooting stuff

            switch (currentState)
            {
                case PlayerStates.StationaryRight:

                    if (Curr_KB.IsKeyDown(Keys.Space) && Old_KB.IsKeyUp(Keys.Space))
                    {
                        BlasterInstance.Play();
                        Bullets.Add(new Bullet(bulletTxr, (int)position.X + rect.Width, (int)(position.Y + rect.Height / 2), speed, new Vector2(6, velocity.Y)));

                    }

                    break;
                case PlayerStates.StationaryLeft:

                    if (Curr_KB.IsKeyDown(Keys.Space) && Old_KB.IsKeyUp(Keys.Space))
                    {
                        BlasterInstance.Play();

                        Bullets.Add(new Bullet(bulletTxr, (int)position.X + rect.Width, (int)(position.Y + rect.Height / 2), speed, new Vector2(-6,velocity.Y)));

                    }

                    break;
            }
            //this is for shooting down
            if (Curr_KB.IsKeyDown(Keys.B) && Old_KB.IsKeyUp(Keys.B))
            {
                BlasterInstance.Play();
                Bullets.Add(new Bullet(bulletTxr, (int)(position.X + rect.Width / 2 - bulletTxr.Width/2), (int)(position.Y + rect.Height), speed, new Vector2(0, (3))));
            }

            foreach (Bullet bull in Bullets)
            {
                bull.Update(gt, screen, Abducts);
            }
            for (int i = 0; i < Bullets.Count; i++)
            {
                if (!Bullets[i].VisibleItIs)
                {
                    Bullets.RemoveAt(i);
                    break;
                }
            }

            #endregion
        }
        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;



            switch (currentState)
            {
                case PlayerStates.StationaryRight:

                    sb.Draw(txr, rect, Color.White);

                    break;

                case PlayerStates.StationaryLeft:

                    sb.Draw(txr, rect, null, new Color(new Vector3(255, 255, Game1.RNG.Next(0, 100))), 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);

                    break;

            }

            #region bullet drawing

            foreach (Bullet bull in Bullets)
            {
                bull.Draw(sb, gt);
            }

            #endregion

#if DEBUG
            sb.Draw(Game1.pixel, rect, Color.Blue * 0.5f);
            sb.DrawString(Game1.debug, "Rect: " + rect + "\nPos: " + position + "\nVelocity: " + velocity + "\nNum of Bullets: " + Bullets.Count, position, Color.White);
#endif
        }
        public Vector2 getPos()
        {
            return new Vector2(-position.X + 960, 0);
        }

    }








    class Bullet : MotionGraphic
    {
        private bool isVisible;
        public bool VisibleItIs
        {
            get
            {
                return isVisible;
            }
        }
        public Bullet(Texture2D txr, int X, int Y, float S, Vector2 velo) : base(txr, X, Y, S)
        {
            velocity = velo;
            velocity.X *= (speed * speed);
            isVisible = true; // this is used for removing them from a list in the other classes
        }
        public void Update(GameTime gt, Rectangle Screen, List<Abducters> Abducts)
        {
            position += velocity * speed;

            if (!rect.Intersects(Screen))
            {
                isVisible = false;
            }
            foreach (Abducters abduct in Abducts)
            {
                if (rect.Intersects(abduct.RECT))
                {
                    isVisible = false;

                }
            }
        }
        public void EnemyUpdate(GameTime gt, Rectangle screen, Player player)
        {
            //this is for the enemy turrets that shoot at the player
            position += velocity * speed;
            if (!rect.Intersects(screen) || velocity.X == 0)
            {
                isVisible = false;
            }
            if (rect.Intersects(player.RECT))
            {
                isVisible = false;
            }
        }
    }






    enum CivStates
    {
        Left,
        Right,
        Captured,
        Falling
    }
    class Civs : AnimatedGraphic
    {
        private CivStates CurrentState;
        public CivStates currentstate
        {
            get
            {
                return CurrentState;
            }
        }
        public Civs(Texture2D txr, int X, int Y, int frame, float S) : base(txr, X, Y, frame, S)
        {
            sourceRectangle.Width /= 6;
            velocity.X = 1;
            CurrentState = (CivStates)Game1.RNG.Next(0, 2);
            switch (CurrentState)
            {
                case CivStates.Right:
                    
                    break;
                default:
                    speed *= -1;
                    break;
            }

        }
        public void update(List<Abducters> abducters)
        {
            switch (CurrentState)
            {
                default:
                    position += (velocity * speed);
                    break;
            }
        }
        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;

            switch (CurrentState)
            {
                case CivStates.Left:
                    updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * FramesPerSecond * 2.5f;

                    if (updateTrigger >= 1)
                    {
                        updateTrigger = 0;
                        sourceRectangle.X += sourceRectangle.Width;
                        if (sourceRectangle.X >= txr.Width - (sourceRectangle.Width *2))
                        {
                            sourceRectangle.X = 0;
                        }

                    }
                    sb.Draw(txr, position, sourceRectangle, Color.White);

                    break;
                case CivStates.Right:

                    updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * FramesPerSecond * 2.5f;

                    if (updateTrigger >= 1)
                    {
                        updateTrigger = 0;
                        sourceRectangle.X += sourceRectangle.Width;
                        if (sourceRectangle.X >= txr.Width - (sourceRectangle.Width * 2))
                        {
                            sourceRectangle.X = 0;
                        }

                    }
                    sb.Draw(txr, position, sourceRectangle, Color.White,0f,Vector2.Zero,1f,SpriteEffects.FlipHorizontally,0f);

                    break;
            }

        }
    }
}
