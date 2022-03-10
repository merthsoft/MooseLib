namespace Merthsoft.Moose.Dungeon.Ux;
public class MapWindow : Window
{
    DungeonGame game;

    public MapWindow(DungeonGame game, Theme theme, float x, float y) : base(theme, x, y, 288, 333)
    {
        this.game = game;
        BackgroundDrawingMode = BackgroundDrawingMode.None;
        this.AddLabel(0, -75, "Map");
        AddControl(new MapPanel(game, this, 0, 45, 288, 288));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}
