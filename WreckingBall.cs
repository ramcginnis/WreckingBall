using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace WreckingBall
{
    public class WreckingBall : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D background;
        private Rectangle backgroundRectangle = new Rectangle(0, 0, 550, 480);        
        private int nextBrick;
        private Texture2D redBrick;
        private Texture2D orangeBrick;
        private Texture2D yellowBrick;
        private Texture2D greenBrick;
        private Texture2D blueBrick;
        private Texture2D purpleBrick;
        private Texture2D nullBrick;
        private Texture2D paddle;
        private Texture2D ball;
        private Texture2D livesTexture;

        private int level = 1;
        private int justHit = 999;
        
        private Rectangle ballRectangle = new Rectangle(270, 440, 10, 10);
        private int ballXImpulse = 2;
        private int ballYImpulse = -2;

        private Rectangle paddleRectangle = new Rectangle(250, 455, 50, 8);

        private List<Brick> brickList = new List<Brick>();      


        private SpriteFont font;
        private int score = 0;
        private int multiplier = 1;

        private int lives = 3;
        private bool clickToStart = false;
        

        private Random brickSelector = new Random();
        

        public WreckingBall()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 550;
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here          

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            background = Content.Load<Texture2D>("Images/Background");
            redBrick = Content.Load<Texture2D>("Images/RedBrick");
            orangeBrick = Content.Load<Texture2D>("Images/OrangeBrick");
            yellowBrick = Content.Load<Texture2D>("Images/YellowBrick");
            greenBrick = Content.Load<Texture2D>("Images/GreenBrick");
            blueBrick = Content.Load<Texture2D>("Images/BlueBrick");
            purpleBrick = Content.Load<Texture2D>("Images/PurpleBrick");
            nullBrick = Content.Load<Texture2D>("Images/NullBrick");
            paddle = Content.Load<Texture2D>("Images/Paddle");
            ball = Content.Load<Texture2D>("Images/Ball");
            livesTexture = Content.Load<Texture2D>("Images/Lives");

            font = Content.Load<SpriteFont>("Score");

            createBricks(level);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Brick begone = new Brick(redBrick, 0, 0, 99);

            // TODO: Add your update logic here
            
            //load new level
            if (brickList.Count == 0)
            {
                ballYImpulse = -2;
                clickToStart = false;
                level++;
                ballRectangle.X = backgroundRectangle.Width/2-5;
                ballRectangle.Y = backgroundRectangle.Height-40;
                paddleRectangle.X = backgroundRectangle.Width / 2 - 25;
                if (level <= 10)
                {
                    createBricks(level);
                }
                else
                {
                    createBricks(10);
                }
            }

            if(Mouse.GetState().LeftButton == ButtonState.Pressed && Mouse.GetState().X >= 0 && Mouse.GetState().X < backgroundRectangle.Width && Mouse.GetState().Y >= 0 && Mouse.GetState().Y < backgroundRectangle.Height && IsActive)
            {
                clickToStart = true;
            }

            //wait for mouse click to move ball
            if (clickToStart)
            {
                ballRectangle.X += ballXImpulse;
                ballRectangle.Y += ballYImpulse;
                //move paddle with mouse
                paddleRectangle.X = Mouse.GetState().X;

                //keep paddle on screen
                if (Mouse.GetState().X > backgroundRectangle.Width - paddleRectangle.Width)
                {
                    paddleRectangle.X = backgroundRectangle.Width - paddleRectangle.Width;
                }
                if (Mouse.GetState().X < 0)
                {
                    paddleRectangle.X = 0;
                }
            }

            //bounce off walls
            if (ballRectangle.Right >= backgroundRectangle.Right || ballRectangle.Left <= backgroundRectangle.Left)
            {
                ballXImpulse *= -1;
            }

            if (ballRectangle.Top <= backgroundRectangle.Top)
            {
                ballYImpulse *= -1;
            }

            //reset on hitting bottom 
            if (ballRectangle.Bottom >= backgroundRectangle.Bottom)
            {
                clickToStart = false;
                lives--;
                ballRectangle.X = backgroundRectangle.Width / 2 - 5;
                ballRectangle.Y = backgroundRectangle.Height - 40;
                paddleRectangle.X = backgroundRectangle.Width / 2 - 25;
                ballYImpulse *= -1;
            }

            //end game
            if (lives == 0)
            {
                clickToStart = false;                
                ballRectangle.X = backgroundRectangle.Width / 2 - 5;
                ballRectangle.Y = backgroundRectangle.Height - 40;
                paddleRectangle.X = backgroundRectangle.Width / 2 - 25;
            }

            //bounce off paddle
            if(Math.Abs(ballRectangle.Bottom - paddleRectangle.Top) < 2 && ballRectangle.Left >  paddleRectangle.Left-5 && ballRectangle.Right < paddleRectangle.Right+5)
            {
                if (ballRectangle.Left + 5 < paddleRectangle.Left + paddleRectangle.Width/4)
                {
                    ballXImpulse = -3;
                    ballYImpulse = -2;
                }
                else if (ballRectangle.Left + 5 < paddleRectangle.Left + paddleRectangle.Width / 2)
                {
                    ballXImpulse = -2;
                    ballYImpulse = -3;
                }
                else if (ballRectangle.Left + 5 < paddleRectangle.Left + paddleRectangle.Width / 4 * 3)
                {
                    ballXImpulse = 2;
                    ballYImpulse = -3;
                }
                else if (ballRectangle.Left + 5 < paddleRectangle.Left + paddleRectangle.Width)
                {
                    ballXImpulse = 3;
                    ballYImpulse = -2;
                }

                multiplier = 1;
            }

            //bounce off and break bricks
            foreach (Brick b in brickList)
            {
                if(ballRectangle.Intersects(b.rectangle))
                {
                    if ((Math.Abs(ballRectangle.Top - b.rectangle.Bottom) < 5 || Math.Abs(ballRectangle.Bottom - b.rectangle.Top) < 5) && justHit != b.id)
                    {
                        b.currentState--;
                        switch(b.currentState)
                        {
                            case 6:
                                b.Texture = purpleBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 5:
                                b.Texture = blueBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 4:
                                b.Texture = greenBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 3:
                                b.Texture = yellowBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 2:
                                b.Texture = orangeBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 1:
                                b.Texture = redBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 0:
                                begone = b;
                                score += b.originalState * 5 * multiplier;
                                multiplier++;
                                break;

                        }
                        ballYImpulse *= -1;
                        break;
                    }
                    if ((Math.Abs(ballRectangle.Left - b.rectangle.Right) < 5 || Math.Abs(ballRectangle.Right - b.rectangle.Left) < 5)&& justHit != b.id)
                    {
                        b.currentState--;
                        switch (b.currentState)
                        {
                            case 6:
                                b.Texture = purpleBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 5:
                                b.Texture = blueBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 4:
                                b.Texture = greenBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 3:
                                b.Texture = yellowBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 2:
                                b.Texture = orangeBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 1:
                                b.Texture = redBrick;
                                score += 5 * multiplier;
                                multiplier++;
                                break;
                            case 0:
                                begone = b;
                                score += b.originalState * 5 * multiplier;
                                multiplier++;
                                break;
                        }
                        ballXImpulse *= -1;
                        justHit = b.id;
                        break;
                    }
                }
                else
                {
                    justHit = 999;
                }
            }

            //remove destroyed brick
            if (brickList.Contains(begone))
            {
                brickList.Remove(begone);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
          

            _spriteBatch.Begin();
            
            _spriteBatch.Draw(background, backgroundRectangle, Color.White);            

            foreach (Brick b in brickList)
            {
                _spriteBatch.Draw(b.Texture, b.rectangle, Color.White);
            }

            for(int i = 1; i <= lives;i++)
            {
                _spriteBatch.Draw(livesTexture, new Rectangle(backgroundRectangle.Width - (i * 30),0, 30, 30), Color.White);
            }

            _spriteBatch.Draw(paddle, paddleRectangle, Color.White);
            _spriteBatch.Draw(ball, ballRectangle, Color.White);
            _spriteBatch.DrawString(font, "Score: " + score, new Vector2(0, 0), Color.Black);


            _spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected void createBricks(int level)
        {
            for (int i = 5 - (level + 1) / 2; i <= 5 + (level + 1) / 2; i++)
            {
                for (int j = 6 - (level + 1) / 2; j <= 6 + (level + 1) / 2; j++)
                {
                    nextBrick = brickSelector.Next(0, 1+(int)(level*1.5));
                    nextBrick = nextBrick % 8;

                    switch (nextBrick)
                    {
                        case 0:
                            break;
                        case 1:
                            brickList.Add(new Brick(redBrick, i * 50, j * 30, 1));
                            break;
                        case 2:
                            brickList.Add(new Brick(orangeBrick, i * 50, j * 30, 2));
                            break;
                        case 3:
                            brickList.Add(new Brick(yellowBrick, i * 50, j * 30, 3));
                            break;
                        case 4:
                            brickList.Add(new Brick(greenBrick, i * 50, j * 30, 4));
                            break;
                        case 5:
                            brickList.Add(new Brick(blueBrick, i * 50, j * 30, 5));
                            break;
                        case 6:
                            brickList.Add(new Brick(purpleBrick, i * 50, j * 30, 6));
                            break;
                        case 7:
                            brickList.Add(new Brick(nullBrick, i * 50, j * 30, 7));
                            break;
                    }
                }
            }
        }
    }
}
