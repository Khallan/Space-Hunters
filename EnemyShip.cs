using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeHunter
{
    public class Enemy
    {
        public Vector2 Position;
        public float Rotation;
        public float Speed = 2f;
        private Texture2D _texture;
        private float _scale = 0.3f;
        public bool IsAlive = true;
        private int _health = 3;
        private Texture2D _laserBlastGreen;
        private float _laserBlastGreenRotate;
        private const float _laserBlastGreenScale = 1.0f;
        private List<Laser> _enemyLasers = new();
        private float _shootCooldown = 1.5f; // seconds between shots
        private float _timeSinceLastShot = 0f;

        public Enemy(Texture2D texture, Vector2 startPos, Texture2D laserBlastGreen)
        {
            _texture = texture;
            Position = startPos;
            _laserBlastGreen = laserBlastGreen;
        }

        public void Update(Vector2 target, GameTime gameTime)
        {
            // Simple AI: move toward player
            Vector2 direction = target - Position;
            if (direction.Length() > 10f)
            {
                direction.Normalize();
                Position += direction * Speed;
                Rotation = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;
            }
            //Track time
            _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Determines if the target is in range and last time it shot
            if (Vector2.Distance(Position, target) < 1200f && _timeSinceLastShot > 1.5f)
            {
                ShootAt(target);
            }

            for (int i = _enemyLasers.Count - 1; i >= 0; i--)
            {
                _enemyLasers[i].Update();

                // Remove laser if off-screen (optional, prevents infinite lasers)
                if (_enemyLasers[i].Position.X < 0 || _enemyLasers[i].Position.X > 1400 ||  // assuming screen width 1400
                    _enemyLasers[i].Position.Y < 0 || _enemyLasers[i].Position.Y > 900)     // assuming screen height 900
                {
                    _enemyLasers.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                spriteBatch.Draw(_texture, Position, null, Color.White, Rotation, new Vector2(_texture.Width * 0.5f, _texture.Height * 0.5f), _scale, SpriteEffects.None, 0f);
            }
            foreach (var laser in _enemyLasers)
            {
                spriteBatch.Draw(
                    _laserBlastGreen,           // Your laser texture for enemy lasers
                    laser.Position,
                    null,
                    Color.Red,
                    laser.Rotation,
                    new Vector2(_laserBlastGreen.Width * 0.5f, _laserBlastGreen.Height * 1.3f),  // Center the laser
                    _laserBlastGreenScale,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        public void TakeDamage()
        {
            _health--;
            if (_health <= 0) { IsAlive = false; }

        }

        public bool CheckCollision(Vector2 laserPos, float hitRadius = 60f)
        {
            return IsAlive && Vector2.Distance(Position, laserPos) < hitRadius;
        }

        public void ShootAt(Vector2 target)
        {
            Vector2 direction = target - Position;
            direction.Normalize();

            float rotation = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;
            Laser newLaser = new Laser(Position, rotation);
            _enemyLasers.Add(newLaser);
            _timeSinceLastShot = 0f;
        }
    }
}
