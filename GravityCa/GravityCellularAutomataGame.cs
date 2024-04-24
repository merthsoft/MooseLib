using Merthsoft.Moose.GravityCa;
using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.Extension;
using Merthsoft.Moose.MooseEngine.Topologies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Reflection;

namespace GravityCa;
public class GravityCellularAutomataGame : MooseGame
{
    public const int MapSize = 150;
    public const int DrawSize = 2;

    public static UInt128 MaxGravity = UInt128.MaxValue >> 1;
    public static UInt128 MaxMass = MaxGravity - 1;
    static readonly GravityMap Map = new(MapSize, MapSize, Topology.Torus);
    bool running = true;
    SpriteFont font = null!;
    private GravityMapRenderer Renderer = null!;
    public UInt128 MassDivisor = (UInt128)Math.Pow(2, 25);
    public bool DrawText = false;
    public object mapLock = new();
    private string version = "Gravity";

    public Point ScreenScale = Point.Zero;

    //Task updateTask;
    List<List<Color>> GravityPalettes = [];

    public GravityCellularAutomataGame()
    {
    }

    protected override StartupParameters Startup()
        => base.Startup() with
        {
            ScreenWidth = 1600*2,
            ScreenHeight = 900*2,
            IsFullscreen = false,
            IsMouseVisible = false,
            DefaultBackgroundColor = Color.DarkGray,
            RenderMode = RenderMode.Map
        };

    protected override void Load()
    {
        IsFixedTimeStep = false;
        Graphics.SynchronizeWithVerticalRetrace = false;
        
        font = ContentManager.BakeFont("Capital_Hill_Monospaced", 24);
        var assembly = Assembly.GetExecutingAssembly();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        version = $"{fileVersionInfo.ProductName} - v{fileVersionInfo.ProductVersion?.Split('+')[0] ?? "??"}";
        Window.Title = version;
        ActiveMaps.Add(Map);
        ScreenScale = new Point(ScreenWidth, ScreenHeight);
        Renderer = AddMapRenderer(Map.RendererKey!, new GravityMapRenderer(SpriteBatch, ScreenScale));
        MainCamera.ZoomIn(0);
        //var div = (UInt128)(1 << 15);
        //var x = MapSize / 2;
        //var y = MapSize / 2;
        //Map.SetMass(x, y, MaxMass / div);
        //Map.SetMass(x + 1, y, MaxMass / div);
        //Map.SetMass(x, y + 1, MaxMass / div);
        //Map.SetMass(x + 1, y + 1, MaxMass / div);
        //for (var x = 0; x < MapSize; x++)
        //    for (var y = 0; y < MapSize; y++)
        //    {
        //        if (Map.MassLayer[x, y] == 0 && Random.NextSingle() <= .00075)
        //        {
        //            var div = (UInt128)(1 << Random.Next(15, 20));
        //            Map.SetMass(x, y, MaxMass / div);
        //            Map.SetMass(x + 1, y, MaxMass / div);
        //            Map.SetMass(x, y+1, MaxMass / div);
        //            Map.SetMass(x + 1, y + 1, MaxMass / div);
        //        }
        //    }

        //updateTask = Task.Factory.StartNew(() =>
        //{
        //    while (true)
        //    {
        //        if (running)
        //        {
        //            lock (mapLock)
        //            {
        //                Map.Update();
        //            }
        //        }
        //        Thread.Sleep(0);
        //    }
        //});
    }

    //protected override void Draw(GameTime gameTime)
    //{
    //    lock (mapLock)
    //    {
    //        base.Draw(gameTime);
    //    }
    //}

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (WasKeyJustPressed(Keys.Escape))
            Exit();
        
        if (running)
            Map.Update();

        if (WasKeyJustPressed(Keys.Space))
            running = !running;

        if (WasKeyJustPressed(Keys.T))
            DrawText = !DrawText;

        if (WasKeyJustPressed(Keys.M))
            Renderer.DrawMass = !Renderer.DrawMass;

