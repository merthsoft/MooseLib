using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;
using Merthsoft.Moose.MooseEngine.Renderers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Merthsoft.Moose.Rts;
public class MooseRtsGame : MooseGame
{
    public static int MapSize = 400;
    RtsMap Map = null!;
    SpriteFont font = null!;
    SpriteFont smallFont = null!;
    int reservation = 2;
    FlowField flowField = null!;

    public MooseRtsGame() : base()
    {
        
    }

    protected override StartupParameters Startup() => base.Startup() with
    {
        ScreenHeight = 1024,
        ScreenWidth = 1024,
        IsMouseVisible = true,
        DefaultBackgroundColor = new(18, 14, 25),
    };

    protected override void Load()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        Window.Title = $"Posso Time - v{fileVersionInfo.ProductVersion?.Split('+').FirstOrDefault() ?? "-1"}";

        IsFixedTimeStep = false;
        font = ContentManager.BakeFont("Capital_Hill_Monospaced", 12);
        smallFont = ContentManager.BakeFont("Capital_Hill_Monospaced", 6);
        AddDef(new UnitDef("Opossum", UnitTile.Opossum));
        AddDef(new UnitDef("Skunk", UnitTile.Skunk));
        AddDef(new UnitDef("Raccoon", UnitTile.Raccoon));

        Map = new(MapSize, MapSize);
        Map.RandomizeMap();
        ActiveMaps.Add(Map);

        //AddDefaultRenderer<TileLayer<int>>("tiles", new SpriteBatchPrimitiveRectangleRenderer(SpriteBatch, TileWidth, TileHeight, [Color.Transparent, Color.Green, Color.Red]));
        var terrainRenderer = new SpriteBatchTileTextureCachedRenderer(SpriteBatch, Map, ContentManager.LoadImage("Tiles/Terrain"));
        AddDefaultRenderer<TileLayer>("terrain", terrainRenderer);

        AddDefaultRenderer<TileLayer>("resource", new SpriteBatchTileTextureCachedRenderer(SpriteBatch, Map, ContentManager.LoadImage("Tiles/Resources")));
        AddDefaultRenderer<TileLayer>("item", new SpriteBatchTileTextureCachedRenderer(SpriteBatch, Map, ContentManager.LoadImage("Tiles/Items")));
        //AddDefaultRenderer<ObjectLayer<Unit>>("units", new SpriteBatchObjectRenderer(SpriteBatch));
        AddDefaultRenderer<ObjectLayer<Unit>>("units", new SpriteBatchObjectTileRenderer(SpriteBatch, TileWidth, TileHeight, ContentManager.LoadImage("Tiles/Units")));

        ResetCamera();
    }

    private void UpdateCamera(GameTime gameTime, Point mousePoint)
    {
        var movementDirection = Vector2.Zero;
        if (IsKeyDown(Keys.Down) || IsKeyDown(Keys.S))
            movementDirection += Vector2.UnitY;
        if (IsKeyDown(Keys.Up) || IsKeyDown(Keys.W))
            movementDirection -= Vector2.UnitY;
        if (IsKeyDown(Keys.Left) || IsKeyDown(Keys.A))
            movementDirection -= Vector2.UnitX;
        if (IsKeyDown(Keys.Right) || IsKeyDown(Keys.D))
            movementDirection += Vector2.UnitX;

        MainCamera.Move(movementDirection * 400 * gameTime.GetElapsedSeconds());

        var delta = GetScrollWheelDelta();
        if (delta != 0)
            Debug.WriteLine($"{delta} - {MainCamera.Zoom}");
        if (delta != 0 && ((delta < 0 && MainCamera.Zoom > .1f) || (delta > 0 && MainCamera.Zoom < 10)))
            MainCamera.ZoomToPoint(Math.Sign(delta) / 12f, new Vector2(mousePoint.X * TileWidth + TileWidth / 2, mousePoint.Y * TileHeight + TileHeight / 2));

        if (IsKeyDown(Keys.Z))
            ResetCamera();
    }

    private void ResetCamera()
    {
        MainCamera.Origin = new(0, 0);
        MainCamera.Position = new(-16, -7);
        MainCamera.Zoom = 0.316667f;
        MainCamera.Rotation = 0;
    }

    protected override void PostUpdate(GameTime gameTime)
    {
        if (!IsActiveAndMouseInBounds)
            return;

        var mouseVec = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
        var mousePoint = new Point((int)mouseVec.X / TileWidth, (int)mouseVec.Y / TileHeight);
        
        UpdateCamera(gameTime, mousePoint);


        if (!Map.CellIsInBounds(mousePoint))
            return;

        if (WasLeftMouseJustPressed() || (IsLeftMouseDown() && WasMouseMoved()))
        {
            flowField = (Map.PathFinder as FlowFieldPathFinder)!.GetFlow(Map.BlockingGrid, mousePoint)!;
            foreach (var unit in ReadObjects.OfType<Unit>())
            {
                unit.SetFlow(flowField);
            }
        }

        if (IsMiddleMouseDown())
        {
            AddObject(new Unit(GetDefs<UnitDef>().ToList().RandomElement(), new(mousePoint.X * TileWidth, mousePoint.Y * TileHeight)), Map);
        }

        if (WasKeyJustPressed(Keys.R))
        {
            Map.RandomizeMap();
            ResetCamera();
        }
    }

    protected override void PostDraw(GameTime gameTime)
    {
        var mouseCell = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
        var mousePoint = new Point((int)mouseCell.X / TileWidth, (int)mouseCell.Y / TileHeight);
        SpriteBatch.Begin();
        if (IsKeyDown(Keys.Tab))
            SpriteBatch.DrawStringShadow(font, $"FPS {FramesPerSecondCounter.FramesPerSecond} - #possos: {ReadObjects.OfType<Unit>().Count()} - {mousePoint} - seed {RandomSeed:X}", new(4, 4), Color.White);
        
        SpriteBatch.End();
    }
}
