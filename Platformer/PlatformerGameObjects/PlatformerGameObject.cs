using Microsoft.Xna.Framework;
using Merthsoft.MooseEngine.GameObjects;

namespace Platformer.PlatformerGameObjects
{
    class PlatformerGameObject : AnimatedGameObject
    {
        public new PlatformerGameObjectDef Def
        {
            get => (base.Def as PlatformerGameObjectDef)!;
            set => base.Def = value;
        }

        public class States
        {
            public const string Idle = "idle";
            public const string Walk = "run";
            public const string Jump = "jump";
            public const string Crouch = "crouch";
            public const string Fall = "fall";
        }

        public bool IsEffectedByGravity => Def.IsEffectedByGravity;

        public Vector2 Veclocity { get; set; } = Vector2.Zero;

        public bool IsFalling => State == States.Fall;
        public bool IsJumping => State == States.Jump;
        public bool IsCrouched => State == States.Crouch;
        public bool IsIdle => State == States.Idle;
        public bool IsWalking => State == States.Walk;

        public PlatformerGameObject(PlatformerGameObjectDef def, Vector2? position = null, int layer = 0, Vector2? transformLocation = null, float rotation = 0, Vector2? scale = null, string state = States.Idle, string direction = Platformer.Direction.Right) 
            : base(def, position, layer, transformLocation, rotation, scale, state, direction)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
