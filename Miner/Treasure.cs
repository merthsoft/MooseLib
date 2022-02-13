using Merthsoft.Moose.MooseEngine.GameObjects;
using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.Miner;

class Treasure : GameObjectBase
{
    public TreasureDef TreasureDef => (Def as TreasureDef)!;
    public Tile Tile => TreasureDef.Tile;

    private int DrawWidth => (int)TreasureDef.DefaultSize.X;
    private int DrawHeight => (int)TreasureDef.DefaultSize.Y;

    private float Fade = 1.0f;
    private float FadeSpeed = 0;

    public Treasure(TreasureDef treasureDef, int x, int y) : base(treasureDef, new(x, y))
    {

    }

    public override void Update(GameTime gameTime)
    {
        Fade -= FadeSpeed;
        FadeSpeed += .0025f;
        if (Fade <= 0)
            RemoveFlag = true;
    }

    public Rectangle GetSourceRectangle()
    {
        var sourceX = ((int)Tile % TreasureDef.NumColumns) * DrawWidth;
        var sourceY = ((int)Tile / TreasureDef.NumColumns) * DrawHeight;

        return new Rectangle(sourceX, sourceY, DrawWidth, DrawHeight);
    }

    public override DrawParameters GetDrawParameters()
        => new(TreasureDef.Texture, (Rectangle)WorldRectangle, GetSourceRectangle(), LayerDepth: 1, Color: Color.White * Fade);
}
