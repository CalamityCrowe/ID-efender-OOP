using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ID_efender_OOP
{
    enum AbductStates
    {
        Down,
        Still,
        Up,
        Reset,
        Death
    }
    class Abducters : AnimatedGraphic
    {
        private AbductStates CurrentState = AbductStates.Down;

        public AbductStates CURRENTSTATE
        {
            get
            {
                return CurrentState;
            }
        }

        private bool isVisible;
        public bool VisibleItIs
        {
            get
            {
                return isVisible;
            }
        }
        private Texture2D Explosion;

        private SoundEffectInstance ExplosionInstance;
        public Abducters(Texture2D TXR, Texture2D EXPLODE, SoundEffect EXPL, int X, int Y, int frame, float S) : base(TXR, X, Y, frame, S)
        {
            isVisible = true;
            ExplosionInstance = EXPL.CreateInstance();
            Explosion = EXPLODE;

        }
        public void update(GameTime gt, Rectangle screen, List<Bullet> plyBullet)
        {
            foreach (Bullet bull in plyBullet)
            {
                if (rect.Intersects(bull.RECT) && CurrentState != AbductStates.Death)
                {
                    CurrentState = AbductStates.Death;
                }
            }
            switch (CurrentState)
            {
                case AbductStates.Down:
                    velocity.Y = 1;

                    if ((position.Y + rect.Height) >= screen.Height - 10)
                    {
                        CurrentState = AbductStates.Still;
                    }
                    break;
                case AbductStates.Still:
                    velocity.Y = 0;
                    break;
                case AbductStates.Up:
                    velocity.Y = -1;
                    if ((position.Y + rect.Height) < screen.Y)
                    {
                        CurrentState = AbductStates.Reset;
                    }
                    break;
                case AbductStates.Reset:
                    // this resets the enemy postion when they get back to the top
                    position.X = Game1.RNG.Next(-3840, 5760 - rect.Width);
                    CurrentState = AbductStates.Down;
                    break;
            }
            position += velocity * speed;



        }
        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            if (CurrentState != AbductStates.Death)
            {
                rect.X = (int)position.X;
                rect.Y = (int)position.Y;

            }
            switch (CurrentState)
            {
                case AbductStates.Death:
                    rect.Y = -2000;
                    speed = 0f;
                    updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * FramesPerSecond * 2.5f;
                    ExplosionInstance.Play();

                    if (updateTrigger >= 1)
                    {
                        updateTrigger = 0;
                        sourceRectangle.X += sourceRectangle.Width;
                        if (sourceRectangle.X >= Explosion.Width)
                        {
                            isVisible = false;
                        }

                    }
                    sb.Draw(Explosion, position, sourceRectangle, Color.White);

                    break;
                default:
                    sb.Draw(txr, rect, Color.White);
                    break;

            }
#if DEBUG
            sb.Draw(Game1.pixel, rect, Color.Crimson * 0.5f);
            sb.DrawString(Game1.debug, "Rect: " + rect + "\nCurrent State: " + CURRENTSTATE, position, Color.Blue);
#endif
        }

    }

    enum ChaserStates
    {
        Left,
        Right,
        Death
    }

    class Chaser : AnimatedGraphic
    {
        private ChaserStates CurrentState;
        public Chaser(Texture2D TXR, Texture2D EXPLODE, SoundEffect EXPL, int X, int Y, int frame, float S) : base(TXR, X, Y, frame, S)
        {
            CurrentState = (ChaserStates)Game1.RNG.Next(0, 2);
        }
        public void Update()
        {

        }
        public void Draw()
        {

        }
    }
    enum TurretState
    {

    }
    class Turret : MotionGraphic
    {
        private int TimerCount, Reset;
        private float Timer;

        private int SpeedTime, ResetSpeed;
        private float SpeedTimer;


        private Texture2D BulletTxr;
        private SoundEffectInstance BlasterInstance;
        private List<Bullet> Bullets;
        public List<Bullet> bullets
        {
            get
            {
                return Bullets;
            }
        }
        public Turret(Texture2D TXR, Texture2D BulletTXR,SoundEffect Instance, int X, int Y, float S) : base(TXR, X, Y, S)
        {
            BulletTxr = BulletTXR;
            Reset = Game1.RNG.Next(3, 11);
            ResetSpeed = Game1.RNG.Next(5, 15);
            Bullets = new List<Bullet>();
            BlasterInstance = Instance.CreateInstance();
            BlasterInstance.Pitch = -1f;
        }
        public void update(Player player, GameTime gt, Rectangle screenbounds)
        {
            #region Movement

            SpeedTimer += (float)gt.ElapsedGameTime.TotalSeconds;
            SpeedTime += (int)SpeedTimer;
            if (SpeedTimer >= 1f)
            {
                SpeedTimer = 0;
            }
            if (SpeedTime == ResetSpeed)
            {
                speed = Game1.RNG.Next(1, 4);
                ResetSpeed = Game1.RNG.Next(5, 15);
                SpeedTime = 0;
            }

            //down movement
            if (position.Y < player.POSITION.Y)
            {
                position.Y += 1 * speed;
            }
            //update movement
            if (position.Y > player.POSITION.Y)
            {
                position.Y += -1 * speed;
            }
            #endregion
            #region setting X
            if (position.X < player.POSITION.X)
            {
                position.X = (player.POSITION.X - screenbounds.Width / 2);
            }
            if (position.X > player.POSITION.X)
            {
                position.X = (player.POSITION.X + screenbounds.Width / 2 - txr.Width);

            }
            #endregion
            #region Shooting of the turret

            Timer += (float)gt.ElapsedGameTime.TotalSeconds;
            TimerCount += (int)Timer;


            if (Timer >= 1f)
            {
                Timer = 0;
            }
            if (TimerCount == Reset)
            {
                if (position.X < player.POSITION.X)
                {
                    Bullets.Add(new Bullet(BulletTxr, (int)(position.X + txr.Width), (int)position.Y + (txr.Height / 2), 2.5f, new Vector2(1, velocity.Y)));
                    BlasterInstance.Play();
                }
                else
                {
                    Bullets.Add(new Bullet(BulletTxr, (int)(position.X), (int)position.Y + (txr.Height / 2), 2.5f, new Vector2(-1, velocity.Y)));
                    BlasterInstance.Play();
                }
                Reset = Game1.RNG.Next(3, 11);
                TimerCount = 0;
            }
            foreach (Bullet bullet in Bullets)
            {
                bullet.EnemyUpdate(gt, screenbounds, player);

            }
            for (int i = 0; i < Bullets.Count; i++)
            {
                if (!Bullets[i].VisibleItIs)
                {
                    Bullets.RemoveAt(i);
                    break;
                }
            }
            //Console.WriteLine(TimerCount);

            #endregion

        }
        public void Draw(SpriteBatch sb, GameTime gt, Player player)
        {
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;

            foreach (Bullet bullet in Bullets)
            {
                bullet.Draw(sb, gt);
            }
            if (position.X > player.POSITION.X)
            {
                sb.Draw(txr, rect, Color.White);
            }
            if (position.X < player.POSITION.X)
            {
                sb.Draw(txr, rect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
            }

#if DEBUG

            //sb.Draw(Game1.pixel, rect, Color.Beige * 0.5f);
            sb.DrawString(Game1.debug, "Rect: " + rect + "\nPos: " + position + "\nVelocity: " + velocity + "\nNum of Bullets: " + Bullets.Count, position, Color.Red);

#endif



        }
    }
}
