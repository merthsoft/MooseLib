using Merthsoft.Moose.MooseEngine.Ui;
using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Reflection;

namespace Merthsoft.Moose.SnowballFight
{
    class MainMenu : SimpleMenu
    {
        private readonly Label logo;
        private readonly Label versionLabel;
        private readonly int screenSize;

        public MainMenu(GraphicsDevice graphicsDevice, Theme theme, int screenSize)
            : base(graphicsDevice, theme, "New Game", "Settings", "About", "Exit")
        {
            MainList.Options[1].Enabled = false;
            MainList.Options[2].Enabled = false;

            this.screenSize = screenSize;

            logo = this.AddLabel(0, 0, "Snowfight Tactics", 0, strokeSize: 3);

            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fileVersionInfo.ProductVersion!.Split('-');

            versionLabel = this.AddLabel(0, 0, $"v{version[0]}{version[1][0]} - {fileVersionInfo.LegalCopyright}", 2, Color.Black, 1, Color.White);

            Center(screenSize, screenSize);
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (IsHidden)
                return;

            var (logoWidth, logoHeight) = logo.CalculateSize();
            logo.Position = new(-logoWidth / 4, -logoHeight - 8);
            versionLabel.Position = new(-X, -Y + screenSize - 24);

            base.Update(updateParameters);
        }
    }
}
