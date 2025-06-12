using System;
using Microsoft.Xna.Framework;

namespace MazeHunter
{
    public class Laser
    {
        public Vector2 Position;
        public float Rotation;
        public float Speed = 10f;
        public Laser(Vector2 startPos, float rotation)
        {
            Position = startPos;
            Rotation = rotation;
        }
        public void Update()
        {
            Vector2 direction = new((float)Math.Sin(Rotation), -(float)Math.Cos(Rotation));
            Position += direction * Speed;
        }

    }

}
