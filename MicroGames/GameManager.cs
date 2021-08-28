using Merthsoft.MooseEngine;
using Merthsoft.MooseEngine.Ui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus;

namespace Merthsoft.MicroGames
{
    public class GameManager : MooseGame
    {
        private readonly List<MooseGame> Games = new();
        private WindowManager WindowManager = null!;

        public GameManager() { }

        private static readonly CharacterRange[] DefaultCharacterRange = new[]
        {
            CharacterRange.BasicLatin,
            CharacterRange.Latin1Supplement,
            CharacterRange.LatinExtendedA,
            CharacterRange.Cyrillic
        };

        private static readonly int DefaultBitmapSize = 1024;

        private SpriteFont BakeFont(string font, int fontPixelHeight)
            => TtfFontBaker.Bake(File.ReadAllBytes($"Content/Fonts/{font}.ttf"), fontPixelHeight, 
                DefaultBitmapSize, DefaultBitmapSize, DefaultCharacterRange
               ).CreateSpriteFont(GraphicsDevice);

        protected override StartupParameters Startup()
            => new()
            {
                ScreenHeight = 768,
                ScreenWidth = 1024,
            };

        protected override void Load()
        {
            var fonts = new[]
            {
                BakeFont("Outward_Bound", 78),
                BakeFont("Tomorrow_Night", 78),
            };

            var windowTextures = new[] {
                Content.Load<Texture2D>("Images/wooden_window"),
                Content.Load<Texture2D>("Images/fancy_window"),
            };

            var themes = new Theme[] {
                new("default", windowTextures[0], 16, 16, fonts) 
                { 
                    ControlDrawOffset = new(5, 5), 
                    TextColor = Color.White, 
                    TextMouseOverColor = Color.Maroon,
                    TileScale = new(7, 7),
                },
                new("fancy", windowTextures[1], 16, 16, fonts) 
                { 
                    ControlDrawOffset = new(7, 7), 
                    TextColor = Color.White, 
                    TextMouseOverColor = Color.Maroon,
                    TileScale = new(7, 7),
                },
            };

            WindowManager = new WindowManager(themes);
            var titleWindow = WindowManager.NewWindow(0, 0, 384, 128, "fancy");
            titleWindow.AddLabel(21, 20, "Games!", 1);
            titleWindow.Center(ScreenWidth, titleWindow.Height);

            DefaultBackgroundColor = new(36, 34, 52);
        }

        protected override void PostUpdate(GameTime gameTime)
        {
            WindowManager.Update(gameTime, CurrentMouseState);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            WindowManager.Draw(SpriteBatch);
            SpriteBatch.End();
        }
    }
}
