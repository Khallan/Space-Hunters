using System;
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

        public Enemy(Texture2D texture, Vector2 startPos)
        {
            _texture = texture;
            Position = startPos;
        }

        public void Update(Vector2 target)
        {
            // Simple AI: move toward player
            Vector2 direction = target - Position;
            if (direction.Length() > 10f)
            {
                direction.Normalize();
                Position += direction * Speed;
                Rotation = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                spriteBatch.Draw(_texture, Position, null, Color.White, Rotation,
                    new Vector2(_texture.Width * 0.5f, _texture.Height * 0.5f),
                    _scale, SpriteEffects.None, 0f);
            }
        }

        public void TakeDamage()
        {
            _health--;
            if (_health <= 0)
                IsAlive = false;
        }

        public bool CheckCollision(Vector2 laserPos, float hitRadius = 60f)
        {
            return IsAlive && Vector2.Distance(Position, laserPos) < hitRadius;
        }
    }
}
