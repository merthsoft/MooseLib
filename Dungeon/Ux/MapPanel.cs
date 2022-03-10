namespace Merthsoft.Moose.Dungeon.Ux;
public class MapPanel : Panel
{
    DungeonGame game;

    public MapPanel(DungeonGame game, IControlContainer container, float x, float y, float w, float h) : base(container, x, y, w, h)
    {
        this.game = game;
        BackgroundDrawingMode = BackgroundDrawingMode.Texture;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
    {
        base.Draw(spriteBatch, parentOffset);
        var player = game.Player;
        var (playerX, playerY) = player.GetCell();
        var map = game.DungeonMap;
        for (var i = 0; i < map.Width; i++)
            for (var j = 0; j < map.Height; j++)
            {
                var position = new Vector2(Position.X + parentOffset.X + i * 8, Position.Y + parentOffset.Y + j * 8);

                var miniMapTile = i == playerX && j == playerY ? MiniMapTile.Player : game.Player.MiniMap[i, j];
                if (miniMapTile != MiniMapTile.None)
                    spriteBatch.Draw(game.MiniMapTiles, position,
                        game.MiniMapTiles.GetSourceRectangle((int)miniMapTile, 8, 8), Color.White);
            }
    }
}
