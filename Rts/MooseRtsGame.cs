﻿using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using Merthsoft.Moose.MooseEngine.PathFinding.PathFinders.FlowField;
using System.Linq;

namespace Merthsoft.Moose.Rts;
public class MooseRtsGame : MooseGame
{
    public static int MapSize = 100;
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
        ScreenHeight = 800,
        ScreenWidth = 800,
        IsMouseVisible = true,
        DefaultBackgroundColor = new(18, 14, 25),
    };

    protected override void Load()
    {
        IsFixedTimeStep = false;
        font = ContentManager.BakeFont("Capital_Hill_Monospaced", 12);
        smallFont = ContentManager.BakeFont("Capital_Hill_Monospaced", 6);
        AddDef(new UnitDef("Opossum"));

        Map = new(MapSize, MapSize);
        ActiveMaps.Add(Map);

        //AddDefaultRenderer<TileLayer<int>>("tiles", new SpriteBatchPrimitiveRectangleRenderer(SpriteBatch, TileWidth, TileHeight, [Color.Transparent, Color.Green, Color.Red]));
        var terrainRenderer = new SpriteBatchTileTextureRenderer(SpriteBatch, TileWidth, TileHeight, ContentManager.LoadImage("Tiles/Terrain"));
        AddDefaultRenderer<TileLayer>("terrain", terrainRenderer);
        
        AddDefaultRenderer<TileLayer>("resource", new SpriteBatchTileTextureRenderer(SpriteBatch, TileWidth, TileHeight, ContentManager.LoadImage("Tiles/Resources")));
        AddDefaultRenderer<ObjectLayer<Unit>>("units", new SpriteBatchObjectRenderer(SpriteBatch));

        MainCamera.Origin = new(0, 0);
        MainCamera.Position = new(-45, -40);
        MainCamera.Zoom = 0.9f;
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

        var delta = GetScrollWheelDelta() / 1200f;
        if (delta != 0 && (delta > 0 || MainCamera.Zoom > .5f))
            MainCamera.ZoomToPoint(delta, new(mousePoint.X * TileWidth + TileWidth / 2, mousePoint.Y * TileHeight + TileHeight / 2));
        if (MainCamera.Zoom < .5f)
            MainCamera.Zoom = .5f;
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

        if (WasMiddleMouseJustPressed())
        {
            AddObject(new Unit(GetDef<UnitDef>()!, new(mousePoint.X * TileWidth, mousePoint.Y * TileHeight)), Map);
        }

        if (WasKeyJustPressed(Keys.R))
        {
            Map.RandomizeMap();
        }
    }

    private void DrawUnderUnits()
    {
        var transformMatrix = MainCamera.GetViewMatrix();
        SpriteBatch.Begin(transformMatrix: transformMatrix);

        var mouseCell = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
        mouseCell = new Vector2((int)mouseCell.X / TileWidth * TileWidth, (int)mouseCell.Y / TileHeight * TileHeight);
        SpriteBatch.DrawRectangle(mouseCell, new(TileWidth, TileHeight), Color.AliceBlue);
        SpriteBatch.End();
    }

    protected override bool PreMapDraw(GameTime gameTime)
    {
        DrawUnderUnits();
        return true;
    }

    protected override void PostDraw(GameTime gameTime)
    {
        var mouseCell = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
        var mousePoint = new Point((int)mouseCell.X / TileWidth, (int)mouseCell.Y / TileHeight);
        SpriteBatch.Begin();
        SpriteBatch.DrawStringShadow(font, $"FPS {FramesPerSecondCounter.FramesPerSecond} - #possos: {ReadObjects.OfType<Unit>().Count()} - {mousePoint}", new(4, 4), Color.White);
        SpriteBatch.End();
    }
}
