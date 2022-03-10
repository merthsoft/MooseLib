using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.MooseEngine.GameObjects;

public class TextureGameObject : GameObjectBase
{
    public new TextureGameObjectDef Def => (base.Def as TextureGameObjectDef)!;

    public SpriteEffects SpriteEffects { get; set; }
    
    private Color color = Color.White;
    public Color Color { 
        get => color;
        set
        {
            color = value;
            colorHsl = color.ToHsl();
        }
    }
    
    private HslColor colorHsl;

    public float ColorH
    {
        get => colorHsl.H;
        set
        {
            var a = Color.A;
            colorHsl = new HslColor(value, colorHsl.S, colorHsl.L);
            color = colorHsl.ToRgb();
            color.A = a;
        }
    }

    public float ColorS
    {
        get => colorHsl.S;
        set
        {
            var a = Color.A;
            colorHsl = new HslColor(colorHsl.H, value, colorHsl.L);
            color = colorHsl.ToRgb();
            color.A = a;
        }
    }

    public float ColorL
    {
        get => colorHsl.L;
        set
        {
            var a = Color.A;
            colorHsl = new HslColor(colorHsl.H, colorHsl.S, value);
            color = colorHsl.ToRgb();
            color.A = a;
        }
    }

    public TextureGameObject(TextureGameObjectDef def, Vector2? position = null, string? direction = null, float? rotation = null, Vector2? size = null, string? layer = null) : base(def, position, direction, rotation, size, layer)
    {
    }

    public override void Update(MooseGame game, GameTime gameTime) { }

    public override void Draw(MooseGame game, GameTime gameTime, SpriteBatch spriteBatch)
        => spriteBatch.Draw(Def.Texture,
                (Rectangle)WorldRectangle, Def.SourceRectangle,
                Color, Rotation, Def.Origin, SpriteEffects, 0);
}
