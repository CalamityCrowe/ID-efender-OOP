using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace ID_efender_OOP
{
    class StaticGraphic
    {
        protected Texture2D txr;

        protected Rectangle rect;

        public Rectangle RECT
        {
            get
            {
                return rect;
            }
        }

        protected Vector2 position;
        public Vector2 POSITION
        {
            get
            {
                return position;
            }

        }

        public StaticGraphic(Texture2D texture, int X, int Y)
        {
            txr = texture;
            position = new Vector2(X, Y);

            rect = new Rectangle(X, Y, texture.Width, texture.Height);
        }
        public virtual void Draw(SpriteBatch sb, GameTime gt)
        {
            sb.Draw(txr, rect, Color.White);
#if DEBUG

            sb.DrawString(Game1.debug, "Rect:" + rect, new Vector2(rect.X, 0), Color.Red);
            //sb.Draw(Game1.pixel, rect, Color.Black * 0.5f);
#endif

        }
    }


    class MotionGraphic : StaticGraphic
    {


        protected Vector2 velocity;
        protected float speed;
        public MotionGraphic(Texture2D txr, int X, int Y, float S) : base(txr, X, Y)
        {

            velocity = Vector2.Zero;
            speed = S;

        }
        public virtual void update(KeyboardState Curr_KB, GameTime gt, KeyboardState Old_KB, Rectangle screen)
        {
            if (Curr_KB.IsKeyDown(Keys.D) && Old_KB.IsKeyDown(Keys.D))
            {
                //right

                if (velocity.X < 5)
                {
                    velocity.X += 0.5f;
                }
            }
            if (Curr_KB.IsKeyDown(Keys.A) && Old_KB.IsKeyDown(Keys.A))
            {
                //left

                if (velocity.X > -5)
                {
                    velocity.X -= 0.5f;
                }

            }

            position += velocity * speed;

            if (velocity.X > 0 && Curr_KB.IsKeyUp(Keys.D))
            {
                velocity.X -= ((float)gt.ElapsedGameTime.TotalSeconds * 2);
                if (velocity.X < 0.15f)
                {
                    velocity.X = 0;
                }
            }
            if (velocity.X < 0 && Curr_KB.IsKeyUp(Keys.A))
            {
                velocity.X += ((float)gt.ElapsedGameTime.TotalSeconds * 2);
                if (velocity.X > -0.15f)
                {
                    velocity.X = 0;
                }
            }
        }
        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;




            sb.Draw(txr, rect, Color.White);

#if DEBUG

            sb.DrawString(Game1.debug, "Rect: " + rect + "\npos: " + position + "\nVelocity: " + velocity, position, Color.White);
            sb.Draw(Game1.pixel, rect, Color.Blue * 0.5f);
#endif
        }
    }
    class AnimatedGraphic : MotionGraphic
    {

        protected Rectangle sourceRectangle;
        protected int FramesPerSecond;
        protected float updateTrigger;

        public AnimatedGraphic(Texture2D txr, int X, int Y, int frame, float S) : base(txr, X, Y, S)
        {
            FramesPerSecond = frame;
            sourceRectangle = new Rectangle(0, 0, rect.Width, rect.Height);

        }
        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * FramesPerSecond * 2.5f;

            if (updateTrigger >= 1)
            {
                updateTrigger = 0;
                sourceRectangle.X += sourceRectangle.Width;
                if (sourceRectangle.X >= txr.Width)
                {
                    sourceRectangle.X = 0;
                }

            }
            sb.Draw(txr, position, sourceRectangle, Color.White);
        }
    }
    class Background : MotionGraphic
    {
        private Rectangle Source;
        public Background(Texture2D txr, int X, int Y, float S) : base(txr, X, Y, S)
        {
            Source = rect;
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            Source.X = (int)position.X;
            sb.Draw(txr, rect, Source, Color.White);
        }
    }
}

