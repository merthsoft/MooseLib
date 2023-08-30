using Merthsoft.Moose.MooseEngine.BaseDriver;
using SimplexNoise;

namespace Merthsoft.Moose.Town;
public class TownMap : BaseMap
{
    public override int Height { get; } = 200;
    public override int Width { get; } = 400;
    public override int TileWidth { get; } = 16;
    public override int TileHeight { get; } = 16;

    public TileLayer<int> BaseLayer;

    public TownMap()
    {
        BaseLayer = AddLayer(new TileLayer<int>("base", Width, Height, -1, 0) { RendererKey = "base" });
    }

    public override int IsBlockedAt(string layer, int x, int y)
        => 0;

    public void RandomizeMap()
    {
        var rockNoise = GenNoise(.01f);
        var goldNoise = GenNoise(.01f);
        var treeNoise = GenNoise(.08f);
        var waterNoise = GenNoise(.01f);
        var stumpNoise = GenNoise(.08f);

        for (var x = 0; x < Width; x += 1)
            for (var y = 0; y < Height; y += 1)
            {
                var value = 0;

                if (rockNoise[x, y] >= 200)
                    value = rockNoise[x, y] > 230 ? 5
                        : goldNoise[x, y] < 55 ? 3 
                            : rockNoise[x,y] < 170 ? 7 : 2 ;
                else if (waterNoise[x, y] < 55)
                    value = waterNoise[x, y] > 45 ? 8 : 4;
                else if (treeNoise[x, y] > 150)
                    value = stumpNoise[x, y] > 215 ? 6 : 1;

                BaseLayer.SetTileValue(x, y, value);
            }
    }

    private float[,] GenNoise(float scale)
    {
        Noise.Seed = (int)DateTime.Now.Ticks;
        return Noise.Calc2D(Width, Height, scale);
    }
}