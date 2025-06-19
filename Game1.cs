using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Security.Principal;
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
    private int _playerHealth = 5;
    private bool playerIsAlive = true;
    private float _hitIndicatorTimer = 0.0f;

    //Player lasers
    private Texture2D _laserBlastRed;
    private float _laserBlastRedRotate;
    private const float _laserBlastRedScale = 1.0f;
    private List<Laser> _activeLasers = new();
    private float _timeSinceLastCollision = 0.0f;

    //Enemy 
    private Texture2D _enemySpaceship;
    private List<Enemy> _enemies = new List<Enemy>();
    Random rng = new Random();
    private Vector2 spawnPos;


    //Game 
    private float _masterGameTime = 0.0f;
    private int _levelCounter;
    private bool _gamePause = false;
    private ButtonState _previousStartButtonState = ButtonState.Released;
    private SpriteFont _pauseText;




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
        _pauseText = Content.Load<SpriteFont>("font/PauseText");

    }

    protected override void Update(GameTime gameTime)
    {
        var currentGameState = GamePad.GetState(PlayerIndex.One);
        ButtonState currentStartState = currentGameState.Buttons.Start;
        if (currentStartState == ButtonState.Pressed && _previousStartButtonState == ButtonState.Released)
        {
            _gamePause = !_gamePause;
        }
        _previousStartButtonState = currentStartState;


        if (!_gamePause)
        {
            _masterGameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Movement ---------------------------------------------------------------------------------------------
            float halfW = _spaceship.Width * _spaceshipScale * 0.5f;
            float halfH = _spaceship.Height * _spaceshipScale * 0.5f;

            //Stick movement --------------------------------------------------------------------------------------

            if (playerIsAlive == true)
            {
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

                // Check against all enemies if an enemy got hit
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
                enemy.Update(_spaceshipPosition, gameTime);
            }


            //Removes all enemies from the list, then adds a new wave of enemies
            _enemies.RemoveAll(e => !e.IsAlive);

            if (_enemies.Count == 0)
            {
                for (int i = 0; i <= _levelCounter; i++)
                {
                    //Random spawning
                    do
                    {
                        int spawn_x = rng.Next(Window.ClientBounds.Width * -1, Window.ClientBounds.Width * 2);
                        int spawn_y = rng.Next(Window.ClientBounds.Height * -1, Window.ClientBounds.Height * 2);
                        spawnPos = new Vector2(spawn_x, spawn_y);
                    } while (spawnPos.X >= 0 && spawnPos.X <= Window.ClientBounds.Width && spawnPos.Y >= 0 && spawnPos.Y <= Window.ClientBounds.Height);
                    _enemies.Add(new Enemy(_enemySpaceship, spawnPos, _laserBlastRed));


                }
                _levelCounter++;
            }

            //Player collison check
            foreach (var enemy in _enemies)
            {
                float collisionRadius = 65f;
                _timeSinceLastCollision += (float)gameTime.ElapsedGameTime.TotalSeconds; //Move this outside the foreach if i want to ignore pileup
                _hitIndicatorTimer += (float)gameTime.ElapsedGameTime.TotalSeconds; //Counts time fore hit indicator
                if (enemy.IsAlive && Vector2.Distance(enemy.Position, _spaceshipPosition) < collisionRadius && _timeSinceLastCollision > 1.0f)
                {
                    _playerHealth--;
                    _timeSinceLastCollision = 0f;
                    _hitIndicatorTimer = 0f;

                }
                for (int i = enemy.EnemyLasers.Count - 1; i >= 0; i--)
                {
                    Laser laser = enemy.EnemyLasers[i];
                    float hitRadius = 40f;
                    if (Vector2.Distance(laser.Position, _spaceshipPosition) < hitRadius)
                    {
                        _playerHealth--;
                        enemy.EnemyLasers.RemoveAt(i);
                        _hitIndicatorTimer = 0f;
                    }
                }

            }
            if (_playerHealth == 0)
            {
                playerIsAlive = false;
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (!_gamePause)
        {
            float currentTriggerValue = GamePad.GetState(PlayerIndex.One).Triggers.Right;
            if (currentTriggerValue > 0.2f && _lastTriggerValue <= 0.2f)
            {
                Vector2 offset = new Vector2((float)Math.Sin(_spaceshipRotate), -(float)Math.Cos(_spaceshipRotate)) * 5f;
                _activeLasers.Add(new Laser(_spaceshipPosition + offset, _spaceshipRotate));
            }

            _lastTriggerValue = currentTriggerValue;
        }

        GraphicsDevice.Clear(new Color(70, 70, 70));

        // Begin the sprite batch to prepare for rendering.
        _spriteBatch.Begin();
        Color shipColor = _hitIndicatorTimer < 0.2f ? Color.Red : Color.White;
        if (playerIsAlive == true)
        {
            _spriteBatch.Draw(_spaceship, _spaceshipPosition, null, shipColor, _spaceshipRotate, new Vector2(_spaceship.Width * 0.5f, _spaceship.Height * 0.5f), _spaceshipScale, SpriteEffects.None, 0.0f);
        }

        foreach (Laser laser in _activeLasers)
        {
            _spriteBatch.Draw(
                _laserBlastRed,
                laser.Position,
                null,
                Color.LightBlue,
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
                Color.LightBlue,
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

        //Pause screen
        if (_gamePause)
        {
            string pauseText = "PAUSED";
            Vector2 textSize = _pauseText.MeasureString(pauseText);
            Vector2 screenCenter = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height) * 0.5f;
            Vector2 textPosition = screenCenter - textSize * 0.5f;

            _spriteBatch.DrawString(_pauseText, pauseText, textPosition, Color.White);
        }
        string _playerHealthText = _playerHealth.ToString();
        Vector2 _playerHealthtextSize = _pauseText.MeasureString(_playerHealthText);
        Vector2 screenTopRight = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height) * 0.0f;
        Vector2 _playerHeathTextPostion = screenTopRight;
        _spriteBatch.DrawString(_pauseText,_playerHealthText, _playerHeathTextPostion, Color.White);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}

