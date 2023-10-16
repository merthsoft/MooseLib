using Merthsoft.Moose.MooseEngine.Renderers;

namespace Merthsoft.Moose.Town;

public class TownGame : MooseGame
{
    SpriteFont MainFont = null!;
    public TownMap TownMap = null!;

    public TownGame()
    {
    }

    protected override StartupParameters Startup() => base.Startup() with
    {
        DefaultBackgroundColor = Color.Black,
        IsMouseVisible = true,
        ScreenHeight = 1000,
        ScreenWidth = 1200,
    };

    protected override void Load()
    {
        TownMap = new();
        TownMap.RandomizeMap();
        ActiveMaps.Add(TownMap);

        MainFont = ContentManager.BakeFont("Tomorrow_Night_Monospaced", 16);

        AddRenderer("base", new SpriteBatchCharacterRenderer(SpriteBatch, MainFont,
            new SpriteBatchCharacterRenderer.CharacterDefinition[] {
                new(' ', Color.White),
                new('t', Color.DarkGreen),
                new('R', Color.Gray),
                new('g', Color.Yellow),
                new('~', Color.Blue, Color.DarkBlue),
                new('X', Color.WhiteSmoke),
                new('.', Color.Brown),
                new('r', Color.DarkGray),
                new(' ', Color.SandyBrown, Color.SandyBrown)
            }));
    }

    private void UpdateCamera(GameTime gameTime)
    {
        var movementDirection = Vector2.Zero;
        if (IsKeyDown(Keys.Down))
            movementDirection += Vector2.UnitY;
        if (IsKeyDown(Keys.Up))
            movementDirection -= Vector2.UnitY;
        if (IsKeyDown(Keys.Left))
            movementDirection -= Vector2.UnitX;
        if (IsKeyDown(Keys.Right))
            movementDirection += Vector2.UnitX;

        MainCamera.Move(movementDirection  * 400 * gameTime.GetElapsedSeconds());

        if (IsKeyDown(Keys.PageDown))
            MainCamera.ZoomIn(.01f);
        else if (IsKeyDown(Keys.PageUp))
            MainCamera.ZoomOut(.01f);
    }

    protected override void PreUpdate(GameTime gameTime)
    {
        base.PreUpdate(gameTime);
        UpdateCamera(gameTime);

        if (WasKeyJustPressed(Keys.R))
            TownMap.RandomizeMap();
    }
}
