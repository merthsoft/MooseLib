using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.Extension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Reflection;

namespace GravityCa;
public class GravityCellularAutomataGame : MooseGame
{
    public const int MapSize = 300;
    public const int DrawSize = 1;

    public static UInt128 MaxValue = UInt64.MaxValue;
    static readonly GravityMap Map = new(MapSize, MapSize);
    bool running = false;
    SpriteFont font = null!;
    private SpriteBatchSpectrumRenderer massRenderer = null!;
    private SpriteBatchSpectrumRenderer gravityRenderer = null!;
    public UInt128 MassDivisor = 8;

    int currentPalette = 0;
    List<List<Color>> GravityPalettes = new();

    public GravityCellularAutomataGame()
    {
    }

    protected override StartupParameters Startup()
        => base.Startup() with
        {
            ScreenHeight = MapSize * DrawSize,
            ScreenWidth = MapSize * DrawSize
        };

    protected override void Load()
    {
        IsFixedTimeStep = false;
        font = ContentManager.BakeFont("Capital_Hill_Monospaced", 12);
        IsMouseVisible = true;
        var assembly = Assembly.GetExecutingAssembly();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        Window.Title = $"Gravity - v{fileVersionInfo.ProductVersion?.Split('+')[0] ?? "??"}";

        GravityPalettes.Add(new List<Color>()
        {
            Color.DarkRed,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.White,
            Color.Black
        });
        GravityPalettes.Add(new List<Color>()
        {
            Color.DarkRed,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet,
            Color.White,
            Color.Black
        });
        GravityPalettes.Add(new List<Color>()
        {
            Color.Black,
            Color.White
        });

        ActiveMaps.Add(Map);
        gravityRenderer = AddRenderer("gravity", new SpriteBatchSpectrumRenderer(SpriteBatch, MapSize, MapSize, MaxValue, GravityPalettes[0].ToArray()) { UseTransparentForZero = true, DrawScale = new(DrawSize, DrawSize) });
        
        massRenderer = AddRenderer("mass", new SpriteBatchSpectrumRenderer(SpriteBatch, MapSize, MapSize, MaxValue, Color.PaleVioletRed, Color.White) { UseTransparentForZero = true, DrawScale = new(DrawSize, DrawSize) });

        MainCamera.ZoomIn(0);

        //Map.SetMass(15, 15, MaxValue / 5000);

        //Map.SetMass(45, 45, MaxValue/50);

        //Map.SetMass(75, 75, MaxValue/20);

        //Map.SetMass(64, 64, MaxValue / 2500);
        //Map.SetMass(64, 65, MaxValue / 2500);
        //Map.SetMass(64, 66, MaxValue / 2500);
        //Map.SetMass(65, 64, MaxValue / 2500);
        //Map.SetMass(65, 65, MaxValue / 2500);
        //Map.SetMass(65, 66, MaxValue / 2500);
        //Map.SetMass(66, 64, MaxValue / 2500);
        //Map.SetMass(66, 65, MaxValue / 2500);
        //Map.SetMass(66, 66, MaxValue / 2500);

        //Map.SetMass(55, 65, MaxValue / 4000);
        //Map.SetMass(75, 65, MaxValue / 3000);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (running)
            Map.Update();

        if (WasKeyJustPressed(Keys.Space))
            running = !running;

        if (WasKeyJustPressed(Keys.M))
            massRenderer.IsActive = !massRenderer.IsActive;

        if (WasKeyJustPressed(Keys.G)) 
        {
            if (gravityRenderer.IsActive)
            {
                currentPalette++;
                if (currentPalette >= GravityPalettes.Count)
                {
                    currentPalette = 0;
                    gravityRenderer.IsActive = false;
                }
            } else
            {
                gravityRenderer.IsActive = true;
            }
            gravityRenderer.Colors = GravityPalettes[currentPalette];
        }

        if (IsLeftMouseDown() || IsRightMouseDown())
        {
            var x = CurrentMouseState.X / DrawSize;
            var y = CurrentMouseState.Y / DrawSize;
            var mass = IsLeftMouseDown() ? MaxValue / MassDivisor : 0;
            Map.SetMass(x, y, mass);
            Map.SetMass(x, y + 1, mass);
            Map.SetMass(x + 1, y, mass);
            Map.SetMass(x + 1, y + 1, mass);
        }

        if (ScrollWheelDelta() > 0)
            MassDivisor++;
        else if (ScrollWheelDelta() < 0)
            MassDivisor--;

        if (MassDivisor < 1)
            MassDivisor = 1;

        if (WasKeyJustPressed(Keys.Z))
        {
            for (var x = 0; x < MapSize; x++)
                for (var y = 0; y < MapSize; y++)
                    Map.SetMass(x, y, 0);
        }

        if (WasKeyJustPressed(Keys.R))
        {
            for (var x = 0; x < MapSize; x++)
                for (var y = 0; y < MapSize; y++)
                {
                    if (Random.NextSingle() <= .1f)
                        Map.SetMass(x, y, MaxValue / MassDivisor);
                }
        }
    }

    protected override void PostDraw(GameTime gameTime)
    {
        SpriteBatch.Begin();
        SpriteBatch.DrawString(font, $"Generation {Map.Generation} - FPS {FramesPerSecondCounter.FramesPerSecond} - Divisor {MassDivisor}", new(4, 4), Color.Black);

        var x = ((int)WorldMouse.X / DrawSize) * DrawSize;
        var y = ((int)WorldMouse.Y / DrawSize) * DrawSize;
        SpriteBatch.FillRect(new(x, y), DrawSize * 2, DrawSize * 2, Color.Pink);

        SpriteBatch.End();
    }
}
