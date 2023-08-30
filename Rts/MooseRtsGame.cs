using Merthsoft.Moose.MooseEngine.BaseDriver;
using Merthsoft.Moose.MooseEngine.BaseDriver.Renderers;
using System.Linq;

namespace Merthsoft.Moose.Rts;
public class MooseRtsGame : MooseGame
{
    public static int MapSize = 40;
    RtsMap Map = new(MapSize, MapSize);
    SpriteFont font = null!;
    int reservation = 2;

    public MooseRtsGame()
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
        font = ContentManager.BakeFont("Capital_Hill_Monospaced", 12);
        ActiveMaps.Add(Map);
        var archerDef = AddDef(new UnitDef("Archer"));

        AddDefaultRenderer<ObjectLayer<Unit>>("units", new SpriteBatchObjectRenderer(SpriteBatch));

        //AddObject(new Unit(archerDef, new(0, 0)), Map);
        //AddObject(new Unit(archerDef, new(0, 16)), Map);
        //AddObject(new Unit(archerDef, new(16, 0)), Map);
        //AddObject(new Unit(archerDef, new(16, 16)), Map);
        //AddObject(new Unit(archerDef, new(24, 32)), Map);
        //AddObject(new Unit(archerDef, new(32, 24)), Map);

        MainCamera.Origin = new(0, 0);
        MainCamera.ZoomIn(2);
    }

    protected override void PostUpdate(GameTime gameTime)
    {
        if (!IsActive)
            return;

        var mousePoint = MainCamera.ScreenToWorld(
                    CurrentMouseState.Position.X / TileWidth,
                    CurrentMouseState.Position.Y / TileHeight).ToPoint();

        if (WasLeftMouseJustPressed() || (IsLeftMouseDown() && WasMouseMoved()))
        {
            var mouseVector = new Vector2(mousePoint.X * TileWidth + TileWidth / 2, mousePoint.Y * TileHeight + TileHeight / 2);
            var spiral = mousePoint.SpiralAround().GetEnumerator();
            foreach (var unit in Map.UnitLayer.Objects.OrderBy(x => x.DistanceSquaredTo(mouseVector)))
            {
                unit.MoveTo(spiral.MoveNextGetCurrent());
            }
        }

        if (WasMiddleMouseJustPressed())
        {
            AddObject(new Unit(GetDef<UnitDef>("Unit_Archer"), new(mousePoint.X * TileWidth, mousePoint.Y * TileHeight)), Map);
        }

        if (WasRightMouseJustPressed())
        {
            reservation = Map.ReservationMap[mousePoint.X, mousePoint.Y] == 0 ? 2 : 0;
        }

        if (IsRightMouseDown())
        {
            Map.ReservationMap[mousePoint.X, mousePoint.Y] = reservation;
        }
    }

    private void DrawUnderUnits()
    {
        var transformMatrix = MainCamera.GetViewMatrix();
        SpriteBatch.Begin(transformMatrix: transformMatrix);

        var mouseCell = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
        mouseCell = new Vector2((int)mouseCell.X / TileWidth * TileWidth, (int)mouseCell.Y / TileHeight * TileHeight);
        SpriteBatch.DrawRectangle(mouseCell, new(TileWidth, TileHeight), Color.AliceBlue);

        foreach (var unit in Map.UnitLayer.Objects.Cast<Unit>())
        {
            if (unit.State != Unit.States.Walk)
                continue;
            var prevCell = new Vector2(unit.Position.X + TileWidth / 2, unit.Position.Y + TileHeight / 2);
            foreach (var cell in unit.MoveQueue)
            {
                var nextCell = new Vector2(cell.X * TileWidth + TileWidth / 2, cell.Y * TileHeight + TileHeight / 2);
                SpriteBatch.DrawLine(prevCell, nextCell, Color.Green);
                prevCell = nextCell;
            }
        }

        for (var x = 0; x < MapWidth; x++)
            for (var y = 0; y < MapHeight; y++)
            {
                var color = Map.ReservationMap[x, y] switch
                {
                    //1 => Color.Green,
                    2 => Color.Red,
                    _ => Color.Transparent
                };
                SpriteBatch.FillRectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight, color);
                //var cell = Map.BlockingGrid.GetNode(x, y);
                //if (!cell.Incoming.Any(e => e.IsConnected))
                //    SpriteBatch.FillRectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight, Color.Red);
                //if (!cell.Outgoing.Any(e => e.IsConnected))
                //SpriteBatch.FillRectangle(x * TileWidth + TileWidth / 2, y * TileHeight, TileWidth / 2, TileHeight, Color.Green);
            }

        SpriteBatch.End();
    }

    protected override bool PreMapDraw(GameTime gameTime)
    {
        DrawUnderUnits();
        return true;
    }

    protected override void PostDraw(GameTime gameTime)
    {
        SpriteBatch.Begin();
        SpriteBatch.DrawStringShadow(font, $"FPS {FramesPerSecondCounter.FramesPerSecond}", new(4, 4), Color.White);
        SpriteBatch.End();
    }
}
