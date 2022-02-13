using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.Miner;

record TreasureDef : MinerObjectDef
{
    public int NumColumns { get; }

    public Tile Tile { get; }

    public Texture2D Texture { get; }

    public TreasureDef(Tile tile, Texture2D texture, int numColumns = 8) : base(tile.ToString())
        => (Texture, Tile, NumColumns)
         = (texture, tile, numColumns);
}
