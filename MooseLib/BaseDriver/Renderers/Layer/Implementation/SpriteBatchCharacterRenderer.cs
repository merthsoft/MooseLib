using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers.Layer.Implementation;

public class SpriteBatchCharacterRenderer : SpriteLayerBatchRenderer
{
    public record CharacterDefinition(
        char Character,
        Color ForegroundColor,
        Color? BackgroundColor = null,
        Color? ShadowColor = null
    );

    public List<CharacterDefinition> Palette { get; } = [];
    public SpriteFont Font { get; set; }

    public SpriteBatchCharacterRenderer(SpriteBatch spriteBatch, SpriteFont font, params CharacterDefinition[] characters) : base(spriteBatch)
    {
        Palette.AddRange(characters);
        Font = font;
    }

    public override void Draw(MooseGame game, GameTime gameTime, ILayer layer)
    {
        if (layer is TileLayer<int> tileLayer)
            DrawTileLayer(tileLayer);
        else
            throw new Exception("TileLayer<int> or IObjectLayer layer expected");
    }

    private void DrawTileLayer(TileLayer<int> tileLayer)
    {
        var (tileWidth, tileHeight) = Font.MeasureString("M");

        for (var i = 0; i < tileLayer.Width; i++)
            for (var j = 0; j < tileLayer.Height; j++)
            {
                var character = Palette[tileLayer.Tiles[i, j]];
                DrawCharacter(tileWidth, tileHeight, i, j, character);
            }
    }

    private void DrawCharacter(float tileWidth, float tileHeight, int i, int j, CharacterDefinition character)
    {
        var x = i * tileWidth + DrawOffset.X;
        var y = j * tileHeight + DrawOffset.Y;

        if (character.BackgroundColor != null)
            SpriteBatch.FillRectangle(x, y, tileWidth, tileHeight, character.BackgroundColor.Value, 0);

        if (character.ShadowColor != null)
            SpriteBatch.DrawString(Font, character.Character.ToString(), new(x - 1, y - 1), character.ShadowColor.Value,
                0, Vector2.Zero, 1f, SpriteEffects.None, .5f);

        SpriteBatch.DrawString(Font, character.Character.ToString(), new(x, y), character.ForegroundColor,
            0, Vector2.Zero, 1f, SpriteEffects.None, 1);
    }
}
