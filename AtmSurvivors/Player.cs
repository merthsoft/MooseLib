using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;
using Microsoft.Xna.Framework;

namespace Merthsoft.Moose.AtmSurvivors;

internal class Player : TextureGameObject
{
    public Player(TextureGameObjectDef def, Vector2? position, int? layer, Vector2? size, string direction) : base(def, position, layer, size, direction)
    {

    }
}
