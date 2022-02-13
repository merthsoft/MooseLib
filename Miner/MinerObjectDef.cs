using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.Miner;

abstract record MinerObjectDef : GameObjectDef
{
    public MinerObjectDef(string defName) : base(defName)
    {
        DefaultLayer = 1;
        DefaultSize = new(16, 24);
    }
}