        if (WasKeyJustPressed(Keys.G))
            Renderer.DrawGravity = !Renderer.DrawGravity;

        if (WasKeyJustPressed(Keys.B))
            Renderer.GravityBlendColors = !Renderer.GravityBlendColors;

        var keyPressed = CurrentKeyState.GetPressedKeys().FirstOrDefault();
        if (keyPressed >= Keys.D0 && keyPressed <= Keys.D9)
            Renderer.GravityColors = Palettes.AllPalettes[keyPressed - Keys.D0];

        if (keyPressed >= Keys.NumPad0 && keyPressed <= Keys.NumPad9)
            Renderer.MassColors = Palettes.AllPalettes[keyPressed - Keys.NumPad0];

        if (IsActive && (IsLeftMouseDown() || IsRightMouseDown()))
        {
            var x = CurrentMouseState.X / DrawSize;
            var y = CurrentMouseState.Y / DrawSize;
            var mass = IsLeftMouseDown() ? MaxMass / MassDivisor : 0;
            Map.SetMass(x, y, mass);
            //Map.SetMass(x, y + 1, mass);
            //Map.SetMass(x + 1, y, mass);
            //Map.SetMass(x + 1, y + 1, mass);
        }

        if (GetScrollWheelDelta() > 0)
            MassDivisor <<= 1;
        else if (GetScrollWheelDelta() < 0)
            MassDivisor >>= 1 ;

        if (MassDivisor < 1)
            MassDivisor = 1;

        if (WasKeyJustPressed(Keys.Z) || WasKeyJustPressed(Keys.C))
        {
            for (var x = 0; x < MapSize; x++)
                for (var y = 0; y < MapSize; y++)
                    Map.SetMass(x, y, 0);
        }

        if (WasKeyJustPressed(Keys.X) || WasKeyJustPressed(Keys.C))
        {
            for (var x = 0; x < MapSize; x++)
                for (var y = 0; y < MapSize; y++)
                    Map.SetGravity(x, y, 0);
        }

        if (WasKeyJustPressed(Keys.C))
            Map.Generation = 0;

        if (WasKeyJustPressed(Keys.Q))
            Renderer.GravityRelativeSpectrum = !Renderer.GravityRelativeSpectrum;

        if (running || WasKeyJustPressed(Keys.R) || WasKeyJustPressed(Keys.F))
        {
            var chance = running || WasKeyJustPressed(Keys.R) ? .2f : 1f;
            for (var x = 0; x < MapSize; x++)
                for (var y = 0; y < MapSize; y++)
                {
                    if (Map.MassLayer[x, y] == 0 && Random.NextSingle() <= chance)
                        if (MassDivisor == 1)
                            Map.SetMass(x, y, MaxMass / MassDivisor);
                        else
                            Map.SetMass(x, y, MaxMass / (MassDivisor + (UInt128)Random.Next(-2, 3)));
                }
        }

        Renderer.MassMinDrawValue = MaxMass / (UInt128)((double)MassDivisor / 50);
        Window.Title =  $"{version} - Generation {Map.Generation:N0} - FPS {FramesPerSecondCounter.FramesPerSecond}";
    }

    private void DrawString(int x, int y, string text)
        => SpriteBatch.DrawStringShadow(font, text, new(x, y), Color.White);

    protected override void PostDraw(GameTime gameTime)
    {
        if (!DrawText)
            return;
        SpriteBatch.Begin();
        
        DrawString(4, 4, $"Generation {Map.Generation:N0} - FPS {FramesPerSecondCounter.FramesPerSecond}");
        
        var massString = Map.TotalMass.ToString().Split('E');
        var mass = double.Parse(massString[0]);
        DrawString(5, 30, $"Total mass {Math.Round(mass, 2, MidpointRounding.ToEven)}E{massString.ElementAtOrDefault(1) ?? "0"}");
        
        SpriteBatch.End();
    }
}
