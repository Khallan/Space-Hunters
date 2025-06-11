using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeHunter;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _spaceship;
    private Vector2 _spaceshipPosition;
    private float _spaceshipRotate;
    private Texture2D _laserBlastRed;
    //private float _laserBlastRedRotate;
    private Vector2 _laserBlastRedPostition;
    private float _laserBlastRedRotate;
    private const float _laserBlastRedScale = 0.3f;

    private const float _spaceshipScale = 0.3f;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _spaceship = Content.Load<Texture2D>("images/spaceship");

        // TODO: use this.Content to load your game content here
        _spaceshipPosition = new Vector2((float)(Window.ClientBounds.Width * 0.0f + _spaceship.Width * 0.5 * 0.3), Window.ClientBounds.Height * 0.5f);
       

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }


        // Movement ---------------------------------------------------------------------------------------------


        float halfW = _spaceship.Width * _spaceshipScale * 0.5f;
        float halfH = _spaceship.Height * _spaceshipScale * 0.5f;

        if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up))
        {
            if (_spaceshipPosition.Y - halfH > 0)
            {
                _spaceshipPosition.Y -= 3.5f; // 5px per frame
                _spaceshipRotate = MathHelper.ToRadians(0);
            }
        }
        if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down))
        {
            if (_spaceshipPosition.Y + halfH < Window.ClientBounds.Height)
            {
                _spaceshipPosition.Y += 3.5f; // 5px per frame
                _spaceshipRotate = MathHelper.ToRadians(180);

            }
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
        {
            if (_spaceshipPosition.X + halfW < Window.ClientBounds.Width)
            {
                _spaceshipPosition.X += 3.5f; // 5px per frame
                _spaceshipRotate = MathHelper.ToRadians(90);
            }
        }
        if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left))
        {
            if (_spaceshipPosition.X - halfW > 0)
            {
                _spaceshipPosition.X -= 3.5f; // 5px per frame
                _spaceshipRotate = MathHelper.ToRadians(270);
            }
        }
        //Stick movement --------------------------------------------------------------------------------------

        // Get the gamepad state
        Vector2 leftStick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
        Vector2 rightStick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
        float rightTrigger = GamePad.GetState(PlayerIndex.One).Triggers.Right;

        // Invert the Y axis so that pushing up gives a positive value
        leftStick.Y *= -1;
        rightStick.Y *= -1;

        // Check if the stick movement is significant enough (to avoid jitter)
        if (leftStick.LengthSquared() > 0.1f)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
            {
                _spaceshipPosition += leftStick * 7.0f; //Speed boost
            }
            else
            {
                _spaceshipPosition += leftStick * 3.5f; //Regular traveling
            }

            // Clamp position to screen bounds
            _spaceshipPosition.X = MathHelper.Clamp(_spaceshipPosition.X, halfW, Window.ClientBounds.Width - halfW);
            _spaceshipPosition.Y = MathHelper.Clamp(_spaceshipPosition.Y, halfH, Window.ClientBounds.Height - halfH);
        }
        if (rightStick.LengthSquared() > 0.1f)
        {
            _spaceshipRotate = (float)Math.Atan2(rightStick.Y, rightStick.X) + MathHelper.PiOver2;
            _laserBlastRedRotate = _spaceshipRotate;

        }




        // Keyboard Boost
        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (_spaceshipPosition.Y - halfH > 0) { _spaceshipPosition.Y -= 5f; }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (_spaceshipPosition.Y + halfH < Window.ClientBounds.Height) { _spaceshipPosition.Y += 5f; }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (_spaceshipPosition.X + halfW < Window.ClientBounds.Width) { _spaceshipPosition.X += 5f; }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (_spaceshipPosition.X - halfW > 0) { _spaceshipPosition.X -= 5f; }
            }
        }

        _laserBlastRed = Content.Load<Texture2D>("images/laserBlastRed");
        _laserBlastRedPostition = new Vector2(_spaceshipPosition.X * 0.25f, _spaceshipPosition.Y * 0.32f);



        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        _spriteBatch.Begin();
        // Draw the spaceship texture
        _spriteBatch.Draw(_spaceship, _spaceshipPosition, null, Color.White, _spaceshipRotate, new Vector2(_spaceship.Width * 0.5f, _spaceship.Height * 0.5f), _spaceshipScale, SpriteEffects.None, 0.0f);
        // Always end the sprite batch when finished.
        _spriteBatch.End();

        if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
        {
            Debug.WriteLine("X button pressed");
            _spriteBatch.Begin();
            _spriteBatch.Draw(_laserBlastRed, _laserBlastRedPostition, null, Color.White, _laserBlastRedRotate, new Vector2(_laserBlastRed.Height * 1.0f, _laserBlastRed.Width * 0.5f), _laserBlastRedScale, SpriteEffects.None, 0.0f);
            _spriteBatch.End();
        }
        base.Draw(gameTime);
    }
}
