using Merthsoft.Moose.MooseEngine.Defs;

namespace Merthsoft.Moose.Platformer.PlatformerGameObjects
{
    record PlatformerGameObjectDef : AnimatedGameObjectDef
    {
        public bool IsEffectedByGravity { get; set; }

        public PlatformerGameObjectDef(string defName, string animationKey) : base(defName, animationKey)
        {
        }
    }
}
