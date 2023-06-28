using Merthsoft.Moose.MooseEngine.Interface;

namespace Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;

public class SpriteBatchCharacterRenderer : SpriteBatchRenderer
{
    public record CharacterDefinition(
        char Character, 
        Color ForegroundColor, 
        Color? BackgroundColor = null,
        Color? ShadowColor = null
    );

    public List<CharacterDefinition> Palette { get; } = new();
    public SpriteFont Font { get; set; }

    public SpriteBatchCharacterRenderer(SpriteBatch spriteBatch, SpriteFont font, params CharacterDefinition[] characters) : base(spriteBatch)
    {
        Palette.AddRange(characters);
        Font = font;
    }

    public override void Draw(MooseGame game, GameTime gameTime, ILayer layer, Vector2 drawOffset)
    {
        if (layer is TileLayer<int> tileLayer)
            DrawTileLayer(drawOffset, tileLayer);
        else
            throw new Exception("TileLayer<int> or IObjectLayer layer expected");
    }

    private void DrawTileLayer(Vector2 drawOffset, TileLayer<int> tileLayer)
    {
        var (tileWidth, tileHeight) = Font.MeasureString("M");

        for (int i = 0; i < tileLayer.Width; i++)
            for (int j = 0; j < tileLayer.Height; j++)
            {
                var character = Palette[tileLayer.Tiles[i, j]];
                DrawCharacter(drawOffset, tileWidth, tileHeight, i, j, character);
            }
    }

    private void DrawCharacter(Vector2 drawOffset, float tileWidth, float tileHeight, int i, int j, CharacterDefinition character)
    {
        var x = i * tileWidth + drawOffset.X;
        var y = j * tileHeight + drawOffset.Y;

        if (character.BackgroundColor != null)
            SpriteBatch.FillRectangle(x, y, tileWidth, tileHeight, character.BackgroundColor.Value, 0);

        if (character.ShadowColor != null)
            SpriteBatch.DrawString(Font, character.Character.ToString(), new(x - 1, y - 1), character.ShadowColor.Value,
                0, Vector2.Zero, 1f, SpriteEffects.None, .5f);

        SpriteBatch.DrawString(Font, character.Character.ToString(), new(x, y), character.ForegroundColor,
            0, Vector2.Zero, 1f, SpriteEffects.None, 1);
    }
}
