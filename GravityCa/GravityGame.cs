using Merthsoft.Moose.GravityCa;
using Merthsoft.Moose.GravityCa.Renderers;
using Merthsoft.Moose.MooseEngine;
using Merthsoft.Moose.MooseEngine.Extension;
using Merthsoft.Moose.MooseEngine.Topologies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Reflection;

namespace GravityCa;
public class GravityGame : MooseGame
{
    public const int MapSize = 125;
    
    public static readonly UInt128 MaxGravity = UInt128.MaxValue;
    public static readonly UInt128 MaxMass = UInt128.MaxValue >> 1;
    public static readonly GravityMap Map = new(MapSize, MapSize, Topology.Torus);
    public static Point ScreenScale { get; private set; } = Point.Zero;
    public static bool DrawGravity { get; set; } = true;
    public static bool DrawMass { get; set; } = true;
    public static Color[] GravityColors { get; set; } = Palettes.AllPalettes[1];
    public static LerpMode GravityColorLerpMode { get; set; } = LerpMode.SystemMinToSystemMax;
    public static LerpMode GravityHeightLerpMode { get; set; } = LerpMode.SystemMinToSystemMax;
    public static Color[] MassColors { get; set; } = Palettes.AllPalettes[0];
    public static UInt128? MassMinDrawValue { get; set; }
    public static bool ConnectCells { get; set; } = true;
    public static GravityRendererMode GravityRendererMode { get; set; } = GravityRendererMode.ThreeDimmensionalRectangularPrism2;

    bool genRandom = true;
    bool hasRenderMinimum = false;
    SpriteFont font = null!;
    private GravityMapRenderer Renderer = null!;
    private Gravity3DRenderer Gravity3DPlaneRenderer = null!;
    public UInt128 MassDivisor = (UInt128)Math.Pow(2, 25);
    public bool DrawText = false;
    public object mapLock = new();
    private string version = "Gravity";

    float ScreenWidthRatio => (float)ScreenWidth / MapSize;
    float ScreenHeightRatio => (float)ScreenHeight / MapSize;
    Vector2 MouseLocation => MainCamera.ScreenToWorld(CurrentMouseState.X / (int)ScreenWidthRatio, CurrentMouseState.Y / (int)ScreenHeightRatio);

    public GravityGame()
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
        Renderer = AddMapRenderer(GravityMapRenderer.RenderKey, new GravityMapRenderer(SpriteBatch, ScreenScale));
        Gravity3DPlaneRenderer = AddMapRenderer(Gravity3DRenderer.RenderKey, new Gravity3DRenderer(GraphicsDevice, new(GraphicsDevice)
        {
            Alpha = 1,
            TextureEnabled = false,
            VertexColorEnabled = true,
            LightingEnabled = false,
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(100), 1, 1f, 1000f),
            World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up),
        }));

        //var div = (UInt128)(2);
        //var x = MapSize / 2 - 1;
        //var y = MapSize / 2 - 1;
        //Map.SetMass(x, y, MaxMass / div);
        //Map.SetMass(x + 1, y, MaxMass / div);
        //Map.SetMass(x, y + 1, MaxMass / div);
        //Map.SetMass(x + 1, y + 1, MaxMass / div);
        //for (var i = 0; i < 10; i++)
        //{
        //    var x = Random.Next(MapSize - 2) + 2;
        //    var y = Random.Next(MapSize - 2) + 2;
        //    Map.SetMass(x, y, MaxMass / div);
        //    Map.SetMass(x + 1, y, MaxMass / div);
        //    Map.SetMass(x, y + 1, MaxMass / div);
        //    Map.SetMass(x + 1, y + 1, MaxMass / div);
        //}
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (WasKeyJustPressed(Keys.Escape))
            Exit();

        if (WasKeyJustPressed(Keys.Enter) && IsKeyDown(Keys.LeftAlt))
            Graphics.ToggleFullScreen();

        if (WasKeyJustPressed(Keys.Space))
            Map.Running = !Map.Running;

        if (WasKeyJustPressed(Keys.T))
            DrawText = !DrawText;

        if (WasKeyJustPressed(Keys.M))
            DrawMass = !DrawMass;

        if (WasKeyJustPressed(Keys.G))
            DrawGravity = !DrawGravity;

        var keyPressed = CurrentKeyState.GetPressedKeys().FirstOrDefault();
        if (keyPressed >= Keys.D0 && keyPressed <= Keys.D9)
        {
            if (IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt))
                MassColors = Palettes.AllPalettes[keyPressed - Keys.D0];
            else
                GravityColors = Palettes.AllPalettes[keyPressed - Keys.D0];
        }
        else if (keyPressed >= Keys.F1 && keyPressed <= Keys.F12)
        {
            var index = keyPressed - Keys.F1;
            if (index < Enum.GetValues(typeof(GravityRendererMode)).Length)
                GravityRendererMode = (GravityRendererMode)index;
        }
        Map.RendererKey = GravityRendererMode == GravityRendererMode.Flat
                            ? GravityMapRenderer.RenderKey
                            : Gravity3DRenderer.RenderKey;

        if (IsActive && (IsLeftMouseDown() || IsRightMouseDown()))
        {
            var l = MouseLocation;
            var x = (int)l.X;
            var y = (int)l.Y;
            var mass = IsLeftMouseDown() ? Map.GetMassAt(x, y, 0) + MaxMass / MassDivisor : 0;
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
            GravityColorLerpMode = GravityColorLerpMode.Next();
        
        if (WasKeyJustPressed(Keys.E))
            GravityHeightLerpMode = GravityHeightLerpMode.Next();

        if (WasKeyJustPressed(Keys.V))
            hasRenderMinimum = !hasRenderMinimum;

        if (WasKeyJustPressed(Keys.B))
            genRandom = !genRandom;

        if (WasKeyJustPressed(Keys.T))
            Map.Topology = Map.Topology.Next();

        if ((genRandom && Map.Running) || WasKeyJustPressed(Keys.R) || WasKeyJustPressed(Keys.H))
        {
            var chance = (genRandom && Map.Running) ? .002 : WasKeyJustPressed(Keys.R) ? .02f : 1f;
            for (var x = 0; x < MapSize; x++)
                for (var y = 0; y < MapSize; y++)
                {
                    if (Map.GetMassAt(x, y, 0) == 0 && Random.NextSingle() <= chance)
                        Map.SetMass(x, y, MaxMass / MassDivisor);
                }
        }


        //if (hasRenderMinimum)
        //    Renderer.MassMinDrawValue = MaxMass / (MassDivisor+1);
        //else
        //    Renderer.MassMinDrawValue = null;

        Window.Title =  $"{version} - {(Map.Running ? "Running" : "Paused")}{(genRandom ? "*" : "")} | Div: {MassDivisor:N0} | Generation {Map.Generation:N0} | FPS {FramesPerSecondCounter.FramesPerSecond} | {Map.Topology}";
    }

    protected override void PostDraw(GameTime gameTime)
    {
        if (Map.RendererKey == Gravity3DRenderer.RenderKey)
            return;

        SpriteBatch.Begin(transformMatrix: Matrix.CreateScale(ScreenWidthRatio, ScreenHeightRatio, 1));
        SpriteBatch.FillRect(MouseLocation, 3, 3, Color.PaleVioletRed);
        SpriteBatch.End();
    }
}
