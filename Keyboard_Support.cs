// /eyboard movement ------------------------------------------------------------------------------------------
// if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up))
// {
//     if (_spaceshipPosition.Y - halfH > 0)
//     {
//         _spaceshipPosition.Y -= 3.5f; // 5px per frame
//         _spaceshipRotate = MathHelper.ToRadians(0);
//     }
// }
// if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down))
// {
//     if (_spaceshipPosition.Y + halfH < Window.ClientBounds.Height)
//     {
//         _spaceshipPosition.Y += 3.5f; // 5px per frame
//         _spaceshipRotate = MathHelper.ToRadians(180);

//     }
// }
// if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
// {
//     if (_spaceshipPosition.X + halfW < Window.ClientBounds.Width)
//     {
//         _spaceshipPosition.X += 3.5f; // 5px per frame
//         _spaceshipRotate = MathHelper.ToRadians(90);
//     }
// }
// if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left))
// {
//     if (_spaceshipPosition.X - halfW > 0)
//     {
//         _spaceshipPosition.X -= 3.5f; // 5px per frame
//         _spaceshipRotate = MathHelper.ToRadians(270);
//     }
// }
// Boost ------------------------------------------------------------------------------------------------------------------------
//  Keyboard Boost
//         if (Keyboard.GetState().IsKeyDown(Keys.Space))
//         {
//             if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.Space))
//             {
//                 if (_spaceshipPosition.Y - halfH > 0) { _spaceshipPosition.Y -= 5f; }
//             }
//             else if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.Space))
//             {
//                 if (_spaceshipPosition.Y + halfH < Window.ClientBounds.Height) { _spaceshipPosition.Y += 5f; }
//             }
//             else if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.Space))
//             {
//                 if (_spaceshipPosition.X + halfW < Window.ClientBounds.Width) { _spaceshipPosition.X += 5f; }
//             }
//             else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.Space))
//             {
//                 if (_spaceshipPosition.X - halfW > 0) { _spaceshipPosition.X -= 5f; }
//             }
//         }
