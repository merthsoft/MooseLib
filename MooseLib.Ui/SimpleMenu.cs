using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.MooseEngine.Ui
{
    public class SimpleMenu : Window
    {
        public Action<Window, string>? Clicked { get; set; }

        public SimpleMenu(Theme theme, params string[] options) : base(new(0, 0, 0, 0), theme)
        {
            var mainList = AddActionList(4, 8, 1, MainMenu_Clicked, options);
            Size = mainList.CalculateSize() + new Vector2(Theme.TileWidth * 2, Theme.TileHeight);
        }

        private void MainMenu_Clicked(Control owner, UpdateParameters @params)
        {
            if (owner is not TextList textList || textList.MouseOverOption == null)
                return;

            Clicked?.Invoke(this, textList.MouseOverOption.Text);
        }
    }
}
