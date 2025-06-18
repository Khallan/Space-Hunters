using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeHunter;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    //Player ship
    private Texture2D _spaceship;
    private Vector2 _spaceshipPosition;
    private float _spaceshipRotate;
    private const float _spaceshipScale = 0.3f;

    //Player lazers
    private Texture2D _laserBlastRed;
    private float _laserBlastRedRotate;
    private const float _laserBlastRedScale = 1.0f;
    private List<Laser> _activeLasers = new();

    //Enemy 
    private Texture2D _enemySpaceship;
    private List<Enemy> _enemies = new List<Enemy>();
    private bool _enemyIsAlive = true;
    private int enemyHealth = 0;

    //Game 
    private int _levelCounter = 0;


    //Controller values
    private float _lastTriggerValue = 0f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 1400;
        _graphics.PreferredBackBufferHeight = 900;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _spaceship = Content.Load<Texture2D>("images/spaceship");
        _laserBlastRed = Content.Load<Texture2D>("images/laserBlastRed");
        _enemySpaceship = Content.Load<Texture2D>("images/spaceship_type2");
        _spaceshipPosition = new Vector2((float)(Window.ClientBounds.Width * 0.0f + _spaceship.Width * 0.5 * 0.3), Window.ClientBounds.Height * 0.5f);
        _enemies.Add(new Enemy(_enemySpaceship, new Vector2((float)(Window.ClientBounds.Width * 1.0f - _spaceship.Width * 0.5 * 0.3), Window.ClientBounds.Height * 0.5f)));
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


        //Stick movement --------------------------------------------------------------------------------------

        // Get the gamepad state
        Vector2 leftStick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
        Vector2 rightStick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;

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

        Vector2 laserOffset = new Vector2((float)Math.Sin(_spaceshipRotate), -(float)Math.Cos(_spaceshipRotate));
        _laserBlastRedRotate = _spaceshipRotate;

        for (int i = _activeLasers.Count - 1; i >= 0; i--)
        {
            _activeLasers[i].Update();

            if (_activeLasers[i].Position.X < 0 || _activeLasers[i].Position.X > Window.ClientBounds.Width ||
                _activeLasers[i].Position.Y < 0 || _activeLasers[i].Position.Y > Window.ClientBounds.Height)
            {
                _activeLasers.RemoveAt(i);
                continue;
            }

            // Check against all enemies
            foreach (var enemy in _enemies)
            {
                if (enemy.CheckCollision(_activeLasers[i].Position))
                {
                    enemy.TakeDamage();
                    _activeLasers.RemoveAt(i);
                    break;
                }
            }
        }

        // Update enemies
        foreach (var enemy in _enemies)
        {
            enemy.Update(_spaceshipPosition);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        float currentTriggerValue = GamePad.GetState(PlayerIndex.One).Triggers.Right;
        if (currentTriggerValue > 0.2f && _lastTriggerValue <= 0.2f)
        {
            Vector2 offset = new Vector2((float)Math.Sin(_spaceshipRotate), -(float)Math.Cos(_spaceshipRotate)) * 5f;
            _activeLasers.Add(new Laser(_spaceshipPosition + offset, _spaceshipRotate));
        }

        _lastTriggerValue = currentTriggerValue;

        GraphicsDevice.Clear(new Color(70, 70, 70));

        // Begin the sprite batch to prepare for rendering.
        _spriteBatch.Begin();
        // Draw the spaceship texture
        _spriteBatch.Draw(_spaceship, _spaceshipPosition, null, Color.White, _spaceshipRotate, new Vector2(_spaceship.Width * 0.5f, _spaceship.Height * 0.5f), _spaceshipScale, SpriteEffects.None, 0.0f);

        foreach (Laser laser in _activeLasers)
        {
            _spriteBatch.Draw(
                _laserBlastRed,
                laser.Position,
                null,
                Color.White,
                laser.Rotation,
                new Vector2(_laserBlastRed.Width * 4.0f, _laserBlastRed.Height * 0.6f),
                _laserBlastRedScale,
                SpriteEffects.None,
                0.0f
            );
            _spriteBatch.Draw(
                _laserBlastRed,
                laser.Position,
                null,
                Color.White,
                laser.Rotation,
                new Vector2(_laserBlastRed.Width * -3.05f, _laserBlastRed.Height * 0.6f),
                _laserBlastRedScale,
                SpriteEffects.None,
                0.0f
            );
        }

        //Enemy Spawning
        foreach (var enemy in _enemies)
        {
            enemy.Draw(_spriteBatch);
        }
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}

