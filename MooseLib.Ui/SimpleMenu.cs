using Merthsoft.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using System;

namespace Merthsoft.MooseEngine.Ui
{
    public class SimpleMenu : Window
    {
        public Action<string>? Clicked { get; set; }

        public SimpleMenu(Theme theme, params string[] options) : base(new(0, 0, 0, 0), theme)
        {
            var mainList = AddActionList(4, 8, 1, MainMenu_Clicked, options);
            Size = mainList.CalculateSize() + new Vector2(Theme.TileWidth * 2, Theme.TileHeight);
        }

        private void MainMenu_Clicked(Control owner, UpdateParameters @params)
        {
            if (owner is not TextList textList)
                return;

            Clicked?.Invoke(textList.Options[textList.MouseOverIndex].Text);
        }
    }
}
