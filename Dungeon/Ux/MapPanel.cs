using Merthsoft.Moose.Dungeon.Entities;

namespace Merthsoft.Moose.Dungeon.Ux;
public class MapPanel : Panel
{
    public MapPanel(IControlContainer container, float x, float y, float w, float h) : base(container, x, y, w, h)
    {
        BackgroundDrawingMode = BackgroundDrawingMode.Texture;
        this.AddLabel(0, -154, "Map");
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
    {
        base.Draw(spriteBatch, parentOffset);

        var position = Position + parentOffset;
        position += Theme.DrawWindow(spriteBatch, Rectangle.Move(parentOffset), BackgroundDrawingMode, ColorShift);

        var map = DungeonGame.Instance.DungeonMap;
        for (var i = 0; i < map.Width; i++)
            for (var j = 0; j < map.Height; j++)
            {
                var pos = new Vector2(position.X + i * 8, position.Y + j * 8);

                var miniMapTile = DungeonPlayer.Instance.GetMiniMapTile(i, j);
                if (miniMapTile != MiniMapTile.None)
                    spriteBatch.Draw(DungeonGame.Instance.MiniMapTiles, pos,
                        DungeonGame.Instance.MiniMapTiles.GetSourceRectangle((int)miniMapTile, 8, 8), Color.White);
            }
    }
}
