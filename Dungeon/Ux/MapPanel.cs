using Merthsoft.Moose.Dungeon.Entities;
using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Ux;
public class MapPanel : Panel
{
    public Vector2 ZoomPosition = new(320, 30);
    public Vector2 NormalPosition = new(0, 475);

    public float Scale = 1f;

    private readonly Picture mapPicture;
    private readonly Picture mapCornerPicture;

    public MapPanel(IControlContainer container, float x, float y, float w, float h) : base(container, x, y, w, h)
    {
        BackgroundDrawingMode = BackgroundDrawingMode.None;
        mapPicture = this.AddPicture(0, 0, DungeonGame.Instance.MapTexture);
        mapCornerPicture = this.AddPicture(0, 0, DungeonGame.Instance.MapCornerTexture);

        NormalPosition = Position;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset, GameTime gameTime)
    {
        mapPicture.Scale = new(Scale, Scale);
        mapCornerPicture.Scale = new(Scale, Scale);
        base.Draw(spriteBatch, parentOffset, gameTime);

        var cornerPosition = Position + parentOffset;
        var position = cornerPosition + new Vector2(10*Scale, 10*Scale);
        position += Theme.DrawWindow(spriteBatch, Rectangle.Move(parentOffset), BackgroundDrawingMode, ColorShift);

        var map = DungeonGame.Instance.DungeonMap;
        for (var i = 0; i < map.Width; i++)
            for (var j = 0; j < map.Height; j++)
            {
                var pos = new Vector2(position.X + i * 8 * Scale, position.Y + j * 8 * Scale) ;

                var miniMapTile = DungeonPlayer.Instance.GetMiniMapTile(i, j);
                if (miniMapTile != MiniMapTile.None)
                    spriteBatch.Draw(DungeonGame.Instance.MiniMapTiles, pos,
                        DungeonGame.Instance.MiniMapTiles.GetSourceRectangle((int)miniMapTile, 8, 8), Color.White,
                        0, Vector2.Zero, Scale, SpriteEffects.None, 1);
            }

        mapCornerPicture.Draw(spriteBatch, cornerPosition, gameTime);
    }

    public override void Update(UpdateParameters updateParameters) {
        var size = 288 * Scale;
        Size = new(size, size);
        base.Update(updateParameters);
    }
}
