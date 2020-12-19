using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;

namespace MooseLib.Ui
{
    public class WindowManager
    {
        public SpriteFont Font { get; set; }
        public Texture2D WindowTexture { get; }
        public List<Window> Windows { get; } = new();

        internal Vector2 ControlDrawOffset { get; }

        internal Rectangle[] TextureRects = new Rectangle[9];
        internal int TileWidth { get; }
        internal int TileHeight { get; }

        internal MouseState CurrentMouseState { get; set; }
        internal MouseState PreviousMouseState { get; set; }

        public WindowManager(Texture2D windowTexture, SpriteFont font, Vector2 controlDrawOffset)
        {
            WindowTexture = windowTexture;
            Font = font;
            ControlDrawOffset = controlDrawOffset;

            TileWidth = WindowTexture.Width / 3;
            TileHeight = WindowTexture.Height / 3;
            for (var index = 0; index < 9; index++)
                TextureRects[index] = new Rectangle(index % 3 * TileWidth, index / 3 * TileHeight, TileWidth, TileHeight);
        }

        public void Update(GameTime gameTime, OrthographicCamera camera)
        {
            Windows.RemoveAll(w => w.Close);
            CurrentMouseState = Mouse.GetState();
            foreach (var w in Windows)
            {
                var worldClick = camera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);

                var updateParams = new UpdateParameters(gameTime, worldClick - w.Position - ControlDrawOffset);

                if (w.Rectangle.Contains(worldClick))
                {
                    updateParams.MouseOver = true;
                    updateParams.LeftMouse = CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;
                    updateParams.RightMouse = CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released;
                }
                w.Update(updateParams);
            }
            PreviousMouseState = CurrentMouseState;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
            => Windows.ForEach(w => w.Draw(gameTime, spriteBatch));

        public Window NewWindow(int x, int y, int width, int height)
        {
            var ret = new Window(this, x, y, width, height);
            Windows.Add(ret);
            return ret;
        }

        internal void DrawWindowTexture(SpriteBatch spriteBatch, int index, Vector2 position, int x, int y)
        {
            var sourceRect = TextureRects[index];
            var destRect = new Rectangle((int)position.X + x * TileWidth, (int)position.Y + y * TileHeight, TileWidth, TileHeight);
            spriteBatch.Draw(WindowTexture, destRect, sourceRect, Color.White);
        }
    }
}
