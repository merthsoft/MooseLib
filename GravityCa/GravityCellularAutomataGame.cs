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
    public const int MapSize = 200;
    
    public static readonly UInt128 MaxGravity = UInt128.MaxValue >> 1;
    public static readonly UInt128 MaxMass = UInt128.MaxValue >> 3;
    static readonly GravityMap Map = new(MapSize, MapSize, Topology.Torus);
    bool genRandom = false;
    bool hasRenderMinimum = false;
    SpriteFont font = null!;
    private GravityMapRenderer Renderer = null!;
    public UInt128 MassDivisor = (UInt128)Math.Pow(2, 25);
    public bool DrawText = false;
    public object mapLock = new();
    private string version = "Gravity";

    public Point ScreenScale = Point.Zero;
    float ScreenWidthRatio => (float)ScreenWidth / MapSize;
    float ScreenHeightRatio => (float)ScreenHeight / MapSize;
    Vector2 MouseLocation => MainCamera.ScreenToWorld(CurrentMouseState.X / (int)ScreenWidthRatio, CurrentMouseState.Y / (int)ScreenHeightRatio);

    public GravityCellularAutomataGame()
    {
    }

    protected override StartupParameters Startup()
        => base.Startup() with
        {
            ScreenWidth = 900*2,
            ScreenHeight = 900*2,
            IsFullscreen = false,
            IsMouseVisible = false,
            DefaultBackgroundColor = Color.Black,
            RenderMode = RenderMode.Map
        };

    protected override void Load()
    {
        IsFixedTimeStep = false;
        Graphics.SynchronizeWithVerticalRetrace = false;
        
        font = ContentManager.BakeFont("Capital_Hill_Monospaced", 8);
        var assembly = Assembly.GetExecutingAssembly();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        version = $"{fileVersionInfo.ProductName} - v{fileVersionInfo.ProductVersion?.Split('+')[0] ?? "??"}";
        Window.Title = version;
        ActiveMaps.Add(Map);
        ScreenScale = new Point(ScreenWidth, ScreenHeight);
        Renderer = AddMapRenderer(Map.RendererKey!, new GravityMapRenderer(SpriteBatch, ScreenScale));
        //var div = (UInt128)(1);
        //var x = MapSize / 2;
        //var y = MapSize / 2;
        //Map.SetMass(x, y, MaxMass / div);
        //Map.SetMass(x + 1, y, MaxMass / div);
        //Map.SetMass(x, y + 1, MaxMass / div);
        //Map.SetMass(x + 1, y + 1, MaxMass / div);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (WasKeyJustPressed(Keys.Escape))
            Exit();

        if (WasKeyJustPressed(Keys.Space))
            Map.Running = !Map.Running;

        if (WasKeyJustPressed(Keys.T))
            DrawText = !DrawText;

        if (WasKeyJustPressed(Keys.M))
            Renderer.DrawMass = !Renderer.DrawMass;

        if (WasKeyJustPressed(Keys.G))
            Renderer.DrawGravity = !Renderer.DrawGravity;

        var keyPressed = CurrentKeyState.GetPressedKeys().FirstOrDefault();
        if (keyPressed >= Keys.D0 && keyPressed <= Keys.D9)
            Renderer.GravityColors = Palettes.AllPalettes[keyPressed - Keys.D0];

        if (keyPressed >= Keys.NumPad0 && keyPressed <= Keys.NumPad9)
            Renderer.MassColors = Palettes.AllPalettes[keyPressed - Keys.NumPad0];

        if (IsActive && (IsLeftMouseDown() || IsRightMouseDown()))
        {
            var l = MouseLocation;
            var x = (int)l.X;
            var y = (int)l.Y;
            var mass = IsLeftMouseDown() ? Map.GetMass(x, y, 0) + MaxMass / MassDivisor : 0;
            for (var i = 0; i <= 2; i++)
                for (var j = 0; j <= 2; j++)
                    Map.SetMass(x + i, y + j, mass);
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
            Map.Reset();

        if (WasKeyJustPressed(Keys.Q))
            Renderer.GravityLerpMode = Renderer.GravityLerpMode.Next();

        if (WasKeyJustPressed(Keys.V))
            hasRenderMinimum = !hasRenderMinimum;

        if (WasKeyJustPressed(Keys.B))
            genRandom = !genRandom;

        if (WasKeyJustPressed(Keys.T))
            Map.Topology = Map.Topology.Next();

        if ((genRandom && Map.Running) || WasKeyJustPressed(Keys.R) || WasKeyJustPressed(Keys.F))
        {
            var chance = (genRandom && Map.Running) ? .002 : WasKeyJustPressed(Keys.R) ? .02f : 1f;
            for (var x = 0; x < MapSize; x++)
                for (var y = 0; y < MapSize; y++)
                {
                    if (Map.GetMass(x, y, 0) == 0 && Random.NextSingle() <= chance)
                        Map.SetMass(x, y, MaxMass / MassDivisor);
                }
        }

        //if (hasRenderMinimum)
        //    Renderer.MassMinDrawValue = MaxMass / (MassDivisor+1);
        //else
        //    Renderer.MassMinDrawValue = null;

        Window.Title =  $"{version} - {(Map.Running ? "Running" : "Paused")}{(genRandom ? "*" : "")} | Div: {MassDivisor:N0} | Generation {Map.Generation:N0} | FPS {FramesPerSecondCounter.FramesPerSecond} | {Map.Topology}";
    }

    private void DrawString(int x, int y, string text)
        => SpriteBatch.DrawStringShadow(font, text, new(x, y), Color.White);

    protected override void PostDraw(GameTime gameTime)
    {
        SpriteBatch.Begin(transformMatrix: Matrix.CreateScale(ScreenWidthRatio, ScreenHeightRatio, 1));
        SpriteBatch.FillRect(MouseLocation, 3, 3, Color.PaleVioletRed);
        SpriteBatch.End();
    }
}
