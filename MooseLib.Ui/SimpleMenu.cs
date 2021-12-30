using Merthsoft.Moose.MooseEngine.Ui.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui
{
    public class SimpleMenu : Window
    {
        public Action<SimpleMenu, string>? Clicked { get; set; }
        public TextList MainList { get; }

        public SimpleMenu(GraphicsDevice graphicsDevice, Theme theme, params string[] options) : base(graphicsDevice, new(0, 0, 0, 0), theme)
        {
            MainList = AddActionList(4, 8, 1, MainMenu_Clicked, options);
            Size = MainList.CalculateSize() + new Vector2(Theme.TileWidth * 2, Theme.TileHeight);
        }

        private void MainMenu_Clicked(Control owner, UpdateParameters @params)
        {
            if (owner is not TextList textList || textList.MouseOverOption == null)
                return;

            Clicked?.Invoke(this, textList.MouseOverOption.Text);
        }
    }
}
