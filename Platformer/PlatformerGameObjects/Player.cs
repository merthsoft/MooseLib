using Microsoft.Xna.Framework;
using Merthsoft.MooseEngine;

namespace Platformer.PlatformerGameObjects
{
    class Player : PlatformerGameObject
    {
        public Player(PlayerDef def, Vector2? position = null, int layer = 0, Vector2? transformLocation = null, float rotation = 0, Vector2? scale = null, string state = States.Idle, string direction = Platformer.Direction.Right) : base(def, position, layer, transformLocation, rotation, scale, state, direction)
        {
            
        }
    }
}
