using Merthsoft.Moose.MooseEngine.Ui;
using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using MonoGame;
using System.Diagnostics;
using System.Reflection;

namespace Merthsoft.Moose.SnowballFight
{
    class MainMenu : SimpleMenu
    {
        private readonly Picture logo;
        private readonly Label versionLabel;
        private readonly int logoHeight;
        private readonly int logoWidth;
        private readonly int screenSize;

        public MainMenu(Theme theme, Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice, int screenSize)
            : base(theme, "New Game", "Settings", "About", "Exit")
        {
            MainList.Options[1].Enabled = false;
            MainList.Options[2].Enabled = false;

            this.screenSize = screenSize;

            var logoText = "Snowfight Tactics";
            var logoTexture = StrokeEffect.CreateStrokeSpriteFont(theme.Fonts[0], logoText, Color.Yellow, Vector2.One, 3, Color.Black, graphicsDevice, StrokeType.OutlineAndTexture);
            (logoWidth, logoHeight) = (logoTexture.Width, logoTexture.Height);

            logo = AddPicture(0, 0, logoTexture);

            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fileVersionInfo.ProductVersion!.Split('-');

            versionLabel = AddLabel(0, 0, $"v{version[0]}{version[1][0]} - {fileVersionInfo.LegalCopyright}", 2, Color.Black);

            RectangleChanged = MainMenu_RectangleChanged;
        }

        private void MainMenu_RectangleChanged(Window? sender, ValueChangedParameters<Rectangle> e)
        {
            logo.Position = new(-logoWidth / 4, -logoHeight - 8);
            versionLabel.Position = new(-X, -Y + screenSize - 24);
        }
    }
}
