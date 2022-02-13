using Merthsoft.Moose.MooseEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.Miner;

class MinerPlayer : GameObjectBase
{
    public const string Left = "Left";
    public const string Right = "Right";

    private MinerPlayerDef PlayerDef => (Def as MinerPlayerDef)!;

    public bool HasTorch { get; set; }
    public bool HasLantern { get; set; }

    public int Silver { get; set; }
    public int Gold { get; set; }
    public int Platinum { get; set; }

    public MinerPlayer(MinerPlayerDef def) : base(def)
    {

    }

    public override DrawParameters GetDrawParameters()
        => new(PlayerDef.Texture, (Rectangle)WorldRectangle,
            Effects: Direction == Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            LayerDepth: .5f);
}
