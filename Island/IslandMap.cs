using SimplexNoise;

namespace Merthsoft.Moose.Island;
public class IslandMap : BaseMap
{
    public readonly TileLayer<IslandTile> Island;
    public override int Height { get; } = 125;
    public override int Width { get; } = 125;
    public override int TileWidth { get; } = 8;
    public override int TileHeight { get; } = 8;

    public float[,,] NoiseMap = null!;
    public int Generation = 0;

    public TimeSpan LastUpdate;

    public IslandMap()
    {
        Island = AddLayer(new TileLayer<IslandTile>("Island", Width, Height, IslandTile.Water, IslandTile.Water));
        RandomizeMap();
    }

    public override int IsBlockedAt(string layer, int x, int y) => 0;

    public void RandomizeMap()
    {
        Noise.Seed = (int)DateTime.Now.Ticks;
        NoiseMap = Noise.Calc3D(Width, Height, 100, .01f);
        Generation = 0;
        DrawGeneration();
    }

    private void DrawGeneration()
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Island.SetTileValue(x, y, NoiseMap[x, y, Generation] > 128 ? IslandTile.Land : IslandTile.Water);
    }

    private void NextGeneration()
    {
        Generation++;
        if (Generation == 100)
        {
            RandomizeMap();
            return;
        }

        DrawGeneration();
    }

    public override void Update(MooseGame _game, GameTime gameTime)
    {
        base.Update(_game, gameTime);
        if ((gameTime.TotalGameTime - LastUpdate).TotalSeconds > .05)
        {
            NextGeneration();
            LastUpdate = gameTime.TotalGameTime;
        }
    }
}
