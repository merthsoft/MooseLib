using Merthsoft.Moose.MooseEngine;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.Miner;

record MinerPlayerDef : MinerObjectDef
{
    public Texture2D Texture { get; private set; } = null!; // Loaded in LoadContent

    public MinerPlayerDef() : base("player") { }

    public override void LoadContent(MooseContentManager contentManager)
        => Texture = contentManager.Load<Texture2D>("player");
}
