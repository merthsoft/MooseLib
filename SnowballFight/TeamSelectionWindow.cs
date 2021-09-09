using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.Ui;
using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.SnowballFight
{
    class TeamSelectionWindow : Window
    {
        private readonly Label headerLabel;
        private readonly Window santaWindow;
        private readonly Window krampusWindow;
        private readonly int screenSize;

        public Action<TeamSelectionWindow, Team>? TeamSelected { get; set; }

        public TeamSelectionWindow(Theme theme, int screenSize, Texture2D santaPicture, Texture2D krampusPicture) : base(new(0, 0, screenSize, screenSize), theme)
        {
            DrawBackground = false;
            this.screenSize = screenSize;

            headerLabel = AddLabel(144, 140, "Choose your team!", strokeSize: 3, forceHighlight: true);
            santaWindow = new(159, 215, 300, 300, theme);
            krampusWindow = new(501, 215, 300, 300, theme);

            santaWindow.AddPicture(5, 5, santaPicture, 14);
            santaWindow.AddLabel(1, 227, "Santa", 1);

            krampusWindow.AddPicture(5, 5, krampusPicture, 14);
            krampusWindow.AddLabel(1, 227, "Krampus", 1);
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (!Visible)
                return;

            var (logoWidth, logoHeight) = headerLabel.CalculateSize();
            headerLabel.Position = new(screenSize/2 - logoWidth / 2, logoHeight * 2);

            santaWindow.Position = headerLabel.Position + new Vector2(15, 75);
            krampusWindow.Position = headerLabel.Position + new Vector2(logoWidth - 315, 75);

            Team? team = null;

            if (HighlightIfHovering(updateParameters, santaWindow))
                team = Team.Santa;

            if (HighlightIfHovering(updateParameters, krampusWindow))
                team = Team.Krampus;

            if (team.HasValue)
                TeamSelected?.Invoke(this, team.Value);

            base.Update(updateParameters);
        }

        private bool HighlightIfHovering(UpdateParameters updateParameters, Window window)
        {
            var intersects = window.Rectangle.Intersects(updateParameters.MouseRectangle);
            window.Controls.OfType<Label>().ForEach(l => l.ForceHighlight = intersects);
            return intersects && updateParameters.LeftMouse;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            santaWindow.Draw(spriteBatch);
            krampusWindow.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }
    }
}
